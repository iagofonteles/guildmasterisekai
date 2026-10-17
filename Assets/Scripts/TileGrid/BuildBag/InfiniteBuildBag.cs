using System.Collections.Generic;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    /// <summary>Optional scene dependency for testing without an inventory.</summary>
    public sealed class InfiniteBuildBag : MonoBehaviour, IBuildBag
    {
        public int GetBuildingMaterials() => int.MaxValue;
        public IEnumerable<BaseTileSo> GetUnlockedTiles() => Game.Database.GetAll<BaseTileSo>();

        public IEnumerable<(T, int)> GetAvailableItems<T>()
        {
            foreach (var item in Game.Database.GetAll<T>())
                yield return (item, int.MaxValue);
        }

        public bool CanPay(OperationCost cost) => true;
        public bool CanRefund(OperationCost cost) => true;
        public void Pay(OperationCost cost) { }
        public void Refund(OperationCost cost) { }
    }
}