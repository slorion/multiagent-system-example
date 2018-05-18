using System.Diagnostics.Contracts;
using System.Reactive;

namespace System.Windows.Input
{
  public static partial class CommandExtensions
  {
    /// <summary>
    /// Notifies the specified action when any instance of the specified <paramref name="type"/> in a visual UI tree executes the <paramref name="command"/>.
    /// </summary>
    /// <param name="command">The <see cref="ICommand"/> from which notifications are received.</param>
    /// <param name="type">The <see cref="Type"/> whose instances execute the specified <paramref name="command"/> from a visual UI tree.</param>
    /// <param name="onExecuted">The action that is executed when the <paramref name="command"/> is executed.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe(
      this ICommand command,
      Type type,
      Action<EventPattern<ExecutedRoutedEventArgs>> onExecuted)
    {
      Contract.Requires(command != null);
      Contract.Requires(type != null);
      Contract.Requires(onExecuted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return command.Subscribe(type, onExecuted, e => e.EventArgs.CanExecute = true);
    }

    /// <summary>
    /// Notifies the specified actions when any instance of the specified <paramref name="type"/> in a visual UI tree queries or executes the <paramref name="command"/>.
    /// </summary>
    /// <param name="command">The <see cref="ICommand"/> from which notifications are received.</param>
    /// <param name="type">The <see cref="Type"/> whose instances query or execute the specified <paramref name="command"/> from a visual UI tree.</param>
    /// <param name="onExecuted">The action that is executed when the <paramref name="command"/> is executed.</param>
    /// <param name="onCanExecute">The action that is executed when the <paramref name="command"/> is queried as to whether it can be executed.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe(
      this ICommand command,
      Type type,
      Action<EventPattern<ExecutedRoutedEventArgs>> onExecuted,
      Action<EventPattern<CanExecuteRoutedEventArgs>> onCanExecute)
    {
      Contract.Requires(command != null);
      Contract.Requires(type != null);
      Contract.Requires(onExecuted != null);
      Contract.Requires(onCanExecute != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return command.AsObservable(type).Subscribe(Observer2.CreateEither(onCanExecute, onExecuted));
    }

    /// <summary>
    /// Notifies the specified <paramref name="observer"/> when any instance of the specified <paramref name="type"/> in a visual UI tree 
    /// queries or executes the <paramref name="command"/>.
    /// </summary>
    /// <param name="command">The <see cref="ICommand"/> from which notifications are received.</param>
    /// <param name="type">The <see cref="Type"/> whose instances query or execute the specified <paramref name="command"/> from a visual UI tree.</param>
    /// <param name="observer">The object that is to receive notifications.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe(
      this ICommand command,
      Type type,
      IObserver<Either<EventPattern<CanExecuteRoutedEventArgs>, EventPattern<ExecutedRoutedEventArgs>>> observer)
    {
      Contract.Requires(command != null);
      Contract.Requires(type != null);
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return command.AsObservable(type).Subscribe(observer);
    }

    /// <summary>
    /// Notifies the specified action when the specified <paramref name="element"/> executes the <paramref name="command"/>.
    /// </summary>
    /// <param name="command">The <see cref="ICommand"/> from which notifications are received.</param>
    /// <param name="element">The <see cref="UIElement"/> that executes the specified <paramref name="command"/>.</param>
    /// <param name="onExecuted">The action that is executed when the <paramref name="command"/> is executed.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe(
      this ICommand command,
      UIElement element,
      Action<EventPattern<ExecutedRoutedEventArgs>> onExecuted)
    {
      Contract.Requires(command != null);
      Contract.Requires(element != null);
      Contract.Requires(onExecuted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return command.Subscribe(element, onExecuted, e => e.EventArgs.CanExecute = true);
    }

    /// <summary>
    /// Notifies the specified actions when the specified <paramref name="element"/> queries or executes the <paramref name="command"/>.
    /// </summary>
    /// <param name="command">The <see cref="ICommand"/> from which notifications are received.</param>
    /// <param name="element">The <see cref="UIElement"/> that queries or executes the specified <paramref name="command"/>.</param>
    /// <param name="onExecuted">The action that is executed when the <paramref name="command"/> is executed.</param>
    /// <param name="onCanExecute">The action that is executed when the <paramref name="element"/> queries whether the <paramref name="command"/> can be executed.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe(
      this ICommand command,
      UIElement element,
      Action<EventPattern<ExecutedRoutedEventArgs>> onExecuted,
      Action<EventPattern<CanExecuteRoutedEventArgs>> onCanExecute)
    {
      Contract.Requires(command != null);
      Contract.Requires(element != null);
      Contract.Requires(onExecuted != null);
      Contract.Requires(onCanExecute != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return command.AsObservable(element).Subscribe(Observer2.CreateEither(onCanExecute, onExecuted));
    }

    /// <summary>
    /// Notifies the specified <paramref name="observer"/> when the specified <paramref name="element"/> queries or executes the <paramref name="command"/>.
    /// </summary>
    /// <param name="command">The <see cref="ICommand"/> from which notifications are received.</param>
    /// <param name="element">The <see cref="UIElement"/> that queries or executes the specified <paramref name="command"/>.</param>
    /// <param name="observer">The object that is to receive notifications.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe(
      this ICommand command,
      UIElement element,
      IObserver<Either<EventPattern<CanExecuteRoutedEventArgs>, EventPattern<ExecutedRoutedEventArgs>>> observer)
    {
      Contract.Requires(command != null);
      Contract.Requires(element != null);
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return command.AsObservable(element).Subscribe(observer);
    }
  }
}