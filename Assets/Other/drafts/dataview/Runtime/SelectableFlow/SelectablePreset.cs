using UnityEngine;
using UnityEngine.UI;

public class SelectablePreset : MonoBehaviour {
	public Selectable.Transition transition;
	public ColorBlock colors;
	public SpriteState sprites;
	public AnimationTriggers triggers;

	private void Start() => gameObject.SetActive(false);

	public Graphic Clone(Transform parent) {
		var s = Instantiate(this, parent);
		var g = s.GetComponent<Graphic>();
		g.gameObject.SetActive(true);
		Destroy(s);
		return g;
	}
}
