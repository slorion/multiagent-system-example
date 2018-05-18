// TCP Qbservable Provider is used to create and observe remote observables
// http://davesexton.com/blog/post/LINQ-to-Cloud-IQbservable-Over-the-Wire.aspx

// Microsoft CCI is used to read PE header of dynamically loaded libraries
// https://github.com/Microsoft/cci

using DLC.Framework.Reactive;
using DLC.Multiagent.Configuration;
using DLC.Multiagent.Logging;
using DLC.Multiagent.Wcf;
using NLog;
using NLog.Config;
using NLog.Fluent;
using QbservableProvider;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Cci = Microsoft.Cci;

namespace DLC.Multiagent
{
	public sealed partial class AgentBroker
		: IDisposable
	{
		private static readonly Lazy<AgentBroker> _instance = new Lazy<AgentBroker>(() => new AgentBroker(), LazyThreadSafetyMode.ExecutionAndPublication);
		public static AgentBroker Instance { get { return _instance.Value; } }

		private readonly HashSet<string> _dependencyFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		//NOTE: lazy values will always be created immediately, so we will never check their IsValueCreated property
		private readonly ConcurrentDictionary<string, Lazy<LocalAgentInformation>> _localAgents = new ConcurrentDictionary<string, Lazy<LocalAgentInformation>>();
		private readonly ConcurrentDictionary<string, Lazy<RemoteAgentInformation>> _remoteAgents = new ConcurrentDictionary<string, Lazy<RemoteAgentInformation>>();

		private readonly ConcurrentStack<IDisposable> _observers = new ConcurrentStack<IDisposable>();

		private readonly SubjectSlim<SerializableAgentInformation> _agentSubject = new SubjectSlim<SerializableAgentInformation>();
		private readonly BehaviorSubjectSlim<ServiceState> _stateSubject = new BehaviorSubjectSlim<ServiceState>(ServiceState.Stopped);

		private readonly MethodInfo _miTryGetAgent;
		private readonly MethodInfo _miServeQbservableTcp;
		private int _nextRxPort;

		public AgentBroker()
		{
			// get Qbservable2.ServeQbservableTcp method overload that will be used by EnsureObservableListening
			// there is no direct way of accomplishing this, so we have to resort to reflection
			_miServeQbservableTcp =
				(
					from mi in typeof(Qbservable2).GetMethods(BindingFlags.Static | BindingFlags.Public)
						.Where(m => m.IsGenericMethodDefinition && m.Name == "ServeQbservableTcp")
					let parameters = mi.GetParameters()
					where parameters.Length == 3
						&& parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IObservable<>)
						&& parameters[1].ParameterType == typeof(IPEndPoint)
						&& parameters[2].ParameterType == typeof(QbservableServiceOptions)
					select mi
				).First();

			// idem for this.TryGetAgent that will be used for non-generic calls
			_miTryGetAgent =
				this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
					.Where(m => m.IsGenericMethodDefinition && m.Name == "TryGetAgentUntyped")
					.First();

			// manage external assemblies loading

			var loadedAssemblies = new ConcurrentDictionary<string, Assembly>();

			AppDomain.CurrentDomain.AssemblyLoad +=
				(s, e) => loadedAssemblies[e.LoadedAssembly.GetName().Name] = e.LoadedAssembly;

			var host = new Cci.PeReader.DefaultHost();
			AppDomain.CurrentDomain.AssemblyResolve +=
				(s, e) =>
				{
					Assembly assembly;
					if (!loadedAssemblies.TryGetValue(e.Name, out assembly))
					{
						try
						{
							var assemblyName = _dependencyFolders
								.Where(folder => Directory.Exists(folder))
								.SelectMany(folder => Directory.EnumerateFiles(folder, "*.dll", SearchOption.AllDirectories))
								.Select(
									file =>
									{
										var module = host.LoadUnitFrom(file) as Cci.IModule;

										if (module != null && module.Kind == Cci.ModuleKind.DynamicallyLinkedLibrary && module.ModuleName != null && !string.IsNullOrEmpty(module.ModuleName.Value))
										{
											if (module.ILOnly
												|| (Environment.Is64BitProcess && module.Requires64bits)
												|| (!Environment.Is64BitProcess && module.Requires32bits))
											{
												return AssemblyName.GetAssemblyName(file);
											}
										}

										// not an assembly, so do not try again
										loadedAssemblies[e.Name] = null;
										return null;
									})
								.Where(an => an != null)
								.FirstOrDefault(
									candidate =>
									{
										// if both names are identical, accept the match
										// otherwise, match with a candidate that has an equal or greater version number and the same public key token

										if (string.Equals(candidate.Name, e.Name, StringComparison.Ordinal) || string.Equals(candidate.FullName, e.Name, StringComparison.Ordinal))
											return true;
										else
										{
											var requested = new AssemblyName(e.Name);
											var requestedToken = requested.GetPublicKeyToken();

											return string.Equals(candidate.Name, requested.Name, StringComparison.Ordinal)
												&& candidate.Version >= requested.Version
												&& (requestedToken == null || requestedToken.SequenceEqual(candidate.GetPublicKeyToken()));
										}
									});

							if (assemblyName == null)
							{
								Log.Warn().Message("No file found for assembly '{0}'.", e.Name).Write();
							}
							else
							{
								assembly = Assembly.Load(assemblyName);
								loadedAssemblies[e.Name] = assembly;
								Log.Debug().Message("Assembly '{0}' loaded from file '{1}'.", assembly.FullName, assembly.Location).Write();
							}
						}
						catch (Exception ex)
						{
							Log.Warn().Message("Loading assembly '{0}' has failed.", e.Name).Exception(ex).Write();
						}
					}

					return assembly;
				};
		}

