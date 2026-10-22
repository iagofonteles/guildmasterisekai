using System;
using System.Collections.Generic;
using Drafts.Inventory;
using Drafts.Localization;
using TileGrid.Build;
using TileGrid.Data;
using TileGrid.Tiles;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Scripting;
using static UnityEngine.UIElements.MouseButton;

namespace TileGrid.BuildTools
{
    [Serializable, Preserve]
    public sealed class ColorTool : IBuildTool
    {
        [SerializeField, Loc] private LocalizedString str;
        [SerializeField] private Sprite icon;
        [SerializeField] private Sprite altIcon;

        private ColorPickTool _pick = new();

        public BuildLayerMask LayerMask => BuildLayerMask.All;
        public string DisplayName => str.GetLocalizedString();
        public Sprite Icon => icon;
        public Sprite AltIcon => altIcon;

        public IEnumerable<ISlot> GetItems(BuildArgs e) => e.bag.GetSlots<Color>();

        public IBuildCommand CreateCommand(BuildArgs e)
        {
            if (e.button is not RightMouse and not LeftMouse)
                return null;
            if (e.alt) return _pick.CreateCommand(e);

            if (e.item.Value is Color color2)
                e.color.Value = color2;

            var cmd = new CellCommand(e.builder);
            var tgt = e.button == LeftMouse ? e.color.Value : Color.clear;

            foreach (var p in e.area.allPositionsWithin)
            {
                var cell = e.grid[p.x, p.y];
                var curr = cell.Tiles[e.layer.Value];
                if (curr == null) continue;
                if (cell.Colors[e.layer.Value] == tgt) continue;

                var after = cell.Clone();
                after.Colors[e.layer.Value] = tgt;
                after.Colors[BuildLayer.Roof] = tgt;
                
                cmd.after.Add(after);
                cmd.before.Add(cell.Clone());
            }

            if (cmd.after.Count == 0)
                return e.Error("no change");
            return cmd;
        }
    }
}