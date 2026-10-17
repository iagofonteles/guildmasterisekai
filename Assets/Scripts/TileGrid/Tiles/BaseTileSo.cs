using System.Collections.Generic;
using Drafts;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/BaseTile")]
    public class BaseTileSo : DatabaseEntrySo, ITileSo
    {
        [SerializeField, SO] private DbAutoTile floor;
        [SerializeField, SO] private DbAutoTile wall;
        [SerializeField, SO] private DbAutoTile roof;

        public int Layer => 0;
        public DbAutoTile Floor => floor;
        public DbAutoTile Wall => wall;
        public DbAutoTile Roof => roof;
        public int Cost => 1;

        public DbAutoTile this[AutoTileType t] => t switch
        {
            AutoTileType.Floor => Floor,
            AutoTileType.Wall => Wall,
            AutoTileType.Roof => Roof,
            _ => null
        };

        private void OnValidate()
        {
            if (floor && (floor.Type != AutoTileType.Floor || floor.Item != this))
                floor.__setup(this, AutoTileType.Floor);
            if (wall && (wall.Type != AutoTileType.Wall || wall.Item != this))
                wall.__setup(this, AutoTileType.Wall);
            if (roof && (roof.Type != AutoTileType.Roof || roof.Item != this))
                roof.__setup(this, AutoTileType.Roof);
        }

        public IEnumerable<ITileEntry> GetEntries()
        {
            if (!floor || !wall || !roof) Debug.LogError("missing tiels", this);
            yield return floor;
            yield return wall;
            yield return roof;
        }
    }
}