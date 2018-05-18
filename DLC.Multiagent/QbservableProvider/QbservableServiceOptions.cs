using System;

namespace QbservableProvider
{
	[Serializable]
	public sealed class QbservableServiceOptions
	{
		public static readonly QbservableServiceOptions Default = new QbservableServiceOptions()
			{
				frozen = true,
				evaluationContext = new ServiceEvaluationContext()
			};

		public bool SendServerErrorsToClients
		{
			get
			{
				return sendServerErrorsToClients;
			}
			set
			{
				if (frozen)
				{
					throw new NotSupportedException();
				}

				sendServerErrorsToClients = value;
			}
		}

		public bool EnableDuplex
		{
			get
			{
				return enableDuplex;
			}
			set
			{
				if (frozen)
				{
					throw new NotSupportedException();
				}

				enableDuplex = value;
			}
		}

		public bool AllowExpressionsUnrestricted
		{
			get
			{
				return allowExpressionsUnrestricted;
			}
			set
			{
				if (frozen)
				{
					throw new NotSupportedException();
				}

				allowExpressionsUnrestricted = value;
			}
		}

		public ExpressionOptions ExpressionOptions
		{
			get
			{
				return expressionOptions;
			}
			set
			{
				if (frozen)
				{
					throw new NotSupportedException();
				}

				expressionOptions = value;
			}
		}

		public ServiceEvaluationContext EvaluationContext
		{
			get
			{
				if (evaluationContext == null)
				{
					evaluationContext = new ServiceEvaluationContext();
				}

				return evaluationContext;
			}
			set
			{
				if (frozen)
				{
					throw new NotSupportedException();
				}

				evaluationContext = value;
			}
		}

		private bool frozen;
		private bool sendServerErrorsToClients;
		private bool enableDuplex;
		private bool allowExpressionsUnrestricted;
		private ExpressionOptions expressionOptions;
		private ServiceEvaluationContext evaluationContext;
	}
}