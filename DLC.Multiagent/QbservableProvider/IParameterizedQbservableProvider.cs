using System.Linq.Expressions;
using System.Reactive.Linq;

namespace QbservableProvider
{
	public interface IParameterizedQbservableProvider : IQbservableProvider
	{
		IQbservable<TResult> CreateQuery<TResult>(Expression expression, object argument);
	}
}