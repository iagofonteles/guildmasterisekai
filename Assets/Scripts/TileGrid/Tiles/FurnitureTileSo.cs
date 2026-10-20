using System.Collections.Generic;
using Drafts;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/FurnitureTile")]
    public class FurnitureTileSo : DatabaseEntrySo, ITileSo
    {
        [SerializeField, SO] private DbTile tile;
        [SerializeField] private AutoTileType buildOn;
        [SerializeField] private Vector2[] toppingSlots;

        public int Layer => 1;
        public DbTile Tile => tile;
        public AutoTileType BuildOn => buildOn;
        public IReadOnlyList<Vector2> ToppingSlots => toppingSlots;

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