using System.Collections.Generic;
using Drafts;
using GuildMasterIsekai.Systems;
using TileGrid.Tiles;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/FloorTile")]
    public class FloorTileSo : BaseTileSo
    {
        [SerializeField, SO] private DbAutoTile floor;
        public override ITileEntry Tile => floor;

        public override Sprite Icon => icon ? icon : floor.m_DefaultSprite;

        public override IEnumerable<ITileEntry> GetEntries()
        {
            if (!floor) Debug.LogError("missing tiels", this);
            yield return floor;
        }

        private void OnValidate()
        {
            floor?.EditorSet("item", this);
            floor?.EditorSet("type", AutoTileType.Floor);
        }
    }
}