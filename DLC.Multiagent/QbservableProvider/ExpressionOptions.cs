using System;

namespace QbservableProvider
{
	[Flags]
	public enum ExpressionOptions
	{
		AllowAll = 2 ^ 20 - 1,

		AllowBasicExpressions = 0,
		AllowAssignments = 1,
		AllowBlocks = 2,
		AllowCatchBlocks = 4 | AllowTryBlocks,
		AllowExtensions = 8,
		AllowGoto = 16,
		AllowDelegateInvoke = 32,
		AllowLoops = 64,
		AllowMemberAssignments = 128,
		AllowConstructors = 256,
		AllowArrayInstantiation = 512,
		AllowTryBlocks = 1024,
		AllowTypeTests = 2048,
		AllowExplicitConversions = 4096,
		AllowVoidMethodCalls = 8192
	}
}