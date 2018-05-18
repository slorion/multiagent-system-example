using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace System
{
  /// <summary>
  /// Provides methods that construct instances of <see cref="Maybe{T}"/>.
  /// </summary>
  public static class Maybe
  {
    /// <summary>
    /// Gets a <see cref="Maybe{T}"/> that represents a missing instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <returns>A <see cref="Maybe{T}"/> with <see cref="Maybe{T}.HasValue"/> set to <see langword="false" />.</returns>
    [SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Static Empty field has no explicit contracts.")]
    public static Maybe<T> Empty<T>()
    {
      Contract.Ensures(!Contract.Result<Maybe<T>>().HasValue);

      return Maybe<T>.Empty;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Maybe{T}" /> with the specified <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="T">Type of <paramref name="value"/></typeparam>
    /// <param name="value">The value assigned to the <see cref="Maybe{T}.Value"/> property.</param>
    /// <returns>A new instance of <see cref="Maybe{T}"/> with the specified <paramref name="value"/> and 
    /// <see cref="Maybe{T}.HasValue"/> set to <see langword="true" />.</returns>
    public static Maybe<T> Return<T>(T value)
    {
      Contract.Ensures(Contract.Result<Maybe<T>>().HasValue);
      Contract.Ensures(object.Equals(Contract.Result<Maybe<T>>().Value, value));

      var maybe = new Maybe<T>(value);

      Contract.Assert(maybe.HasValue);

      return maybe;
    }
  }
}