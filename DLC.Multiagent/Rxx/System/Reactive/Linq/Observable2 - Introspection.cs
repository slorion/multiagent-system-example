using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Pairs the specified observable sequence with an observable for each value that indicates 
    /// the duration of the observation of that value.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable sequence to introspect.</param>
    /// <returns>An observable sequence with two notification channels containing introspection windows in the left 
    /// channel and the values from the specified observable in the right.</returns>
    public static IObservable<Either<IObservable<TSource>, TSource>> Introspect<TSource>(
      this IObservable<TSource> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<Either<IObservable<TSource>, TSource>>>() != null);

      return Introspect(source, Scheduler.Immediate);
    }

    /// <summary>
    /// Pairs the specified observable sequence with an observable for each value that indicates 
    /// the duration of the observation of that value.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable sequence to introspect.</param>
    /// <param name="scheduler">Schedules the observations of values in the right notification channel.</param>
    /// <returns>An observable sequence with two notification channels containing introspection windows in the left 
    /// channel and the values from the specified observable in the right.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The introspection subject does not need to be disposed.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "All exceptions are sent to observers.")]
    public static IObservable<Either<IObservable<TSource>, TSource>> Introspect<TSource>(
      this IObservable<TSource> source,
      IScheduler scheduler)
    {
      Contract.Requires(source != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<Either<IObservable<TSource>, TSource>>>() != null);

      return Observable2.CreateEither<IObservable<TSource>, TSource>(
        observer =>
        {
          var subject = new Subject<Tuple<TSource, ISubject<TSource>>>();

          var observations = Subject.Synchronize(subject, scheduler);

          int pendingOnNext = 0;
          bool sourceCompleted = false;
          object gate = new object();

          var observationsSubscription = observations.Subscribe(
            next =>
            {
              var value = next.Item1;
              var introspection = next.Item2;

              try
              {
                lock (gate)
                {
                  observer.OnNextRight(value);
                }
              }
              catch (Exception ex)
              {
                introspection.OnError(ex);
                return;
              }

              introspection.OnCompleted();

              lock (gate)
              {
                if (--pendingOnNext == 0 && sourceCompleted)
                {
                  observer.OnCompleted();
                }
              }
            },
            ex =>
            {
              lock (gate)
              {
                observer.OnError(ex);
              }
            },
            () =>
            {
              lock (gate)
              {
                observer.OnCompleted();
              }
            });

          var sourceSubscription = source.Subscribe(
            value =>
            {
              var introspection = new ReplaySubject<TSource>(1);

              lock (gate)
              {
                observer.OnNextLeft(introspection.AsObservable());

                pendingOnNext++;
              }

              introspection.OnNext(value);

              observations.OnNext(Tuple.Create(value, (ISubject<TSource>)introspection));
            },
            observations.OnError,
            () =>
            {
              bool completeNow = false;

              lock (gate)
              {
                sourceCompleted = true;
                completeNow = pendingOnNext == 0;
              }

              if (completeNow)
              {
                observations.OnCompleted();
              }
            });

          return new CompositeDisposable(sourceSubscription, observationsSubscription, subject);
        });
    }

    /// <summary>
    /// Generates a sequence of windows where each window contains all values that were observed from 
    /// the <paramref name="source"/> while the values in the previous window were being observed.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable sequence from which to create introspection windows.</param>
    /// <returns>The source observable sequence buffered into introspection windows.</returns>
    public static IObservable<IObservable<TSource>> WindowIntrospective<TSource>(
      this IObservable<TSource> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<IObservable<TSource>>>() != null);

      return WindowIntrospective(source, PlatformSchedulers.Concurrent);
    }

    /// <summary>
    /// Generates a sequence of windows where each window contains all values that were observed from 
    /// the <paramref name="source"/> while the values in the previous window were being observed.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable sequence from which to create introspection windows.</param>
    /// <param name="scheduler">Schedules when windows are observed as well as the values in each window.</param>
    /// <returns>The source observable sequence buffered into introspection windows.</returns>
    public static IObservable<IObservable<TSource>> WindowIntrospective<TSource>(
      this IObservable<TSource> source,
      IScheduler scheduler)
    {
      Contract.Requires(source != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<IObservable<TSource>>>() != null);

      return Observable.Create<IObservable<TSource>>(
        observer =>
        {
          var queue = new Queue<TSource>();
          var window = new Subject<TSource>();

          bool pendingDrain = false;
          bool sourceCompleted = false;
          object gate = new object();

          var sourceSubscription = new SingleAssignmentDisposable();
          var drainSchedule = new SerialDisposable();
          var schedules = new CompositeDisposable(drainSchedule);

          Action ensureDraining = () =>
            {
              if (pendingDrain)
              {
                return;
              }

              pendingDrain = true;

              drainSchedule.Disposable =
                scheduler.Schedule(self =>
                {
                  Queue<TSource> currentQueue;

                  lock (gate)
                  {
                    currentQueue = queue;
                    queue = new Queue<TSource>();
                  }

                  currentQueue.ForEach(window.OnNext);

                  window.OnCompleted();

                  Contract.Assume(pendingDrain);

                  bool loop, completeNow;

                  lock (gate)
                  {
                    pendingDrain = queue.Count > 0;
                    completeNow = !pendingDrain && sourceCompleted;

                    if (completeNow)
                    {
                      loop = false;
                    }
                    else
                    {
                      window = new Subject<TSource>();

                      // Must push the new window before unlocking the gate to avoid a race condition when pendingDrain is false.
                      observer.OnNext(window.AsObservable());

                      // Ensure pendingDrain is read again after making a call to OnNext, in case of re-entry.
                      loop = pendingDrain;
                    }
                  }

                  if (completeNow)
                  {
                    observer.OnCompleted();
                  }
                  else if (loop)
                  {
                    Contract.Assert(pendingDrain);

                    self();
                  }
                });
            };

          schedules.Add(
            scheduler.Schedule(() =>
              {
                observer.OnNext(window.AsObservable());

                sourceSubscription.Disposable = source.Subscribe(
                  value =>
                  {
                    lock (gate)
                    {
                      queue.Enqueue(value);

                      ensureDraining();
                    }
                  },
                  ex => schedules.Add(scheduler.Schedule(() => observer.OnError(ex))),
                  () =>
                  {
                    bool completeNow = false;

                    lock (gate)
                    {
                      sourceCompleted = true;
                      completeNow = !pendingDrain;
                    }

                    if (completeNow)
                    {
                      schedules.Add(scheduler.Schedule(() =>
                        {
                          window.OnCompleted();

                          observer.OnCompleted();
                        }));
                    }
                  });
              }));

          return new CompositeDisposable(sourceSubscription, drainSchedule, schedules);
        });
    }

    /// <summary>
    /// Generates a sequence of lists where each list contains all values that were observed from 
    /// the <paramref name="source"/> while the previous list was being observed.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable sequence from which to create introspection lists.</param>
    /// <returns>The source observable sequence buffered into introspection lists.</returns>
    public static IObservable<IList<TSource>> BufferIntrospective<TSource>(
      this IObservable<TSource> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<IList<TSource>>>() != null);

      return from window in source.WindowIntrospective()
             from list in window.ToList()
             select list;
    }

    /// <summary>
    /// Generates a sequence of lists where each list contains all values that were observed from 
    /// the <paramref name="source"/> while the previous list was being observed.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable sequence from which to create introspection lists.</param>
    /// <param name="scheduler">Schedules when lists are observed.</param>
    /// <returns>The source observable sequence buffered into introspection lists.</returns>
    public static IObservable<IList<TSource>> BufferIntrospective<TSource>(
      this IObservable<TSource> source,
      IScheduler scheduler)
    {
      Contract.Requires(source != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<IList<TSource>>>() != null);

      return from window in source.WindowIntrospective(scheduler)
             from list in window.ToList()
             select list;
    }

    /// <summary>
    /// Samples the last value in the sequence while the previous value was being observed.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable sequence from which to sample introspectively.</param>
    /// <returns>An observable sequence of sampled values.</returns>
    public static IObservable<TSource> SampleIntrospective<TSource>(
      this IObservable<TSource> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return SampleIntrospective(source, PlatformSchedulers.Concurrent);
    }

    /// <summary>
    /// Samples the last value in the sequence while the previous value was being observed.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable sequence from which to sample introspectively.</param>
    /// <param name="scheduler">Schedules when samples are observed.</param>
    /// <returns>An observable sequence of sampled values.</returns>
    public static IObservable<TSource> SampleIntrospective<TSource>(
      this IObservable<TSource> source,
      IScheduler scheduler)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return Observable.Defer(() =>
        {
          var completed = false;

          return source
            .Do(_ => { }, () => completed = true)
            .WindowIntrospective(scheduler)
            .TakeWhile(_ => !completed)
            .SelectMany(window => window.TakeLast(1));
        });
    }
  }
}