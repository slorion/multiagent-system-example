using System.Diagnostics.Contracts;
#if UNIVERSAL
using Windows.UI.Xaml;
#endif

namespace System.Windows.Reactive
{
  /// <summary>
  /// Provides the attached property <see cref="ModelProperty"/>, which attaches a model factory to a <see cref="FrameworkElement"/> 
  /// object by specifying the <see cref="Type"/> of the model.
  /// </summary>
  public static class View
  {
    /// <summary>
    /// Represents the <strong>Model</strong> attached property in XAML, which specifies the <see cref="Type"/> of the model to be attached 
    /// to a <see cref="FrameworkElement"/>.  The type must define a parameterless default constructor that will be used to create a new 
    /// instance each time that the target <see cref="FrameworkElement"/> is loaded.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The specified <see cref="Type"/> does not have to implement any particular interfaces or derive from any particular base class; 
    /// however, a few special interfaces are supported to provide additional functionality.  See the 
    /// <see cref="FrameworkElementExtensions"/> documentation for more information.
    /// </para>
    /// <alert type="warning">
    /// Do not attempt to get or set the value of <see cref="ModelProperty"/> in code; instead, use the 
    /// <see cref="FrameworkElementExtensions"/> class directly.  Though it does not reflect whether <see cref="ModelProperty"/>
    /// was used to create an attachment, it always retrieves the current attachment object for the specified <see cref="FrameworkElement"/>.
    /// </alert>
    /// </remarks>
#if UNIVERSAL
    [CLSCompliant(false)]
#endif
    public static readonly DependencyProperty ModelProperty = DependencyProperty.RegisterAttached(
      "Model",
      typeof(Type),
      typeof(View),
      CreateModelPropertyMetadata());

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines",
      Justification = "Readability of flag combination.")]
    private static PropertyMetadata CreateModelPropertyMetadata()
    {
#if !SILVERLIGHT
      return new FrameworkPropertyMetadata(
        null,
          FrameworkPropertyMetadataOptions.AffectsArrange
        | FrameworkPropertyMetadataOptions.AffectsMeasure
        | FrameworkPropertyMetadataOptions.AffectsRender,
        ModelChanged);
#else
      return new PropertyMetadata(null, ModelChanged);
#endif
    }

    /// <summary>
    /// Gets the <see cref="Type"/> of the model assigned to the <see cref="ModelProperty"/> attached property for the specified 
    /// <paramref name="element"/>.  Do not use this method.  See the remarks section for more information.
    /// </summary>
    /// <remarks>
    /// <see cref="GetModel"/> only returns the model type that was assigned by the <see cref="ModelProperty"/> attached property.
    /// To get the currently attached model, regardless of how it was attached, call the <see cref="FrameworkElementExtensions.GetViewModel"/> 
    /// method instead.
    /// </remarks>
    /// <param name="element">The <see cref="FrameworkElement"/> from which to retrieve the value of the <see cref="ModelProperty"/> 
    /// attached property.</param>
    /// <returns>The <see cref="Type"/> of the model that is attached to the specified <paramref name="element"/> via the 
    /// <see cref="ModelProperty"/> attached property, or <see langword="null"/> if no model was attached via that property.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
      Justification = "Only FrameworkElement is supported because it defines the DataContext property, so the parameter must be constrained to this type.")]
#if UNIVERSAL
    [CLSCompliant(false)]
#endif
    public static Type GetModel(FrameworkElement element)
    {
      Contract.Requires(element != null);

      return (Type)element.GetValue(ModelProperty);
    }

    /// <summary>
    /// Sets the value of the <see cref="ModelProperty"/> attached property for the specified <paramref name="element"/> to the specified
    /// <paramref name="modelType"/> and attaches a factory that creates instances of the specified model Type
    /// for the specified <paramref name="element"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="SetModel"/> sets the <see cref="Type"/> of the model for the <see cref="ModelProperty"/> attached property and also 
    /// attaches a factory for the model using the <see cref="FrameworkElementExtensions.SetViewModel(FrameworkElement,Func{object})"/> method.
    /// Using <see cref="FrameworkElementExtensions"/> directly is recommended instead of calling <see cref="SetModel"/>, even though it 
    /// doesn't set the value of the <see cref="ModelProperty"/> attached property.
    /// </remarks>
    /// <param name="element">The <see cref="FrameworkElement"/> to which the specified <paramref name="modelType"/> will be attached.</param>
    /// <param name="modelType">The <see cref="Type"/> of the object to be attached to the specified <paramref name="element"/>.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
      Justification = "Only FrameworkElement is supported because it defines the DataContext property, so the parameter must be constrained to this type.")]
#if UNIVERSAL
    [CLSCompliant(false)]
#endif
    public static void SetModel(FrameworkElement element, Type modelType)
    {
      Contract.Requires(element != null);

      element.SetValue(ModelProperty, modelType);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily",
      Justification = "Code contract.")]
    private static void ModelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      Contract.Requires(sender is FrameworkElement);

      var element = (FrameworkElement)sender;

      var type = (Type)e.NewValue;

      if (type == null)
      {
        element.DataContext = null;
      }
      else
      {
        element.SetViewModel(type);
      }
    }
  }
}