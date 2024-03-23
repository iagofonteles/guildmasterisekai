using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class CharacterDistance : MonoBehaviour {

	[SerializeField] Transform[] targets;
	[SerializeField] Material material;
	[SerializeField] Vector2[] offset;

	float distClamp => .5f;

	private void Awake() => offset = new Vector2[targets.Length];

	private void Update() {
		for(int i = 0; i < targets.Length; i++) {
			var center = GetCenter();
			var offset = (Vector2)targets[i].position - center;
			offset.x = Mathf.Clamp(offset.x, -distClamp, distClamp);
			offset.y = Mathf.Clamp(offset.y, -distClamp, distClamp);
			material.SetVector("_offset_" + i, offset);
			this.offset[i] = offset;
		}
	}

	public static Vector2 GetCentroid(List<Vector3> poly) { // not mine, ty stackoverflow
		return new Vector2(
			poly.Average(t => t.x),
			poly.Average(t => t.y)
		);

		float accumulatedArea = 0.0f;
		float centerX = 0.0f;
		float centerY = 0.0f;


		for(int i = 0, j = poly.Count - 1; i < poly.Count; j = i++) {
			float temp = poly[i].x * poly[j].y - poly[j].x * poly[i].y;
			accumulatedArea += temp;
			centerX += (poly[i].x + poly[j].x) * temp;
			centerY += (poly[i].y + poly[j].y) * temp;
		}

		if(Mathf.Abs(accumulatedArea) < 1E-7f)
			return Vector2.zero;  // Avoid division by zero

		accumulatedArea *= 3f;
		return new Vector2(centerX / accumulatedArea, centerY / accumulatedArea);
	}

	Vector2 GetCenter() => GetCentroid(targets.Select(t => t.position).ToList());

	private void OnDrawGizmos() {
		if(targets == null || targets.Length < 2) return;
		Gizmos.DrawSphere(GetCentroid(targets.Select(t => t.position).ToList()), 1f);
	}

}