		public ServiceState State { get { return _stateSubject.Value; } }
		public IObservable<ServiceState> StateDataSource { get { return _stateSubject.DistinctUntilChanged().ObserveOn(NewThreadScheduler.Default); } }
		public string ConfigurationFilePath { get; private set; }
		public string LogConfigurationFilePath { get; private set; }
		public PeerNode LocalPeerNode { get; private set; }

		public AgentBrokerConfiguration Configuration { get; private set; }
		private TimeSpan MinOperationRetryDelay { get; set; }
		private TimeSpan MaxOperationRetryDelay { get; set; }

		public IObservable<AgentInformation> AgentDataSource { get { return _agentSubject.ObserveOn(NewThreadScheduler.Default); } }
		public IObservable<BrokerLogEntry> LogDataSource { get { return BrokerLogDataSource.DataSource; } }

		public void LoadConfiguration(string configurationFilePath = null)
		{
			if (string.IsNullOrEmpty(configurationFilePath))
				configurationFilePath = AgentBrokerConfiguration.DefaultConfigurationFilePath;

			this.ConfigurationFilePath = configurationFilePath;
			var config = AgentBrokerConfiguration.Load(configurationFilePath);
			LoadConfiguration(config);
		}

		public void LoadConfiguration(AgentBrokerConfiguration configuration)
		{
			if (configuration == null) throw new ArgumentNullException("configuration");
			if (this.State != ServiceState.Stopped) throw new InvalidOperationException("The service must be stopped before configuration can be loaded.");

			configuration.Validate();

			this.Configuration = configuration;
			this.LogConfigurationFilePath = configuration.LogConfigurationFilePath;

			InitializeLogging(configuration);
		}

