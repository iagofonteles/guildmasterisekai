using System;
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
        [SerializeField] private Vector2Int size = Vector2Int.one;
        [SerializeField] private BuildLayerEnum layer;
        [SerializeField] private Sprite[] rotations;
        [SerializeField, ReadOnly] private Sprite[] encodedRotations;

        public uint Id => id;
        public string DisplayName => displayName.Localize(this);
        public string Description => description.Localize(this);
        public Sprite Icon => icon ? icon : rotations[0];
        public Vector2Int Size => size;
        public int Layer => (int)layer;
        TileBase ITileEntry.Tile => this;

        public override void GetTileData(Vector3Int pos, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(pos, tilemap, ref tileData);
            tileData.color = tilemap.GetColor(pos);
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                tileData.color = Color.clear;
                tileData.flags |= TileFlags.LockColor;
            }
#endif
            var matrix = tilemap.GetTransformMatrix(pos);
            var sprites = tileData.color == Color.clear ? rotations : encodedRotations;
            var index = Mathf.RoundToInt(matrix.rotation.x) % sprites.Length;
            tileData.sprite = sprites[index];
        }

        public bool CanReplace(ITileEntry other) => other is DbTile t && t.size == size;

        private void Reset()
        {
            __RerollId();
            flags = TileFlags.None;
            colliderType = ColliderType.None;
        }

        [ContextMenu("Reroll Id")]
        public void __RerollId() => id = UIntGenerator.Instance.Generate();

        public override void OnValidate()
        {
            base.OnValidate();
            if (rotations.Length < 1)
                rotations = new Sprite[1];
        }

#if UNITY_EDITOR
        public static Action<DbTile> __EncodeSprites;
        public static Action<DbTile> __GenerateMask;
        [ContextMenu("Encode Sprites")] private void __EncodeSprites_() => __EncodeSprites(this);
        [ContextMenu("Generate Mask")] private void __GenerateMask_() => __GenerateMask(this);
#endif
    }
}