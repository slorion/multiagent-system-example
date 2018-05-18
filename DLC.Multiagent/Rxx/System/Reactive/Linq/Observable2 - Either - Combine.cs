using System.Diagnostics.Contracts;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Combines the latest values from both notification channels and projects the results into a new sequence.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="source">The observable from which values are combined.</param>
    /// <param name="selector">Combines values from both notification channels.</param>
    /// <remarks>
    /// <see cref="Combine"/> is similar to <see cref="Observable.CombineLatest{TLeft,TRight,TResult}(IObservable{TLeft},IObservable{TRight},Func{TLeft,TRight,TResult})"/>.
    /// </remarks>
    /// <returns>An observable of results from the combination of the latest values in both notification channels.</returns>
    public static IObservable<TResult> Combine<TLeft, TRight, TResult>(
      this IObservable<Either<TLeft, TRight>> source,
      Func<TLeft, TRight, TResult> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      return source.Scan(
        Tuple.Create(System.Maybe.Empty<TLeft>(), System.Maybe.Empty<TRight>()),
        (acc, cur) =>
        {
          if (cur.IsLeft)
          {
            return Tuple.Create(System.Maybe.Return(cur.Left), acc.Item2);
          }
          else
          {
            return Tuple.Create(acc.Item1, System.Maybe.Return(cur.Right));
          }
        })
        .Where(tuple => tuple.Item1.HasValue && tuple.Item2.HasValue)
        .Select(tuple => selector(tuple.Item1.Value, tuple.Item2.Value));
    }
  }
}