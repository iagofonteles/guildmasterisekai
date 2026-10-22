using System;
using Drafts.Inventory;
using UnityEngine;

namespace TileGrid.Build
{
    [Serializable]
    public class BuildColor : ISlot
    {
        [SerializeField] private string name;
        [SerializeField] private uint id;
        [SerializeField] private byte region1Ramp;
        [SerializeField] private byte region2Ramp;
        [SerializeField] private byte region3Ramp;
        [SerializeField] private byte region4Ramp;

        public string Name => name;
        public uint Id => id;
        public byte Region1Ramp => region1Ramp;
        public byte Region2Ramp => region2Ramp;
        public byte Region3Ramp => region3Ramp;
        public byte Region4Ramp => region4Ramp;

        public object Item => this;
        public int Amount => -1;
        public bool Favorite => true;
        event Action<object> ISlot.OnItemChanged { add { } remove { } }
        event Action<int> ISlot.OnAmountChanged { add { } remove { } }
        event Action<bool> ISlot.OnFavoriteChanged { add { } remove { } }

        public BuildColor(string name, uint id,
            byte region1Ramp = 0, byte region2Ramp = 0,
            byte region3Ramp = 0, byte region4Ramp = 0)
        {
            this.name = name;
            this.id = id;
            this.region1Ramp = region1Ramp;
            this.region2Ramp = region2Ramp;
            this.region3Ramp = region3Ramp;
            this.region4Ramp = region4Ramp;
        }

        // Each channel selects a shared ramp row; zero preserves original colors.
        // Alpha selects region 4's ramp, not opacity. Do not apply gamma conversion.
        public Color GetColor() => new Color32(region1Ramp, region2Ramp, region3Ramp, region4Ramp);
    }
}