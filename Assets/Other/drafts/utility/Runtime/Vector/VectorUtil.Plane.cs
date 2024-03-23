using UnityEngine;

namespace Drafts {
	public static partial class VectorUtil {

		public static bool RaycastYPlane(this Ray ray, out Vector3 intersectionPoint)
			=> RaycastPlane(ray.origin, ray.direction, Vector3.up, Vector3.zero, out intersectionPoint);

		public static bool RaycastPlane(this Ray ray, Vector3 planeNormal, Vector3 planePoint, out Vector3 intersectionPoint)
			=> RaycastPlane(ray.origin, ray.direction, planeNormal, planePoint, out intersectionPoint);

		public static bool RaycastPlane(Vector3 rayOrigin, Vector3 rayDirection, Vector3 planeNormal, Vector3 planePoint, out Vector3 intersectionPoint) {
			intersectionPoint = Vector3.zero;

			float dot = Vector3.Dot(rayDirection, planeNormal);
			if(Mathf.Approximately(dot, 0f)) return false;

			float t = Vector3.Dot(planeNormal, planePoint - rayOrigin) / dot;
			intersectionPoint = rayOrigin + t * rayDirection;

			return true;
		}

	}
}