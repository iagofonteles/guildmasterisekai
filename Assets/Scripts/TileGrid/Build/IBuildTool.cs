using System.Collections.Generic;
using Drafts.Inventory;
using TileGrid.Tiles;
using UnityEngine;

namespace TileGrid.Build
{
    /// <summary>Prepares a grid command or performs a selection-only operation.</summary>
    public interface IBuildTool
    {
        Sprite Icon => null;
        BuildLayerMask LayerMask { get; }

        IEnumerable<ISlot> GetItems(GestureArgs e)
        {
            foreach (var slot in e.bag.GetSlots<ITile>())
                if (slot.Item is ITile t && ((int)t.LayerMask & e.layer) > 0)
                    yield return slot;
        }

        /// <returns>Null for selection-only success or failure; a non-null error indicates failure.</returns>
        IBuildCommand CreateCommand(GestureArgs e);
    }
}