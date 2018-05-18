using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive.Disposables;
#if !UNIVERSAL
using System.Windows;
#endif
using System.Windows.Reactive;
#if UNIVERSAL
using Windows.UI.Xaml;
#endif

namespace Rxx
{
#if !SILVERLIGHT
  /// <summary>
  /// Represents a <see cref="FrameworkElement"/> attachment that may provide binding targets for properties and events, and controls the 
  /// lifetime of its composited resources by permanently disposing of them when the <see cref="FrameworkElement"/> is unloaded.
  /// </summary>
  /// <include file='ViewModel.xml' path='//remarks[@name="Full"]'/>
  /// <threadsafety instance="false" static="true" />
  public abstract class ViewModel : DependencyObject, IViewModel, IDisposable
#elif WINDOWS_PHONE
  /// <summary>
  /// Represents a <see cref="FrameworkElement"/> attachment that may provide binding targets for properties and events, and controls the 
  /// lifetime of its composited resources by permanently disposing of them when the <see cref="FrameworkElement"/> is unloaded.
  /// </summary>
  /// <include file='ViewModel.xml' path='//remarks[@name="Phone"]'/>
  /// <threadsafety instance="false" static="true" />
#if UNIVERSAL
  [CLSCompliant(false)]
#endif
  public abstract class ViewModel : DependencyObject, IViewModel, IDisposable
#elif SILVERLIGHT_4
	/// <summary>
	/// Represents a <see cref="FrameworkElement"/> attachment that may provide binding targets for properties and events, and controls the 
	/// lifetime of its composited resources by permanently disposing of them when the <see cref="FrameworkElement"/> is unloaded.
	/// </summary>
	/// <include file='ViewModel.xml' path='//remarks[@name="Silverlight"][@version="4"]'/>
	/// <threadsafety instance="false" static="true" />
	public abstract class ViewModel : DependencyObject, IViewModel, IDisposable
#else
	/// <summary>
	/// Represents a <see cref="FrameworkElement"/> attachment that may provide binding targets for properties and events, and controls the 
	/// lifetime of its composited resources by permanently disposing of them when the <see cref="FrameworkElement"/> is unloaded.
	/// </summary>
	/// <include file='ViewModel.xml' path='//remarks[@name="Silverlight"][@version="5"]'/>
	/// <threadsafety instance="false" static="true" />
	public abstract class ViewModel : DependencyObject, IViewModel, IDisposable
#endif
  {
    #region Public Properties
    /// <summary>
    /// Gets a value indicating whether the view model is currently attached to a <see cref="FrameworkElement"/>.
    /// </summary>
    /// <value><see langword="True"/> if the view model is attached; otherwise, <see langword="false"/>.</value>
    public bool IsAttached
    {
      get
      {
        Contract.Ensures(Contract.Result<bool>() == (Element != null));

        return element != null;
      }
    }

#if !UNIVERSAL
    /// <summary>
    /// Gets a value indicating whether this view model instance is currently being used in a designer.
    /// </summary>
    public bool IsInDesignMode
    {
      get
      {
        return System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);
      }
    }
#endif
    #endregion

