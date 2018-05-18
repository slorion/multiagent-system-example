using System.Diagnostics.Contracts;
#if UNIVERSAL
using Windows.UI.Xaml;
#endif

namespace System.Windows.Reactive
{
  /// <summary>
  /// When implemented, represents a <see cref="FrameworkElement"/> attachment that may provide binding targets for properties and events.
  /// </summary>
  [ContractClass(typeof(IViewModelContract))]
  public interface IViewModel
  {
    /// <summary>
    /// Gets a value indicating whether the view model is currently attached to a <see cref="FrameworkElement"/>.
    /// </summary>
    /// <value><see langword="True"/> if the view model is attached; otherwise, <see langword="false"/>.</value>
    bool IsAttached { get; }

    /// <summary>
    /// Attaches this view model to the specified <paramref name="element"/>.
    /// </summary>
    /// <param name="element">The <see cref="FrameworkElement"/> to which this view model must be attached.</param>
#if UNIVERSAL
    [CLSCompliant(false)]
#endif
    void Attach(FrameworkElement element);

    /// <summary>
    /// Detaches this view model if it's currently attached; otherwise, performs no action.
    /// </summary>
    void Detach();
  }

  [ContractClassFor(typeof(IViewModel))]
  internal abstract class IViewModelContract : IViewModel
  {
    public bool IsAttached
    {
      get
      {
        return false;
      }
    }

    public void Attach(FrameworkElement element)
    {
      Contract.Requires(element != null);
      Contract.Requires(!IsAttached);
      Contract.Ensures(IsAttached);
    }

    public void Detach()
    {
      Contract.Requires(IsAttached);
      Contract.Ensures(!IsAttached);
    }
  }
}