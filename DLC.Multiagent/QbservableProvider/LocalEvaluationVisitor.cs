using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QbservableProvider.Properties;

namespace QbservableProvider
{
	internal sealed class LocalEvaluationVisitor : ExpressionVisitor
	{
		private readonly LocalEvaluator evaluator;
		private readonly QbservableProtocol protocol;

		public LocalEvaluationVisitor(LocalEvaluator evaluator, QbservableProtocol protocol)
		{
			this.evaluator = evaluator;
			this.protocol = protocol;
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (node.NodeType == ExpressionType.Assign)
			{
				MethodCallExpression newNode = null;

				if (evaluator.EnsureKnownType(
					node.Left.Type,
					replaceCompilerGeneratedType: _ => newNode = CompilerGenerated.Set(Visit(node.Left), Visit(node.Right))))
				{
					return newNode;
				}
			}

			return base.VisitBinary(node);
		}

		protected override Expression VisitBlock(BlockExpression node)
		{
			if (evaluator.EnsureKnownType(
				node.Type,
				genericArgumentsUpdated: updatedType => node = Expression.Block(
					updatedType,
					VisitAndConvert(node.Variables, "VisitBlock-Variables"),
					Visit(node.Expressions))))
			{
				return node;
			}
			else
			{
				return base.VisitBlock(node);
			}
		}

		protected override CatchBlock VisitCatchBlock(CatchBlock node)
		{
			if (evaluator.EnsureKnownType(
				node.Test,
				genericArgumentsUpdated: updatedType => node = Expression.MakeCatchBlock(
					updatedType,
					VisitAndConvert(node.Variable, "VisitCatchBlock-Variable"),
					Visit(node.Body),
					Visit(node.Filter))))
			{
				return node;
			}
			else
			{
				return base.VisitCatchBlock(node);
			}
		}

		protected override Expression VisitConditional(ConditionalExpression node)
		{
			if (evaluator.EnsureKnownType(
				node.Type,
				genericArgumentsUpdated: updatedType => node = Expression.Condition(
					Visit(node.Test),
					Visit(node.IfTrue),
					Visit(node.IfFalse),
					updatedType)))
			{
				return node;
			}
			else
			{
				return base.VisitConditional(node);
			}
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			var type = node.Value == null ? null : node.Value.GetType();

			if (evaluator.EnsureKnownType(
				type,
				replaceCompilerGeneratedType: _ =>
				{
					throw new InvalidOperationException(Errors.ExpressionClosureBug);
				}))
			{
				return node;
			}
			else
			{
				return base.VisitConstant(node);
			}
		}

		protected override Expression VisitDefault(DefaultExpression node)
		{
			if (evaluator.EnsureKnownType(
				node.Type,
				genericArgumentsUpdated: updatedType => node = Expression.Default(updatedType)))
			{
				return node;
			}
			else
			{
				return base.VisitDefault(node);
			}
		}

		protected override Expression VisitGoto(GotoExpression node)
		{
			evaluator.EnsureKnownType(node.Type);
			evaluator.EnsureKnownType(node.Target);

			return base.VisitGoto(node);
		}

		protected override Expression VisitIndex(IndexExpression node)
		{
			evaluator.EnsureKnownType(node.Indexer);

			return base.VisitIndex(node);
		}

		protected override Expression VisitInvocation(InvocationExpression node)
		{
			evaluator.EnsureKnownType(node.Type);

			return base.VisitInvocation(node);
		}

		protected override Expression VisitLabel(LabelExpression node)
		{
			evaluator.EnsureKnownType(node.Target);

			return base.VisitLabel(node);
		}