		public Task Start()
		{
			if (this.State != ServiceState.Stopped)
				throw new InvalidOperationException("Another instance of this service is already executing.");

			if (this.Configuration == null)
				throw new InvalidOperationException("The service must first be configured by calling LoadConfiguration.");

			return Task.Run(
				async () =>
				{
					_stateSubject.OnNext(ServiceState.Starting);

					// initial setup

					// see https://stackoverflow.com/questions/2977630/wcf-instance-already-exists-in-counterset-error-when-reopening-servicehost
					GC.Collect();
					GC.WaitForPendingFinalizers();

					_localAgents.Clear();
					_remoteAgents.Clear();

					WcfFactory.BindingListenBacklog = this.Configuration.WcfBindingListenBacklog;
					WcfFactory.BindingMaxConnections = this.Configuration.WcfBindingMaxConnections;
					WcfFactory.BehaviorMaxConcurrentCalls = this.Configuration.WcfBehaviorMaxConcurrentCalls;
					WcfFactory.BehaviorMaxConcurrentSessions = this.Configuration.WcfBehaviorMaxConcurrentSessions;
					WcfFactory.BehaviorMaxConcurrentInstances = this.Configuration.WcfBehaviorMaxConcurrentInstances;

					this.MinOperationRetryDelay = TimeSpan.FromMilliseconds(this.Configuration.MinOperationRetryDelayInMs);
					this.MaxOperationRetryDelay = TimeSpan.FromMilliseconds(this.Configuration.MaxOperationRetryDelayInMs);

					foreach (var folder in this.Configuration.DependencyFolders)
						_dependencyFolders.Add(folder);

					// create local peer
					this.LocalPeerNode = await PeerNode.Create(this.Configuration.Host, this.Configuration.Port, this.Configuration.RxPort, this.Configuration.Description).ConfigureAwait(false);
					Log.Debug().Message("Adresse assignée au AgentBroker: '{0}'.", this.LocalPeerNode).Write();

					_nextRxPort = this.Configuration.RxPort;

					// peers and agents creation

					// create local PeerCommunicationAgent
					var peerAgentInfo = await GetOrRegisterLocalAgent(PeerCommunicationAgent.Configuration, isInternal: true, forceRegister: true).ConfigureAwait(false);

					// create local agents
					await Task.WhenAll(
						this.Configuration.Agents
							.Where(agentConfig => agentConfig.Enabled)
							.Select(agentConfig => GetOrRegisterLocalAgent(agentConfig, isInternal: false, forceRegister: true))).ConfigureAwait(false);

					// activate internal agents
					await Task.WhenAll(GetLocalAgentInfos<IAgent>().Where(a => a.IsInternal).Select(a => a.Agent.Activate())).ConfigureAwait(false);

					// get remote peers list
					var remotePeers =
						(
							await Task.WhenAll(
								this.Configuration.Peers
									.Where(p => p.Enabled)
									.Select(p => PeerNode.Create(p.Host, p.Port, p.RxPort, p.Description)))
								.ConfigureAwait(false)
						).AsEnumerable();

					// filter out this multiagent from the configuration's peer list by comparing their IP and port (host is resolved if required)
					// note that == and != operators are NOT overloaded by IPAddress, the Equals method must be used instead
					remotePeers = remotePeers
						.Where(p =>
							p.Port != this.LocalPeerNode.Port
							|| !p.Host.Equals(this.LocalPeerNode.Host));

					// create remote peers
					foreach (var peer in remotePeers)
						RegisterPeer(peer);

					_heartbeatCts = new CancellationTokenSource();
					_heartbeatTask = StartHeartbeat(_heartbeatCts.Token);

					_stateSubject.OnNext(ServiceState.Started);

					// activate all local agents if requested
					// must be done AFTER service state transition to Started because ObserveXXX are active only when the service is started
					if (this.Configuration.AutoActivateAgents)
						await Task.WhenAll(GetLocalAgentInfos<IAgent>().Select(a => a.Agent.Activate())).ConfigureAwait(false);
				})
				.ContinueWith(
					async t =>
					{
						if (t.IsCanceled)
						{
							throw (Exception) t.Exception ?? new OperationCanceledException();
						}
						else if (t.IsFaulted)
						{
							await Stop(true).ConfigureAwait(false);
							throw t.Exception;
						}
					})
				.Unwrap();
		}

		public Task Stop()
		{
			return Stop(false);
		}

		private async Task Stop(bool force)
		{
			if (this.State == ServiceState.Stopped && !force)
				return;

			if (this.State != ServiceState.Stopping && this.State != ServiceState.Stopped)
				_stateSubject.OnNext(ServiceState.Stopping);

			if (_heartbeatCts != null)
				_heartbeatCts.Cancel();

			if (_heartbeatTask != null)
			{
				try { await _heartbeatTask.ConfigureAwait(false); }
				catch (Exception ex) { Log.Error().Exception(ex).Write(); }
			}

			_heartbeatCts = null;
			_heartbeatTask = null;

			foreach (var observer in _observers)
				observer.Dispose();
			_observers.Clear();

			var localAgents = _localAgents.Values;
			var remoteAgents = _remoteAgents.Values;

			await Task.WhenAll(localAgents.Where(a => a.IsValueCreated).Select(
				async a =>
				{
					try { await a.Value.Agent.Deactivate().ConfigureAwait(false); }
					catch (Exception ex) { Log.Error().Exception(ex).WithAgent(a.Value.AgentId).Write(); }
					finally
					{
						try { a.Value.Agent.Dispose(); }
						catch (Exception ex) { Log.Error().Exception(ex).WithAgent(a.Value.AgentId).Write(); }
					}

					try { await Task.Factory.FromAsync(a.Value.ServiceHost.BeginClose, a.Value.ServiceHost.EndClose, null).ConfigureAwait(false); }
					catch (Exception ex) { Log.Error().Exception(ex).WithAgent(a.Value.AgentId).Write(); }
				})).ConfigureAwait(false);

			await Task.WhenAll(remoteAgents.Where(a => a.IsValueCreated).Select(
				async a =>
				{
					try
					{
						await a.Value.ServiceClientFactory.Close().ConfigureAwait(false);
						a.Value.ServiceClientFactory.Dispose();
					}
					catch (Exception ex) { Log.Error().Exception(ex).WithAgent(a.Value.AgentId).Write(); }
				})).ConfigureAwait(false);

			foreach (var item in localAgents.Where(a => a.IsValueCreated).SelectMany(a => a.Value.RxServices.Values.Where(s => s.IsValueCreated).Select(s => new { AgentId = a.Value.AgentId, RxService = s.Value })))
			{
				try { item.RxService.Connection.Dispose(); }
				catch (Exception ex) { Log.Error().Exception(ex).WithAgent(item.AgentId).Write(); }
			}

			_localAgents.Clear();
			_remoteAgents.Clear();

			if (this.State != ServiceState.Stopped)
				_stateSubject.OnNext(ServiceState.Stopped);
		}

