using Drafts;
using Drafts.Database;
using GuildMasterIsekai.Systems;
using TileGrid.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/DbAutoTile")]
    public class DbAutoTile : AutoTile, ITileEntry
    {
        [SerializeField, ReadOnly] private uint id;
        [SerializeField] private ScriptableObject item;
        [SerializeField] private AutoTileType type;
        [SerializeField] private TileFlags flags = TileFlags.LockTransform;

        public uint Id => id;
        public ITileSo Item => (ITileSo)item;
        public AutoTileType Type => type;
        TileBase ITileEntry.Tile => this;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.flags = flags;
        }

        protected override bool IsMatch(TileBase other)
        {
            return other is DbAutoTile a && a.type == type;
        }

        [ContextMenu("Reroll Id")]
        private void RerollId()
        {
            id = UIntGenerator.Instance.Generate();
        }
    }
}
