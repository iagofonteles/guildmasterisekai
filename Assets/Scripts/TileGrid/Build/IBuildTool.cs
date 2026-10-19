using System.Collections.Generic;
using Drafts.Inventory;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    /// <summary>Prepares a grid command or performs a selection-only operation.</summary>
    public interface IBuildTool
    {
        Sprite Icon => null;
        IEnumerable<ISlot> GetItems(IBuildBag bag);

        /// <returns>Null for selection-only success or failure; a non-null error indicates failure.</returns>
        IBuildCommand CreateCommand(OperationArgs e, out string error);
    }
}