# GameCanvas UI Toolkit conversion

Use `GameCanvasUIToolkit.prefab` in place of the existing `Assets/Prefabs/GameCanvas.prefab` instance to review this implementation. Disable the old canvas while reviewing to avoid overlapping interfaces. Existing scenes and the original prefab are unchanged.

The prefab creates its UIDocument and a private PanelSettings instance at runtime. It loads the UXML and custom runtime theme from this folder's Resources directory. The panel scales against 1920 × 1080. No scene wiring, package installation, or editor asset-generation step is required. The UXML can also be opened directly in UI Builder.

## Coverage

| Original canvas area | Replacement |
| --- | --- |
| MainOverlay, money, rank, bottom buttons | Persistent treasury/rank header and screen navigation |
| Clients Panel | Observable client list with names, IDs, descriptions and portraits |
| Problem request template and notifications | Observable request list with count in navigation; request details |
| Problem Panel and Possible Solutions | All outcome groups, nested quest cards, stats, fees and total fee per solution |
| Confirm/refuse | Calls `ProblemRequest.Reply` with the actual zero-based outcome index or `-1` |
| Mission Board and Quest Panel | Observable quest board, quest details, requirements, fees and rewards; name search |
| Quest requests | Adventurer/quest summary; accept/refuse using `QuestRequest.Reply(quest/null)` |
| Adventurers and Select Adventurer | Guild and freelancer lists, name search, watched rank/stats, party selection |
| Set Party and Total Attributes | Temporary selection, remove member, summed attributes |
| Guild, map and options buttons | Closeable panels; unavailable features identified |

This is a functional re-layout, not a pixel-for-pixel reproduction of the RectTransform hierarchy. Static sample text and unsupported metrics are not presented as live values. Name searches are supported; class and attribute filter menus from the old canvas have no configured filtering behavior to preserve.

## Replacement for DataView

`UiBinding<T>` binds a model to a VisualElement through a typed render callback. `Bind` releases the previous subscriptions before rendering a new model. `Watch` supports the project's IDisplay and IWatcher contracts. Use `Own` for nested bindings and `OnUnbind` for other cleanup. `Dispose` is idempotent.

`UiCollectionBinding<T>` accepts an enumerable and subscribes to INotifyCollectionChanged when available, including the existing Drafts ObservableList. It rebuilds rows on collection changes, disposes removed row bindings, supports a predicate filter, reports the unfiltered source count, and supplies an empty state. Non-observable enumerables can be updated explicitly with `Refresh`. These new bindings have no dependency on Drafts.DataView. Existing legacy views and the package remain for unchanged scenes.

`GameCanvasUi` owns and disposes bindings when disabled or destroyed, rebuilds the UI when re-enabled, and detects replacement save objects/collections. `Rebind()` is available for explicit refresh. Gold and guild rank have a small header refresh because rank has no change event. Adventure rank and stats use Watcher events. Model `IDisplay.OnChanged` is subscribed where available; some existing models never raise it.

## Problems found

- Several original view fields are null. Other serialized fields use old names/shapes such as `guid`, `knoledge`, and nested `fee.text`, which do not match the current scripts.
- Most original button callbacks only toggle GameObjects. The existing prefab does not provide functional map/options, class filtering, or party dispatch logic.
- Assignment task lookup throws NotImplementedException. There is no party dispatch/completion service, so sending adventurers is disabled. Party selection is local UI state and is cleared on rebind or disable.
- Guild name/fame, client region, quest expiration/completion time, and success percentage have no current model values. These are omitted or marked unavailable.
- Problem outcomes are generated lazily and are not persisted by the model. This conversion uses the existing behavior without changing save formats.
- Reply actions keep the existing ownership contract: visitor AI handles removing requests and applying quest/reward changes. The UI checks that a request is still pending before replying and closes expired problem details.

## Validation

Runtime and editor scripts compile with Roslyn against the installed Unity 6000.5.9f1 and project assemblies. XML structure, unique element names, and literal controller queries were checked outside Unity.

`Editor/CanvasTaskValidation.cs` runs after editor script reload and is also available under **Tools > Canvas Task > Validate UI Toolkit conversion**. It checks imported UXML/theme, controls, prefab reference, collection add/replace/reset/filter/rebind, and subscription cleanup. Results are written to `Temp/CanvasTaskValidation/editor-validation.txt`. This check does not enter Play mode, alter save data, or modify scenes.

Focus Unity to allow asset import, then inspect Console warnings and open the UXML in UI Builder. In Play mode review screen layout at multiple sizes, client/problem notifications, all outcome choices and refusal, timed request expiry, quest request acceptance/refusal, searches, party selection/removal, and disabling/re-enabling the prefab. Actual layout, input, Addressables portraits, and visitor interactions require this Editor review; external compilation does not verify them.
