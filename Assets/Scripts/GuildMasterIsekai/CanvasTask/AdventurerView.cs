using System;
using GuildMasterIsekai.Database;
using GuildMasterIsekai.Model;
using TileGrid.UI;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.CanvasTask
{
    public sealed class AdventurerView : ViewElement<Adventurer>
    {
        private readonly Label _title = new();
        private readonly StatsView _stats = new();
        private readonly Button _action;
        public event Action<Adventurer> Selected;
        public bool PartyMember { get; set; }

        public AdventurerView()
        {
            AddToClassList("card");
            Add(_title);
            Add(_stats);
            _action = new Button(() => Selected?.Invoke(Data));
            Add(_action);
        }

        protected override void Subscribe()
        {
            Data.OnChanged += Refresh;
            Data.Rank.OnChanged += RankChanged;
            _stats.Data = Data.Stats;
        }

        protected override void Unsubscribe()
        {
            Data.OnChanged -= Refresh;
            Data.Rank.OnChanged -= RankChanged;
            _stats.Data = null;
        }

        private void RankChanged(Rank rank) => Refresh();

        public override void Refresh()
        {
            _title.text = Data == null ? "" : $"{Data.DisplayName} - Rank {Data.Rank.Value}";
            _action.text = PartyMember ? "Remove" : "Add to party";
            _action.SetEnabled(Data != null && (PartyMember || Data.assignment == null));
        }

        public override void SetSelected(object selected) =>
            EnableInClassList("selected", ReferenceEquals(Data, selected));
    }
}
