using System;
using GuildMasterIsekai.Model;
using TileGrid.UI;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.CanvasTask
{
    public sealed class QuestStripView : ViewElement<Quest>
    {
        private readonly Image _icon = new();
        private readonly Label _title = new();
        private readonly Label _description = new();
        private readonly Label _summary = new();
        private readonly Label _rewards = new();
        private readonly Button _details;
        public event Action<Quest> Selected;
        public bool ShowDetailsButton { get; set; } = true;

        public QuestStripView()
        {
            AddToClassList("card");
            _icon.AddToClassList("portrait");
            Add(_icon);
            Add(_title);
            Add(_description);
            Add(_summary);
            Add(_rewards);
            _details = new Button(() => Selected?.Invoke(Data)) { text = "Details" };
            Add(_details);
        }

        protected override void Subscribe()
        {
            Data.OnChanged += Refresh;
            _icon.sprite = Data.Icon;
        }

        protected override void Unsubscribe() => Data.OnChanged -= Refresh;

        public override void Refresh()
        {
            _title.text = Data == null ? "" : $"{Data.DisplayName} (#{Data.Id})";
            _description.text = Data == null ? "" : Data.Description;
            _summary.text = Data == null ? "" :
                $"Rank {Data.Rank} | Fee: {Data.Fee:N0} | Reward: {Data.Reward.Gold:N0} gold\n{StatsView.Summary(Data.Requisite)}";
            _rewards.text = Data == null ? "" : string.Join(", ", System.Linq.Enumerable.Select(Data.Reward.Items, item => item.DisplayName));
            _details.EnableInClassList("hidden", !ShowDetailsButton || Data == null);
            if (Data == null) _icon.sprite = null;
        }

        public override void SetSelected(object selected) { }
    }
}
