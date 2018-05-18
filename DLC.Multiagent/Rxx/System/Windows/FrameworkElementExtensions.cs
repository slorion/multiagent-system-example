#if SILVERLIGHT_4 || WINDOWS_PHONE
using System.Collections.Generic;
#elif !UNIVERSAL
using System.ComponentModel;
#endif
using System.Diagnostics.Contracts;
#if (SILVERLIGHT_4 || WINDOWS_PHONE) && !UNIVERSAL
using System.Windows.Data;
#endif
using System.Windows.Reactive;
#if UNIVERSAL
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

namespace System.Windows
{
  /// <summary>
  /// Provides <see langword="static" /> methods for attaching view models to <see cref="FrameworkElement"/> objects.
  /// </summary>
  public static class FrameworkElementExtensions
  {
    /// <summary>
    /// Gets the object that is currently attached to the specified <paramref name="element"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="GetViewModel"/> actually returns the value of the <see cref="FrameworkElement.DataContext"/> property of the 
    /// specified <paramref name="element"/>, whether or not it represents a view model object.  This may change in a future version.
    /// </remarks>
    /// <param name="element">The <see cref="FrameworkElement"/> from which the attachment will be returned.</param>
    /// <returns>The object that is currently attached to the specified <paramref name="element"/>.</returns>
#if UNIVERSAL
    [CLSCompliant(false)]
#endif
    public static object GetViewModel(this FrameworkElement element)
    {
      Contract.Requires(element != null);

      return element.DataContext;
    }

    internal static void SetViewModel(this FrameworkElement element, Type type)
    {
      Contract.Requires(element != null);
      Contract.Requires(type != null);

#if !SILVERLIGHT
      element.SetViewModel(() => Activator.CreateInstance(type, nonPublic: true));
#else
      element.SetViewModel(() => Activator.CreateInstance(type));
#endif
    }

