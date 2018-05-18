using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace System.Reactive
{
  internal class CoercingObserver<TSource, TTarget> : IObserver<TSource>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    private readonly IObserver<TTarget> target;
    #endregion

    #region Constructors
    public CoercingObserver(IObserver<TTarget> target)
    {
      Contract.Requires(target != null);

      this.target = target;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(target != null);
    }

    public void OnCompleted()
    {
      target.OnCompleted();
    }

    public void OnError(Exception error)
    {
      target.OnError(error);
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.SpacingRules", "SA1008:OpeningParenthesisMustBeSpacedCorrectly",
      Justification = "Double cast.")]
    public void OnNext(TSource value)
    {
      if (typeof(TTarget) == typeof(Unit))
      {
        target.OnNext((TTarget)(object)Unit.Default);
      }
      else
      {
        target.OnNext(Convert(value));
      }
    }

    [ContractVerification(false)]
    [SuppressMessage("Microsoft.StyleCop.CSharp.SpacingRules", "SA1008:OpeningParenthesisMustBeSpacedCorrectly",
      Justification = "Double cast.")]
    protected virtual TTarget Convert(TSource value)
    {
      return (TTarget)(object)value;
    }
    #endregion
  }
}