    #region Private / Protected
    /// <summary>
    /// Gets the <see cref="FrameworkElement"/> to which this view model is attached.
    /// </summary>
    /// <value>The <see cref="FrameworkElement"/> to which this view model is attached when <see cref="IsAttached"/>
    /// is <see langword="true"/>; otherwise, <see langword="null"/>.</value>
    protected FrameworkElement Element
    {
      get
      {
        Contract.Ensures((Contract.Result<FrameworkElement>() == null) == !IsAttached);
        Contract.Ensures(Contract.Result<FrameworkElement>() == element);

        return element;
      }
    }

    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private IDisposable attachmentDisposable;
    private FrameworkElement element;
    private bool disposed;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="ViewModel" /> class for derived classes.
    /// </summary>
    protected ViewModel()
    {
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(disposables != null);
      Contract.Invariant(!IsAttached || element != null);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "element", Scope = "member",
      Justification = "Field is prefixed with 'this'.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member",
      Justification = "The Attaching method is provided for derived types.  Attach should never be called by user code.")]
    void IViewModel.Attach(FrameworkElement element)
    {
      EnsureNotDisposed();

      this.element = element;

      var d = Attaching();

      if (d != null)
      {
        attachmentDisposable = new CompositeDisposable(d);

        disposables.Add(attachmentDisposable);
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member",
      Justification = "The Detaching method is provided for derived types.  Detach should never be called by user code.")]
    void IViewModel.Detach()
    {
      EnsureNotDisposed();

      Detaching();

      element = null;

      if (attachmentDisposable != null)
      {
        disposables.Remove(attachmentDisposable);

        attachmentDisposable = null;
      }
    }

    /// <summary>
    /// Called after the <see cref="ViewModel"/> is attached to the <see cref="Element"/>, providing an opportunity to create a set 
    /// of disposable resources that must be associated with the lifetime of the <see cref="Element"/>.
    /// </summary>
    /// <returns>A sequence of <see cref="IDisposable"/> objects to be associated with the lifetime of the <see cref="Element"/> 
    /// to which this <see cref="ViewModel"/> is attached.  This method can return <see langword="null"/>.</returns>
    protected virtual IEnumerable<IDisposable> Attaching()
    {
      Contract.Requires(IsAttached);
      Contract.Ensures(IsAttached);

      // for derived classes
      return null;
    }

    /// <summary>
    /// Called immediately before the <see cref="ViewModel"/> is detached from the <see cref="Element"/>.
    /// </summary>
    protected virtual void Detaching()
    {
      Contract.Requires(IsAttached);

      // for derived classes
    }

    /// <summary>
    /// Adds the specified <paramref name="disposables"/> to the <see cref="ViewModel"/>, associating their lifetime with the lifetime 
    /// of the view model.
    /// </summary>
    /// <param name="disposables">An array of <see cref="IDisposable"/> objects to be added.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "disposables",
      Justification = "The purpose of the parameter is clear.")]
    protected void AddDisposables(params IDisposable[] disposables)
    {
      Contract.Requires(disposables != null);

      foreach (var disposable in disposables)
      {
        if (disposable != null)
        {
          this.disposables.Add(disposable);
        }
      }
    }

    /// <summary>
    /// Removes and immediately disposes of the specified <paramref name="disposables"/>.
    /// </summary>
    /// <param name="disposables">An array of <see cref="IDisposable"/> objects to be removed.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "disposables",
      Justification = "The purpose of the parameter is clear.")]
    protected void RemoveDisposables(params IDisposable[] disposables)
    {
      Contract.Requires(disposables != null);

      foreach (var disposable in disposables)
      {
        if (disposable != null)
        {
          this.disposables.Remove(disposable);
        }
      }
    }

    /// <summary>
    /// Returns whether the <see cref="ViewModel"/> contains the specified <paramref name="disposable"/>.
    /// </summary>
    /// <param name="disposable">The disposable to search for.</param>
    /// <returns><see langword="True"/> if the specified <paramref name="disposable"/> is contained by the <see cref="ViewModel"/>; otherwise, 
    /// <see langword="false"/>.</returns>
    protected bool ContainsDisposable(IDisposable disposable)
    {
      return disposables.Contains(disposable);
    }

    /// <summary>
    /// Removes and immediately disposes of all disposable resources that are currently contained in the view model.
    /// </summary>
    protected void ClearDisposables()
    {
      disposables.Clear();
    }

    private void EnsureNotDisposed()
    {
      Contract.Ensures(IsAttached == Contract.OldValue(IsAttached));

      if (disposed)
      {
        throw new ObjectDisposedException(GetType().FullName);
      }
    }

    /// <summary>
    /// Releases all resources used by an instance of the <see cref="ViewModel" /> class, including all composited disposables that 
    /// were added by the <see cref="AddDisposables"/> method or returned by the <see cref="Attaching"/> method.
    /// </summary>
    /// <remarks>
    /// This method calls the virtual <see cref="Dispose(bool)" /> method, passing in <see langword="true"/>, and then suppresses 
    /// finalization of the instance.
    /// </remarks>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by an instance of the <see cref="ViewModel" /> class and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><see langword="True"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "attachmentDisposable",
      Justification = "The attachmentDisposable object is composited by the disposables collection.")]
    protected virtual void Dispose(bool disposing)
    {
      if (disposing && !disposed)
      {
        disposables.Dispose();

        disposed = true;
      }
    }
    #endregion
  }
}