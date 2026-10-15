using System;
using UnityEngine;

namespace GuildMasterIsekai.System
{
    public interface IDisplay
    {
        uint Id { get; }
        string DisplayName { get; }
        string Description => "";
        Sprite Icon { get; }
        event Action OnChanged;
    }
}