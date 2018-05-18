using System;
using System.Linq.Expressions;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal abstract class SerializableExpression
	{
		public ExpressionType NodeType
		{
			get
			{
				return nodeType;
			}
		}

		public Type Type
		{
			get
			{
				return type;
			}
		}

		private readonly ExpressionType nodeType;
		private readonly Type type;

		[NonSerialized]
		private Expression converted;

		public SerializableExpression(Expression expression)
		{
			this.nodeType = expression.NodeType;
			this.type = expression.Type;
		}

		/* Caching is required to ensure that expressions referring to the same objects actually refer to the same
		 * instances in memory.  For example, when a lambda expression's Body uses parameters, they must reference 
		 * the actual expression objects that are defined in the lambda expression's Parameters collection; otherwise, 
		 * compiling the lambda throws an exception with a message similar to the following: 
		 * 
		 * Expression variable 'p' of type 'System.Int32' referenced from scope '', but it is not defined
		 */
		internal Expression ConvertWithCache()
		{
			return converted ?? (converted = Convert());
		}

		internal abstract Expression Convert();
	}
}