		public IEnumerable<Task<ExecutionResult<string>>> GetBrokerLogs(bool archive)
		{
			return TryExecuteOnAll<IPeerCommunicationAgent, string>(a => a.GetBrokerLog(archive));
		}

		public IEnumerable<Task<ExecutionResult>> RecycleAgent(IEnumerable<string> agentIds)
		{
			if (agentIds == null) throw new ArgumentNullException("agentIds");

			return agentIds.Select(id => RecycleAgent(id));
		}

		public async Task<ExecutionResult> RecycleAgent(string agentId)
		{
			if (string.IsNullOrEmpty(agentId)) throw new ArgumentNullException("agentId");

			Lazy<LocalAgentInformation> lazyLocal;
			Lazy<RemoteAgentInformation> lazyRemote;

			if (_localAgents.TryRemove(agentId, out lazyLocal))
			{
				var result = new ExecutionResult { AgentId = agentId };
				try
				{
					var local = lazyLocal.Value;

					local.IsRecycled = true;
					local.Agent.Dispose();
					local.ServiceHost.Close();

					foreach (var rxService in local.RxServices.Where(s => s.Value.IsValueCreated))
						rxService.Value.Value.Connection.Dispose();

					var agentInfo = await GetOrRegisterLocalAgent(local.Configuration, local.IsInternal, forceRegister: true).ConfigureAwait(false);

					if (this.Configuration.AutoActivateAgents)
						await agentInfo.Agent.Activate().ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					result.Exception = ex;
				}

				return result;
			}
			else if (_remoteAgents.TryRemove(agentId, out lazyRemote))
			{
				try
				{
					var remote = lazyRemote.Value;

					remote.IsRecycled = true;
					remote.SetIsReachable(false);
					remote.SetLastKnownState(AgentState.Disposed);

					_agentSubject.OnNext(new SerializableAgentInformation(remote));

					await remote.ServiceClientFactory.Close().ConfigureAwait(false);

					var peerAgentId = GetAgentId(remote.PeerNode, PeerCommunicationAgent.Configuration.Name);
					var result = await TryExecuteOnOne<IPeerCommunicationAgent, bool>(peerAgentId, a => a.RecycleAgent(remote.AgentId)).ConfigureAwait(false);

					GetOrRegisterRemoteAgent(remote.PeerNode, remote.AgentId, remote.DisplayData, remote.Contracts, remote.IsInternal, forceRegister: true);

					if (!result.IsSuccessful)
						return result;
					else if (!result.Result)
						return new ExecutionResult { AgentId = agentId, Exception = new InvalidOperationException(string.Format("Recycling of remote agent '{0}' failed.", agentId)) };
					else
						return new ExecutionResult { AgentId = agentId };
				}
				catch (Exception ex)
				{
					return new ExecutionResult { AgentId = agentId, Exception = ex };
				}
			}
			else
			{
				return new ExecutionResult { AgentId = agentId, Exception = new ArgumentException(string.Format("Agent '{0}' cannot be found.", agentId), "agentId") };
			}
		}

		public IReadOnlyDictionary<PeerNode, bool> GetRemotePeersStatus()
		{
			return GetRemoteAgentInfos<IPeerCommunicationAgent>().ToDictionary(info => info.PeerNode, info => info.IsReachable);
		}

		public async Task<bool> GetRemotePeerStatus(string host, int port)
		{
			var peerNode = await PeerNode.Create(host, port, 0).ConfigureAwait(false);
			return GetRemotePeerStatus(peerNode);
		}

		public bool GetRemotePeerStatus(IPAddress ip, int port)
		{
			if (ip == null) throw new ArgumentNullException("ip");

			return GetRemotePeerStatus(PeerNode.Create(ip, port, 0));
		}

		public bool GetRemotePeerStatus(PeerNode peer)
		{
			if (peer == null) throw new ArgumentNullException("peer");

			var peerInfo = GetRemoteAgentInfos<IPeerCommunicationAgent>().FirstOrDefault(info => info.PeerNode == peer);
			return peerInfo == null ? false : peerInfo.IsReachable;
		}

		public bool TryGetAgentInformation(string agentId, out AgentInformation agentInfo)
		{
			if (string.IsNullOrEmpty(agentId)) throw new ArgumentNullException("agentId");

			Lazy<LocalAgentInformation> local;
			Lazy<RemoteAgentInformation> remote;

			if (_localAgents.TryGetValue(agentId, out local))
			{
				agentInfo = local.Value;
				return true;
			}
			else if (_remoteAgents.TryGetValue(agentId, out remote))
			{
				agentInfo = remote.Value;
				return true;
			}
			else
			{
				agentInfo = null;
				return false;
			}
		}

