using UnityEngine;
namespace Drafts.Tilemap3D {
	[CreateAssetMenu(menuName = "Drafts/Tilemap3D/Prefab Tile")]
	public class PrefabTile : Tile {
		[SerializeField] GameObject prefab;
		public GameObject Prefab => prefab;

		public virtual GameObject Instantiate(Vector3 position, Quaternion rotation, Transform parent)
			=> DraftsUtil.InstantiatePrefabHybrid(prefab, position, Quaternion.identity, parent);
		//public virtual void Destroy(GameObject go) => DraftsUtil.DestroyHybrid(go);
	}
}