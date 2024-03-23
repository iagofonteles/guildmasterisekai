using UnityEngine;
using UnityEditor;
using Drafts.Editing;
using System.Linq;
namespace DraftsEditor.Editing {
	public partial class ChunkManagerViewTool {

		static GUIStyle leftAlign;

		void HandleGUI() {
			var view = target as ChunkManagerView;
			var old = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 40;
			Handles.BeginGUI();
			EditorGUILayout.BeginVertical(GUILayout.Width(150));

			//var size = view.ChunkManager.TileSize;
			//var pos = Manipulator.FreePosition;
			//pos = new(pos.x / size.x, pos.y / size.y, pos.z / size.z);

			EditorGUILayout.Vector3Field(GUIContent.none, Manipulator.FreePosition);
			brushId = EditorGUILayout.Popup(new GUIContent("Tool"), brushId, brushesNames);

			rulerDraw = (RulerDraw)EditorGUILayout.EnumFlagsField(rulerDraw);
			Rulers.drawChunk = (rulerDraw & RulerDraw.Chunk) > 0;
			Rulers.drawAxis = (rulerDraw & RulerDraw.Axis) > 0;

			if(GUILayout.Button(tileset?.name ?? "{ Choose Tileset }"))
				searchProvider.OpenWindow(s => {
					tileset = (Tileset)s;
					Manipulator.Tile = tileset.Tiles.FirstOrDefault();
				});
			EditorGUILayout.Space();

			if(leftAlign == null) {
				leftAlign = new(EditorStyles.toolbarButton);
				leftAlign.alignment = TextAnchor.MiddleLeft;
			}

			if(tileset)
				foreach(var tile in tileset.Tiles)
					if(GUILayout.Button(new GUIContent(tile.name, tile.Icon), leftAlign, GUILayout.Width(120), GUILayout.Height(24)))
						Manipulator.Tile = tile;

			EditorGUILayout.EndVertical();
			Handles.EndGUI();
			EditorGUIUtility.labelWidth = old;
		}
	}
}
