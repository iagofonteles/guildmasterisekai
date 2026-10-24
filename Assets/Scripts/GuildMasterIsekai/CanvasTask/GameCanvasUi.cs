using System;
using System.Collections.Generic;
using System.Linq;
using GuildMasterIsekai.Model;
using GuildMasterIsekai.Systems;
using TileGrid.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.CanvasTask
{
    public sealed class GameCanvasUi : ViewElement<GuildHall>
    {
        private readonly GuildView _guildView;
        private readonly ListView<CostomerView> _clients;
        private readonly ListView<ProblemRequestView> _problems;
        private readonly ListView<QuestStripView> _quests;
        private readonly ListView<QuestRequestView> _requests;
        private readonly ListView<AdventurerView> _adventurers;
        private readonly ListView<AdventurerView> _freelancers;
        private readonly ProblemView _problemView;
        private readonly QuestStripView _questView;
        private readonly PartyView _partyView;
        private readonly HashSet<Adventurer> _party = new();
        private Guild _guild;
        private ProblemRequest _selectedProblem;
        private object[] _sources;
        private bool _binding;

        public GameCanvasUi()
        {
            var layout = Resources.Load<VisualTreeAsset>("CanvasTask/GameCanvas");
            if (!layout) throw new InvalidOperationException("GameCanvas layout is missing.");
            layout.CloneTree(this);
            AddToClassList("game-canvas");
            pickingMode = PickingMode.Ignore;
            _guildView = Replace("guildHeader", new GuildView());
            _clients = Replace("clientsList", new ListView<CostomerView>());
            _problems = Replace("problemsList", new ListView<ProblemRequestView>());
            _quests = Replace("questsList", new ListView<QuestStripView>());
            _requests = Replace("questRequestsList", new ListView<QuestRequestView>());
            _adventurers = Replace("adventurersList", new ListView<AdventurerView>());
            _freelancers = Replace("freelancersList", new ListView<AdventurerView>());
            _partyView = Replace("partyList", new PartyView());
            _problemView = new ProblemView();
            this.Q<ScrollView>("solutionsList").Add(_problemView);
            _questView = new QuestStripView { ShowDetailsButton = false };
            this.Q<ScrollView>("questDetails").Add(_questView);

            foreach (var panelName in new[] { "guild", "clients", "problems", "quests", "adventurers", "party", "map", "options" })
            {
                var name = panelName;
                this.Q<Button>(name + "Button").clicked += () => Show(name + "Panel");
                this.Q<Button>(name + "Close").clicked += () => Hide(name + "Panel");
            }
            this.Q<Button>("problemClose").clicked += ClearProblem;
            this.Q<Button>("questClose").clicked += () => { _questView.Data = null; Hide("questPanel"); };
            this.Q<Button>("refuseProblem").clicked += () => ReplyProblem(-1);

            _problems.ConfigureItem = view => view.Selected += SelectProblem;
            _quests.ConfigureItem = view => view.Selected += SelectQuest;
            _requests.ConfigureItem = view => view.Replied += ReplyQuest;
            _adventurers.ConfigureItem = ConfigureAdventurer;
            _freelancers.ConfigureItem = ConfigureAdventurer;
            _problemView.SolutionSelected += ReplyProblem;
            _problemView.QuestSelected += SelectQuest;
            _partyView.Removed += member => { _party.Remove(member); RefreshParty(); };
            _clients.CountChanged += count => Text("clientsButton", $"Clients ({count})");
            _problems.CountChanged += count =>
            {
                Text("problemsButton", $"Problems ({count})");
                if (!_binding && _selectedProblem != null && !Data.ProblemRequests.Contains(_selectedProblem))
                    ClearProblem();
            };
            _requests.CountChanged += count => Text("questsButton", $"Mission board ({count} requests)");
            _adventurers.CountChanged += _ => { if (!_binding) RefreshParty(); };
            _freelancers.CountChanged += _ => { if (!_binding) RefreshParty(); };

            this.Q<TextField>("adventurerSearch").RegisterValueChangedCallback(evt =>
            {
                Predicate<object> filter = item => Matches(((Adventurer)item).DisplayName, evt.newValue);
                _adventurers.Filter = filter;
                _freelancers.Filter = filter;
            });
            this.Q<TextField>("questSearch").RegisterValueChangedCallback(evt =>
                _quests.Filter = item => Matches(((Quest)item).DisplayName, evt.newValue));
            RegisterCallback<DetachFromPanelEvent>(_ => Data = null);
        }

        private T Replace<T>(string name, T view) where T : VisualElement
        {
            var placeholder = this.Q(name);
            view.name = name;
            foreach (var className in placeholder.GetClasses()) view.AddToClassList(className);
            var parent = placeholder.parent;
            var index = parent.IndexOf(placeholder);
            placeholder.RemoveFromHierarchy();
            parent.Insert(index, view);
            return view;
        }

        public void Rebind() => Data = Game.SaveMngr.Get<GuildHall>();

        protected override void Subscribe()
        {
            _binding = true;
            _guild = Game.SaveMngr.Get<Guild>();
            _sources = CurrentSources();
            _guildView.Data = _guild;
            _clients.Data = Data.Costomers;
            _problems.Data = Data.ProblemRequests;
            _quests.Data = Data.QuestBoard;
            _requests.Data = Data.QuestRequests;
            _adventurers.Data = _guild.Adventurers;
            _freelancers.Data = Data.Freelancers;
            this.Q<TextField>("adventurerSearch").SetValueWithoutNotify("");
            this.Q<TextField>("questSearch").SetValueWithoutNotify("");
            _adventurers.Filter = null;
            _freelancers.Filter = null;
            _quests.Filter = null;
            _binding = false;
            RefreshParty();
        }

        protected override void Unsubscribe()
        {
            _binding = true;
            _guildView.Data = null;
            _clients.Data = null;
            _problems.Data = null;
            _quests.Data = null;
            _requests.Data = null;
            _adventurers.Data = null;
            _freelancers.Data = null;
            _partyView.Data = null;
            _questView.Data = null;
            ClearProblem();
            Hide("questPanel");
            _party.Clear();
            _guild = null;
            _sources = null;
            _binding = false;
        }

        public void Tick()
        {
            if (Data == null) return; // Detached canvases are deliberately unbound.
            if (!ReferenceEquals(Data, Game.SaveMngr.Get<GuildHall>()) ||
                !ReferenceEquals(_guild, Game.SaveMngr.Get<Guild>()) ||
                !_sources.SequenceEqual(CurrentSources()))
            {
                Rebind();
                return;
            }
            Refresh();
        }

        private object[] CurrentSources() => new object[]
        {
            _guild.Adventurers, Data.Freelancers, Data.Costomers,
            Data.QuestBoard, Data.QuestRequests, Data.ProblemRequests
        };

        public override void Refresh()
        {
            _guildView.Refresh(); // Guild rank has no change event.
            foreach (var view in _adventurers.Views.Concat(_freelancers.Views)) view.Refresh();
        }

        private void ConfigureAdventurer(AdventurerView view) => view.Selected += member =>
        {
            if (member.assignment != null ||
                (!_guild.Adventurers.Contains(member) && !Data.Freelancers.Contains(member))) return;
            _party.Add(member);
            RefreshParty();
            Show("partyPanel");
        };

        private void RefreshParty()
        {
            _party.RemoveWhere(member => !_guild.Adventurers.Contains(member) && !Data.Freelancers.Contains(member));
            _partyView.Data = _party.ToArray();
        }

        private void SelectProblem(ProblemRequest request)
        {
            if (!Data.ProblemRequests.Contains(request)) return;
            _selectedProblem = request;
            Text("problemClient", request.Costomer?.DisplayName ?? "");
            _problemView.Data = request.Problem;
            this.Q<Button>("refuseProblem").SetEnabled(true);
            Show("problemPanel");
        }

        private void ClearProblem()
        {
            _selectedProblem = null;
            _problemView.Data = null;
            this.Q<Button>("refuseProblem").SetEnabled(false);
            Hide("problemPanel");
        }

        private void ReplyProblem(int index)
        {
            var request = _selectedProblem;
            if (request == null || !Data.ProblemRequests.Contains(request)) return;
            if (index < -1 || (index >= 0 && index >= request.Problem.Outcomes.Length)) return;
            ClearProblem();
            request.Reply(index);
        }

        private void ReplyQuest(QuestRequest request, bool accept)
        {
            if (!Data.QuestRequests.Contains(request)) return;
            if (accept && !Data.QuestBoard.Contains(request.Quest)) return;
            request.Reply(accept ? request.Quest : null);
        }

        private void SelectQuest(Quest quest)
        {
            _questView.Data = quest;
            Show("questPanel");
        }

        private static bool Matches(string value, string search) =>
            string.IsNullOrEmpty(search) || value.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

        private void Text(string name, string text) => this.Q<TextElement>(name).text = text;
        private void Show(string name) => this.Q(name).RemoveFromClassList("hidden");
        private void Hide(string name) => this.Q(name).AddToClassList("hidden");
        public override void SetSelected(object selected) { }
    }
}
