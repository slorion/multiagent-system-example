using System.Diagnostics.Contracts;

namespace System.Windows.Input
{
  /// <summary>
  /// Represents a functional implementation of <see cref="ICommand"/>.
  /// </summary>
  public sealed class AnonymousCommand : ICommand
  {
    #region Public Properties
    /// <summary>
    /// Gets a value indicating whether the <see cref="CanExecuteChanged"/> event can be raised 
    /// by calling the <see cref="RaiseCanExecuteChanged"/> method.
    /// </summary>
    public bool CanRaiseCanExecuteChanged
    {
      get
      {
        Contract.Ensures(Contract.Result<bool>() == (canExecuteChanged != null));

        return canExecuteChanged != null;
      }
    }
    #endregion

    #region Private / Protected
    private readonly Func<object, bool> canExecute;
    private readonly Action<object> execute;
    private readonly Action<EventHandler> addCanExecuteChanged, removeCanExecuteChanged;
    private readonly ThreadSafeEvent canExecuteChanged;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="AnonymousCommand" /> class that always returns <see langword="true"/>
    /// from its <see cref="CanExecute"/> method.
    /// </summary>
    /// <param name="execute">An action that is invoked when <see cref="Execute"/> is called.</param>
    public AnonymousCommand(Action<object> execute)
      : this(_ => true, execute, _ => { }, _ => { })
    {
      Contract.Requires(execute != null);
      Contract.Ensures(!CanRaiseCanExecuteChanged);
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="AnonymousCommand" /> class.
    /// </summary>
    /// <param name="canExecute">A function that is invoked when <see cref="CanExecute"/> is called.</param>
    /// <param name="execute">An action that is invoked when <see cref="Execute"/> is called.</param>
    /// <param name="addCanExecuteChanged">An action that is called when an event handler is added to <see cref="CanExecuteChanged"/>.</param>
    /// <param name="removeCanExecuteChanged">An action that is called when an event handler is removed from <see cref="CanExecuteChanged"/>.</param>
    public AnonymousCommand(
      Func<object, bool> canExecute,
      Action<object> execute,
      Action<EventHandler> addCanExecuteChanged,
      Action<EventHandler> removeCanExecuteChanged)
    {
      Contract.Requires(canExecute != null);
      Contract.Requires(execute != null);
      Contract.Requires(addCanExecuteChanged != null);
      Contract.Requires(removeCanExecuteChanged != null);
      Contract.Ensures(!CanRaiseCanExecuteChanged);

      this.canExecute = canExecute;
      this.execute = execute;
      this.addCanExecuteChanged = addCanExecuteChanged;
      this.removeCanExecuteChanged = removeCanExecuteChanged;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="AnonymousCommand" /> class.
    /// </summary>
    /// <param name="canExecute">A function that is invoked when <see cref="CanExecute"/> is called.</param>
    /// <param name="execute">An action that is invoked when <see cref="Execute"/> is called.</param>
    /// <remarks>
    /// To raise the <see cref="CanExecuteChanged"/> event, call the <see cref="RaiseCanExecuteChanged"/> method.
    /// </remarks>
    public AnonymousCommand(
      Func<object, bool> canExecute,
      Action<object> execute)
    {
      Contract.Requires(canExecute != null);
      Contract.Requires(execute != null);
      Contract.Ensures(CanRaiseCanExecuteChanged);

      this.canExecute = canExecute;
      this.execute = execute;

      canExecuteChanged = new ThreadSafeEvent(this);
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(canExecute != null);
      Contract.Invariant(execute != null);
      Contract.Invariant(canExecuteChanged != null || addCanExecuteChanged != null);
      Contract.Invariant(canExecuteChanged != null || removeCanExecuteChanged != null);
    }

    /// <summary>
    /// Raises the <see cref="CanExecuteChanged"/> event.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
      Justification = "It anonymously raises an event.")]
    public void RaiseCanExecuteChanged()
    {
      Contract.Requires(CanRaiseCanExecuteChanged);

      canExecuteChanged.RaiseEvent();
    }

    /// <summary>
    /// Determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, 
    /// this object can be set to <see langword="null"/>.</param>
    /// <returns><see langword="True"/> if this command can be executed; otherwise, <see langword="false"/>.</returns>
    public bool CanExecute(object parameter)
    {
      return canExecute(parameter);
    }

    /// <summary>
    /// Invokes the command.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, 
    /// this object can be set to <see langword="null"/>.</param>
    public void Execute(object parameter)
    {
      execute(parameter);
    }
    #endregion

    #region Events
    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
      add
      {
        if (canExecuteChanged == null)
        {
          addCanExecuteChanged(value);
        }
        else
        {
          canExecuteChanged.Event += value;
        }
      }
      remove
      {
        if (canExecuteChanged == null)
        {
          removeCanExecuteChanged(value);
        }
        else
        {
          canExecuteChanged.Event -= value;
        }
      }
    }
    #endregion

    #region Nested
    /// <summary>
    /// Provides a thread-safe event that is generated by the compiler.
    /// </summary>
    private sealed class ThreadSafeEvent
    {
      #region Public Properties
      #endregion

      #region Private / Protected
      private readonly object sender;
      #endregion

      #region Constructors
      public ThreadSafeEvent(object sender)
      {
        this.sender = sender;
      }
      #endregion

      #region Methods
      public void RaiseEvent()
      {
        var handler = Event;

        if (handler != null)
        {
          handler(sender, EventArgs.Empty);
        }
      }
      #endregion

      #region Events
      public event EventHandler Event;
      #endregion
    }
    #endregion
  }
}