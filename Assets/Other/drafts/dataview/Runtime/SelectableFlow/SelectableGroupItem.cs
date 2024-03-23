using Drafts.Extensions;
using UnityEngine;
using UnityEngine.UI;
namespace Drafts.UI.Flow {
	public class SelectableGroupItem : MonoBehaviour {

		[SerializeField] SelectableGroup group;
		Selectable selectable;

		void Awake() {
			selectable = GetComponent<Selectable>();
			group.OnCurrentChanged.AddListener(selectable.TrySetInteractable);
		}
		private void OnDestroy() => group.OnCurrentChanged.AddListener(selectable.TrySetInteractable);
		private void OnEnable() { selectable.interactable = group.enabled; group.itens.Add(selectable); }
		private void OnDisable() { selectable.interactable = false; group.itens.Remove(selectable); }
	}
}