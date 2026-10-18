using System.Collections.Generic;
using Drafts;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/BaseTile")]
    public class FloorTileSo : BaseTileSo
    {
        [SerializeField, SO] private DbAutoTile floor;
        public DbAutoTile Floor => floor;

        public override IEnumerable<ITileEntry> GetEntries()
        {
            if (!floor) Debug.LogError("missing tiels", this);
            yield return floor;
        }
        
        private void OnValidate()
        {
            if (floor && (floor.Type != AutoTileType.Floor || floor.Item != this))
                floor.__setup(this, AutoTileType.Floor);
        }
    }
}