using System;
using UnityEngine;

namespace GuildMasterIsekai
{
    public interface IGuid : IComparable<IGuid>, IComparable<string>
    {
        string Guid { get; }

        int IComparable<IGuid>.CompareTo(IGuid other)
            => Guid.CompareTo(other.Guid);

        int IComparable<string>.CompareTo(string other)
            => Guid.CompareTo(other);
    }

    public interface IDisplay
    {
        uint Id { get; }
        string DisplayName { get; }
        string Description => "";
        Sprite Icon { get; }
        event Action OnChanged;
    }
}