    /// <summary>
    /// Attaches a view model returned by the specified function to the <see cref="FrameworkElement.DataContext"/> of the 
    /// specified <paramref name="element"/> each time that the <paramref name="element"/> is loaded, and detaches the current 
    /// view model each time that the <paramref name="element"/> is unloaded.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="SetViewModel(FrameworkElement,Func{object})"/> allows the element's <see cref="FrameworkElement.DataContext"/> 
    /// to be assigned to different objects or view models after a view model is attached.  When a different object or view 
    /// model is assigned, the current view model is permanently detached and the element's events are no longer tracked for 
    /// the specified <paramref name="viewModelFactory"/>.  To reassign the factory to the <paramref name="element"/>, call 
    /// <see cref="SetViewModel(FrameworkElement,Func{object})"/> again.
    /// </para>
    /// <para>
    /// The factory can return the same view model instance every time that it's called or it can create new view models.
    /// The factory can also return <see langword="null"/>.
    /// </para>
    /// <alert type="warning">
    /// Do not return the same view model instance more than once if it implements <see cref="IDisposable"/>; otherwise, the
    /// view model will be disposed the first time that it's detached, which may cause problems if it's reattached.
    /// </alert>
    /// <para>
    /// The objects returned by the factory do not have to derive from any particular base class or implement any particular 
    /// interfaces; however, a couple of special interfaces are supported to provide additional functionality.  If an object
    /// implements <see cref="IDisposable"/>, then its <see cref="IDisposable.Dispose"/> method is called when the object is 
    /// detached from the <paramref name="element"/>.  If an object implements <see cref="IViewModel"/>, then its 
    /// <see cref="IViewModel.Attach"/> and <see cref="IViewModel.Detach"/> methods are called when appropriate.  Implementations
    /// for both of these interfaces is provided by the base <see cref="Rxx.ViewModel"/> class.
    /// </para>
    /// </remarks>
    /// <param name="element">The <see cref="FrameworkElement"/> to which a view model will be attached.</param>
    /// <param name="viewModelFactory">A function that returns view model objects or <see langword="null"/>.</param>
#if UNIVERSAL
    [CLSCompliant(false)]
#endif
    public static void SetViewModel(this FrameworkElement element, Func<object> viewModelFactory)
    {
      Contract.Requires(element != null);
      Contract.Requires(viewModelFactory != null);

      if (PrepareForDesigner(element, viewModelFactory))
      {
        return;
      }

      bool attached = false, attaching = false, detaching = false;
      IViewModel attachment = null;
      object viewModel = null;

      RoutedEventHandler loaded = (sender, e) =>
        {
          //// When unloading the element in Silverlight its Loaded event was raised before Unloaded, causing a contract failure on an
          //// explicit assumption: attached == false.  Can't repro.
          ////
          //// Update 11/8/2012: 
          //// I was able to repro this in WPF 4.5 by attaching a class deriving from ViewModel to a child control of a TabItem.  I attached 
          //// different view models to multiple tabs, each dislpaying unique bound content.  The app loads fine and the first tab binds to
          //// the view model correctly.  The other tabs also raise their Loaded events and attach to their respective view models immediately, 
          //// even though they aren't dispayed to the user yet.  But switching to another tab for the first time causes the Loaded event to 
          //// be raised again for the controls in that TabItem, without having raised the Unloaded.  That's two Loaded events in a row without
          //// any Unloaded event in between, so attached == true is possible in this event handler.
          if (!attached)
          {
            viewModel = Attach(element, viewModelFactory, ref attached, ref attaching, out attachment);
          }
        };

      RoutedEventHandler unloaded = (sender, e) =>
        {
          detaching = true;

          try
          {
            // The DataContextChanged event handler detaches and disposes of the current view model.
            element.DataContext = null;
          }
          finally
          {
            detaching = false;
          }
        };

      DependencyPropertyChangedEventHandler dataContextChanged = null;
      dataContextChanged = (sender, e) =>
        {
          if (!attaching && !detaching)
          {
            var isInherited = element.ReadLocalValue(FrameworkElement.DataContextProperty) == DependencyProperty.UnsetValue;

            if (isInherited)
            {
              return;
            }

            element.Loaded -= loaded;
            element.Unloaded -= unloaded;

#if !SILVERLIGHT
            element.DataContextChanged -= dataContextChanged;
#elif !SILVERLIGHT_4 && !WINDOWS_PHONE
						// Silverlight 5 throws "collection was modified" if the handler is removed synchronously.
						element.Dispatcher.BeginInvoke(() => element.DataContextChanged -= dataContextChanged);
#else
            RemoveDataContextChangedHandler(element, dataContextChanged);
#endif
          }

          /* If attaching is true, then Attach hasn't returned yet, so viewModel still references the old object; although, 
           * the DataContext property has already been assigned to the new viewModel.
           * 
           * If detaching is true, then the Unloaded event is being handled and viewModel still references the old object.
           * 
           * (Otherwise, the DataContext was simply changed; perhaps by another call to SetViewModel.)
           */
          Detach(element, ref viewModel, ref attachment, ref attached);
        };

      element.Loaded += loaded;
      element.Unloaded += unloaded;

#if SILVERLIGHT
      if (element.Parent == null)
      {
        /* The data context must be assigned (even if it's to null) so that it's not inherited automatically when the 
         * element is loaded.  In testing, this occurred and it caused the DataContextChanged event to be raised, which
         * caused the view model to be detached even before the element was loaded.
         */
        element.DataContext = null;
      }
#endif

#if !SILVERLIGHT_4 && !WINDOWS_PHONE
      element.DataContextChanged += dataContextChanged;
#else
      AddDataContextChangedHandler(element, dataContextChanged);
#endif

#if !SILVERLIGHT
      if (element.IsLoaded)
#else
      if (element.Parent != null)
#endif
      {
        loaded(element, null);
      }
    }

