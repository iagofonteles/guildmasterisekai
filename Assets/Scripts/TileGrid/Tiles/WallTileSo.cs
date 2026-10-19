using System.Collections.Generic;
using Drafts;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/WallTile")]
    public class WallTileSo : BaseTileSo
    {
        [SerializeField, SO] private DbAutoTile wall;
        [SerializeField, SO] private DbAutoTile roof;
        public DbAutoTile Wall => wall;
        public DbAutoTile Roof => roof;

        public override Sprite Icon => icon ? icon : wall.m_DefaultSprite;
        
        public override IEnumerable<ITileEntry> GetEntries()
        {
            if (!wall || !roof) Debug.LogError("missing tiels", this);
            yield return wall;
            yield return roof;
        }

        private void OnValidate()
        {
            wall?.EditorSet("item", this);
            wall?.EditorSet("type", AutoTileType.Wall);
            roof?.EditorSet("item", this);
            roof?.EditorSet("type", AutoTileType.Roof);
        }
    }
}