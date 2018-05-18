using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Windows.Threading;

namespace System.Windows.Data
{
  internal sealed class DispatchChangesEnumerable : IEnumerable, INotifyCollectionChanged
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    private readonly Dispatcher dispatcher;
    private readonly DispatcherPriority priority;
    private readonly IEnumerable enumerable;
    #endregion

    #region Constructors
    public DispatchChangesEnumerable(Dispatcher dispatcher, DispatcherPriority priority, IEnumerable enumerable)
    {
      Contract.Requires(dispatcher != null);
      Contract.Requires(enumerable != null);

      this.dispatcher = dispatcher;
      this.priority = priority;
      this.enumerable = enumerable;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(dispatcher != null);
      Contract.Invariant(enumerable != null);
    }
    #endregion

    #region Events
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      var handler = CollectionChanged;

      if (handler != null)
      {
        dispatcher.BeginInvoke(handler, priority, enumerable, e);
      }
    }

    public IEnumerator GetEnumerator()
    {
      return enumerable.GetEnumerator();
    }
    #endregion
  }
}