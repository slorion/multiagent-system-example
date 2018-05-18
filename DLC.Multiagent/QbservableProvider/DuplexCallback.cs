using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QbservableProvider.Properties;

namespace QbservableProvider
{
	[Serializable]
	internal class DuplexCallback
	{
		protected IServerDuplexQbservableProtocolSink Sink
		{
			get
			{
				return sink;
			}
		}

		protected QbservableProtocol Protocol
		{
			get
			{
				return protocol;
			}
		}

		protected int Id
		{
			get
			{
				return id;
			}
		}

		private static readonly MethodInfo serverInvokeMethod = typeof(DuplexCallback)
			.GetMethods()
			.Where(m => m.IsGenericMethod && m.Name == "ServerInvoke")
			.First();

		private static readonly MethodInfo serverInvokeVoidMethod = typeof(DuplexCallback)
			.GetMethods()
			.Where(m => !m.IsGenericMethod && m.Name == "ServerInvoke")
			.First();

		[NonSerialized]
		private IServerDuplexQbservableProtocolSink sink;
		[NonSerialized]
		private QbservableProtocol protocol;
		private readonly int id;

		protected DuplexCallback(int id)
		{
			this.id = id;
		}

		private DuplexCallback(QbservableProtocol protocol, Func<int, object[], object> callback)
		{
			this.id = protocol
				.GetOrAddSink(protocol.CreateClientDuplexSinkInternal)
				.RegisterInvokeCallback(arguments => callback(this.id, arguments));
		}

		public static Expression Create(QbservableProtocol protocol, object instance, PropertyInfo property)
		{
			return CreateInvoke(
				new DuplexCallback(protocol, (id, __) => ConvertIfSequence(protocol, id, property.GetValue(instance))),
				property.PropertyType);
		}

		public static Expression Create(QbservableProtocol protocol, object instance, FieldInfo field)
		{
			return CreateInvoke(
				new DuplexCallback(protocol, (id, __) => ConvertIfSequence(protocol, id, field.GetValue(instance))),
				field.FieldType);
		}

		public static Expression Create(QbservableProtocol protocol, object instance, MethodInfo method, IEnumerable<Expression> argExpressions)
		{
			return CreateInvoke(
				new DuplexCallback(protocol, (id, arguments) => ConvertIfSequence(protocol, id, method.Invoke(instance, arguments))),
				method.ReturnType,
				argExpressions);
		}

		public static Expression CreateEnumerable(QbservableProtocol protocol, object instance, Type dataType, Type type)
		{
			return Expression.Constant(
				CreateRemoteEnumerable(protocol, (IEnumerable) instance, dataType),
				type);
		}

		public static Expression CreateObservable(QbservableProtocol protocol, object instance, Type dataType, Type type)
		{
			return Expression.Constant(
				CreateRemoteObservable(protocol, instance, dataType),
				type);
		}

		private static Expression CreateInvoke(DuplexCallback callback, Type returnType, IEnumerable<Expression> arguments = null)
		{
			return Expression.Call(
				Expression.Constant(callback),
				returnType == typeof(void) ? DuplexCallback.serverInvokeVoidMethod : DuplexCallback.serverInvokeMethod.MakeGenericMethod(returnType),
				Expression.NewArrayInit(
					typeof(object),
					(arguments == null ? new Expression[0] : arguments.Select(a => (Expression) Expression.Convert(a, typeof(object))))));
		}

		private static object ConvertIfSequence(QbservableProtocol protocol, int id, object instance)
		{
			if (instance != null)
			{
				var type = instance.GetType();

				if (!type.IsSerializable)
				{
					var observableType = type.GetGenericInterfaceFromDefinition(typeof(IObservable<>));

					if (observableType != null)
					{
						return CreateRemoteObservable(protocol, instance, observableType.GetGenericArguments()[0]);
					}

					var enumerableType = type.GetGenericInterfaceFromDefinition(typeof(IEnumerable<>));

					if (enumerableType != null)
					{
						return CreateRemoteEnumerable(protocol, (IEnumerable) instance, enumerableType.GetGenericArguments()[0]);
					}
					else if (instance is IEnumerable)
					{
						var enumerable = (IEnumerable) instance;

						return CreateRemoteEnumerable(protocol, enumerable.Cast<object>(), typeof(object));
					}
				}
			}

			return instance;
		}

		private static object CreateRemoteEnumerable(QbservableProtocol protocol, IEnumerable instance, Type dataType)
		{
			var sink = protocol.GetOrAddSink(protocol.CreateClientDuplexSinkInternal);

			int id = 0;
			id = sink.RegisterEnumerableCallback(instance.GetEnumerator);

			return Activator.CreateInstance(typeof(DuplexCallbackEnumerable<>).MakeGenericType(dataType), id);
		}

		private static object CreateRemoteObservable(QbservableProtocol protocol, object instance, Type dataType)
		{
			var sink = protocol.GetOrAddSink(protocol.CreateClientDuplexSinkInternal);

			int id = 0;
			id = sink.RegisterObservableCallback(serverId => Subscribe(sink, new DuplexCallbackId(id, serverId), instance, dataType));

			return Activator.CreateInstance(typeof(DuplexCallbackObservable<>).MakeGenericType(dataType), id);
		}

		private static IDisposable Subscribe(IClientDuplexQbservableProtocolSink sink, DuplexCallbackId id, object instance, Type dataType)
		{
			return dataType.UpCast(instance).Subscribe(
				value => sink.SendOnNext(id, value),
				ex => sink.SendOnError(id, ex),
				() => sink.SendOnCompleted(id));
		}

		public void SetServerProtocol(QbservableProtocol protocol)
		{
			Contract.Requires(protocol != null);

			this.protocol = protocol;
			this.sink = protocol.FindSink<IServerDuplexQbservableProtocolSink>();

			if (sink == null)
			{
				throw new InvalidOperationException(Errors.ProtocolDuplexSinkUnavailableForClientCallback);
			}
		}

		public TResult ServerInvoke<TResult>(object[] arguments)
		{
			Contract.Requires(sink != null);

			var value = (TResult) sink.Invoke(id, arguments);

			var callback = value as DuplexCallback;

			if (callback != null)
			{
				callback.sink = sink;
			}

			return value;
		}

		public void ServerInvoke(object[] arguments)
		{
			Contract.Requires(sink != null);

			sink.Invoke(id, arguments);
		}
	}
}