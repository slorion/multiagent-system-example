﻿using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Retrieves the new elements of an observable sequence of collection notifications where <see cref="CollectionNotification{TSource}.Kind"/>
    /// is <see cref="CollectionNotificationKind.OnAdded"/> or <see cref="CollectionNotificationKind.OnReplaced"/>.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">An observable sequence of collection notifications from which to retrieve added and replacement items.</param>
    /// <returns>An observable sequence of items that have been added or that have replaced other items.</returns>
    public static IObservable<TSource> AddedOrReplacements<TSource>(this IObservable<CollectionNotification<TSource>> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return from n in source
             where n.Kind == CollectionNotificationKind.OnAdded
                || n.Kind == CollectionNotificationKind.OnReplaced
             select n.Value;
    }

    /// <summary>
    /// Retrieves the new elements of an observable sequence of collection notifications where <see cref="CollectionNotification{TSource}.Kind"/>
    /// is <see cref="CollectionNotificationKind.Exists"/>, <see cref="CollectionNotificationKind.OnAdded"/> or <see cref="CollectionNotificationKind.OnReplaced"/>.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">An observable sequence of collection notifications from which to retrieve existing, added and replacement items.</param>
    /// <returns>An observable sequence of items that already exist or have been added or that have replaced other items.</returns>
    public static IObservable<TSource> ExistingOrAddedOrReplacements<TSource>(this IObservable<CollectionNotification<TSource>> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return from n in source
             where n.Kind == CollectionNotificationKind.Exists
                || n.Kind == CollectionNotificationKind.OnAdded
                || n.Kind == CollectionNotificationKind.OnReplaced
             from value in n.HasValue ? Scalar.Return(n.Value) : n.ExistingValues
             select value;
    }

    /// <summary>
    /// Retrieves the existing elements of an observable sequence of collection notifications, where <see cref="CollectionNotification{TSource}.Kind"/>
    /// is <see cref="CollectionNotificationKind.Exists"/>.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">An observable sequence of collection notifications from which to retrieve existing items.</param>
    /// <returns>An observable sequence of items that already exist.</returns>
    public static IObservable<TSource> Existing<TSource>(this IObservable<CollectionNotification<TSource>> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return from n in source
             where n.Kind == CollectionNotificationKind.Exists
             from value in n.ExistingValues
             select value;
    }

    /// <summary>
    /// Retrieves the elements of an observable sequence of collection notifications where <see cref="CollectionNotification{TSource}.Kind"/>
    /// is <see cref="CollectionNotificationKind.OnRemoved"/> or <see cref="CollectionNotificationKind.OnReplaced"/>.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">An observable sequence of collection notifications from which to retrieve removed and replaced items.</param>
    /// <returns>An observable sequence of items that have been removed or replaced.</returns>
    public static IObservable<TSource> RemovedOrReplaced<TSource>(this IObservable<CollectionNotification<TSource>> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return from n in source
             where n.Kind == CollectionNotificationKind.OnReplaced
                || n.Kind == CollectionNotificationKind.OnRemoved
             select n.Kind == CollectionNotificationKind.OnReplaced
              ? n.ReplacedValue
              : n.Value;
    }

    /// <summary>
    /// Returns an observable sequence indicating when the specified sequence contains
    /// <see cref="CollectionNotificationKind.OnCleared"/> notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">An observable sequence of collection notifications to be examined.</param>
    /// <returns>An observable sequence indicating when the <paramref name="source"/> contains cleared notifications.</returns>
    public static IObservable<Unit> Cleared<TSource>(this IObservable<CollectionNotification<TSource>> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

      return from n in source
             where n.Kind == CollectionNotificationKind.OnCleared
             select Unit.Default;
    }

    /// <summary>
    /// Adds the elements from the specified observable sequence into a <see cref="ReadOnlyListSubject{T}"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The sequence from which elements are collected.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that receives the elements from the specified sequence.</returns>
    public static ReadOnlyListSubject<T> Collect<T>(this IObservable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<T>>() != null);

      var subscription = new SingleAssignmentDisposable();

      var list = new ListSubject<T>(subscription);

      subscription.Disposable = source.Subscribe(list.Add, list.OnError, list.OnCompleted);

      return new ReadOnlyListSubject<T>(list);
    }

    /// <summary>
    /// Adds the elements from the specified observable sequence into a <see cref="ReadOnlyListSubject{T}"/>, or removes
    /// individual elements or clears all elements, depending upon the <see cref="CollectionModificationKind"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The sequence from which collection modifications are received.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that receives the elements from the specified sequence.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "All disposables are composited by the subject that is returned to the caller.")]
    public static ReadOnlyListSubject<T> Collect<T>(this IObservable<CollectionModification<T>> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<T>>() != null);

      var subscription = new SingleAssignmentDisposable();

      var list = new ListSubject<T>(subscription);

      subscription.Disposable = source.Subscribe(list);

      return new ReadOnlyListSubject<T>(list);
    }

    /// <summary>
    /// Concurrently populates a <see cref="ReadOnlyListSubject{T}"/> with elements from the first sequence while also handling changes 
    /// from the second sequence of collection modifications, reconciling conflicts using the default equality comparer for 
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="existing">The sequence from which the <see cref="ReadOnlyListSubject{T}"/> is initially populated.</param>
    /// <param name="changes">The sequence from which collection notifications that modify the <see cref="ReadOnlyListSubject{T}"/> are received.</param>
    /// <include file='Observable2 - Collection and Dictionary.xml' path='//extension[@name="Collect"]/remarks'/>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that receives the elements from the specified sequences.</returns>
    public static ReadOnlyListSubject<T> Collect<T>(this IEnumerable<T> existing, IObservable<CollectionModification<T>> changes)
    {
      Contract.Requires(existing != null);
      Contract.Requires(changes != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<T>>() != null);

      return Collect(existing, changes, EqualityComparer<T>.Default);
    }

    /// <summary>
    /// Concurrently populates a <see cref="ReadOnlyListSubject{T}"/> with elements from the first sequence while also handling changes 
    /// from the second sequence of collection modifications, reconciling conflicts using the specified equality comparer for 
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="existing">The sequence from which the <see cref="ReadOnlyListSubject{T}"/> is initially populated.</param>
    /// <param name="changes">The sequence from which collection notifications that modify the <see cref="ReadOnlyListSubject{T}"/> are received.</param>
    /// <param name="comparer">The object that compares two instances of <typeparamref name="T"/> for equality and generates a hash code 
    /// that is suitable for use when keying a dictionary.</param>
    /// <include file='Observable2 - Collection and Dictionary.xml' path='//extension[@name="Collect"]/remarks'/>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that receives the elements from the specified sequences.</returns>
    public static ReadOnlyListSubject<T> Collect<T>(this IEnumerable<T> existing, IObservable<CollectionModification<T>> changes, IEqualityComparer<T> comparer)
    {
      Contract.Requires(existing != null);
      Contract.Requires(changes != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<T>>() != null);

      return Collect(existing.ToObservable(PlatformSchedulers.Concurrent), changes, comparer);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with elements from the first sequence while also handling changes 
    /// from the second sequence of collection modifications, reconciling conflicts using the default equality comparer for 
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="existing">The sequence from which the <see cref="ReadOnlyListSubject{T}"/> is initially populated.</param>
    /// <param name="changes">The sequence from which collection notifications that modify the <see cref="ReadOnlyListSubject{T}"/> are received.</param>
    /// <include file='Observable2 - Collection and Dictionary.xml' path='//extension[@name="Collect"]/remarks'/>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that receives the elements from the specified sequences.</returns>
    public static ReadOnlyListSubject<T> Collect<T>(this IObservable<T> existing, IObservable<CollectionModification<T>> changes)
    {
      Contract.Requires(existing != null);
      Contract.Requires(changes != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<T>>() != null);

      return Collect(existing, changes, EqualityComparer<T>.Default);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with elements from the first sequence while also handling changes 
    /// from the second sequence of collection modifications, reconciling conflicts using the specified equality comparer for 
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="existing">The sequence from which the <see cref="ReadOnlyListSubject{T}"/> is initially populated.</param>
    /// <param name="changes">The sequence from which collection notifications that modify the <see cref="ReadOnlyListSubject{T}"/> are received.</param>
    /// <param name="comparer">The object that compares two instances of <typeparamref name="T"/> for equality and generates a hash code 
    /// that is suitable for keying a dictionary.</param>
    /// <include file='Observable2 - Collection and Dictionary.xml' path='//extension[@name="Collect"]/remarks'/>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that receives the elements from the specified sequences.</returns>
    public static ReadOnlyListSubject<T> Collect<T>(this IObservable<T> existing, IObservable<CollectionModification<T>> changes, IEqualityComparer<T> comparer)
    {
      Contract.Requires(existing != null);
      Contract.Requires(changes != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<T>>() != null);

      return Collect(existing, changes, all => all.SelectMany(n => n.ToModifications()), comparer);
    }

    /// <summary>
    /// Concurrently populates a <see cref="ReadOnlyListSubject{T}"/> with elements from the first sequence while also handling changes 
    /// from the second sequence of collection modifications, reconciling conflicts using the default equality comparer for 
    /// <typeparamref name="TSource"/> and projecting notifications using the specified <paramref name="selector"/> function.
    /// </summary>
    /// <typeparam name="TSource">The type of objects in the source sequences.</typeparam>
    /// <typeparam name="TResult">The type of objects to which reconciled elements are projected.</typeparam>
    /// <param name="existing">The sequence from which the <see cref="ReadOnlyListSubject{T}"/> is initially populated.</param>
    /// <param name="changes">The sequence from which collection notifications that modify the <see cref="ReadOnlyListSubject{T}"/> are received.</param>
    /// <param name="selector">Projects a sequence of reconciled collection notifications, combining the <paramref name="existing"/> and 
    /// <paramref name="changes"/> sequences, into a sequence from which the list is populated.</param>
    /// <include file='Observable2 - Collection and Dictionary.xml' path='//extension[@name="Collect"]/remarks'/>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that receives the elements from the specified sequences.</returns>
    public static ReadOnlyListSubject<TResult> Collect<TSource, TResult>(
      this IEnumerable<TSource> existing,
      IObservable<CollectionModification<TSource>> changes,
      Func<IObservable<CollectionNotification<TSource>>, IObservable<CollectionModification<TResult>>> selector)
    {
      Contract.Requires(existing != null);
      Contract.Requires(changes != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return Collect(existing, changes, selector, EqualityComparer<TSource>.Default);
    }

    /// <summary>
    /// Concurrently populates a <see cref="ReadOnlyListSubject{T}"/> with elements from the first sequence while also handling changes 
    /// from the second sequence of collection modifications, reconciling conflicts using the specified equality comparer for 
    /// <typeparamref name="TSource"/> and projecting notifications using the specified <paramref name="selector"/> function.
    /// </summary>
    /// <typeparam name="TSource">The type of objects in the source sequences.</typeparam>
    /// <typeparam name="TResult">The type of objects to which reconciled elements are projected.</typeparam>
    /// <param name="existing">The sequence from which the <see cref="ReadOnlyListSubject{T}"/> is initially populated.</param>
    /// <param name="changes">The sequence from which collection notifications that modify the <see cref="ReadOnlyListSubject{T}"/> are received.</param>
    /// <param name="selector">Projects a sequence of reconciled collection notifications, combining the <paramref name="existing"/> and 
    /// <paramref name="changes"/> sequences, into a sequence from which the list is populated.</param>
    /// <param name="comparer">The object that compares two instances of <typeparamref name="TSource"/> for equality and generates a hash code 
    /// that is suitable for use when keying a dictionary.</param>
    /// <include file='Observable2 - Collection and Dictionary.xml' path='//extension[@name="Collect"]/remarks'/>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that receives the elements from the specified sequences.</returns>
    public static ReadOnlyListSubject<TResult> Collect<TSource, TResult>(
      this IEnumerable<TSource> existing,
      IObservable<CollectionModification<TSource>> changes,
      Func<IObservable<CollectionNotification<TSource>>, IObservable<CollectionModification<TResult>>> selector,
      IEqualityComparer<TSource> comparer)
    {
      Contract.Requires(existing != null);
      Contract.Requires(changes != null);
      Contract.Requires(selector != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return Collect(existing.ToObservable(PlatformSchedulers.Concurrent), changes, selector, comparer);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with elements from the first sequence while also handling changes 
    /// from the second sequence of collection modifications, reconciling conflicts using the default equality comparer for 
    /// <typeparamref name="TSource"/> and projecting notifications using the specified <paramref name="selector"/> function.
    /// </summary>
    /// <typeparam name="TSource">The type of objects in the source sequences.</typeparam>
    /// <typeparam name="TResult">The type of objects to which reconciled elements are projected.</typeparam>
    /// <param name="existing">The sequence from which the <see cref="ReadOnlyListSubject{T}"/> is initially populated.</param>
    /// <param name="changes">The sequence from which collection notifications that modify the <see cref="ReadOnlyListSubject{T}"/> are received.</param>
    /// <param name="selector">Projects a sequence of reconciled collection notifications, combining the <paramref name="existing"/> and 
    /// <paramref name="changes"/> sequences, into a sequence from which the list is populated.</param>
    /// <include file='Observable2 - Collection and Dictionary.xml' path='//extension[@name="Collect"]/remarks'/>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that receives the elements from the specified sequences.</returns>
    public static ReadOnlyListSubject<TResult> Collect<TSource, TResult>(
      this IObservable<TSource> existing,
      IObservable<CollectionModification<TSource>> changes,
      Func<IObservable<CollectionNotification<TSource>>, IObservable<CollectionModification<TResult>>> selector)
    {
      Contract.Requires(existing != null);
      Contract.Requires(changes != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return Collect(existing, changes, selector, EqualityComparer<TSource>.Default);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with elements from the first sequence while also handling changes 
    /// from the second sequence of collection modifications, reconciling conflicts using the specified equality comparer for 
    /// <typeparamref name="TSource"/> and projecting notifications using the specified <paramref name="selector"/> function.
    /// </summary>
    /// <typeparam name="TSource">The type of objects in the source sequences.</typeparam>
    /// <typeparam name="TResult">The type of objects to which reconciled elements are projected.</typeparam>
    /// <param name="existing">The sequence from which the <see cref="ReadOnlyListSubject{T}"/> is initially populated.</param>
    /// <param name="changes">The sequence from which collection notifications that modify the <see cref="ReadOnlyListSubject{T}"/> are received.</param>
    /// <param name="selector">Projects a sequence of reconciled collection notifications, combining the <paramref name="existing"/> and 
    /// <paramref name="changes"/> sequences, into a sequence from which the list is populated.</param>
    /// <param name="comparer">The object that compares two instances of <typeparamref name="TSource"/> for equality and generates a hash code 
    /// that is suitable for keying a dictionary.</param>
    /// <include file='Observable2 - Collection and Dictionary.xml' path='//extension[@name="Collect"]/remarks'/>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that receives the elements from the specified sequences.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Disposable is composited by the subject that is returned to the caller.")]
    public static ReadOnlyListSubject<TResult> Collect<TSource, TResult>(
      this IObservable<TSource> existing,
      IObservable<CollectionModification<TSource>> changes,
      Func<IObservable<CollectionNotification<TSource>>, IObservable<CollectionModification<TResult>>> selector,
      IEqualityComparer<TSource> comparer)
    {
      Contract.Requires(existing != null);
      Contract.Requires(changes != null);
      Contract.Requires(selector != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return new ReadOnlyListSubject<TResult>(
        Collect(
          d => new ListSubject<TResult>(d),
          existing,
          changes,
          k => k,
          k => k,
          (k, v) => CollectionNotification.CreateExists(v),
          selector,
          comparer));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "The closures are perhaps easier to maintain than the alternatives, and refactoring would require methods with several parameters or a context class.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposable is composited by the subject that is returned to the caller.")]
    internal static TSubject Collect<TSubject, TExisting, TKey, TNotify, TResult>(
      Func<IDisposable, TSubject> subjectFactory,
      IObservable<TExisting> existing,
      IObservable<CollectionModification<TNotify>> changes,
      Func<TExisting, TKey> existingKeySelector,
      Func<TNotify, TKey> changeKeySelector,
      Func<TKey, TExisting, CollectionNotification<TNotify>> existsNotificationFactory,
      Func<IObservable<CollectionNotification<TNotify>>, IObservable<CollectionModification<TResult>>> selector,
      IEqualityComparer<TKey> comparer)
      where TSubject : ISubject<CollectionModification<TResult>, CollectionNotification<TResult>>
    {
      Contract.Requires(subjectFactory != null);
      Contract.Requires(existing != null);
      Contract.Requires(changes != null);
      Contract.Requires(existingKeySelector != null);
      Contract.Requires(changeKeySelector != null);
      Contract.Requires(existsNotificationFactory != null);
      Contract.Requires(selector != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<TSubject>() != null);

      var disposables = new CompositeDisposable();

      var subject = subjectFactory(disposables);

      Contract.Assume(subject != null);

      var converterProxy = new Subject<CollectionNotification<TNotify>>();

      disposables.Add(converterProxy);

      var converted = selector(converterProxy.AsObservable());

      Contract.Assume(converted != null);

      disposables.Add(converted.Subscribe(subject));

      Action<Exception> onError = error =>
        {
          converterProxy.OnError(error);
          subject.OnError(error);
        };

      Action onCompleted = () =>
        {
          converterProxy.OnCompleted();
          subject.OnCompleted();
        };

      var gate = new object();

      var reconciliation = new Dictionary<TKey, bool>(comparer);
      var changesCompleted = false;

      /* The changes sequence has precedence over the existing sequence.  Changes must be subscribed first and it always indicates 
       * the actual known state of the collection.  The existing sequence is simply used to populate the collection with "existing" 
       * items, since the changes sequence doesn't provide that information.  If no changes occur, then the final collection will be 
       * the same as the existing sequence. It's also possible for the changes subscription to receive notifications for items that 
       * have not yet been observed by the existing subscription.  An OnRemoved notification is automatically dropped until the 
       * item has been added to the collection, either by the existing subscription or by an OnAdded notification from the changes 
       * sequence.  There are also a few possible race conditions; in any case, the changes sequence always wins.
       */
      disposables.Add(changes.Subscribe(
        change =>
        {
          lock (gate)
          {
            if (reconciliation == null)
            {
              foreach (var notification in change.ToNotifications())
              {
                converterProxy.OnNext(notification);
              }
            }
            else
            {
              TKey key;
              IList<TNotify> values;

              switch (change.Kind)
              {
                case CollectionModificationKind.Add:
                  values = change.Values;

                  for (int i = 0; i < values.Count; i++)
                  {
                    var value = values[i];

                    key = changeKeySelector(value);

                    /*
                     * The Removed case sets the item to false to distinguish between two race conditions.  The first race condition 
                     * is described in the comments for the Remove case.  The second race condition is one that might cause the existing 
                     * subscription to be notified about an item after it's created, but before the changes subscription is notified.
                     * For this race, since the item already exists in the reconciliation list the changes subscription will not push 
                     * it into the subject, which normally prevents duplicate notifications; although in this case, since the item's 
                     * value is false, the new OnAdded notification is valid and must be included.
                     */
                    if (!reconciliation.ContainsKey(key) || !reconciliation[key])
                    {
                      reconciliation[key] = true;

                      converterProxy.OnNext(CollectionNotification.CreateOnAdded<TNotify>(value));
                    }
                  }
                  break;
                case CollectionModificationKind.Remove:
                  values = change.Values;

                  for (int i = 0; i < values.Count; i++)
                  {
                    var value = values[i];

                    key = changeKeySelector(value);

                    bool remove = reconciliation.ContainsKey(key);

                    /* Even though the item has been removed it stil must be added to the reconciliation dictionary anyway.  Adding the 
                     * item avoids a race condition between the "changes" and "existing" observables that might cause the existing 
                     * subscription to be notified about an item after it has already been removed.  Since the item already exists in the 
                     * reconciliation list, even though its flag is set to false, the existing subscription will not push it into the subject.
                     * Reconciliation also avoids duplicate OnAdded notifications due to another race condition; although, in this case it 
                     * is meant to prevent an invalid OnAdded notification for an item that has already been removed.  Assigning the flag 
                     * to false, however, causes subsequent Add modifications to be accepted for items that have already been removed.
                     */
                    reconciliation[key] = false;

                    if (remove)
                    {
                      converterProxy.OnNext(CollectionNotification.CreateOnRemoved<TNotify>(value));
                    }
                  }
                  break;
                case CollectionModificationKind.Clear:
                  reconciliation = null;
                  converterProxy.OnNext(CollectionNotification.CreateOnCleared<TNotify>());
                  break;
              }
            }
          }
        },
        onError,
        () =>
        {
          bool completeNow;

          lock (gate)
          {
            changesCompleted = true;
            completeNow = reconciliation == null;
          }

          if (completeNow)
          {
            onCompleted();
          }
        }));

      disposables.Add(existing.Subscribe(
        value =>
        {
          lock (gate)
          {
            var key = existingKeySelector(value);

            if (reconciliation != null && !reconciliation.ContainsKey(key))
            {
              reconciliation.Add(key, true);

              converterProxy.OnNext(existsNotificationFactory(key, value));
            }
          }
        },
        onError,
        () =>
        {
          bool completeNow;

          lock (gate)
          {
            reconciliation = null;
            completeNow = changesCompleted;
          }

          if (completeNow)
          {
            onCompleted();
          }
        }));

      return subject;
    }
  }
}