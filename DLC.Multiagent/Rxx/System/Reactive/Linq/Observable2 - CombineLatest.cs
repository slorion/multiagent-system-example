using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Disposables;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Merges two or more observable sequences into one observable sequence of lists, each containing the latest values from the latest consecutive 
    /// observable sequences whenever one of the sequences produces an element.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="sources">An observable sequence containing two or more observable sequences to be merged.</param>
    /// <remarks>
    /// <para>
    /// At least two consecutive observable sequences from the beginning of the outer observable sequence must produce at least one element; otherwise, 
    /// the generated sequence will be empty.  Furthermore, if an observable sequence produces more than one element before each of the consecutive observable 
    /// sequences have produced their first elements, then all of the older elements are discarded and will not be included in the generated sequence.
    /// Only the latest elements from each of the consecutive sequences are included.  As new sequences arrive, the size of the generated lists are increased
    /// to accommodate them if they start producing values.
    /// </para>
    /// <para>
    /// The latest value of an observable sequence is always located in the generated lists at the same index in which that sequence is located in the outer sequence.
    /// For example, the values from the first observable sequence in the outer sequence will always be at index zero (0) in the lists that are generated.
    /// Furthermore, once a generated list contains the value for a particular observable sequence, all subsequent lists will also contain the latest value for that 
    /// sequence.  As a result, the number of items in the generated lists may stay the same or grow when new observable sequences arrive, but the size of the lists 
    /// will never shrink.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence containing the result of combining the latest elements of all sources into lists.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is composited by the subscription.")]
    public static IObservable<IList<TSource>> CombineLatest<TSource>(this IObservable<IObservable<TSource>> sources)
    {
      Contract.Requires(sources != null);
      Contract.Ensures(Contract.Result<IObservable<IList<TSource>>>() != null);

      return Observable.Create<IList<TSource>>(
        observer =>
        {
          var gate = new object();

          var latest = new List<TSource>();
          var hasValueFlags = new List<bool>();

          var sourceCount = 0;
          var consecutiveActiveSourcesCount = 0;
          var outerCompleted = false;

          var outerSubscription = new SingleAssignmentDisposable();
          var disposables = new CompositeDisposable(outerSubscription);

          outerSubscription.Disposable = sources.Subscribe(
            source =>
            {
              int index;

              lock (gate)
              {
                sourceCount++;

                index = latest.Count;

                latest.Add(default(TSource));
                hasValueFlags.Add(false);
              }

              var subscription = new SingleAssignmentDisposable();

              disposables.Add(subscription);

              subscription.Disposable = source.Subscribe(
                value =>
                {
                  lock (gate)
                  {
                    latest[index] = value;

                    if (consecutiveActiveSourcesCount < hasValueFlags.Count)
                    {
                      hasValueFlags[index] = true;

                      while (consecutiveActiveSourcesCount < hasValueFlags.Count && hasValueFlags[consecutiveActiveSourcesCount])
                      {
                        consecutiveActiveSourcesCount++;
                      }
                    }

                    if (consecutiveActiveSourcesCount >= 2)
                    {
                      observer.OnNext(latest.Take(consecutiveActiveSourcesCount).ToList().AsReadOnly());
                    }
                  }
                },
                observer.OnError,
                () =>
                {
                  bool completeNow;

                  lock (gate)
                  {
                    disposables.Remove(subscription);

                    sourceCount--;

                    completeNow = outerCompleted && sourceCount == 0;
                  }

                  if (completeNow)
                  {
                    observer.OnCompleted();
                  }
                });
            },
            observer.OnError,
            () =>
            {
              bool completeNow;

              lock (gate)
              {
                outerCompleted = true;

                completeNow = sourceCount == 0;
              }

              if (completeNow)
              {
                observer.OnCompleted();
              }
            });

          return disposables;
        });
    }
  }
}