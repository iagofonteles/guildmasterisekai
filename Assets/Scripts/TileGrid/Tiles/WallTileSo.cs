using System.Collections.Generic;
using Drafts;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/BaseTile")]
    public class WallTileSo : BaseTileSo
    {
        [SerializeField, SO] private DbAutoTile wall;
        [SerializeField, SO] private DbAutoTile roof;
        public DbAutoTile Wall => wall;
        public DbAutoTile Roof => roof;

        public override IEnumerable<ITileEntry> GetEntries()
        {
            if (!wall || !roof) Debug.LogError("missing tiels", this);
            yield return wall;
            yield return roof;
        }

        private void OnValidate()
        {
            if (wall && (wall.Type != AutoTileType.Wall || wall.Item != this))
                wall.__setup(this, AutoTileType.Wall);
            if (roof && (roof.Type != AutoTileType.Roof || roof.Item != this))
                roof.__setup(this, AutoTileType.Roof);
        }
    }
}