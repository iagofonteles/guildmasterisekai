using Drafts;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/DbAutoTile")]
    public class DbAutoTile : AutoTile, ITileEntry
    {
        [SerializeField, ReadOnly] private uint id;
        [SerializeField] private BaseTileSo item;
        [SerializeField] private AutoTileType type;
        [SerializeField] private TileFlags flags = TileFlags.None;

        public uint Id => id;
        public BaseTileSo Item => item;
        public AutoTileType Type => type;
        TileBase ITileEntry.Tile => this;

        public int Cost => type switch
        {
            AutoTileType.Floor => 1,
            AutoTileType.Wall => 1,
            _ => 0
        };

        public void __setup(BaseTileSo i, AutoTileType t)
        {
            item = i;
            type = t;
        }

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
            do
            {
                id = Game.Id.Generate();
            } while (Game.Tiles.Find(id) != null);
        }
    }
}
