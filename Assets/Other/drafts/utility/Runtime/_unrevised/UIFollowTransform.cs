using Drafts.Extensions;
using UnityEngine;
namespace Drafts.Components {

	public class UIFollowTransform : MonoBehaviour {
		public Transform target;
		public Vector2 offset;
		public bool ignoreX, ignoreY;

		private void OnEnable() => Update();

		private void Update() {
			if(!target) return;
			var pos = target.position.ToScreen();
			if(ignoreX) pos.x = transform.position.x; 
			if(ignoreY) pos.y = transform.position.y;
			transform.position = pos + (Vector3)offset;
		}
	}
}
