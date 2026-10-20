using Drafts;
using Drafts.Database;
using Drafts.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Tilemaps;

namespace TileGrid.Tiles
{
    [CreateAssetMenu(menuName = "Tiles/DbTile")]
    public class DbTile : Tile, ITileEntry
    {
        [SerializeField, ReadOnly] private uint id;
        [SerializeField, Loc] private LocalizedString displayName;
        [SerializeField, Loc] private LocalizedString description;
        [SerializeField] private Sprite icon;
        [SerializeField] private BuildLayerMask layerMask;

        public uint Id => id;
        public string DisplayName => displayName.Localize(this);
        public string Description => description.Localize(this);
        public Sprite Icon => icon ? icon : sprite;
        TileBase ITileEntry.Tile => this;
        public BuildLayerMask LayerMask => layerMask;

        [ContextMenu("Reroll Id")]
        private void RerollId() => id = UIntGenerator.Instance.Generate();

        private void Reset() => flags = TileFlags.None;
    }
}