		protected override Expression VisitLambda<T>(Expression<T> node)
		{
			LambdaExpression newNode = null;

			var delegateType = node.Type;

			if (evaluator.EnsureKnownType(
				node.Type,
				replaceCompilerGeneratedType: _ =>
				{
					Type unboundReturnType;

					if (!delegateType.IsGenericType
						|| !(unboundReturnType = delegateType.GetGenericTypeDefinition().GetMethod("Invoke").ReturnType).IsGenericParameter)
					{
						throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Errors.ExpressionDelegateReturnsNonGenericAnonymousType, Environment.NewLine, delegateType));
					}

					var genericArguments = delegateType.GetGenericArguments().ToArray();

					genericArguments[unboundReturnType.GenericParameterPosition] = typeof(CompilerGenerated);

					delegateType = delegateType.GetGenericTypeDefinition().MakeGenericType(genericArguments);

					newNode = Expression.Lambda(
						delegateType,
						Visit(node.Body),
						node.Name,
						node.TailCall,
						VisitAndConvert(node.Parameters, "VisitLambda-Parameters"));
				},
				genericArgumentsUpdated: updatedType =>
					newNode = Expression.Lambda(
						updatedType,
						Visit(node.Body),
						node.Name,
						node.TailCall,
						VisitAndConvert(node.Parameters, "VisitLambda-Parameters"))))
			{
				return newNode;
			}
			else
			{
				return base.VisitLambda<T>(node);
			}
		}

		protected override Expression VisitListInit(ListInitExpression node)
		{
			evaluator.EnsureKnownTypes(node.Initializers.Select(i => i.AddMethod));

			return base.VisitListInit(node);
		}

		protected override Expression VisitLoop(LoopExpression node)
		{
			evaluator.EnsureKnownType(node.BreakLabel);
			evaluator.EnsureKnownType(node.ContinueLabel);

			return base.VisitLoop(node);
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			Expression newNode = null;

			if (evaluator.EnsureKnownType(
				node.Member,
				replaceCompilerGeneratedType: _ =>
				{
					newNode = evaluator.EvaluateCompilerGenerated(node, protocol)
								 ?? CompilerGenerated.Get(
											Visit(node.Expression),
											node.Member,
											type =>
											{
												evaluator.EnsureKnownType(
													type,
													replaceCompilerGeneratedType: __ => type = typeof(CompilerGenerated),
													genericArgumentsUpdated: updatedType => type = updatedType);

												return type;
											});
				},
				unknownType: (_, __) => newNode = evaluator.GetValue(node, this, protocol)))
			{
				return newNode;
			}
			else
			{
				return base.VisitMember(node);
			}
		}

		protected override Expression VisitMemberInit(MemberInitExpression node)
		{
			evaluator.EnsureKnownType(node.Type, replaceCompilerGeneratedType: _ => { });

			foreach (var binding in node.Bindings)
			{
				evaluator.EnsureKnownType(binding.Member);

				var list = binding as MemberListBinding;

				if (list != null)
				{
					evaluator.EnsureKnownTypes(list.Initializers.Select(i => i.AddMethod));
				}
			}

			return base.VisitMemberInit(node);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			Expression newNode = null;

			if (evaluator.EnsureKnownType(
				node.Method,
				unknownType: (_, __) => newNode = evaluator.Invoke(node, this, protocol),
				genericMethodArgumentsUpdated: updatedMethod => newNode = Expression.Call(Visit(node.Object), updatedMethod, Visit(node.Arguments))))
			{
				return newNode;
			}
			else
			{
				return base.VisitMethodCall(node);
			}
		}

		protected override Expression VisitNew(NewExpression node)
		{
			NewExpression newNode = null;

			if (evaluator.EnsureKnownType(
				node.Constructor,
				replaceCompilerGeneratedType: _ => newNode = CompilerGenerated.New(node.Members, Visit(node.Arguments)),
				genericArgumentsUpdated: updatedType =>
				{
					/* Overload resolution should be unaffected here, so keep the same constructor index.  We're just swapping a 
					 * compiler-generated type for the internal CompilerGenerated class, neither of which can be statically 
					 * referenced by users.
					 */
					var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

					var oldConstructorIndex = Array.IndexOf(node.Constructor.DeclaringType.GetConstructors(flags), node.Constructor);
					var newConstructor = updatedType.GetConstructors(flags)[oldConstructorIndex];

					newNode = Expression.New(newConstructor, Visit(node.Arguments), node.Members);
				}))
			{
				return newNode;
			}

			return base.VisitNew(node);
		}

		protected override Expression VisitNewArray(NewArrayExpression node)
		{
			NewArrayExpression newNode = null;

			if (evaluator.EnsureKnownType(
				node.Type,
				genericArgumentsUpdated: updatedType => newNode = Expression.NewArrayInit(updatedType, Visit(node.Expressions))))
			{
				return newNode;
			}
			else
			{
				return base.VisitNewArray(node);
			}
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			// TODO: The current caching implementation is weak.  It must support name scopes, instead of just globally keying by name.
			if (evaluator.ReplacedParameters.ContainsKey(node.Name))
			{
				return evaluator.ReplacedParameters[node.Name];
			}

			ParameterExpression newNode = null;

			if (evaluator.EnsureKnownType(
				node.Type,
				replaceCompilerGeneratedType: _ => newNode = Expression.Parameter(typeof(CompilerGenerated), node.Name),
				genericArgumentsUpdated: updatedType => newNode = Expression.Parameter(updatedType, node.Name)))
			{
				evaluator.ReplacedParameters.Add(newNode.Name, newNode);

				return newNode;
			}
			else
			{
				return base.VisitParameter(node);
			}
		}

		protected override Expression VisitSwitch(SwitchExpression node)
		{
			evaluator.EnsureKnownType(node.Comparison);

			SwitchExpression newNode = null;

			if (evaluator.EnsureKnownType(
				node.Type,
				genericArgumentsUpdated: updatedType => newNode = Expression.Switch(
					updatedType,
					Visit(node.SwitchValue),
					Visit(node.DefaultBody),
					node.Comparison,
					Visit(node.Cases, VisitSwitchCase))))
			{
				return newNode;
			}
			else
			{
				return base.VisitSwitch(node);
			}
		}

		protected override Expression VisitTry(TryExpression node)
		{
			TryExpression newNode = null;

			if (evaluator.EnsureKnownType(
				node.Type,
				genericArgumentsUpdated: updatedType => newNode = Expression.MakeTry(
					updatedType,
					Visit(node.Body),
					Visit(node.Finally),
					Visit(node.Fault),
					Visit(node.Handlers, VisitCatchBlock))))
			{
				return newNode;
			}
			else
			{
				return base.VisitTry(node);
			}
		}

		protected override Expression VisitTypeBinary(TypeBinaryExpression node)
		{
			TypeBinaryExpression newNode = null;

			if (evaluator.EnsureKnownType(
				node.TypeOperand,
				genericArgumentsUpdated: updatedType => Expression.TypeIs(Visit(node.Expression), updatedType)))
			{
				return newNode;
			}
			else
			{
				return base.VisitTypeBinary(node);
			}
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			evaluator.EnsureKnownType(node.Method);

			UnaryExpression newNode = null;

			if (node.NodeType != ExpressionType.Quote
				&& evaluator.EnsureKnownType(
					node.Type,
					genericArgumentsUpdated: updatedType => newNode = Expression.MakeUnary(
						node.NodeType,
						Visit(node.Operand),
						updatedType,
						node.Method)))
			{
				return newNode;
			}
			else
			{
				return base.VisitUnary(node);
			}
		}
	}
}