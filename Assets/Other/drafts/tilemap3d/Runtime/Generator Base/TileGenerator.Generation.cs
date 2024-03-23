#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Drafts.Tilemap3D {
	public abstract partial class TileGenerator : ScriptableObject {

		[ContextMenu("GenerateRuleTile")]
		public void GenerateMeshes() {
			maskRuleMap = null;
			ResizeGeneratedList();
			Generate();
			AssetDatabase.SaveAssets();
			OnChanged?.Invoke();
		}

		[ContextMenu("CheckRuleClash")]
		public void CheckRuleClash() {
			var none = true;
			maskRuleMap = CreateMaskRuleMap();
			for(int m = 0; m < maskRuleMap.Length; m++) {
				var matches = new List<int>();
				for(int i = 0; i < Rules.Count; i++) {
					var isMatch = Rules[i].Match(m);
					if(isMatch) matches.Add(i);
				}
				if(matches.Count == 0) Debug.Log($"mask {m} has no matches");
				if(matches.Count > 1) {
					Debug.Log($"mask {m} matches rules: {string.Join(", ", matches)}");
					none = false;
				}
			}
			if(none) Debug.Log("No rule clash found!");
		}

		protected void ResizeGeneratedList() {
			while(generated.Count < Corners.Count * Rules.Count) {
				var mesh = new Mesh();
				AssetDatabase.AddObjectToAsset(mesh, this);
				generated.Add(mesh);
			}
			while(generated.Count > Corners.Count * Rules.Count) {
				AssetDatabase.RemoveObjectFromAsset(generated[generated.Count - 1]);
				generated.RemoveAt(generated.Count - 1);
			}
			for(int i = 0; i < generated.Count; i++)
				generated[i].name = $"{i / Rules.Count} {i % Rules.Count}";
			AssetDatabase.SaveAssets();
		}
	}
}
#endif