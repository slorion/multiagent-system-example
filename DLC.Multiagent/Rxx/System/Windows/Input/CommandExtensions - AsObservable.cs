using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace System.Windows.Input
{
  public static partial class CommandExtensions
  {
    /* This dictionary is required to support memory-efficient subscriptions to class command bindings and the
     * unsubscribing of observers because CommandManager.RegisterClassCommandBinding does not support unregistration.
     */
    private static readonly Dictionary<TypeCommandPair, Subject<Either<EventPattern<CanExecuteRoutedEventArgs>, EventPattern<ExecutedRoutedEventArgs>>>> classBindings =
      new Dictionary<TypeCommandPair, Subject<Either<EventPattern<CanExecuteRoutedEventArgs>, EventPattern<ExecutedRoutedEventArgs>>>>();

    /// <summary>
    /// Registers a class command binding for the specified <paramref name="type"/> and returns an observable sequence with two notification channels, 
    /// with the right channel receiving notifications when the <paramref name="command"/> is executed and the left channel 
    /// receiving notifications when the <paramref name="command"/> is queried as to whether it can be executed.
    /// </summary>
    /// <param name="command">The <see cref="ICommand"/> from which notifications are received.</param>
    /// <param name="type">The <see cref="Type"/> whose instances query or execute the specified <paramref name="command"/> from a visual UI tree.</param>
    /// <returns>An observable sequence with two notification channels, with the right channel receiving notifications when the <paramref name="command"/> is executed and 
    /// the left channel receiving notifications when the <paramref name="command"/> is queried as to whether it can be executed.</returns>
    public static IObservable<Either<EventPattern<CanExecuteRoutedEventArgs>, EventPattern<ExecutedRoutedEventArgs>>> AsObservable(
      this ICommand command,
      Type type)
    {
      Contract.Requires(command != null);
      Contract.Requires(type != null);
      Contract.Ensures(Contract.Result<IObservable<Either<EventPattern<CanExecuteRoutedEventArgs>, EventPattern<ExecutedRoutedEventArgs>>>>() != null);

      lock (classBindings)
      {
        var key = new TypeCommandPair(command, type);

        Subject<Either<EventPattern<CanExecuteRoutedEventArgs>, EventPattern<ExecutedRoutedEventArgs>>> subject;

        if (classBindings.ContainsKey(key))
        {
          subject = classBindings[key];

          Contract.Assume(subject != null);
        }
        else
        {
          subject = new Subject<Either<EventPattern<CanExecuteRoutedEventArgs>, EventPattern<ExecutedRoutedEventArgs>>>();

          CommandManager.RegisterClassCommandBinding(
            type,
            new CommandBinding(
              command,
              (sender, e) =>
              {
                subject.OnNext(Either.Right<EventPattern<CanExecuteRoutedEventArgs>, EventPattern<ExecutedRoutedEventArgs>>(
                  new EventPattern<ExecutedRoutedEventArgs>(sender, e)));
              },
              (sender, e) =>
              {
                subject.OnNext(Either.Left<EventPattern<CanExecuteRoutedEventArgs>, EventPattern<ExecutedRoutedEventArgs>>(
                  new EventPattern<CanExecuteRoutedEventArgs>(sender, e)));
              }));

          classBindings.Add(key, subject);
        }

        return subject.AsObservable();
      }
    }

    /// <summary>
    /// Adds a command binding to the specified <paramref name="element"/> and returns an observable sequence with two notification channels,
    /// with the right channel receiving notifications when the <paramref name="command"/> is executed and the left channel 
    /// receiving notifications when the <paramref name="command"/> is queried as to whether it can be executed.
    /// </summary>
    /// <param name="command">The <see cref="ICommand"/> from which notifications are received.</param>
    /// <param name="element">The <see cref="UIElement"/> that queries or executes the specified <paramref name="command"/>.</param>
    /// <returns>An observable sequence with two notification channels, with the right channel receiving notifications when the <paramref name="command"/> is executed and 
    /// the left channel receiving notifications when the <paramref name="command"/> is queried as to whether it can be executed.</returns>
    public static IObservable<Either<EventPattern<CanExecuteRoutedEventArgs>, EventPattern<ExecutedRoutedEventArgs>>> AsObservable(
      this ICommand command,
      UIElement element)
    {
      Contract.Requires(command != null);
      Contract.Requires(element != null);
      Contract.Ensures(Contract.Result<IObservable<Either<EventPattern<CanExecuteRoutedEventArgs>, EventPattern<ExecutedRoutedEventArgs>>>>() != null);

      return Observable2.CreateEither<EventPattern<CanExecuteRoutedEventArgs>, EventPattern<ExecutedRoutedEventArgs>>(
        observer =>
        {
          var binding = new CommandBinding(
            command,
            (sender, e) => observer.OnNextRight(new EventPattern<ExecutedRoutedEventArgs>(sender, e)),
            (sender, e) => observer.OnNextLeft(new EventPattern<CanExecuteRoutedEventArgs>(sender, e)));

          element.CommandBindings.Add(binding);

          return () => element.CommandBindings.Remove(binding);
        });
    }
  }
}