    private static bool PrepareForDesigner(FrameworkElement element, Func<object> viewModelFactory)
    {
#if UNIVERSAL
      var isInDesigner = false;
#else
      var isInDesigner = DesignerProperties.GetIsInDesignMode(element);
#endif

      if (isInDesigner && element != null && viewModelFactory != null)
      {
        /* If a designer DataContext has been assigned, do not replace it.
         * 
         * In testing WPF, the DataContext property returned null until the Loaded event was raised, hence the following code.
         * 
         * A WPF designer DataContext can be assigned in XAML as follows: 
         * 
         *		xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
         *		d:DataContext="{StaticResource MyDesignerViewModel}"
         *		mc:Ignorable="d"
         *		
         * The order in which the d:DataContext and View.Model attributes are specified in XAML is irrelevant.
         */
        element.Loaded += (sender, e) =>
        {
          if (element.DataContext == null)
          {
            bool attached = false, attaching = false;
            IViewModel attachment;

            Attach(element, viewModelFactory, ref attached, ref attaching, out attachment);
          }
        };
      }

      return isInDesigner;
    }

    private static object Attach(
      FrameworkElement element,
      Func<object> viewModelFactory,
      ref bool attached,
      ref bool attaching,
      out IViewModel attachment)
    {
      Contract.Requires(element != null);
      Contract.Requires(viewModelFactory != null);
      Contract.Requires(!attached);
      Contract.Ensures(Contract.ValueAtReturn(out attached));

      object viewModel;

      attaching = true;

      try
      {
        viewModel = viewModelFactory();

        element.DataContext = viewModel;

        attachment = viewModel as IViewModel;

        if (attachment != null)
        {
          if (attachment.IsAttached)
          {
            attachment.Detach();
          }

          attachment.Attach(element);
        }
      }
      finally
      {
        attaching = false;
      }

      attached = true;

      return viewModel;
    }

    private static void Detach(
      FrameworkElement element,
      ref object viewModel,
      ref IViewModel attachment,
      ref bool attached)
    {
      Contract.Requires(element != null);

      if (attached && !object.ReferenceEquals(element.DataContext, viewModel))
      {
        attached = false;

        var d = viewModel as IDisposable;

        viewModel = null;

        if (attachment != null)
        {
          var a = attachment;

          attachment = null;

          if (a.IsAttached)
          {
            a.Detach();
          }
        }

        if (d != null)
        {
          d.Dispose();
        }
      }
    }

#if SILVERLIGHT_4 || WINDOWS_PHONE
    private static readonly Dictionary<FrameworkElement, int> boundElements = new Dictionary<FrameworkElement, int>();

    private static readonly DependencyProperty DataContextInternalProperty = DependencyProperty.RegisterAttached(
      "DataContextInternal",
      typeof(object),
      typeof(FrameworkElementExtensions),
      new PropertyMetadata(null, OnDataContextChanged));

    private static void AddDataContextChangedHandler(FrameworkElement element, DependencyPropertyChangedEventHandler handler)
    {
      Contract.Requires(element != null);
      Contract.Requires(handler != null);

      DataContextChanged += handler;

      if (!boundElements.ContainsKey(element))
      {
        element.SetBinding(
          DataContextInternalProperty,
          new Binding()
          {
            Mode = BindingMode.OneWay
          });

        boundElements.Add(element, 1);
      }
      else
      {
        boundElements[element]++;
      }
    }

    private static void RemoveDataContextChangedHandler(FrameworkElement element, DependencyPropertyChangedEventHandler handler)
    {
      Contract.Requires(element != null);
      Contract.Requires(handler != null);

      DataContextChanged -= handler;

      if (boundElements.ContainsKey(element))
      {
        var count = boundElements[element];

        count--;

        if (count == 0)
        {
          element.ClearValue(DataContextInternalProperty);

          boundElements.Remove(element);
        }
        else
        {
          boundElements[element] = count;
        }
      }
    }

    private static event DependencyPropertyChangedEventHandler DataContextChanged;

    private static void OnDataContextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var handler = DataContextChanged;

      if (handler != null)
      {
        handler(sender, e);
      }
    }
#endif
  }
}