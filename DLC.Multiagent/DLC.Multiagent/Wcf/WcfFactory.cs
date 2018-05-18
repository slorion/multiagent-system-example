using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace DLC.Multiagent.Wcf
{
	internal static class WcfFactory
	{
		public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

		public static int? BindingListenBacklog { get; internal set; }
		public static int? BindingMaxConnections { get; internal set; }
		public static int? BehaviorMaxConcurrentCalls { get; internal set; }
		public static int? BehaviorMaxConcurrentSessions { get; internal set; }
		public static int? BehaviorMaxConcurrentInstances { get; internal set; }

		public static Tuple<ServiceHost, IEnumerable<Type>> CreateServer(object module, Uri address)
		{
			if (module == null) throw new ArgumentNullException("module");
			if (address == null) throw new ArgumentNullException("address");

			var host = new ServiceHost(module, address);
			host.Description.Behaviors.Add(new ServiceMetadataBehavior());

			var behavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
			behavior.AutomaticSessionShutdown = false;
			behavior.ConcurrencyMode = ConcurrencyMode.Multiple;
			behavior.IncludeExceptionDetailInFaults = true;
			behavior.InstanceContextMode = InstanceContextMode.Single;
			behavior.MaxItemsInObjectGraph = int.MaxValue;

			// .NET 4.5 default values, can be increased if needed (see WCF performance counters "Percent of Max Concurrent XXX")
			// MaxConcurrentCalls = Environment.ProcessorCount * 16
			// MaxConcurrentSessions = Environment.ProcessorCount * 100
			// MaxConcurrentInstances = MaxConcurrentCalls + MaxConcurrentSessions
			if (BehaviorMaxConcurrentCalls != null || BehaviorMaxConcurrentSessions != null || BehaviorMaxConcurrentInstances != null)
			{
				var throttling = new ServiceThrottlingBehavior();

				if (BehaviorMaxConcurrentCalls != null) throttling.MaxConcurrentCalls = BehaviorMaxConcurrentCalls.Value;
				if (BehaviorMaxConcurrentSessions != null) throttling.MaxConcurrentSessions = BehaviorMaxConcurrentSessions.Value;
				if (BehaviorMaxConcurrentInstances != null) throttling.MaxConcurrentInstances = BehaviorMaxConcurrentInstances.Value;

				host.Description.Behaviors.Add(throttling);
			}

			IEnumerable<Type> contracts =
				module.GetType().GetInterfaces()
					.Where(t => Attribute.IsDefined(t, typeof(ServiceContractAttribute), true));

			foreach (Type contract in contracts)
			{
				var binding = CreateBinding(address.Scheme);
				host.AddServiceEndpoint(contract, binding, contract.FullName);
			}

			var mexBinding = CreateMexBinding();
			host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, new Uri(address, "mex"));

			return Tuple.Create(host, contracts);
		}

		public static Tuple<IChannelFactory, TContract> CreateClient<TContract>(Uri address)
		{
			if (address == null) throw new ArgumentNullException("address");

			var binding = CreateBinding(address.Scheme);
			var endpoint = new EndpointAddress(new Uri(address, typeof(TContract).FullName));
			var channelFactory = new ChannelFactory<TContract>(binding, endpoint);

			foreach (var behavior in channelFactory.Endpoint.Contract.Operations.SelectMany(op => op.OperationBehaviors.OfType<DataContractSerializerOperationBehavior>()))
			{
				if (behavior.MaxItemsInObjectGraph < int.MaxValue)
					behavior.MaxItemsInObjectGraph = int.MaxValue;
			}

			return Tuple.Create((IChannelFactory) channelFactory, channelFactory.CreateChannel());
		}

		private static Binding CreateBinding(string scheme)
		{
			if (string.Equals(scheme, "net.tcp", StringComparison.OrdinalIgnoreCase))
				return CreateNetTcpBinding();
			else
				return CreateNetHttpBinding();
		}

		private static NetTcpBinding CreateNetTcpBinding()
		{
			var binding = new NetTcpBinding(SecurityMode.None);

			binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
			binding.ReaderQuotas.MaxDepth = int.MaxValue;
			binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
			binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
			binding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;

			// .NET 4.5 default is CPU count * 2, can be increased if needed
			if (BindingListenBacklog != null) binding.ListenBacklog = BindingListenBacklog.Value;

			// .NET 4.5 default is CPU count * 12, can be increased if needed
			if (BindingMaxConnections != null) binding.MaxConnections = BindingMaxConnections.Value;

			binding.MaxBufferSize = int.MaxValue;
			binding.MaxBufferPoolSize = long.MaxValue;
			binding.MaxReceivedMessageSize = long.MaxValue;

			binding.TransferMode = TransferMode.Streamed;
			binding.TransactionFlow = false;

			binding.OpenTimeout = DefaultTimeout;
			binding.CloseTimeout = DefaultTimeout;
			binding.SendTimeout = TimeSpan.MaxValue;
			binding.ReceiveTimeout = TimeSpan.MaxValue;

			return binding;
		}

		private static NetHttpBinding CreateNetHttpBinding()
		{
			var binding = new NetHttpBinding(BasicHttpSecurityMode.None);

			binding.BypassProxyOnLocal = true;
			binding.MessageEncoding = NetHttpMessageEncoding.Binary;

			binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
			binding.ReaderQuotas.MaxDepth = int.MaxValue;
			binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
			binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
			binding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;

			binding.TransferMode = TransferMode.Streamed;

			binding.MaxBufferSize = int.MaxValue;
			binding.MaxBufferPoolSize = long.MaxValue;
			binding.MaxReceivedMessageSize = long.MaxValue;

			binding.OpenTimeout = DefaultTimeout;
			binding.CloseTimeout = DefaultTimeout;
			binding.SendTimeout = TimeSpan.MaxValue;
			binding.ReceiveTimeout = TimeSpan.MaxValue;

			return binding;
		}

		private static Binding CreateMexBinding()
		{
			// on .NET 4.5, the default MEX binding conflicts with net.tcp if ListenBacklog or MaxConnections is modified (AddressAlreadyInUseException is thrown)
			// so net.tcp is also used for MEX
			// see http://blogs.msdn.com/b/amitbhatia/archive/2013/10/01/upgrading-machine-to-net-framework-4-5-wcf-service-built-on-net-framework-4-0-using-more-than-1-nettcpbinding-fails-with-addressalreadyinuseexception.aspx

			return CreateNetTcpBinding();
			// return MetadataExchangeBindings.CreateMexTcpBinding();
		}
	}
}