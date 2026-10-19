using System;
using System.Collections.Generic;
using System.Linq;
using GuildMasterIsekai.Model;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.CanvasTask
{
    public sealed class GameCanvasUi : MonoBehaviour
    {
        private readonly List<IDisposable> _bindings = new();
        private UIDocument _document;
        private PanelSettings _ownedPanel;
        private VisualElement _root;
        private Guild _guild;
        private GuildHall _hall;
        private ProblemRequest _selectedProblem;
        private UiBinding<ProblemRequest> _problemBinding;
        private UiCollectionBinding<Quest> _quests;
        private UiCollectionBinding<Adventurer> _adventurers;
        private bool _started;
        private object[] _sources;
        private readonly HashSet<Adventurer> _party = new();

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
            if (!_document) _document = gameObject.AddComponent<UIDocument>();
            if (!_document.panelSettings)
            {
                _ownedPanel = ScriptableObject.CreateInstance<PanelSettings>();
                _ownedPanel.name = "GameCanvas runtime panel";
                _ownedPanel.scaleMode = PanelScaleMode.ScaleWithScreenSize;
                _ownedPanel.referenceResolution = new Vector2Int(1920, 1080);
                _ownedPanel.themeStyleSheet = Resources.Load<ThemeStyleSheet>("CanvasTask/GameCanvasTheme");
                _document.panelSettings = _ownedPanel;
            }
            if (!_document.visualTreeAsset)
                _document.visualTreeAsset = Resources.Load<VisualTreeAsset>("CanvasTask/GameCanvas");
        }

        private void Start()
        {
            _started = true;
            Build();
        }

        private void OnEnable()
        {
            if (_document) _document.enabled = true;
            if (_started) Build();
        }

        private void OnDisable()
        {
            ClearBindings();
            if (_document) _document.enabled = false;
        }

        private void OnDestroy()
        {
            ClearBindings();
            if (_ownedPanel) Destroy(_ownedPanel);
        }

        public void Rebind()
        {
            if (isActiveAndEnabled && _started) Build();
        }

        private void ClearBindings()
        {
            foreach (var binding in _bindings) binding.Dispose();
            _bindings.Clear();
            _selectedProblem = null;
            _party.Clear();
        }

        private void Build()
        {
            ClearBindings();
            if (!_document.visualTreeAsset)
            {
                Debug.LogError("GameCanvas UI Toolkit layout is missing.", this);
                return;
            }
            _root = _document.rootVisualElement;
            _root.Clear();
            _document.visualTreeAsset.CloneTree(_root);
            _root.pickingMode = PickingMode.Ignore;
            _guild = Game.SaveMngr.Get<Guild>();
            _hall = Game.SaveMngr.Get<GuildHall>();
            if (_guild == null || _hall == null)
            {
                Debug.LogError("GameCanvas requires Guild and GuildHall save data.", this);
                return;
            }
            _sources = CurrentSources();

            foreach (var panel in new[] { "guild", "clients", "problems", "quests", "adventurers", "party", "map", "options" })
            {
                var name = panel;
                _root.Q<Button>(name + "Button").clicked += () => Show(name + "Panel");
                _root.Q<Button>(name + "Close").clicked += () => Hide(name + "Panel");
            }
            _root.Q<Button>("problemClose").clicked += () => Hide("problemPanel");
            _root.Q<Button>("questClose").clicked += () => Hide("questPanel");
            _root.Q<Button>("refuseProblem").clicked += () => ReplyProblem(-1);

            _problemBinding = new UiBinding<ProblemRequest>(_root.Q("problemPanel"), RenderProblem);
            _bindings.Add(_problemBinding);
            var clients = Collection<Costomer>("clientsList", customer => DisplayRow(customer, null));
            clients.Bind(_hall.Costomers);
            clients.CountChanged += count => Text("clientsButton", $"Clients ({count})");
            Text("clientsButton", $"Clients ({_hall.Costomers.Count})");

            var problems = Collection<ProblemRequest>("problemsList", request =>
                Row<ProblemRequest>((data, binding) =>
                {
                    Label(binding.Root, data.Costomer?.DisplayName ?? "Unknown client");
                    Label(binding.Root, data.Problem?.DisplayName ?? "Missing problem");
                    Button(binding.Root, "View problem", () => SelectProblem(data));
                }));
            problems.CountChanged += count =>
            {
                Text("problemsButton", $"Problems ({count})");
                if (_selectedProblem != null && !_hall.ProblemRequests.Contains(_selectedProblem))
                {
                    _selectedProblem = null;
                    _problemBinding.Bind(null);
                    Hide("problemPanel");
                }
            };
            problems.Bind(_hall.ProblemRequests);

            _quests = Collection<Quest>("questsList", quest => DisplayRow(quest, () => SelectQuest(quest)));
            _quests.Bind(_hall.QuestBoard);
            var requests = Collection<QuestRequest>("questRequestsList", request =>
                Row<QuestRequest>((data, binding) =>
                {
                    Label(binding.Root, $"{data.Adventurer?.DisplayName}: {data.Quest?.DisplayName}");
                    Button(binding.Root, "Accept", () =>
                    {
                        if (_hall.QuestRequests.Contains(data) && _hall.QuestBoard.Contains(data.Quest))
                            data.Reply(data.Quest);
                    });
                    Button(binding.Root, "Refuse", () =>
                    {
                        if (_hall.QuestRequests.Contains(data)) data.Reply(null);
                    });
                }));
            requests.CountChanged += count => Text("questsButton", $"Mission board ({count} requests)");
            requests.Bind(_hall.QuestRequests);

            _adventurers = Collection<Adventurer>("adventurersList", adventurer => AdventurerRow(adventurer));
            _adventurers.Bind(_guild.Adventurers);
            var freelancers = Collection<Adventurer>("freelancersList", adventurer => AdventurerRow(adventurer));
            freelancers.Bind(_hall.Freelancers);
            _adventurers.CountChanged += _ => RenderParty();
            freelancers.CountChanged += _ => RenderParty();
            _root.Q<TextField>("adventurerSearch").RegisterValueChangedCallback(evt =>
            {
                Predicate<Adventurer> filter = item => Matches(item.DisplayName, evt.newValue);
                _adventurers.Filter = filter;
                freelancers.Filter = filter;
                _adventurers.Refresh();
                freelancers.Refresh();
            });
            _root.Q<TextField>("questSearch").RegisterValueChangedCallback(evt =>
            {
                _quests.Filter = item => Matches(item.DisplayName, evt.newValue);
                _quests.Refresh();
            });
            RenderParty();
            RefreshHeader();
        }

        private void Update()
        {
            if (_root == null || _guild == null || _hall == null) return;
            if (!ReferenceEquals(_guild, Game.SaveMngr.Get<Guild>()) ||
                !ReferenceEquals(_hall, Game.SaveMngr.Get<GuildHall>()) || SourcesChanged())
            {
                Build();
                return;
            }
            RefreshHeader();
        }

        private object[] CurrentSources() => new object[]
        {
            _guild.Adventurers, _hall.Freelancers, _hall.Costomers, _hall.QuestBoard,
            _hall.QuestRequests, _hall.ProblemRequests
        };

        private bool SourcesChanged() => _sources == null ||
            !ReferenceEquals(_sources[0], _guild.Adventurers) ||
            !ReferenceEquals(_sources[1], _hall.Freelancers) ||
            !ReferenceEquals(_sources[2], _hall.Costomers) ||
            !ReferenceEquals(_sources[3], _hall.QuestBoard) ||
            !ReferenceEquals(_sources[4], _hall.QuestRequests) ||
            !ReferenceEquals(_sources[5], _hall.ProblemRequests);

        private void RefreshHeader()
        {
            Text("gold", $"Gold: {_guild.Gold.Value:N0}");
            Text("rank", $"Rank: {_guild.Rank}");
        }

        private static bool Matches(string value, string search) =>
            string.IsNullOrEmpty(search) || (value ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

        private UiCollectionBinding<T> Collection<T>(string name, Func<T, UiBinding<T>> factory)
        {
            var binding = new UiCollectionBinding<T>(_root.Q<ScrollView>(name).contentContainer, factory);
            _bindings.Add(binding);
            return binding;
        }

        private static UiBinding<T> Row<T>(Action<T, UiBinding<T>> render)
        {
            var element = new VisualElement();
            element.AddToClassList("card");
            return new UiBinding<T>(element, (data, binding) =>
            {
                element.Clear();
                render(data, binding);
            });
        }

        private static UiBinding<T> DisplayRow<T>(T item, Action select) where T : IDisplay =>
            Row<T>((data, binding) =>
            {
                var image = new Image();
                image.AddToClassList("portrait");
                binding.Root.Add(image);
                var title = Label(binding.Root, "");
                var description = Label(binding.Root, "");
                void Refresh()
                {
                    title.text = $"{data.DisplayName} (#{data.Id})";
                    description.text = data.Description;
                }
                Refresh();
                image.sprite = data.Icon;
                binding.Watch(data, Refresh);
                if (data is Quest quest)
                    Label(binding.Root, QuestSummary(quest));
                if (select != null) Button(binding.Root, "Details", select);
            });

        private UiBinding<Adventurer> AdventurerRow(Adventurer adventurer) =>
            Row<Adventurer>((data, binding) =>
            {
                var title = Label(binding.Root, "");
                var stats = Label(binding.Root, "");
                void Refresh()
                {
                    title.text = $"{data.DisplayName} — Rank {data.Rank.Value}";
                    stats.text = StatsSummary(data.Stats);
                }
                Refresh();
                binding.Watch((IDisplay)data, Refresh);
                binding.Watch(data.Rank, Refresh);
                binding.Watch(data.Stats.Power, Refresh);
                binding.Watch(data.Stats.Knowledge, Refresh);
                binding.Watch(data.Stats.Vigor, Refresh);
                Button(binding.Root, "Add to party", () =>
                {
                    _party.Add(data);
                    RenderParty();
                    Show("partyPanel");
                }).SetEnabled(data.assignment == null);
            });

        private void SelectProblem(ProblemRequest request)
        {
            _selectedProblem = request;
            _problemBinding.Bind(request);
            Show("problemPanel");
        }

        private void RenderProblem(ProblemRequest request, UiBinding<ProblemRequest> binding)
        {
            var solutions = _root.Q<ScrollView>("solutionsList");
            solutions.Clear();
            Text("problemTitle", request?.Problem?.DisplayName ?? "Problem");
            Text("problemDescription", request?.Problem?.Description ?? "");
            Text("problemClient", request?.Costomer?.DisplayName ?? "");
            _root.Q<Button>("refuseProblem").SetEnabled(request != null);
            if (request?.Problem == null) return;
            var outcomes = request.Problem.Outcomes;
            for (var i = 0; i < outcomes.Length; i++)
            {
                var index = i;
                var group = new VisualElement();
                group.AddToClassList("card");
                Label(group, $"Solution {i + 1} — Total fee: {outcomes[i].Sum(quest => quest.Fee):N0}");
                foreach (var quest in outcomes[i])
                {
                    var row = DisplayRow(quest, () => SelectQuest(quest));
                    binding.Own(row);
                    group.Add(row.Root);
                    row.Bind(quest);
                }
                Button(group, "Confirm solution", () => ReplyProblem(index));
                solutions.Add(group);
            }
        }

        private void ReplyProblem(int index)
        {
            var request = _selectedProblem;
            if (request == null || !_hall.ProblemRequests.Contains(request)) return;
            if (index < -1 || (index >= 0 && index >= request.Problem.Outcomes.Length)) return;
            _selectedProblem = null;
            _problemBinding.Bind(null);
            Hide("problemPanel");
            request.Reply(index);
        }

        private void SelectQuest(Quest quest)
        {
            Text("questTitle", quest.DisplayName);
            Text("questDescription", quest.Description);
            Text("questSummary", QuestSummary(quest));
            Show("questPanel");
        }

        private static string QuestSummary(Quest quest) =>
            $"Rank {quest.Rank} | Fee: {quest.Fee:N0} | Reward: {quest.Reward?.Gold ?? 0:N0}\n" +
            StatsSummary(quest.Requisite);

        private static string StatsSummary(IStats stats) => stats == null ? "" :
            $"Power: {stats.Power} | Knowledge: {stats.Knowledge} | Vigor: {stats.Vigor}";

        private void RenderParty()
        {
            var list = _root.Q<ScrollView>("partyList");
            list.Clear();
            _party.RemoveWhere(item => !_guild.Adventurers.Contains(item) && !_hall.Freelancers.Contains(item));
            foreach (var member in _party.ToArray())
            {
                var row = new VisualElement();
                row.AddToClassList("card");
                Label(row, member.DisplayName);
                Label(row, StatsSummary(member.Stats));
                Button(row, "Remove", () => { _party.Remove(member); RenderParty(); });
                list.Add(row);
            }
            Text("partyTotals", $"Party: {_party.Count} | Power: {_party.Sum(a => a.Stats.Power.Value)} | " +
                $"Knowledge: {_party.Sum(a => a.Stats.Knowledge.Value)} | Vigor: {_party.Sum(a => a.Stats.Vigor.Value)}");
        }

        private void Text(string name, string text)
        {
            var element = _root.Q<TextElement>(name);
            if (element != null && element.text != text) element.text = text;
        }

        private void Show(string name) => _root.Q(name).RemoveFromClassList("hidden");
        private void Hide(string name) => _root.Q(name).AddToClassList("hidden");

        private static Label Label(VisualElement parent, string text)
        {
            var label = new Label(text);
            parent.Add(label);
            return label;
        }

        private static Button Button(VisualElement parent, string text, Action action)
        {
            var button = new Button(action) { text = text };
            parent.Add(button);
            return button;
        }
    }
}
