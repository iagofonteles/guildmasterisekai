using System.Collections.Generic;
using System.Linq;
using Drafts.Inventory;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    /// <summary>Optional scene dependency for testing without an inventory.</summary>
    public sealed class InfiniteBuildBag : MonoBehaviour, IBuildBag
    {
        public int GetBuildingMaterials() => int.MaxValue;

        private ListInventory<DatabaseEntrySo> _inventory;

        private static IEnumerable<Slot<DatabaseEntrySo>> FetchItems()
            => Game.Database.Items.Select(i => new Slot<DatabaseEntrySo>(i, -1, true));

        public IEnumerable<ISlot> GetItems<T>() where T : DatabaseEntrySo
        {
            _inventory ??= new(FetchItems(), _ => int.MaxValue);
            foreach (var slot in _inventory)
                if (slot.Item is T)
                    yield return slot;
        }

        public bool CanPay(OperationCost cost) => true;
        public bool CanRefund(OperationCost cost) => true;
        public void Pay(OperationCost cost) { }
        public void Refund(OperationCost cost) { }
    }
}