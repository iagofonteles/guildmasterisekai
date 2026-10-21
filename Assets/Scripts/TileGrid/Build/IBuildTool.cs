using System.Collections.Generic;
using Drafts.Inventory;
using TileGrid.Tiles;
using UnityEngine;

namespace TileGrid.Build
{
    /// <summary>Prepares a grid command or performs a selection-only operation.</summary>
    public interface IBuildTool
    {
        BuildLayerMask LayerMask { get; }
        public string DisplayName => null;
        public Sprite Icon =>  null;
        public Sprite AltIcon =>  null;
        
        IEnumerable<ISlot> GetItems(BuildArgs e)
        {
            foreach (var slot in e.bag.GetSlots<ITile>())
                if (slot.Item is ITile t && ((int)t.LayerMask & (1 << e.layer.Value)) > 0)
                    yield return slot;
        }

        /// <returns>Null for selection-only success or failure; a non-null error indicates failure.</returns>
        IBuildCommand CreateCommand(BuildArgs e);
    }
}