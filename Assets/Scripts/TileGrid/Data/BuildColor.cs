using System;
using Drafts.Inventory;
using UnityEngine;

namespace TileGrid.Build
{
    [Serializable]
    public class BuildColor : ISlot
    {
        public object Item => this;
        public int Amount => -1;
        public bool Favorite => true;
        event Action<object> ISlot.OnItemChanged { add { } remove { } }
        event Action<int> ISlot.OnAmountChanged{ add { } remove { } }
        event Action<bool> ISlot.OnFavoriteChanged{ add { } remove { } }

        public Color GetColor()
        {
            throw new NotImplementedException();
        }
    }
}