using System;
using Drafts;
using Drafts.Database;
using Drafts.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Tilemaps;

namespace TileGrid.Tiles
{
    [CreateAssetMenu(menuName = "Tiles/DbAutoTile")]
    public class DbAutoTile : AutoTile, ITileEntry
    {
        [SerializeField, ReadOnly] private uint id;
        [SerializeField, Loc] private LocalizedString displayName;
        [SerializeField, Loc] private LocalizedString description;
        [SerializeField] private Sprite icon;
        [SerializeField, SO] private DbAutoTile roof;
        [SerializeField] private BuildLayerMask layerMask;
        [SerializeField] private TileFlags flags = TileFlags.LockTransform;

        public uint Id => id;
        public string DisplayName => displayName.Localize(this);
        public string Description => description.Localize(this);
        public Sprite Icon => icon ? icon : m_DefaultSprite;
        public ITileEntry Roof => roof;
        public BuildLayerMask LayerMask => layerMask;
        TileBase ITileEntry.Tile => this;

        public bool CanReplace(ITileEntry other) => other is DbAutoTile;
        protected override bool IsMatch(TileBase other) => other is AutoTile;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.flags = flags;
        }

        [ContextMenu("Reroll Id")]
        private void RerollId() => id = UIntGenerator.Instance.Generate();

        private void Reset()
        {
            RerollId();
            m_MaskType = AutoTileMaskType.Mask_3x3;
            m_DefaultColliderType = Tile.ColliderType.Grid;
        }

        private void OnValidate()
        {
            if (!roof) return;
            layerMask = BuildLayerMask.World;
            m_DefaultColliderType = Tile.ColliderType.Grid;
            roof.layerMask = BuildLayerMask.Roof;
            roof.m_DefaultColliderType = Tile.ColliderType.None;
        }
    }
}