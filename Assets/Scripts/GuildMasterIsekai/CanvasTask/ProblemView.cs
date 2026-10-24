using System;
using System.Collections.Generic;
using System.Linq;
using GuildMasterIsekai.Model;
using TileGrid.UI;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.CanvasTask
{
    public sealed class ProblemView : ViewElement<Problem>
    {
        private readonly List<QuestStripView> _quests = new();
        public event Action<int> SolutionSelected;
        public event Action<Quest> QuestSelected;

        protected override void Subscribe() => Data.OnChanged += Refresh;
        protected override void Unsubscribe()
        {
            Data.OnChanged -= Refresh;
            ReleaseQuests();
        }

        private void ReleaseQuests()
        {
            foreach (var quest in _quests) quest.Data = null;
            _quests.Clear();
        }

        public override void Refresh()
        {
            ReleaseQuests();
            Clear();
            if (Data == null) return;
            Add(new Label(Data.DisplayName) { name = "problemTitle" });
            Add(new Label(Data.Description) { name = "problemDescription" });
            var outcomes = Data.Outcomes;
            for (var i = 0; i < outcomes.Length; i++)
            {
                var index = i;
                var group = new VisualElement();
                group.AddToClassList("card");
                group.Add(new Label($"Solution {i + 1} - Total fee: {outcomes[i].Sum(quest => quest.Fee):N0}"));
                foreach (var quest in outcomes[i])
                {
                    var view = new QuestStripView();
                    view.Selected += selected => QuestSelected?.Invoke(selected);
                    _quests.Add(view);
                    group.Add(view);
                    view.Data = quest;
                }
                group.Add(new Button(() => SolutionSelected?.Invoke(index)) { text = "Confirm solution" });
                Add(group);
            }
        }

        public override void SetSelected(object selected) { }
    }
}
