using System;
using System.Collections.Generic;
using Drafts.Inventory;
using UnityEngine.Scripting;

namespace GuildMasterIsekai.TileGrid
{
    [Serializable, Preserve]
    public sealed class BaseLayerPickTool : IBuildTool
    {
        public GridBuilder Builder { get; set; }

        public IEnumerable<ISlot> GetItems() => throw new System.InvalidOperationException();

        public IBuildCommand CreateCommand(OperationArgs e, out string error)
        {
            //TODO
            var item = e.grid[e.area.xMax, e.area.yMax].Floor;
            if (item) e.builder.SelectedItem = item;
            error = "";
            return null;
        }
    }
}