using System;
using System.Collections.Generic;
using Drafts.Inventory;
using GuildMasterIsekai.Systems;
using TileGrid.Build;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    public sealed class InventoryBuildBag : MonoBehaviour, IBuildBag
    {
        public DatabaseEntrySo buildingMaterial;
        public ListInventory<DatabaseEntrySo> inventory;
        public List<BuildColor> colors;
        private bool _applying;

        private void Start()
        {
            inventory.GetStackSize = _ => int.MaxValue;
        }

        public int GetBuildingMaterials() => inventory.Count(buildingMaterial);

        public IEnumerable<ISlot> GetSlots<T>()
        {
            if (typeof(T) == typeof(BuildColor))
            {
                foreach (var color in colors)
                    yield return color;
                yield break;
            }

            foreach (var slot in inventory)
                if (slot.Item is T)
                    yield return slot;
        }

        public bool CanPay(OperationCost cost) => CanApply(cost, false);
        public bool CanRefund(OperationCost cost) => CanApply(cost, true);
        public void Pay(OperationCost cost) => Apply(cost, false);
        public void Refund(OperationCost cost) => Apply(cost, true);

        private bool CanApply(OperationCost cost, bool refund)
        {
            if (_applying || cost == null) return false;

            foreach (var pair in cost.Items)
            {
                if (pair.Key is not DatabaseEntrySo item || !item) return false;
                var delta = refund ? pair.Value : -(long)pair.Value;
                var count = inventory.Count(item) + delta;
                if (count < 0 || count > int.MaxValue || delta > int.MaxValue || delta < -int.MaxValue ||
                    (delta > 0 && inventory.Fits(item) < delta)) return false;
            }

            return true;
        }

        private void Apply(OperationCost cost, bool refund)
        {
            if (!CanApply(cost, refund))
                throw new InvalidOperationException("Insufficient items or inventory capacity.");
            _applying = true;

            try
            {
                foreach (var pair in cost.Items)
                {
                    var item = (DatabaseEntrySo)pair.Key;
                    var delta = refund ? pair.Value : checked(-pair.Value);
                    if (delta > 0) inventory.Add(item, delta);
                    else if (delta < 0) inventory.Remove(item, -delta);
                }
            }
            finally
            {
                _applying = false;
            }
        }
    }
}
