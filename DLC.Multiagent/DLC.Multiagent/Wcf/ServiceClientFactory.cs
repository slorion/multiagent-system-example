using NLog.Fluent;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;

namespace DLC.Multiagent.Wcf
{
	internal class ServiceClientFactory
		: IDisposable
	{
		private readonly ConcurrentDictionary<Type, Lazy<Tuple<IChannelFactory, object>>> _clients = new ConcurrentDictionary<Type, Lazy<Tuple<IChannelFactory, object>>>();
		private readonly MethodInfo _miCreateServiceClient;

		public ServiceClientFactory(Uri uri)
		{
			if (uri == null) throw new ArgumentNullException("uri");

			_miCreateServiceClient = this.GetType().GetMethod("CreateServiceClient", new Type[] { });
			this.Uri = uri;
		}

		public Uri Uri { get; private set; }

		public object CreateServiceClient(Type serviceType)
		{
			if (serviceType == null) throw new ArgumentNullException("serviceType");

			var miGeneric = _miCreateServiceClient.MakeGenericMethod(serviceType);
			return miGeneric.Invoke(this, null);
		}

		public TService CreateServiceClient<TService>()
		{
			var client =
				_clients.GetOrAdd(
					typeof(TService),
					type =>
						new Lazy<Tuple<IChannelFactory, object>>(
							() =>
							{
								var serviceUri = new Uri(this.Uri, typeof(TService).FullName);
								var tuple = WcfFactory.CreateClient<TService>(serviceUri);

								// communication object should transition to opened state later by default,
								// but lets ensure it does
								if (tuple.Item1.State == CommunicationState.Created)
									tuple.Item1.Open(WcfFactory.DefaultTimeout);

								return Tuple.Create(tuple.Item1, (object) tuple.Item2);
							}, LazyThreadSafetyMode.ExecutionAndPublication));

			return (TService) client.Value.Item2;
		}

		public async Task Close()
		{
			if (_clients != null)
			{
				try
				{
					var channelFactories = _clients.Values.Where(v => v.IsValueCreated).Select(v => v.Value.Item1).ToArray();
					await Task.WhenAll(channelFactories.Select(f => Task.Factory.FromAsync(f.BeginClose, f.EndClose, null))).ConfigureAwait(false);
				}
				finally
				{
					_clients.Clear();
				}
			}
		}

		#region IDisposable members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			Close().Wait(TimeSpan.FromSeconds(30));
		}

		~ServiceClientFactory()
		{
			Log.Warn().Message("Object was not disposed correctly.").Write();
			Dispose(false);
		}

		#endregion
	}
}