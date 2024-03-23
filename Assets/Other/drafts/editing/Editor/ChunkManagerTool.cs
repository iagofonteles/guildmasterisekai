using UnityEngine;
using UnityEditor;
using Drafts;
using Drafts.Editing;
using UnityEditor.EditorTools;
using System.Linq;

namespace DraftsEditor.Editing {
	enum RulerDraw { None = 0, Chunk = 1, Axis = 2, Both = 3 }

	[EditorTool("Tilemap 3D", typeof(ChunkManagerView))]
	public partial class ChunkManagerViewTool : EditorTool {
		static float pierce = .2f;
		static AxisRulers Rulers = new AxisRulers() { drawChunk = true };
		static string[] brushesNames;
		static Brush[] brushes;
		static int brushId = 0;
		static Brush CurrentBrush => brushes[brushId];
		static IManipulator Manipulator { get; } = new Manipulator();
		static SearchProvider searchProvider;
		static Tileset tileset;

		RulerDraw rulerDraw = RulerDraw.Chunk;

		static ChunkManagerViewTool() {
			brushes = new Brush[] {
				new Drafts.Editing.Brushes.Pixel(Manipulator),
				new Drafts.Editing.Brushes.Box(Manipulator),
			};
			brushesNames = brushes.Select(s => s.Name).ToArray();
			Manipulator.Gizmo.Size = Vector3.zero;
		}

		public override void OnActivated() {
			searchProvider ??= new AssetSearchSettings(typeof(Tileset));
			var view = target as ChunkManagerView;
			Manipulator.ChunkManager = view.ChunkManager;
			Manipulator.Gizmo.Size = view.ChunkManager.TileSize;
			Rulers.SetSize(view.ChunkManager.ChunkLength, view.ChunkManager.TileSize);
			view.Subscribe();
		}

		public override void OnWillBeDeactivated() {
			var view = target as ChunkManagerView;
			view.Unsubscribe();
			foreach(var brush in brushes) brush.OnDisable();
		}

		public override void OnToolGUI(EditorWindow window) {
			Selection.activeObject = target;
			HandleGUI();
			HandleInput();
			Manipulator?.Gizmo?.Draw();
		}

		void HandleInput() {
			var view = target as ChunkManagerView;
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			var empty = !Physics.Raycast(ray, out var hit);
			if(empty & !ray.RaycastYPlane(out var point)) return;
			if(!empty) point = hit.point + hit.normal * pierce;

			var pos = view.ChunkManager.SnapToGridCenter(point);
			var dir = Vector3.Scale(hit.normal.RountToInt(), view.ChunkManager.TileSize);

			if(view.ChunkManager.GetTile(pos, out _) == null) {
				Manipulator.Position = pos - dir;
				Manipulator.FreePosition = pos;
			} else {
				Manipulator.Position = pos;
				Manipulator.FreePosition = pos;
			}

			Rulers?.DrawAt(Manipulator.FreePosition);
			SceneView.RepaintAll();

			if(Manipulator.Tile == null) return;

			if(Event.current.type == EventType.MouseDown && Event.current.button == 0) {
				CurrentBrush.Click(Event.current.shift);
				Event.current.Use();
			}
			CurrentBrush.Update(Event.current.shift);
		}
	}
}
