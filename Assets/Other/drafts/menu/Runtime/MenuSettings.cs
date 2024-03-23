using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Drafts.Templates {

	public class MenuSettings : MonoBehaviour {
		public TextMeshProUGUI title, subtitle;
		public Button back, prev, next;

		public void Set(PageSettings s) {
			if(title) title.text = s.title;
			if(subtitle) subtitle.text = s.subtitle;
			SetBtn(back, s.back);
			SetBtn(prev, s.prev);
			SetBtn(next, s.next);
		}

		void SetBtn(Button b, UnityEngine.Events.UnityEvent ev) {
			if(b) {
				b.onClick.RemoveAllListeners();
				b.onClick.AddListener(ev.Invoke);
			}
		}
	}
}
