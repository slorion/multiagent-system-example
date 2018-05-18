using System.Diagnostics.Contracts;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    internal static IObservable<TSource> UsingHot<TSource, TResource>(TResource resource, Func<TResource, IObservable<TSource>> hotObservableFactory)
      where TResource : IDisposable
    {
      Contract.Requires(resource != null);
      Contract.Requires(hotObservableFactory != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      IObservable<TSource> observable;

      try
      {
        observable = hotObservableFactory(resource);
      }
      catch
      {
        resource.Dispose();
        throw;
      }

      Contract.Assume(observable != null);

      observable.Finally(resource.Dispose).Subscribe(
        _ => { },
        ex => { });		// Safe to ignore because the observable is hot; i.e., all observers receive the same error.

      return observable;
    }
  }
}