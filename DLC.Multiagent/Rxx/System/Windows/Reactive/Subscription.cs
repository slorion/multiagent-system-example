using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace System.Windows.Reactive
{
  /// <summary>
  /// Provides high-level access to the definition of a binding that supports <see cref="IObservable{T}"/>, <see cref="IObserver{T}"/> 
  /// and event handlers.  A binding connects the dependency property of a WPF element to any observable or scalar data source, or 
  /// routed events (as of .NET 4.5) to any observer or handler-returning property.
  /// </summary>
  /// <remarks>
  /// <para>
  /// <see cref="Subscription"/> behaves almost identically to <see cref="Binding"/>, expect that <see cref="IObservable{T}"/> 
  /// data sources update the target with each element in the sequence and <see cref="IObserver{T}"/> data sources are notified when the 
  /// target changes.
  /// </para>
  /// <para>
  /// Furthermore, as of .NET 4.5, routed events are supported as binding targets.  Use <see cref="Subscription"/> in XAML to bind any
  /// routed event to a property that returns <see cref="System.Windows.RoutedEventHandler" />, <see cref="EventHandler" />, 
  /// <see cref="Action" />, <see cref="Action{T}" />, <see cref="System.Windows.Input.ICommand"/> or an object that implements 
  /// <see cref="IObserver{T}"/>, such as <see cref="System.Reactive.Subjects.Subject{T}"/>.  The generic type parameters can match either 
  /// the sender and event argument types of the event handler used by the event, or they may vary by assignable types used in 
  /// <see cref="System.Reactive.EventPattern{T}" /> or <see cref="System.Reactive.EventPattern{TSender,TEventArgs}" />.
  /// </para>
  /// <para>
  /// Binding to objects that do not implement <see cref="IObservable{T}"/> is supported in the direction from the source to the target, 
  /// just like <see cref="Binding"/>.  Updates from the target to the source are supported only if the source implements 
  /// <see cref="IObserver{T}"/>.
  /// </para>
  /// <para>
  /// A source that does not implement either <see cref="IObserver{T}"/> or <see cref="IObservable{T}"/> behaves like a normal 
  /// <see cref="Binding"/> with its <see cref="Binding.Mode"/> property set to <see cref="BindingMode.OneWay"/>.
  /// </para>
  /// <para>
  /// The target may require a scalar value.  For example, the <see cref="TextBlock.Text"/> property is usually bound to a property that 
  /// returns a <see cref="string"/>.  When the data source is an <see cref="IObservable{T}"/> sequence, instead of a scalar value, it is 
  /// treated like a changing scalar value.  Each element in the <see cref="IObservable{T}"/> is pushed to the target as the latest value.
  /// Essentially, subscribing to a property that returns <see cref="IObservable{T}"/> is a reactive alternative to binding to a property 
  /// that returns a scalar value, implementing <see cref="INotifyPropertyChanged"/> and raising an event each time the scalar value changes.
  /// </para>
  /// <para>
  /// <see cref="Subscription"/> also supports <see cref="System.Collections.IEnumerable"/> targets.  For example, <see cref="Subscription"/> 
  /// can be used to bind an observable data source to the <see cref="ItemsControl.ItemsSource"/> property.  The behavior of this kind of 
  /// subscription depends upon the capabilities of the source.  If the source implements <see cref="System.Collections.IEnumerable"/>, then 
  /// the source is sent to the target unmodified.  If the source also implements <see cref="System.Collections.Specialized.INotifyCollectionChanged"/>, 
  /// then the target is updated as per the specifications of that interface.  For example, this behavior is applied when subscribing to 
  /// <see cref="System.Reactive.Subjects.IListSubject{T}"/> or <see cref="System.Reactive.Subjects.IDictionarySubject{TKey,TValue}"/> 
  /// data sources.
  /// </para>
  /// <para>
  /// If the source does not implement <see cref="System.Collections.Specialized.INotifyCollectionChanged"/>, although it does implement 
  /// <see cref="IObservable{T}"/>, then the target is reset for every element in the observable sequence.  This causes the target to 
  /// iterate the entire enumerable sequence each time that an element is observed in the observable sequence.  In other words, the observable 
  /// is used as a change notification for the enumerable, although the actual data in the observable sequence is ignored.
  /// </para>
  /// <para>
  /// If the source does not implement <see cref="System.Collections.IEnumerable"/>, but does implement <see cref="IObservable{T}"/>, 
  /// then the target is bound to an empty collection of objects.  Then, each <see cref="System.Reactive.CollectionNotification{T}"/> 
  /// that is observed from the source observable sequence is applied to the bound collection.  Any values that aren't a 
  /// <see cref="System.Reactive.CollectionNotification{T}"/> are simply added to the bound collection.
  /// </para>
  /// <alert type="tip">
  /// All observed notifications are automatically marshaled to the dispatcher thread, unlike <see cref="Binding"/>.  For <see cref="IObservable{T}"/>
  /// sources, this means that you don't have to add <strong>ObserveOnDispatcher</strong> to your query.  For 
  /// <see cref="System.Collections.Specialized.INotifyCollectionChanged"/> sources, you can make concurrent updates to the source collection 
  /// (provided that it's thread-safe already) and the <see cref="Subscription"/> will automatically marshal changes to the dispatcher thread.
  /// </alert>
  /// <para>
  /// Otherwise, if the source does not implement either <see cref="System.Collections.IEnumerable"/> or <see cref="IObservable{T}"/>, 
  /// then the behavior used is the same as <see cref="Binding"/>.  In this case, you'll probably want to assign a <see cref="Converter"/> 
  /// that can convert the source into an <see cref="System.Collections.IEnumerable"/> for the target.
  /// </para>
  /// <alert type="warning">
  /// WPF may process bindings even before a UI has been completely loaded.  This can cause <see cref="Subscription"/> to receive a reference 
  /// to an observable data source before the UI is displayed, or even if it's never displayed.  Since <see cref="Subscription"/> does not know
  /// about the context in which its target element is being used, it will subscribe to the observable query right away.  In cases when you 
  /// need to instantiate an element that may contain <see cref="Subscription"/> bindings in XAML, but it's not added to a visual tree right 
  /// away or it's never going to be added, then it's recommended that you use <see cref="System.Reactive.Subjects.Subject{T}"/> as your 
  /// data source so that you can control when the source is connected to an observable that may cause side-effects.  When the element 
  /// finally raises its <see cref="FrameworkElement.Loaded"/> event, create the real observable sequence and subscribe the subject to it
  /// so that the binding starts receiving values.
  /// </alert>
  /// <para>
  /// The lifetime of the subscription to an observable data source is only partially controlled by the <see cref="Subscription"/> object.
  /// When the source property returns a different object, then the <see cref="Subscription"/> object will automatically dispose of 
  /// the previous observable's subscription, if any.  However, the latest subscription is never disposed by <see cref="Subscription"/>.
  /// For this reason, it remains the responsibility of data source objects to ensure that bound observables are cleaned up when they are 
  /// no longer needed.  This can be accomplished in a few different ways as follows.
  /// </para>
  /// <para>
  /// The most direct way is to call <see cref="Dispose"/> on the <see cref="Subscription"/> object, which disposes of any active subscription.
  /// </para>
  /// <para>
  /// In other cases you can ensure that the observable calls <strong>OnCompleted</strong>.  Or you can explicitly dispose of the 
  /// observable source, for example if you're using <see cref="System.Reactive.Subjects.Subject{T}"/> then you can call its 
  /// <see cref="IDisposable.Dispose"/> method when the bound element or its root container raises the <see cref="FrameworkElement.Unloaded"/> 
  /// event.
  /// </para>
  /// </remarks>
  public sealed class Subscription : MarkupExtension, IDisposable
  {
    #region Public Properties
    /// <summary>
    /// Gets or sets opaque data passed to the asynchronous data dispatcher.
    /// </summary>
    /// <value>Data passed to the asynchronous data dispatcher.</value>
    [DefaultValue("")]
    public object AsyncState
    {
      get
      {
        return binding.AsyncState;
      }
      set
      {
        binding.AsyncState = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to evaluate the <see cref="Path"/> relative to the data item or 
    /// the <see cref="DataSourceProvider"/> object.
    /// </summary>
    /// <value><see langword="False"/> to evaluate the path relative to the data item itself; otherwise, <see langword="true"/>.
    /// The default is <see langword="false"/>.</value>
    [DefaultValue(false)]
    public bool BindsDirectlyToSource
    {
      get
      {
        return binding.BindsDirectlyToSource;
      }
      set
      {
        binding.BindsDirectlyToSource = value;
      }
    }

    /// <summary>
    /// Gets or sets the name of the element to use as the binding source object.
    /// </summary>
    /// <value>The value of the <strong>Name</strong> property or <strong>x:Name</strong> Attribute of the element of interest.
    /// You can refer to elements in code only if they are registered to the appropriate <see cref="NameScope"/> through <strong>RegisterName</strong>.
    /// The default is <see langword="null" />.</value>
    [DefaultValue("")]
    public string ElementName
    {
      get
      {
        return binding.ElementName;
      }
      set
      {
        binding.ElementName = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="Subscription"/> should get values asynchronously.
    /// </summary>
    /// <remarks>
    /// Do not confuse <see cref="IsAsync"/> with the asynchronous nature of observables.  <see cref="IsAsync"/>
    /// applies only to reading the source property that returns the data source object, which may or may not 
    /// be an observable sequence.  If reading the source property takes a long time to execute, then it will 
    /// block the UI thread for a long time.  In this case, setting <see cref="IsAsync"/> to true ensures that 
    /// the UI thread is not blocked while the source property is being read.  If the property just-so-happens 
    /// to return an object that is an observable sequence, then the subscription to that observable will be 
    /// unaffected by the value of <see cref="IsAsync"/>.
    /// </remarks>
    /// <value>The default is <see langword="false" />.</value>
    [DefaultValue(false)]
    public bool IsAsync
    {
      get
      {
        return binding.IsAsync;
      }
      set
      {
        binding.IsAsync = value;
      }
    }

    /// <summary>
    /// Gets or sets the path to the binding source property.
    /// </summary>
    /// <value>The path to the binding source.  The default is <see langword="null"/>.</value>
    public PropertyPath Path
    {
      get
      {
        return binding.Path;
      }
      set
      {
        binding.Path = value;
      }
    }

    /// <summary>
    /// Gets or sets the binding source by specifying its location relative to the position of the binding target.
    /// </summary>
    /// <value>A <see cref="RelativeSource"/> object specifying the relative location of the binding source to use.
    /// The default is <see langword="null"/>.</value>
    [DefaultValue("")]
    public RelativeSource RelativeSource
    {
      get
      {
        return binding.RelativeSource;
      }
      set
      {
        binding.RelativeSource = value;
      }
    }

    /// <summary>
    /// Gets or sets the object to use as the binding source.
    /// </summary>
    /// <value>The object to use as the binding source.</value>
    public object Source
    {
      get
      {
        return binding.Source;
      }
      set
      {
        binding.Source = value;
      }
    }

    /// <summary>
    /// Gets or sets an XPath query that returns the value on the XML binding source to use.
    /// </summary>
    /// <value>The XPath query.  The default is <see langword="null"/>.</value>
    [DefaultValue("")]
    public string XPath
    {
      get
      {
        return binding.XPath;
      }
      set
      {
        binding.XPath = value;
      }
    }

    /// <summary>
    /// Gets or sets the name of the <see cref="BindingGroup"/> to which this binding belongs.
    /// </summary>
    /// <value>The name of the <see cref="BindingGroup"/> to which this binding belongs.</value>
    [DefaultValue("")]
    public string BindingGroupName
    {
      get
      {
        return binding.BindingGroupName;
      }
      set
      {
        binding.BindingGroupName = value;
      }
    }

    /// <summary>
    /// Gets or sets the value to use when the binding is unable to return a value.
    /// </summary>
    /// <value>The default value is <see cref="DependencyProperty.UnsetValue"/>.</value>
    public object FallbackValue
    {
      get
      {
        return multiBinding.FallbackValue;
      }
      set
      {
        multiBinding.FallbackValue = value;
      }
    }

    /// <summary>
    /// Gets or sets a string that specifies how to format the binding if it displays the bound value as a string.
    /// </summary>
    /// <value>A string that specifies how to format the binding if it displays the bound value as a string.</value>
    [DefaultValue("")]
    public string StringFormat
    {
      get
      {
        return multiBinding.StringFormat;
      }
      set
      {
        multiBinding.StringFormat = value;
      }
    }

    /// <summary>
    /// Gets or sets the value that is used in the target when the value of the source is <see langword="null"/>.
    /// </summary>
    /// <value>The value that is used in the target when the value of the source is <see langword="null"/>.</value>
    public object TargetNullValue
    {
      get
      {
        return multiBinding.TargetNullValue;
      }
      set
      {
        multiBinding.TargetNullValue = value;
      }
    }

    /// <summary>
    /// Gets or sets the converter to use to convert the source values to or from the target value.
    /// </summary>
    /// <value>A value of type <see cref="IValueConverter"/> that indicates the converter to use.
    /// The default value is <see langword="null" />.</value>
    [DefaultValue("")]
    public IValueConverter Converter
    {
      get
      {
        return converter.ValueConverter;
      }
      set
      {
        converter.ValueConverter = value;
      }
    }

    /// <summary>
    /// Gets or sets the culture in which to evaluate the converter.
    /// </summary>
    /// <value>The default is <see langword="null" />.</value>
    [DefaultValue("")]
    [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
    public CultureInfo ConverterCulture
    {
      get
      {
        return converter.ValueConverterCulture;
      }
      set
      {
        converter.ValueConverterCulture = value;
      }
    }

    /// <summary>
    /// Gets or sets an optional parameter to pass to the <see cref="Converter"/> as additional information.
    /// </summary>
    /// <value>The parameter to pass to the <see cref="Converter"/>.  The default is <see langword="null"/>.</value>
    [DefaultValue("")]
    public object ConverterParameter
    {
      get
      {
        return converter.ValueConverterParameter;
      }
      set
      {
        converter.ValueConverterParameter = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to raise the <see cref="FrameworkElement.SourceUpdated"/>
    /// event when a value is transferred from the binding target to the binding source.
    /// </summary>
    /// <value><see langword="True"/> if the <see cref="FrameworkElement.SourceUpdated"/> event will be raised
    /// when the binding source value is updated; otherwise, <see langword="false"/>.
    /// The default value is <see langword="false"/>.</value>
    [DefaultValue(false)]
    public bool NotifyOnSourceUpdated
    {
      get
      {
        return multiBinding.NotifyOnSourceUpdated;
      }
      set
      {
        multiBinding.NotifyOnSourceUpdated = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to raise the <see cref="FrameworkElement.TargetUpdated"/>
    /// event when a value is transferred from the binding source to the binding target.
    /// </summary>
    /// <value><see langword="True"/> if the <see cref="FrameworkElement.TargetUpdated"/> event will be raised
    /// when the binding target value is updated; otherwise, <see langword="false"/>.
    /// The default value is <see langword="false"/>.</value>
    [DefaultValue(false)]
    public bool NotifyOnTargetUpdated
    {
      get
      {
        return multiBinding.NotifyOnTargetUpdated;
      }
      set
      {
        multiBinding.NotifyOnTargetUpdated = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to raise the <see cref="System.Windows.Controls.Validation.ErrorEvent"/>
    /// attached event on the bound element.
    /// </summary>
    /// <value><see langword="True"/> if the <see cref="System.Windows.Controls.Validation.ErrorEvent"/> attached event will
    /// be raised on the bound element when there is a validation error during source updates; otherwise, 
    /// <see langword="false"/>.  The default value is <see langword="false"/>.</value>
    [DefaultValue(false)]
    public bool NotifyOnValidationError
    {
      get
      {
        return multiBinding.NotifyOnValidationError;
      }
      set
      {
        multiBinding.NotifyOnValidationError = value;
      }
    }

    /// <summary>
    /// Gets or sets a handler you can use to provide custom logic for handling exceptions
    /// that the binding engine encounters during the update of the binding source
    /// value.  This is only applicable if you have associated the <see cref="System.Windows.Controls.ExceptionValidationRule"/>
    /// with your <see cref="Subscription"/> object.
    /// </summary>
    /// <value>A method that provides custom logic for handling exceptions that the binding
    /// engine encounters during the update of the binding source value.</value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public UpdateSourceExceptionFilterCallback UpdateSourceExceptionFilter
    {
      get
      {
        return multiBinding.UpdateSourceExceptionFilter;
      }
      set
      {
        multiBinding.UpdateSourceExceptionFilter = value;
      }
    }

    /// <summary>
    /// Gets or sets a value that determines the timing of binding source updates.
    /// </summary>
    /// <value>One of the <see cref="UpdateSourceTrigger"/> values.  The default value
    /// is <see cref="System.Windows.Data.UpdateSourceTrigger.Default"/>, which returns the default 
    /// <see cref="System.Windows.Data.UpdateSourceTrigger"/> value of the target dependency property.
    /// However, the default value for most dependency properties is <see cref="System.Windows.Data.UpdateSourceTrigger.PropertyChanged"/>,
    /// while the <see cref="System.Windows.Controls.TextBox.Text"/> property has a default value 
    /// of <see cref="System.Windows.Data.UpdateSourceTrigger.LostFocus"/>.  A programmatic way to determine the default 
    /// <see cref="Binding.UpdateSourceTrigger"/> value of a dependency property is to get the property 
    /// metadata of the property using <see cref="DependencyProperty.GetMetadata(Type)"/> and then
    /// check the value of the <see cref="FrameworkPropertyMetadata.DefaultUpdateSourceTrigger"/> property.</value>
    public UpdateSourceTrigger UpdateSourceTrigger
    {
      get
      {
        return multiBinding.UpdateSourceTrigger;
      }
      set
      {
        multiBinding.UpdateSourceTrigger = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to include the <see cref="System.Windows.Controls.DataErrorValidationRule"/>.
    /// </summary>
    /// <value><see langword="True"/> to include the <see cref="System.Windows.Controls.DataErrorValidationRule"/>; 
    /// otherwise, <see langword="false"/>.</value>
    [DefaultValue(false)]
    public bool ValidatesOnDataErrors
    {
      get
      {
        return multiBinding.ValidatesOnDataErrors;
      }
      set
      {
        multiBinding.ValidatesOnDataErrors = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to include the <see cref="System.Windows.Controls.ExceptionValidationRule"/>.
    /// </summary>
    /// <value><see langword="True"/> to include the <see cref="System.Windows.Controls.ExceptionValidationRule"/>; 
    /// otherwise, <see langword="false"/>.</value>
    [DefaultValue(false)]
    public bool ValidatesOnExceptions
    {
      get
      {
        return multiBinding.ValidatesOnExceptions;
      }
      set
      {
        multiBinding.ValidatesOnExceptions = value;
      }
    }

    /// <summary>
    /// Gets a collection of rules that check the validity of the user input.
    /// </summary>
    /// <value>A collection of <see cref="System.Windows.Controls.ValidationRule"/> objects.</value>
    public Collection<ValidationRule> ValidationRules
    {
      get
      {
        return multiBinding.ValidationRules;
      }
    }
    #endregion

    #region Private / Protected
    private readonly ObservableValueConverter converter = new ObservableValueConverter();
    private readonly Binding binding, proxyBinding;
    private readonly MultiBinding multiBinding;
    private Type @event;
    private RoutedEventHandler sourceHandler;
    private Type senderType, eventArgsType;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="Subscription" /> class.
    /// </summary>
    [ContractVerification(false)]
    public Subscription()
    {
      binding = new Binding()
      {
        Mode = BindingMode.OneWay
      };

      proxyBinding = new Binding("Value")
      {
        Mode = BindingMode.TwoWay,
        Source = converter
      };

      multiBinding = new MultiBinding()
      {
        Converter = converter,
        Bindings =
				{
					binding, 
					proxyBinding
				}
      };
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="Subscription" /> class with an initial path.
    /// </summary>
    /// <param name="path">The initial <see cref="Path"/> for the binding.</param>
    public Subscription(string path)
      : this()
    {
      binding.Path = new PropertyPath(path);
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(converter != null);
      Contract.Invariant(binding != null);
      Contract.Invariant(proxyBinding != null);
      Contract.Invariant(multiBinding != null);
    }

    /// <summary>
    /// Returns an object that should be set on the property where this binding and extension are applied.
    /// </summary>
    /// <param name="serviceProvider">The object that can provide services for the markup extension.  May be <see langword="null" />.</param>
    /// <returns>The value to set on the binding target property.</returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      DependencyObject target = null;

      if (serviceProvider != null)
      {
        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

        if (service != null)
        {
          target = service.TargetObject as DependencyObject;

          if (target != null && DesignerProperties.GetIsInDesignMode(target))
          {
            var property = service.TargetProperty as DependencyProperty;

            return property != null && property.PropertyType == typeof(System.Collections.IEnumerable)
              ? Binding.DoNothing
              : null;
          }

          @event = TryGetEventHandlerType(service.TargetProperty);
        }
      }

      if (@event != null)
      {
        return ProvideEventHandler(target);
      }

      var expression = multiBinding.ProvideValue(serviceProvider) as MultiBindingExpression;

      if (expression == null)
      {
        // We could be inside of a template, in which case WPF expects a reference to the markup extension itself.
        return this;
      }

      converter.Expression = expression;

      return expression;
    }

    private static Type TryGetEventHandlerType(object value)
    {
      var @event = value as EventInfo;

      if (@event != null)
      {
        return @event.EventHandlerType;
      }

      // An attached event in XAML is represented by a static method
      var method = value as MethodInfo;

      if (method != null)
      {
        var parameters = method.GetParameters();

        if (parameters.Length == 2)
        {
          var first = parameters[0];
          var second = parameters[1];

          Contract.Assume(first != null);
          Contract.Assume(second != null);

          if (first.ParameterType.IsAssignableFrom(typeof(DependencyObject))
            && second.ParameterType.BaseType == typeof(MulticastDelegate))
          {
            return second.ParameterType;
          }
        }
      }

      return null;
    }

    private object ProvideEventHandler(DependencyObject target)
    {
      Contract.Requires(@event != null);

      var element = target as FrameworkElement;

      if (element == null)
      {
        Contract.Assume(PresentationTraceSources.DataBindingSource != null);

        PresentationTraceSources.DataBindingSource.TraceEvent(
          TraceEventType.Verbose,
          0,
          "Subscription: The target \"{0}\" for event binding \"{1}\" does not derive from FrameworkElement.",
          target == null ? null : target.GetType(),
          binding.XPath ?? (binding.Path == null ? string.Empty : binding.Path.Path));

        return DependencyProperty.UnsetValue;
      }
      else
      {
        InitializeBindingForEventTarget();

        RoutedEventHandler proxyHandler = InvokeForEventBinding;

        // Using a dynamic property allows multiple events to have bindings on the same target instance.
        var property = DependencyProperty.RegisterAttached(
          Guid.NewGuid().ToString(),
          typeof(object),
          typeof(Subscription),
          new FrameworkPropertyMetadata(SourceEventHandlerChanged));

        element.SetBinding(property, binding);

        return proxyHandler;
      }
    }

    private void InitializeBindingForEventTarget()
    {
      multiBinding.Bindings.Clear();

      binding.ValidatesOnExceptions = multiBinding.ValidatesOnExceptions;
      binding.ValidatesOnDataErrors = multiBinding.ValidatesOnDataErrors;
      binding.UpdateSourceTrigger = multiBinding.UpdateSourceTrigger;
      binding.UpdateSourceExceptionFilter = multiBinding.UpdateSourceExceptionFilter;
      binding.NotifyOnValidationError = multiBinding.NotifyOnValidationError;
      binding.NotifyOnTargetUpdated = multiBinding.NotifyOnTargetUpdated;
      binding.NotifyOnSourceUpdated = multiBinding.NotifyOnSourceUpdated;
      binding.ConverterParameter = converter.ValueConverterParameter;
      binding.ConverterCulture = converter.ValueConverterCulture;
      binding.Converter = converter.ValueConverter;
      binding.TargetNullValue = multiBinding.TargetNullValue;
      binding.StringFormat = multiBinding.StringFormat;
      binding.FallbackValue = multiBinding.FallbackValue;

      binding.ValidationRules.Clear();

      foreach (var rule in multiBinding.ValidationRules)
      {
        binding.ValidationRules.Add(rule);
      }
    }

    private void InvokeForEventBinding(object sender, RoutedEventArgs e)
    {
      if (sourceHandler != null)
      {
        sourceHandler(sender, e);
      }
    }

    private void SourceEventHandlerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      Contract.Requires(@event != null);

      var handler = e.NewValue;

      if (handler == null)
      {
        sourceHandler = null;
      }
      else
      {
        if (senderType == null || eventArgsType == null)
        {
          var invoke = @event.GetMethod("Invoke");

          Contract.Assume(invoke != null);

          var parameters = invoke.GetParameters();

          Contract.Assume(parameters.Length == 2);

          var first = parameters[0];
          var second = parameters[1];

          Contract.Assume(first != null);
          Contract.Assume(second != null);

          senderType = first.ParameterType;
          eventArgsType = second.ParameterType;
        }

        var handlerType = handler.GetType();

        sourceHandler = TryGetHandler(handler, handlerType, senderType, eventArgsType);

        if (sourceHandler == null)
        {
          Contract.Assume(PresentationTraceSources.DataBindingSource != null);

          PresentationTraceSources.DataBindingSource.TraceEvent(
            TraceEventType.Verbose,
            0,
            "Subscription: Target \"{0}\" event binding \"{1}\" evaluated to a handler of an unsupported type \"{2}\".",
            sender == null ? null : sender.GetType(),
            binding.XPath ?? (binding.Path == null ? string.Empty : binding.Path.Path),
            handlerType);
        }
      }
    }

    private static RoutedEventHandler TryGetHandler(object handler, Type handlerType, Type senderType, Type eventArgsType)
    {
      Contract.Requires(handlerType != null);
      Contract.Requires(senderType != null);
      Contract.Requires(eventArgsType != null);

      var routed = handler as RoutedEventHandler;

      if (routed != null)
      {
        return (sender, e) => routed(sender, e);
      }

      var eventHandler = handler as EventHandler;

      if (eventHandler != null)
      {
        return (sender, e) => eventHandler(sender, e);
      }

      var action = handler as Action;

      if (action != null)
      {
        return (sender, e) => action();
      }

      var eventPatternType = senderType == typeof(object)
        ? typeof(EventPattern<>).MakeGenericType(eventArgsType)
        : typeof(EventPattern<,>).MakeGenericType(senderType, eventArgsType);

      var command = handler as ICommand;

      if (command != null)
      {
        return (sender, e) => command.Execute(Activator.CreateInstance(eventPatternType, sender, e));
      }

      if (handlerType.BaseType != typeof(MulticastDelegate))
      {
        var onNext = TryGetObserverOnNext(handler, handlerType, senderType, eventArgsType, eventPatternType);

        if (onNext != null)
        {
          return onNext;
        }
      }

      return TryGetInvoke(handler, handlerType, senderType, eventArgsType, eventPatternType);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "The LINQ query is easier to maintain than the alternatives.")]
    private static RoutedEventHandler TryGetObserverOnNext(object handler, Type handlerType, Type senderType, Type eventArgsType, Type eventPatternType)
    {
      Contract.Requires(handlerType != null);
      Contract.Requires(senderType != null);
      Contract.Requires(eventArgsType != null);
      Contract.Requires(eventPatternType != null);

      if (handlerType.IsGenericType)
      {
#if NET_45
        var typeArgs = handlerType.GenericTypeArguments;
#else
				var typeArgs = handlerType.GetGenericArguments();
#endif

        Contract.Assume(typeArgs != null);

        var onNext = (from type in typeArgs
                      let assignableToArgs = type.IsAssignableFrom(eventArgsType)
                      let assignableToPatternResult = IsAssignableFromEventPattern(type, eventPatternType)
                      let assignableToPattern = assignableToPatternResult.Item1
                      let targetEventPatternType = assignableToPatternResult.Item2
                      let assignableToSender = type.IsAssignableFrom(senderType)
                      where assignableToArgs || assignableToPattern || assignableToSender
                      orderby type == eventArgsType || type == targetEventPatternType ? 0 : (!assignableToSender ? 1 : 2)
                      let io = typeof(IObserver<>).MakeGenericType(type)
                      where handlerType.GetInterfaces().Any(i => i == io)
                      let map = handlerType.GetInterfaceMap(io)
                      from method in map.TargetMethods ?? new MethodInfo[0]
                      where string.Equals(method.Name, "OnNext", StringComparison.Ordinal)
                      select new
                      {
                        method,
                        assignableToSender,
                        assignableToPattern,
                        targetEventPatternType
                      })
                     .FirstOrDefault();

        if (onNext != null)
        {
          if (onNext.assignableToSender)
          {
            return (sender, e) => onNext.method.Invoke(handler, new[] { sender });
          }
          else if (onNext.assignableToPattern)
          {
            return (sender, e) => onNext.method.Invoke(handler, new[] { Activator.CreateInstance(onNext.targetEventPatternType, sender, e) });
          }
          else
          {
            return (sender, e) => onNext.method.Invoke(handler, new[] { e });
          }
        }
      }

      return null;
    }

    private static RoutedEventHandler TryGetInvoke(object handler, Type handlerType, Type senderType, Type eventArgsType, Type eventPatternType)
    {
      Contract.Requires(handlerType != null);
      Contract.Requires(senderType != null);
      Contract.Requires(eventArgsType != null);

      var invoke = handlerType.GetMethod("Invoke");

      if (invoke != null)
      {
        var parameters = invoke.GetParameters();

        if (parameters.Length == 1)
        {
          var first = parameters[0];

          Contract.Assume(first != null);

          var type = first.ParameterType;

          if (type.IsAssignableFrom(eventArgsType))
          {
            return (sender, e) => invoke.Invoke(handler, new[] { e });
          }
          else if (IsAssignableFromEventPattern(type, ref eventPatternType))
          {
            return (sender, e) => invoke.Invoke(handler, new[] { Activator.CreateInstance(eventPatternType, sender, e) });
          }
          else if (type.IsAssignableFrom(senderType))
          {
            return (sender, e) => invoke.Invoke(handler, new[] { sender });
          }
        }
        else if (parameters.Length == 2)
        {
          var first = parameters[0];
          var second = parameters[1];

          Contract.Assume(first != null);
          Contract.Assume(second != null);

          if (first.ParameterType.IsAssignableFrom(senderType)
            && second.ParameterType.IsAssignableFrom(eventArgsType))
          {
            return (sender, e) => invoke.Invoke(handler, new[] { sender, e });
          }
        }
      }

      return null;
    }

    private static Tuple<bool, Type> IsAssignableFromEventPattern(Type type, Type eventPatternType)
    {
      return Tuple.Create(IsAssignableFromEventPattern(type, ref eventPatternType), eventPatternType);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1108:BlockStatementsMustNotContainEmbeddedComments", Justification = "It's readable.")]
    private static bool IsAssignableFromEventPattern(Type type, ref Type eventPatternType)
    {
      if (type.IsAssignableFrom(eventPatternType))
      {
        return true;
      }

      if (!type.IsGenericType
        || (type.GetGenericTypeDefinition() != typeof(EventPattern<>)
          && type.GetGenericTypeDefinition() != typeof(EventPattern<,>)))
      {
        return false;
      }

#if NET_45
      var sourceTypeArgs = eventPatternType.GenericTypeArguments;
      var targetTypeArgs = type.GenericTypeArguments;
#else
			var sourceTypeArgs = eventPatternType.GetGenericArguments();
			var targetTypeArgs = type.GetGenericArguments();
#endif

      if (targetTypeArgs.Length == sourceTypeArgs.Length
        || (targetTypeArgs.Length == 2 && sourceTypeArgs.Length == 1))	// sender contravariance
      {
        /* Let the binding fail at runtime if either the sender or EventArgs are not assignable from the source types.
         * 
         * Although most times we fail silently when binding, in the case of event bindings in XAML it can be annoying 
         * to figure out which specific types are used by a RoutedEventHandler and how they should map to EventPattern.
         * By failing at runtime, the actual type information of the source delegate will be revealed to the developer.
         * 
         * This behavior also enables variance of generic type arguments for EventPattern so that consumers can specify 
         * the base types or derived types that they actually need for the type arguments in EventPattern, instead of 
         * having to match the signature of the event handler perfectly.
         */
        eventPatternType = type;
        return true;
      }

      return false;
    }

    /// <summary>
    /// Disposes of the active subscription, if any, to an observable data source.
    /// </summary>
    public void Dispose()
    {
      converter.Dispose();
    }
    #endregion
  }
}