		internal Tuple<TryGetAgentResult, AgentInformation, IAgent> TryGetAgent(Type agentType, string agentId)
		{
			if (agentType == null) throw new ArgumentNullException("agentType");
			if (string.IsNullOrEmpty(agentId)) throw new ArgumentNullException("agentId");

			var miGeneric = _miTryGetAgent.MakeGenericMethod(agentType);
			return (Tuple<TryGetAgentResult, AgentInformation, IAgent>) miGeneric.Invoke(this, new[] { agentId });
		}

		// avoid the use of Reflection or Dynamic
		// note that Dynamic will not work when the agent type is not public and from an external assembly
		// e.g.
		// dynamic result = miGeneric.Invoke(this, new[] { agentId });
		// return Tuple.Create((TryGetAgentResult) result.Item1, (AgentInformation) result.Item2, (IAgent) result.Item3);
		// --> if TAgent is not public, the above will throw an exception with the message "dynamic 'object' does not contain a definition for 'Item1'"
		private Tuple<TryGetAgentResult, AgentInformation, IAgent> TryGetAgentUntyped<TAgent>(string agentId)
			where TAgent : IAgent
		{
			var result = TryGetAgent<TAgent>(agentId);
			return Tuple.Create(result.Item1, result.Item2, (IAgent) result.Item3);
		}

