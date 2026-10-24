using GuildMasterIsekai.Model;
using TileGrid.UI;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.Interactables
{
    [UxmlElement]
    public partial class QuestStripUI : ViewElement<Quest>
    {
        private readonly Label _rank = new();
        private readonly Image _icon = new();
        private readonly Label _name = new();
        private readonly Label _power = new();
        private readonly Label _knowledge = new();
        private readonly Label _vigor = new();
        private readonly Label _fee = new();
        private readonly Button _expand;
        private readonly VisualElement _details = new();
        private Quest _bound;

        public QuestStripUI()
        {
            AddToClassList("quest-strip");
            var header = new VisualElement();
            header.AddToClassList("quest-header");
            Add(header);
            _icon.AddToClassList("quest-icon");
            _name.AddToClassList("quest-name");
            _power.AddToClassList("quest-power");
            _knowledge.AddToClassList("quest-knowledge");
            _vigor.AddToClassList("quest-vigor");
            _power.tooltip = "Power";
            _knowledge.tooltip = "Knowledge";
            _vigor.tooltip = "Vigor";
            _expand = new Button(() => SetExpanded(!_details.ClassListContains("expanded")));
            _expand.tooltip = "Show or hide description and rewards";
            foreach (var element in new VisualElement[] { _rank, _icon, _name, _power, _knowledge, _vigor, _fee, _expand })
                header.Add(element);
            _details.AddToClassList("quest-details");
            Add(_details);
            SetExpanded(false);
            RegisterCallback<AttachToPanelEvent>(_ => { if (Data != null) Refresh(); });
            RegisterCallback<DetachFromPanelEvent>(_ => Unsubscribe());
        }

        protected override void Subscribe()
        {
            _bound = Data;
            _bound.OnChanged += Refresh;
        }

        protected override void Unsubscribe()
        {
            if (_bound == null) return;
            _bound.OnChanged -= Refresh;
            _bound = null;
        }

        public override void Refresh()
        {
            var changed = !ReferenceEquals(_bound, Data);
            Unsubscribe();
            if (changed) SetExpanded(false);
            _details.Clear();
            if (Data == null) return; // Unbound virtualized rows have no data.
            Subscribe();
            _rank.text = Data.Rank.ToString();
            _icon.sprite = Data.Icon;
            _name.text = Data.DisplayName;
            _power.text = Data.Requisite.Power.ToString();
            _knowledge.text = Data.Requisite.Knowledge.ToString();
            _vigor.text = Data.Requisite.Vigor.ToString();
            _fee.text = $"$ {Data.Fee:N0}";
            _details.Add(new Label(Data.Description));
            _details.Add(new Label($"Reward: {Data.Reward.Gold:N0} gold"));
            foreach (var item in Data.Reward.Items)
                _details.Add(new Label(item.DisplayName) { tooltip = item.Description });
        }

        private void SetExpanded(bool expanded)
        {
            _details.EnableInClassList("expanded", expanded);
            _expand.text = expanded ? "▲" : "▼";
        }

        public override void SetSelected(object selected) { }
    }
}
