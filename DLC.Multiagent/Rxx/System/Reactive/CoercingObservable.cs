using System.Diagnostics.Contracts;

namespace System.Reactive
{
  internal class CoercingObservable<TSource, TTarget> : IObservable<TTarget>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    private readonly IObservable<TSource> source;
    #endregion

    #region Constructors
    public CoercingObservable(IObservable<TSource> source)
    {
      Contract.Requires(source != null);

      this.source = source;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(source != null);
    }

    public IDisposable Subscribe(IObserver<TTarget> observer)
    {
      return source.Subscribe(CreateObserver(observer));
    }

    protected virtual CoercingObserver<TSource, TTarget> CreateObserver(IObserver<TTarget> observer)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<CoercingObserver<TSource, TTarget>>() != null);

      return new CoercingObserver<TSource, TTarget>(observer);
    }
    #endregion
  }
}
