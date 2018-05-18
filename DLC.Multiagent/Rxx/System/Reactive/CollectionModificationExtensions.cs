using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Reactive
{
  /// <summary>
  /// Provides <see langword="static"/> extension methods for <see cref="CollectionModification{T}"/> objects.
  /// </summary>
  public static class CollectionModificationExtensions
  {
    /// <summary>
    /// Converts a <see cref="CollectionModification{T}"/> to a list of <see cref="CollectionNotification{T}"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides modification information.</typeparam>
    /// <param name="modification">The <see cref="CollectionModification{T}"/> to be converted.</param>
    /// <returns>A list of <see cref="CollectionNotification{T}"/> containing 
    /// <see cref="CollectionNotificationKind.OnAdded"/> when the specified <paramref name="modification"/> is <see cref="CollectionModificationKind.Add"/>, 
    /// <see cref="CollectionNotificationKind.OnRemoved"/> when the specified <paramref name="modification"/> is <see cref="CollectionModificationKind.Remove"/>, or
    /// <see cref="CollectionNotificationKind.OnCleared"/> when the specified <paramref name="modification"/> is <see cref="CollectionModificationKind.Clear"/>.</returns>
    public static IList<CollectionNotification<T>> ToNotifications<T>(this CollectionModification<T> modification)
    {
      Contract.Requires(modification != null);
      Contract.Ensures(Contract.Result<IList<CollectionNotification<T>>>() != null);
      Contract.Ensures(Contract.Result<IList<CollectionNotification<T>>>().IsReadOnly);

      var list = new List<CollectionNotification<T>>();

      IList<T> values;

      switch (modification.Kind)
      {
        case CollectionModificationKind.Add:
          values = modification.Values;

          for (int i = 0; i < values.Count; i++)
          {
            list.Add(CollectionNotification.CreateOnAdded<T>(values[i]));
          }
          break;
        case CollectionModificationKind.Remove:
          values = modification.Values;

          for (int i = 0; i < values.Count; i++)
          {
            list.Add(CollectionNotification.CreateOnRemoved<T>(values[i]));
          }
          break;
        case CollectionModificationKind.Clear:
          list.Add(CollectionNotification.CreateOnCleared<T>());
          break;
      }

      IList<CollectionNotification<T>> result = list.AsReadOnly();

      Contract.Assume(result.IsReadOnly);

      return result;
    }
  }
}