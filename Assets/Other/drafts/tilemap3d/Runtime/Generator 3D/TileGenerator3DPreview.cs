using System.Linq;
using UnityEngine;
namespace Drafts.Tilemap3D.Generators {
	[SelectionBase, ExecuteAlways]
	public class TileGeneratorPreview : MonoBehaviour {

		[SerializeField] Tile tile;
		[SerializeField] MeshFilter[] corners;
		[SerializeField] Vector3 tileSize = Vector3.one;
		[SerializeField] bool showNormals;
		[SerializeField] Vector3 normalsSize = new(.25f, .25f, .25f);

		public Tile Tile { get => tile; set => tile = value; }
		CornerConfig Corners => tile.Generator.Corners;

		void CreateCorners() {
			corners = new MeshFilter[Corners.Count];
			for(int i = 0; i < corners.Length; i++) {
				var go = new GameObject(i.ToString());
				go.transform.parent = transform;
				go.transform.localPosition = Vector3.zero;
				go.AddComponent<MeshRenderer>();
				go.hideFlags = HideFlags.HideInHierarchy;
				corners[i] = go.AddComponent<MeshFilter>();
			}
		}

		private void Start() => TileGenerator.OnChanged += UpdateCorners;
		private void OnDestroy() => TileGenerator.OnChanged -= UpdateCorners;

		[ContextMenu("UpdateCorners All")]
		public void UpdateCornersAll() {
			for(int i = 0; i < transform.parent.childCount; i++)
				if(transform.parent.GetChild(i).gameObject.activeInHierarchy)
					transform.parent.GetChild(i).GetComponent<TileGeneratorPreview>().UpdateCorners();
		}

		[ContextMenu("UpdateCorners")]
		public void UpdateCorners() {
			TileGenerator.OnChanged -= UpdateCorners;
			TileGenerator.OnChanged += UpdateCorners;
			if(!GetComponent<BoxCollider>()) gameObject.AddComponent<BoxCollider>();
			if((corners?.Length ?? 0) == 0) CreateCorners();
			SetCornersMesh(CheckConnections());
			foreach(var c in corners) c.gameObject.hideFlags = HideFlags.None;
			foreach(var c in corners) c.GetComponent<MeshRenderer>().materials
				= tile.Materials.Take(c.sharedMesh?.subMeshCount ?? 0).ToArray();
		}

		byte[] CheckConnections() {
			var connections = new byte[Corners.Directions.Length];
			var p = transform.position;
			var size = tileSize / 2.1f;
			int Check(Vector3 dir) => Physics.OverlapBox(p + Vector3.Scale(dir, tileSize), size)
				.Any(c => c.GetComponent<TileGeneratorPreview>()) ? 1 : 0;

			for(int i = 0; i < Corners.Directions.Length; i++)
				connections[i] = (byte)Check(Corners.Directions[i]);
			return connections;
		}

		private void SetCornersMesh(byte[] connections) {
			var rules = tile.Generator.GetCornersRule(connections);
			for(int i = 0; i < rules.Length; i++) {
				corners[i].sharedMesh = tile.Generator.GetMesh(i, rules[i]);
				corners[i].name = $"{i} - {rules[i] % tile.Generator.Rules.Count}";
			}
		}

		private void OnDrawGizmos() {
			if(!showNormals) return;
			Gizmos.color = Color.magenta;
			foreach(var filter in corners) {
				var vertices = filter.sharedMesh.vertices;
				var normals = filter.sharedMesh.normals;
				for(int i = 0; i < vertices.Length; i++) {
					var start = filter.transform.position + vertices[i];
					var end = start + Vector3.Scale(normals[i], normalsSize);
					Gizmos.DrawLine(start, end);
				}
			}
		}
	}
}
