using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Data;
using System.Windows.Threading;

namespace System.Windows.Reactive
{
  internal sealed class ObservableValueConverter : IMultiValueConverter, IDisposable
  {
    #region Public Properties
    public object Value
    {
      get
      {
        return currentValue;
      }
      set
      {
        this.currentValue = value;

        if (observer != null)
        {
          observer.OnNext(value);
        }
      }
    }

    public MultiBindingExpression Expression
    {
      get;
      set;
    }

    public IValueConverter ValueConverter
    {
      get;
      set;
    }

    public CultureInfo ValueConverterCulture
    {
      get;
      set;
    }

    public object ValueConverterParameter
    {
      get;
      set;
    }
    #endregion

    #region Private / Protected
    private BindingExpressionBase ValueExpression
    {
      get
      {
        Contract.Ensures(Contract.Result<BindingExpressionBase>() != null);

        Contract.Assume(Expression != null);
        Contract.Assume(Expression.BindingExpressions.Count == 2);

        var expression = Expression.BindingExpressions[1];

        Contract.Assume(expression != null);

        return expression;
      }
    }

    private readonly SerialDisposable subscription = new SerialDisposable();
    private DispatcherOperation subscribing;
    private IObserver<object> observer;
    private IObservable<object> source;
    private IEnumerable listSource;
    private object currentValue, boundSource;
    private bool hasValue, disposed;
    #endregion

    #region Constructors
    public ObservableValueConverter()
    {
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(subscription != null);
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      Contract.Assume(values != null);
      Contract.Assume(values.Length == 2);

      if (targetType == null)
      {
        targetType = typeof(object);
      }

      Bind(values[0], targetType);

      if (!hasValue)
      {
        return DependencyProperty.UnsetValue;
      }
      else if (ValueConverter != null)
      {
        return ValueConverter.Convert(currentValue, targetType, ValueConverterParameter ?? parameter, ValueConverterCulture ?? culture);
      }
      else if (currentValue == null)
      {
        return null;
      }
      else if (targetType.IsAssignableFrom(currentValue.GetType()))
      {
        return currentValue;
      }
      else
      {
        var converter = TypeDescriptor.GetConverter(currentValue);

        if (converter == null || !converter.CanConvertTo(targetType))
        {
          Contract.Assume(PresentationTraceSources.DataBindingSource != null);

          PresentationTraceSources.DataBindingSource.TraceEvent(
            TraceEventType.Verbose,
            0,
            "Subscription: The value \"{0}\" cannot be converted to the specified type \"{1}\".",
            currentValue.GetType(),
            targetType);

          return DependencyProperty.UnsetValue;
        }

        return converter.ConvertTo(null, ValueConverterCulture ?? culture, currentValue, targetType);
      }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      Contract.Assume(targetTypes != null);
      Contract.Assume(targetTypes.Length == 2);

      var targetType = targetTypes[1];

      Contract.Assume(targetType != null);

      object convertedValue;

      if (ValueConverter != null)
      {
        convertedValue = ValueConverter.ConvertBack(value, targetType, ValueConverterParameter ?? parameter, ValueConverterCulture ?? culture);
      }
      else if (value == null)
      {
        convertedValue = null;
      }
      else if (targetType.IsAssignableFrom(value.GetType()))
      {
        convertedValue = value;
      }
      else
      {
        var converter = TypeDescriptor.GetConverter(targetType);

        if (converter == null || !converter.CanConvertFrom(value.GetType()))
        {
          Contract.Assume(PresentationTraceSources.DataBindingSource != null);

          PresentationTraceSources.DataBindingSource.TraceEvent(
            TraceEventType.Verbose,
            0,
            "Subscription: The value \"{0}\" cannot be converted back to the specified type \"{1}\".",
            value.GetType(),
            targetType);

          convertedValue = DependencyProperty.UnsetValue;
        }
        else
        {
          convertedValue = converter.ConvertFrom(null, ValueConverterCulture ?? culture, value);
        }
      }

      return new[] { null, convertedValue };
    }

    private void Bind(object obj, Type targetType)
    {
      Contract.Requires(targetType != null);

      if (!disposed && !object.Equals(obj, boundSource))
      {
        boundSource = obj;

        CancelSubscribe();

        observer = Observable2.CoerceObserver<object>(obj);

        if (targetType == typeof(IEnumerable))
        {
          TrySubscribeEnumerable(obj);
        }
        else
        {
          TrySubscribe(obj);
        }
      }
    }

    private void SetScalarValue(object obj)
    {
      currentValue = obj;
      hasValue = true;

      if (source != null || listSource != null)
      {
        subscription.Disposable = Disposable.Empty;

        source = null;
        listSource = null;
      }
    }

