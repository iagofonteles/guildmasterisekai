using Drafts.Extensions;
using UnityEngine;
namespace Drafts.Components {

	public class PositionConstraint : MonoBehaviour {
		public enum Conversion { None, ToWorld, ToScreen }

		[Tooltip("Use targetPosition.position if set")]
		public Transform targetPosition;
		public Vector3 fixedPosition;
		public Conversion conversion;
		public Vector3 offset;
		public bool ignoreX, ignoreY, ignoreZ;

		private void OnEnable() => LateUpdate();

		private void LateUpdate() {
			Vector3 pos = targetPosition ? targetPosition.position : fixedPosition;
			if(conversion == Conversion.ToWorld) pos = pos.ToWorld();
			if(conversion == Conversion.ToScreen) pos = pos.ToScreen();
			if(ignoreX) pos.x = transform.position.x;
			if(ignoreY) pos.y = transform.position.y;
			if(ignoreZ) pos.z = transform.position.z;
			transform.position = pos + offset;
		}
	}
}