using System;
using GuildMasterIsekai.Model;
using TileGrid.UI;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.CanvasTask
{
    public sealed class QuestRequestView : ViewElement<QuestRequest>
    {
        private readonly Label _summary = new();
        public event Action<QuestRequest, bool> Replied;

        public QuestRequestView()
        {
            AddToClassList("card");
            Add(_summary);
            Add(new Button(() => Replied?.Invoke(Data, true)) { text = "Accept" });
            Add(new Button(() => Replied?.Invoke(Data, false)) { text = "Refuse" });
        }

        protected override void Subscribe()
        {
            if (Data.Adventurer != null) Data.Adventurer.OnChanged += Refresh;
            if (Data.Quest != null) Data.Quest.OnChanged += Refresh;
        }

        protected override void Unsubscribe()
        {
            if (Data.Adventurer != null) Data.Adventurer.OnChanged -= Refresh;
            if (Data.Quest != null) Data.Quest.OnChanged -= Refresh;
        }

        public override void Refresh() =>
            _summary.text = Data == null ? "" : $"{Data.Adventurer?.DisplayName}: {Data.Quest?.DisplayName}";

        public override void SetSelected(object selected) { }
    }
}