    private void TrySubscribeEnumerable(object obj)
    {
      var enumerable = obj as IEnumerable;

      if (enumerable == null)
      {
        var observable = Observable2.Coerce<object>(obj);

        if (observable == null)
        {
          SetScalarValue(obj);
        }
        else
        {
          var collection = new ObservableCollection<object>();

          SetScalarValue(collection);

          source = observable;
          listSource = collection;

          subscribing = Dispatcher.CurrentDispatcher.BeginInvoke((Action)SubscribeCollection, DispatcherPriority.DataBind);
        }
      }
      else
      {
        listSource = enumerable;

        var dispatcher = Dispatcher.CurrentDispatcher;

        var notifier = new DispatchChangesEnumerable(dispatcher, DispatcherPriority.DataBind, enumerable);

        SetScalarValue(notifier);

        var notifyingSource = obj as INotifyCollectionChanged;

        if (notifyingSource != null)
        {
          NotifyCollectionChangedEventHandler handler = (sender, e) => notifier.OnCollectionChanged(e);

          notifyingSource.CollectionChanged += handler;

          subscription.Disposable = Disposable.Create(() => notifyingSource.CollectionChanged -= handler);
        }
        else
        {
          var observable = Observable2.Coerce<object>(obj);

          if (observable != null)
          {
            source = observable;

            subscribing = dispatcher.BeginInvoke((Action<DispatchChangesEnumerable>)SubscribeNotify, DispatcherPriority.DataBind, notifier);
          }
        }
      }
    }

    private void TrySubscribe(object obj)
    {
      var observable = Observable2.Coerce<object>(obj);

      if (observable == null)
      {
        SetScalarValue(obj);
      }
      else
      {
        source = observable;

        currentValue = null;
        hasValue = false;

        subscribing = Dispatcher.CurrentDispatcher.BeginInvoke((Action)Subscribe, DispatcherPriority.DataBind);
      }
    }

    private void CancelSubscribe()
    {
      if (subscribing != null && subscribing.Status == DispatcherOperationStatus.Pending)
      {
        subscribing.Abort();
        subscribing = null;
      }
    }

    private void SubscribeNotify(DispatchChangesEnumerable notifier)
    {
      Contract.Requires(notifier != null);
      Contract.Requires(source != null);

      var reset = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

      subscription.SetDisposableIndirectly(() =>
        source.Subscribe(
          _ => notifier.OnCollectionChanged(reset),
          () => PresentationTraceSources.DataBindingSource.TraceEvent(
            TraceEventType.Verbose,
            0,
            "Subscription: Notification binding completed.")));
    }

    private void SubscribeCollection()
    {
      Contract.Requires(source != null);
      Contract.Requires(listSource != null);

      var collection = (ObservableCollection<object>)listSource;

      subscription.SetDisposableIndirectly(() =>
        source.ObserveOn(DispatcherSynchronizationContext.Current).Subscribe(
          value =>
          {
            var type = value == null ? null : value.GetType();

            if (type != null
              && type.IsGenericType
              && type.GetGenericTypeDefinition() == typeof(CollectionNotification<>))
            {
              var kind = (CollectionNotificationKind)type.GetProperty("Kind").GetValue(value, null);

              switch (kind)
              {
                case CollectionNotificationKind.OnAdded:
                  collection.Add(type.GetProperty("Value").GetValue(value, null));
                  break;
                case CollectionNotificationKind.OnRemoved:
                  collection.Remove(type.GetProperty("Value").GetValue(value, null));
                  break;
                case CollectionNotificationKind.OnCleared:
                  collection.Clear();
                  break;
              }
            }
            else
            {
              collection.Add(value);
            }
          },
          () => PresentationTraceSources.DataBindingSource.TraceEvent(
            TraceEventType.Verbose,
            0,
            "Subscription: Collection binding completed.")));
    }

    private void Subscribe()
    {
      Contract.Requires(source != null);

      var binding = ValueExpression;

      subscription.SetDisposableIndirectly(() =>
        source.ObserveOn(DispatcherSynchronizationContext.Current).Subscribe(
          newValue =>
          {
            currentValue = newValue;
            hasValue = true;

            binding.UpdateTarget();
          },
          () => PresentationTraceSources.DataBindingSource.TraceEvent(
            TraceEventType.Verbose,
            0,
            "Subscription: Binding completed.")));
    }

    public void Dispose()
    {
      disposed = true;

      subscription.Dispose();

      CancelSubscribe();

      currentValue = null;
      boundSource = null;
      source = null;
      listSource = null;
      observer = null;
    }
    #endregion
  }
}