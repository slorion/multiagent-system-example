using System.Diagnostics.Contracts;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Returns the elements of the specified sequence as a sequence of <see cref="System.Maybe{T}"/>, 
    /// starting with <see cref="System.Maybe.Empty{T}()"/>.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable to be projected into <see cref="System.Maybe{T}"/> values.</param>
    /// <returns>A sequence m <see cref="System.Maybe{T}"/> values that contain the values from the specified
    /// observable, starting with <see cref="System.Maybe.Empty{T}()"/>.</returns>
    public static IObservable<Maybe<TSource>> Maybe<TSource>(this IObservable<TSource> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<Maybe<TSource>>>() != null);

      return Observable.Create<Maybe<TSource>>(
        observer =>
        {
          observer.OnNext(System.Maybe.Empty<TSource>());

          return source.Subscribe(
            value => observer.OnNext(System.Maybe.Return(value)),
            observer.OnError,
            observer.OnCompleted);
        });
    }
  }
}