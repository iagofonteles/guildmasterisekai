using System.Collections.Generic;
using Drafts;
using GuildMasterIsekai.Systems;
using TileGrid.Tiles;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/FurnitureTile")]
    public class FurnitureTileSo : DatabaseEntrySo, ITileSo, ITileProvider
    {
        [SerializeField, SO] private DbTile tile;
        [SerializeField] private AutoTileType buildOn;

        public int Layer => (int)BuildLayer.Furniture;
        public ITileEntry Tile => tile;
        public AutoTileType BuildOn => buildOn;

        public override Sprite Icon => icon ? icon : tile.sprite;
        
        public IEnumerable<ITileEntry> GetEntries()
        {
            if (!tile) Debug.LogError("missing tile", this);
            yield return tile;
        }
        
        private void OnValidate()
        {
            tile?.EditorSet("item", this);
        }
    }
}