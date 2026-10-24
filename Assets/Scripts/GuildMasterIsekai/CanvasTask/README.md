# GameCanvas UI Toolkit

Use `GameCanvasUIToolkit.prefab` in place of the old `Assets/Prefabs/GameCanvas.prefab` instance. Disable the old canvas while reviewing to avoid overlapping interfaces. The existing UI Toolkit prefab reference remains valid; its component now points to `GameCanvasHost`.

`GameCanvasHost` creates a UIDocument and a private PanelSettings instance using the Resources/CanvasTask layout and theme. It scales against 1920 x 1080, releases the view on disable, and recreates it on enable. No scene wiring or package installation is required.

## Views

`GameCanvasUi : ViewElement<GuildHall>` handles navigation, pending-request checks, search, and temporary party selection. Model rendering is in separate files:

- `GuildView`: guild rank and treasury, with observable gold updates.
- `CostomerView`: client portrait, name, ID, and description.
- `AdventurerView`: rank, stats, and add/remove action.
- `StatsView`: watches Power, Knowledge, and Vigor.
- `ProblemRequestView`: pending problem summary and details action.
- `ProblemView`: problem description, outcome groups, total fees, and solution confirmation.
- `QuestStripView`: quest portrait, name, description, rank, requirements, fees, gold, and item rewards.
- `QuestRequestView`: adventurer/quest summary and accept/refuse actions.
- `PartyView`: selected adventurers and live summed stats.

All these use the existing `TileGrid.UI.ViewElement<T>` data lifecycle. The deleted UiBinding and UiCollectionBinding helpers are not required.

`ListView<TView>` owns a UI Toolkit ListView with dynamic row heights. It accepts any enumerable, observes collection notifications, supports name-search predicates, and reports the unfiltered count. An unfiltered single-item replacement refreshes that item; structural or filtered changes rebuild the snapshot. Recycled rows are unbound, old source subscriptions are removed on rebind/detach, and removed rows release model subscriptions.

The host checks for replacement save models/collections and calls `Rebind()`. Guild rank and assignment availability are refreshed because their fields have no change events. Party selection is temporary and clears on rebind or disable.

## Gameplay contracts

Problem confirmation/refusal calls `ProblemRequest.Reply(index/-1)`. Quest acceptance/refusal calls `QuestRequest.Reply(quest/null)`. Actions verify the request is still pending; acceptance also requires the quest to remain on the board. Visitor AI continues to own request removal and reward/quest changes. Expired selected problem details close automatically.

Map, options, fame/progression, and party dispatch remain unavailable because the corresponding gameplay systems are not implemented. Problem outcomes still use the existing lazy generation and serialization behavior. Portraits use the existing Addressables sprite loader.

## Validation

Run **Tools > Canvas Task > Check Game Canvas** after Unity imports the scripts. The check exercises collection replacement/add/filter/rebind/unbind, enumerable sources, row recycling, gold subscriptions, party stat totals, navigation controls, and the prefab host. It creates local models and does not modify scenes or save data.

The task's external C# build passed against Unity 6000.5.9f1 with four existing CS0067 unused-event warnings. The Editor self-check and Play-mode interaction were not executed from this session. External compilation and XML checks do not establish Unity layout, input behavior, Addressables portrait loading, or visitor interactions. Review those in Play mode, including disable/re-enable and loading a different save.
