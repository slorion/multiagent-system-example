using System.Diagnostics.Contracts;

namespace System.Windows.Input
{
  public static partial class CommandExtensions
  {
    private struct TypeCommandPair : IEquatable<TypeCommandPair>
    {
      #region Public Properties
      #endregion

      #region Private / Protected
      private readonly ICommand command;
      private readonly Type type;
      #endregion

      #region Constructors
      public TypeCommandPair(ICommand command, Type type)
      {
        Contract.Requires(command != null);
        Contract.Requires(type != null);

        this.command = command;
        this.type = type;
      }
      #endregion

      #region Methods
      [ContractInvariantMethod]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
      private void ObjectInvariant()
      {
        Contract.Invariant(command != null);
        Contract.Invariant(type != null);
      }

      public static bool operator ==(TypeCommandPair first, TypeCommandPair second)
      {
        return first.Equals(second);
      }

      public static bool operator !=(TypeCommandPair first, TypeCommandPair second)
      {
        return !first.Equals(second);
      }

      public bool Equals(TypeCommandPair other)
      {
        return type == other.type
            && command == other.command;
      }

      public override bool Equals(object obj)
      {
        return obj is TypeCommandPair && Equals((TypeCommandPair)obj);
      }

      public override int GetHashCode()
      {
        return type.GetHashCode() ^ command.GetHashCode();
      }
      #endregion
    }
  }
}