using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Reactive
{
  /// <summary>
  /// Provides <see langword="static"/> extension methods for <see cref="CollectionNotification{T}"/> objects.
  /// </summary>
  public static class CollectionNotificationExtensions
  {
    /// <summary>
    /// Converts a <see cref="CollectionNotification{T}"/> to a list of <see cref="CollectionModification{T}"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="notification">The <see cref="CollectionNotification{T}"/> to be converted.</param>
    /// <returns>A list of <see cref="CollectionModification{T}"/> containing
    /// <see cref="CollectionModificationKind.Add"/> when the specified <paramref name="notification"/> is <see cref="CollectionNotificationKind.Exists"/> or <see cref="CollectionNotificationKind.OnAdded"/>, 
    /// <see cref="CollectionModificationKind.Remove"/> followed by <see cref="CollectionModificationKind.Add"/> when the specified <paramref name="notification"/> is <see cref="CollectionNotificationKind.OnReplaced"/>, 
    /// <see cref="CollectionModificationKind.Remove"/> when the specified <paramref name="notification"/> is <see cref="CollectionNotificationKind.OnRemoved"/>, or 
    /// <see cref="CollectionModificationKind.Clear"/> when the specified <paramref name="notification"/> is <see cref="CollectionNotificationKind.OnCleared"/>.</returns>
    public static IList<CollectionModification<T>> ToModifications<T>(this CollectionNotification<T> notification)
    {
      Contract.Requires(notification != null);
      Contract.Ensures(Contract.Result<IList<CollectionModification<T>>>() != null);
      Contract.Ensures(Contract.Result<IList<CollectionModification<T>>>().IsReadOnly);

      var list = new List<CollectionModification<T>>();

      switch (notification.Kind)
      {
        case CollectionNotificationKind.Exists:
          list.Add(CollectionModification.CreateAdd<T>(notification.ExistingValues));
          break;
        case CollectionNotificationKind.OnAdded:
          list.Add(CollectionModification.CreateAdd<T>(notification.Value));
          break;
        case CollectionNotificationKind.OnRemoved:
          list.Add(CollectionModification.CreateRemove<T>(notification.Value));
          break;
        case CollectionNotificationKind.OnReplaced:
          list.Add(CollectionModification.CreateRemove<T>(notification.ReplacedValue));
          list.Add(CollectionModification.CreateAdd<T>(notification.Value));
          break;
        case CollectionNotificationKind.OnCleared:
          list.Add(CollectionModification.CreateClear<T>());
          break;
      }

      IList<CollectionModification<T>> result = list.AsReadOnly();

      Contract.Assume(result.IsReadOnly);

      return result;
    }
  }
}