		public Tuple<TryGetAgentResult, AgentInformation, TAgent> TryGetAgent<TAgent>(string agentId)
			where TAgent : IAgent
		{
			if (string.IsNullOrEmpty(agentId)) throw new ArgumentNullException("agentId");

			LocalAgentInformation local;
			RemoteAgentInformation remote;

			AgentInformation agentInfo;
			if (!TryGetAgentInformation(agentId, out agentInfo))
			{
				return Tuple.Create(TryGetAgentResult.NotFound, (AgentInformation) null, default(TAgent));
			}
			else if ((local = agentInfo as LocalAgentInformation) != null)
			{
				if (!(local.Agent is TAgent))
				{
					return Tuple.Create(TryGetAgentResult.ContractNotImplemented, agentInfo, default(TAgent));
				}
				else
				{
					var result = local.LastKnownState == AgentState.Activated ? TryGetAgentResult.Success : TryGetAgentResult.NotActivated;
					return Tuple.Create(result, agentInfo, (TAgent) local.Agent);
				}
			}
			else if ((remote = agentInfo as RemoteAgentInformation) != null)
			{
				if (!remote.Contracts.Contains(typeof(TAgent).AssemblyQualifiedName))
				{
					return Tuple.Create(TryGetAgentResult.ContractNotImplemented, agentInfo, default(TAgent));
				}
				else
				{
					var agent = (TAgent) remote.ServiceClientFactory.CreateServiceClient<TAgent>();
					TryGetAgentResult result;

					if (remote.IsReachable)
						result = remote.LastKnownState == AgentState.Activated ? TryGetAgentResult.Success : TryGetAgentResult.NotActivated;
					else
						result = TryGetAgentResult.Unreachable;

					return Tuple.Create(result, agentInfo, agent);
				}
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		public IEnumerable<Tuple<AgentInformation, TAgent>> GetAgents<TAgent>(ExecutionScopeOptions scope, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			var agents = Enumerable.Empty<Tuple<AgentInformation, TAgent>>();

			if (scope.HasFlag(ExecutionScopeOptions.Local))
				agents = agents.Concat(GetLocalAgentInfos<TAgent>().Select(info => Tuple.Create((AgentInformation) info, (TAgent) info.Agent)));

			if (scope.HasFlag(ExecutionScopeOptions.Remote))
				agents = agents.Concat(GetRemoteAgentInfos<TAgent>().Select(info => Tuple.Create((AgentInformation) info, (TAgent) info.ServiceClientFactory.CreateServiceClient<TAgent>())));

			if (!ignoreAgentState)
				agents = agents.Where(a => a.Item1.LastKnownState == AgentState.Activated);

			return agents;
		}

		internal IEnumerable<AgentInformation> GetAgentInfos<TAgent>(ExecutionScopeOptions scope)
			where TAgent : IAgent
		{
			var agents = Enumerable.Empty<AgentInformation>();

			if (scope.HasFlag(ExecutionScopeOptions.Local))
				agents = agents.Concat(GetLocalAgentInfos<TAgent>());

			if (scope.HasFlag(ExecutionScopeOptions.Remote))
				agents = agents.Concat(GetRemoteAgentInfos<TAgent>());

			return agents;
		}

		internal IEnumerable<LocalAgentInformation> GetLocalAgentInfos<TAgent>()
			where TAgent : IAgent
		{
			return _localAgents.Values.Where(kv => kv.Value.Agent is TAgent).Select(kv => kv.Value);
		}

		internal IEnumerable<RemoteAgentInformation> GetRemoteAgentInfos<TAgent>()
			where TAgent : IAgent
		{
			string contract = typeof(TAgent).AssemblyQualifiedName;
			return _remoteAgents.Values.Where(kv => kv.Value.Contracts.Contains(contract)).Select(kv => kv.Value);
		}

		private async Task<LocalAgentInformation> GetOrRegisterLocalAgent(AgentConfiguration configuration, bool isInternal, bool forceRegister = false)
		{
			if (configuration == null) throw new ArgumentNullException("configuration");

			foreach (var folder in configuration.DependencyFolders)
				_dependencyFolders.Add(folder);

			var agentId = GetAgentId(this.LocalPeerNode, configuration.Name);

			var newLazyLocal = new Lazy<LocalAgentInformation>(
				() =>
				{
					var agentType = Type.GetType(configuration.TypeName, true, true);
					var agent = (IAgent) Activator.CreateInstance(agentType);
					agent.LoadConfiguration(agentId, configuration);

					var hostAndContracts = WcfFactory.CreateServer(agent, GetAgentUri(this.LocalPeerNode, agent.Id));
					Log.Trace().Message("Serveur WCF créé à l'adresse '{0}'.", string.Join(",", hostAndContracts.Item1.BaseAddresses.Select(uri => uri.ToString()))).WithAgent(agentId).Write();

					// start the ServiceHost
					// that should normally be done async, but we do not want to exit this function with a server that is not listening
					try
					{
						hostAndContracts.Item1.Open();
					}
					catch (Exception ex)
					{
						Log.Warn().Message("Opening WCF server at address '{0}' has failed.", string.Join(",", hostAndContracts.Item1.BaseAddresses.Select(uri => uri.ToString()))).Exception(ex).WithAgent(agentId).Write();
						throw;
					}

					return new LocalAgentInformation(this.LocalPeerNode, agentId, configuration, hostAndContracts.Item2.Select(t => t.AssemblyQualifiedName), isInternal, agent, hostAndContracts.Item1);
				}, LazyThreadSafetyMode.ExecutionAndPublication);

			Lazy<LocalAgentInformation> lazyLocal;

			if (forceRegister)
				lazyLocal = _localAgents.AddOrUpdate(agentId, newLazyLocal, (id, old) => newLazyLocal);
			else
				lazyLocal = _localAgents.GetOrAdd(agentId, newLazyLocal);

			if (!lazyLocal.IsValueCreated)
			{
				if (lazyLocal.Value.ServiceHost.State != CommunicationState.Opened)
					await Task.Factory.FromAsync(lazyLocal.Value.ServiceHost.BeginOpen, lazyLocal.Value.ServiceHost.EndOpen, null).ConfigureAwait(false);

				_agentSubject.OnNext(new SerializableAgentInformation(lazyLocal.Value));
				RegisterObserver(lazyLocal.Value.Agent.StateDataSource.Subscribe(state => _agentSubject.OnNext(new SerializableAgentInformation(lazyLocal.Value))));
			}

			return lazyLocal.Value;
		}

		private void RegisterPeer(PeerNode peer)
		{
			if (peer == null) throw new ArgumentNullException("peer");

			var peerAgentId = GetAgentId(peer, PeerCommunicationAgent.Configuration.Name);

			var peerAgentInfo = GetOrRegisterRemoteAgent(
				peer,
				peerAgentId,
				new AgentDisplayData(PeerCommunicationAgent.Configuration.Name, PeerCommunicationAgent.Configuration.ShortName, PeerCommunicationAgent.Configuration.Description),
				new[] { typeof(IPeerCommunicationAgent).AssemblyQualifiedName, typeof(IAgent).AssemblyQualifiedName },
				isInternal: true,
				forceRegister: true);

			// broadcast new peer communication agent, sending connection state immediately to the remote peer
			_agentSubject.OnNext(new SerializableAgentInformation(peerAgentInfo));
		}

		private CancellationTokenSource _heartbeatCts;
		private Task _heartbeatTask;

		private Task StartHeartbeat(CancellationToken ct)
		{
			return Task.Factory.StartNew(
				async () =>
				{
					while (!ct.IsCancellationRequested)
					{
						try
						{
							var tasks = _remoteAgents.Values
								.Select(lazy => lazy.Value)
								.GroupBy(info => info.PeerNode)
								.Select(
									async agentInfoGroupedByPeer =>
									{
										var peerAgentInfo = agentInfoGroupedByPeer.Single(info => info.Contracts.Contains(typeof(IPeerCommunicationAgent).AssemblyQualifiedName));
										var ping = await TryExecuteOnOneUnsafe<IAgent, bool>(peerAgentInfo.AgentId, a => a.Ping(), ignoreAgentState: true, ignoreUnreachable: true).ConfigureAwait(false);

										foreach (var info in agentInfoGroupedByPeer)
										{
											bool hasChanged = false;

											var oldIsReachable = info.IsReachable;
											info.SetIsReachable(ping.IsSuccessful && ping.Result);

											if (!ct.IsCancellationRequested && info.IsReachable)
											{
												var stateResult = await TryExecuteOnOne<IAgent, AgentState>(info.AgentId, a => a.State, ignoreAgentState: true).ConfigureAwait(false);

												if (!stateResult.IsSuccessful)
												{
													info.SetIsReachable(false);

													if (stateResult.Exception != null)
														Log.Debug().Exception(stateResult.Exception).WithAgent(stateResult.AgentId).Write();
												}
												else
												{
													hasChanged |= (info.LastKnownState != stateResult.Result);
													info.SetLastKnownState(stateResult.Result);
												}
											}

											// if a IPeerCommunicationAgent connects/reconnects, refresh remote agents list of the related peer
											// the agents in that list will be refreshed on next heartbeat
											if (!ct.IsCancellationRequested && info == peerAgentInfo && !oldIsReachable && info.IsReachable)
											{
												var agentListResult = await TryExecuteOnOne<IPeerCommunicationAgent, IEnumerable<Tuple<string, AgentDisplayData, IEnumerable<string>>>>(info.AgentId, a => a.GetAgentList()).ConfigureAwait(false);

												if (!agentListResult.IsSuccessful)
												{
													info.SetIsReachable(false);

													if (agentListResult.Exception != null)
														Log.Debug().Exception(agentListResult.Exception).WithAgent(agentListResult.AgentId).Write();
												}
												else
												{
													foreach (var remote in agentListResult.Result)
														GetOrRegisterRemoteAgent(info.PeerNode, remote.Item1, remote.Item2, remote.Item3, isInternal: false);
												}
											}

											hasChanged |= (oldIsReachable != info.IsReachable);

											if (!ct.IsCancellationRequested && hasChanged)
												_agentSubject.OnNext(new SerializableAgentInformation(info));
										}
									}).ToArray();

							await Task.WhenAll(tasks).ConfigureAwait(false);

							if (!ct.IsCancellationRequested)
								await Task.Delay(this.Configuration.HeartbeatFrequencyInMs, ct).ConfigureAwait(false);
						}
						catch (TaskCanceledException) { }
					}
				}, TaskCreationOptions.LongRunning);
		}

		private RemoteAgentInformation GetOrRegisterRemoteAgent(PeerNode peer, string agentId, AgentDisplayData displayData, IEnumerable<string> contracts, bool isInternal, bool forceRegister = false)
		{
			if (peer == null) throw new ArgumentNullException("peer");
			if (string.IsNullOrEmpty(agentId)) throw new ArgumentNullException("agentId");
			if (displayData == null) throw new ArgumentNullException("displayData");
			if (contracts == null) throw new ArgumentNullException("contracts");

			var newLazyRemote = new Lazy<RemoteAgentInformation>(
				() =>
				{
					var serviceClientFactory = new ServiceClientFactory(GetAgentUri(peer, agentId));
					return new RemoteAgentInformation(peer, agentId, displayData, contracts, isInternal, serviceClientFactory);
				});

			Lazy<RemoteAgentInformation> lazyRemote;

			if (!forceRegister)
				lazyRemote = _remoteAgents.GetOrAdd(agentId, newLazyRemote);
			else
				lazyRemote = _remoteAgents.AddOrUpdate(agentId, newLazyRemote, (id, old) => newLazyRemote);

			// do not broadcast agent immediately, it will be done on next heartbeat (see StartHeartbeat)

			return lazyRemote.Value;
		}

		private Uri GetAgentUri(PeerNode peer, string agentId)
		{
			if (peer == null) throw new ArgumentNullException("peer");
			if (string.IsNullOrEmpty(agentId)) throw new ArgumentNullException("agentId");

			return new Uri(string.Format("{0}://{1}:{2}/{3}/", Uri.UriSchemeNetTcp, peer.Host, peer.Port, agentId));
		}

		public string GetAgentId(PeerNode peer, string agentName)
		{
			if (peer == null) throw new ArgumentNullException("peer");
			if (string.IsNullOrEmpty(agentName)) throw new ArgumentNullException("agentName");

			return string.Format("{0}:{1} - {2}", peer.Host, peer.Port, agentName);
		}

		private void RegisterObserver(IDisposable observer)
		{
			if (observer == null) throw new ArgumentNullException("observer");

			_observers.Push(observer);
		}

		internal RxService EnsureObservableListening(string agentId, string propertyName)
		{
			if (string.IsNullOrEmpty(agentId)) throw new ArgumentNullException("agentId");
			if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

			Lazy<LocalAgentInformation> lazyLocal;
			if (_localAgents.TryGetValue(agentId, out lazyLocal))
			{
				var local = lazyLocal.Value;

				var lazyService = new Lazy<RxService>(
					() =>
					{
						//TODO: add support for HttpListener to TCP Qbservable Provider (based on QbservableTcpServer)
						// see https://rxx.codeplex.com/workitem/25850
						// setup procedure to follow to give permissions to service if HttpListener is used:
						// https://stackoverflow.com/questions/169904/can-i-listen-on-a-port-using-httplistener-or-other-net-code-on-vista-without

						// for now, use a different port for each Qbservable
						var rxPort = Interlocked.Increment(ref _nextRxPort);

						//var source = local.<propertyName>;
						var piDataSource = local.Agent.GetType().GetProperty(propertyName);
						var dataSource = piDataSource.GetValue(local.Agent);

						var dataType = piDataSource.PropertyType.GetGenericArguments().First();

						var knownTypes = new List<Type>();
						knownTypes.Add(dataType);
						if (dataType.IsGenericType)
							knownTypes.AddRange(dataType.GenericTypeArguments);

						var qbservableOptions = new QbservableServiceOptions { AllowExpressionsUnrestricted = true, EnableDuplex = true, SendServerErrorsToClients = true };
						foreach (var type in knownTypes)
							qbservableOptions.EvaluationContext.AddKnownType(type);

						Log.Debug().Message("Creation of a Qbservable server at '{0}:{1}' for the property '{2}'.", this.LocalPeerNode.Host, rxPort, propertyName).WithAgent(agentId).Write();

						//var service = source.ServeQbservableTcp(new IPEndPoint(IPAddress.Parse(this.LocalPeerNode.Host), rxPort), new QbservableServiceOptions { AllowExpressionsUnrestricted = true, EnableDuplex = true, SendServerErrorsToClients = true });
						var miServeQbservableTcp = _miServeQbservableTcp.MakeGenericMethod(dataType);
						var service = (IObservable<TcpClientTermination>) miServeQbservableTcp
							.Invoke(null, new object[] { dataSource, new IPEndPoint(this.LocalPeerNode.Host, rxPort), qbservableOptions });

						return new RxService(service, rxPort,
							service.Subscribe(
								termination =>
								{
									LogLevel logLevel;
									switch (termination.Reason)
									{
										case QbservableProtocolShutDownReason.None:
										case QbservableProtocolShutDownReason.ProtocolTerminated:
										case QbservableProtocolShutDownReason.ObservableTerminated:
										case QbservableProtocolShutDownReason.ClientTerminated:
											logLevel = LogLevel.Debug;
											break;
										default:
											logLevel = LogLevel.Warn;
											break;
									}

									var logBuilder = Log.Level(logLevel).Message("Qbservable server located at '{0}' for property '{1}' has terminated with the reason '{2}'.", termination.LocalEndPoint, propertyName, termination.Reason);

									if (termination.Exceptions.Count > 0)
										logBuilder = logBuilder.Exception(new AggregateException(termination.Exceptions.Select(info => info.SourceException)));

									logBuilder.WithAgent(agentId).Write();
								}));
					});

				return local.RxServices.GetOrAdd(propertyName, lazyService).Value;
			}
			else
			{
				throw new ArgumentException(string.Format("Local agent '{0}' cannot be found.", agentId));
			}
		}

		private static void InitializeLogging(AgentBrokerConfiguration configuration)
		{
			if (configuration == null) throw new ArgumentNullException("configuration");

			ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition(MultiagentLayoutRenderer.Name, typeof(MultiagentLayoutRenderer));

			if (!string.IsNullOrEmpty(configuration.LogConfigurationFilePath))
			{
				var logConfig = new XmlLoggingConfiguration(configuration.LogConfigurationFilePath);
				LogManager.Configuration = logConfig;
			}

			BrokerLogDataSource.EnsureInitialize();
		}

		#region IDisposable members

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			try
			{
				this.Stop().Wait(TimeSpan.FromSeconds(10));
			}
			catch (Exception ex)
			{
				if (disposing)
					throw;
				else
				{
					try { Log.Error().Exception(ex).Write(); }
					catch { }
				}
			}
		}

		~AgentBroker()
		{
			Log.Warn().Message("Object was not disposed correctly.").Write();
			Dispose(false);
		}

		#endregion
	}
}