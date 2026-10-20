using System;
using System.Collections.Generic;
using Drafts.Inventory;
using TileGrid.Build;
using TileGrid.Tiles;
using UnityEngine;
using UnityEngine.Scripting;

namespace GuildMasterIsekai.TileGrid
{
    [Serializable, Preserve]
    public sealed class TilePickTool : IBuildTool
    {
        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;
        public BuildLayerMask LayerMask => BuildLayerMask.All;

        public IEnumerable<ISlot> GetItems(GestureArgs e) => null;

        public IBuildCommand CreateCommand(GestureArgs e)
        {
            var pos = e.area.max - Vector2Int.one;
            var cell = e.grid[pos.x, pos.y];
            var curr = cell.Tiles[e.layer];
            if (curr == null) return null;

            e.item = curr;
            return null;
        }
    }
}