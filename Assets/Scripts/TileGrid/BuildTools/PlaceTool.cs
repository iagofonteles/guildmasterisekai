using System;
using Drafts.Localization;
using TileGrid.Build;
using TileGrid.Data;
using TileGrid.Tiles;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace TileGrid.BuildTools
{
    [Preserve, Serializable]
    public sealed class PlaceTool : IBuildTool
    {
        [SerializeField, Loc] private LocalizedString str;
        [SerializeField] private Sprite icon;
        [SerializeField] private Sprite altIcon;

        private RemoveTool _remove = new();
        private TilePickTool _pick = new();

        public BuildLayerMask LayerMask => BuildLayerMask.All;
        public string DisplayName => str.GetLocalizedString();
        public Sprite Icon => icon;
        public Sprite AltIcon => altIcon;

        public IBuildCommand CreateCommand(BuildArgs e)
        {
            if (e.button == MouseButton.RightMouse)
                return _remove.CreateCommand(e);
            if(e.alt) return _pick.CreateCommand(e);

            if (e.button != MouseButton.LeftMouse)
                return null;
            
            if (e.item.Value is not ITileEntry tile)
                return e.Error("not ITileEntry");
            if (((int)tile.LayerMask & (1 << e.layer.Value)) == 0)
                return e.Error("incompatible layer");

            if (tile is DbTile t && t.Size != Vector2Int.one)
                return PlaceSizedTile(e, t);

            var cmd = new CellCommand(e.builder);
            var cost = 0;

            foreach (var p in e.area.allPositionsWithin)
            {
                var cell = e.grid[p.x, p.y];
                var curr = cell.Tiles[e.layer.Value];
                if (curr == tile) continue;

                var after = cell.Clone();
                after.Tiles[e.layer.Value] = tile;
                after.Colors[e.layer.Value] = e.color.Value;
                after.Tiles[BuildLayer.Roof] = tile.Roof;
                after.Colors[BuildLayer.Roof] = e.color.Value;

                cmd.after.Add(after);
                cmd.before.Add(cell.Clone());

                if (curr == null) cost++;
            }

            if (cmd.after.Count == 0)
                return e.Error("no change");

            cmd.cost.AddItem(e.builder.Material, cost);
            return cmd;
        }

        private IBuildCommand PlaceSizedTile(BuildArgs buildArgs, DbTile dbTile)
        {
            throw new NotImplementedException();
        }
    }
}