using System.Diagnostics.Contracts;
using System.Threading;
using System.Windows.Input;

namespace System.Reactive.Subjects
{
  /// <summary>
  /// Represents an object that is an <see cref="ICommand"/>, an observable sequence of execution parameters and 
  /// an observer of values indicating whether the command can be executed.
  /// </summary>
  /// <threadsafety instance="true" />
  public sealed class CommandSubject : ICommand, ISubject<bool, object>, IDisposable
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    private readonly Subject<object> subject = new Subject<object>();
    private readonly Func<object, bool> canExecute;
    private bool? canExecuteAnyParameter;
    private Exception exception;
    private int stopped;
    private bool disposed;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="CommandSubject" /> class that observes notifications indicating 
    /// whether the command can be executed.  By default, <see cref="CanExecute"/> returns <see langord="true"/> until 
    /// a notification is observed that sets it to <see langword="false"/>.
    /// </summary>
    public CommandSubject()
    {
      canExecuteAnyParameter = true;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="CommandSubject" /> class that observes notifications indicating 
    /// whether the command can be executed.
    /// </summary>
    /// <param name="defaultCanExecute">The default value to be returned by the <see cref="CanExecute"/> method.</param>
    public CommandSubject(bool defaultCanExecute)
    {
      canExecuteAnyParameter = defaultCanExecute;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="CommandSubject" /> class that observes notifications indicating 
    /// whether the specified function should be invoked to determine whether the command can be executed.
    /// </summary>
    /// <remarks>
    /// The actual values of the notifications are ignored.  Essentially, they are treated like <see cref="System.Reactive.Unit"/>.
    /// </remarks>
    /// <param name="canExecute">A function that receives arbitrary data or <see langword="null"/> and returns whether 
    /// the command can be executed for that data.</param>
    public CommandSubject(Func<object, bool> canExecute)
    {
      Contract.Requires(canExecute != null);

      this.canExecute = canExecute;

      Contract.Assume(!canExecuteAnyParameter.HasValue);
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(subject != null);
      Contract.Invariant(canExecuteAnyParameter.HasValue == (canExecute == null));
    }

    /// <summary>
    /// Returns an <see cref="ICommand"/> wrapper for this subject that prevents callers from using it as a subject.
    /// </summary>
    /// <returns>An <see cref="ICommand"/> wrapper for this subject.</returns>
    public ICommand AsCommand()
    {
      Contract.Ensures(Contract.Result<ICommand>() != null);

      return new AnonymousCommand(CanExecute, Execute, h => CanExecuteChanged += h, h => CanExecuteChanged -= h);
    }

    /// <summary>
    /// Determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object 
    /// can be set to <see langword="null"/>.</param>
    /// <returns><see langword="True"/> if this command can be executed; otherwise, <see langword="false" />.</returns>
    public bool CanExecute(object parameter)
    {
      return disposed || stopped == 1
        ? false
        : canExecute == null
          ? canExecuteAnyParameter.Value
          : canExecute(parameter);
    }

    /// <summary>
    /// Invokes the command.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object 
    /// can be set to <see langword="null"/>.</param>
    /// <exception cref="Exception">Throws the exception that was passed to <see cref="OnError"/>.</exception>
    public void Execute(object parameter)
    {
      CheckDisposed();

      if (exception != null)
      {
        throw exception;
      }

      subject.OnNext(parameter);
    }

    /// <summary>
    /// Notifies the subject that an observer is to receive notifications when <see cref="Execute"/> is called.
    /// </summary>
    /// <param name="observer">The object that is to receive notifications.</param>
    /// <returns>The observer's interface that enables resources to be disposed.</returns>
    public IDisposable Subscribe(IObserver<object> observer)
    {
      CheckDisposed();

      return subject.Subscribe(observer);
    }

    /// <summary>
    /// Notifies the command to reevaluate whether it can execute.
    /// </summary>
    /// <remarks>
    /// The behavior of <see cref="OnNext"/> differs based on how the <see cref="CommandSubject"/> was constructed.
    /// The <see cref="CommandSubject(Func{object,bool})"/> constructor causes <see cref="OnNext"/> to do nothing but
    /// raise the <see cref="CanExecuteChanged"/> event, regardless of the specified <paramref name="value"/>.
    /// All other constructors cause <see cref="OnNext"/> to set the return value of <see cref="CanExecute"/> to the
    /// specified <paramref name="value"/> and then it raises the <see cref="CanExecuteChanged"/> event, but only if the
    /// value has actually changed.
    /// </remarks>
    /// <param name="value">If the <see cref="CommandSubject"/> was not created by the <see cref="CommandSubject(Func{object,bool})"/>
    /// constructor, then <see langword="true"/> if the command can be executed or <see langword="false"/> if it cannot; 
    /// otherwise, this value is ignored.</param>
    public void OnNext(bool value)
    {
      CheckDisposed();

      if (canExecute == null)
      {
        Contract.Assert(canExecuteAnyParameter.HasValue);

        if (canExecuteAnyParameter.Value == value)
        {
          return;
        }

        canExecuteAnyParameter = value;
      }

      /* The race condition is acceptable because it's still present after CanExecuteChanged is invoked.
       * In other words, this class does not guarantee that the object isn't already stopped when the 
       * event's handler attempts to read CanExecute, which also cannot guarantee that its return value 
       * accurately reflects the state of the command, even immediately after it's returned.
       */
      if (stopped == 0)
      {
        OnCanExecuteChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Notifies the subject that the provider has experienced an error condition.
    /// </summary>
    /// <param name="error">An object that provides additional information about the error.</param>
    public void OnError(Exception error)
    {
      if (Interlocked.CompareExchange(ref stopped, 1, 0) == 0)
      {
        this.exception = error;

        subject.OnError(error);
      }

      Contract.Assume(stopped == 1);
    }

    /// <summary>
    /// Notifies the observer that the provider has finished sending push-based notifications.
    /// </summary>
    public void OnCompleted()
    {
      if (Interlocked.CompareExchange(ref stopped, 1, 0) == 0)
      {
        subject.OnCompleted();
      }

      Contract.Assume(stopped == 1);
    }

    private void CheckDisposed()
    {
      /* Race conditions are acceptable here since they are unavoidable anyway.  Also, the subject's disposal is 
       * thread-safe (internally, to itself) and permanent, so no locking is required.
       */
      if (disposed)
      {
        throw new ObjectDisposedException("CommandSubject");
      }
    }

    /// <summary>
    /// Releases all resources used by an instance of the <see cref="CommandSubject" /> class.
    /// </summary>
    public void Dispose()
    {
      if (!disposed)
      {
        subject.Dispose();

        disposed = true;
      }
    }
    #endregion

    #region Events
    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    /// <remarks>
    /// <see cref="CanExecuteChanged"/> is raised when <see cref="OnNext"/> is called and the return value 
    /// of <see cref="CanExecute"/> may have changed.
    /// </remarks>
    public event EventHandler CanExecuteChanged;

    private void OnCanExecuteChanged(EventArgs e)
    {
      var handler = CanExecuteChanged;

      if (handler != null)
      {
        handler(this, e);
      }
    }
    #endregion
  }
}