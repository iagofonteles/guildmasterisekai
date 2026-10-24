using System;
using System.Linq;
using GuildMasterIsekai.Model;
using TileGrid.UI;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.CanvasTask
{
    public sealed class PartyView : ViewElement<Adventurer[]>
    {
        private readonly ListView<AdventurerView> _members = new();
        private readonly Label _totals = new() { name = "partyTotals" };
        public event Action<Adventurer> Removed;

        public PartyView()
        {
            AddToClassList("list");
            _members.ConfigureItem = view =>
            {
                view.PartyMember = true;
                view.Selected += member => Removed?.Invoke(member);
            };
            Add(_members);
            Add(_totals);
        }

        protected override void Subscribe()
        {
            foreach (var member in Data)
            {
                member.Stats.Power.OnChanged += StatsChanged;
                member.Stats.Knowledge.OnChanged += StatsChanged;
                member.Stats.Vigor.OnChanged += StatsChanged;
            }
        }

        protected override void Unsubscribe()
        {
            foreach (var member in Data)
            {
                member.Stats.Power.OnChanged -= StatsChanged;
                member.Stats.Knowledge.OnChanged -= StatsChanged;
                member.Stats.Vigor.OnChanged -= StatsChanged;
            }
            _members.Data = null;
        }

        private void StatsChanged(int value) => RefreshTotals();

        public override void Refresh()
        {
            _members.Data = Data;
            RefreshTotals();
        }

        private void RefreshTotals() => _totals.text = Data == null ? "" :
            $"Party: {Data.Length} | Power: {Data.Sum(member => member.Stats.Power.Value)} | " +
            $"Knowledge: {Data.Sum(member => member.Stats.Knowledge.Value)} | Vigor: {Data.Sum(member => member.Stats.Vigor.Value)}";

        public override void SetSelected(object selected) { }
    }
}
