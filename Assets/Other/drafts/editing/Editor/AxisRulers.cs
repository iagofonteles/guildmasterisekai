using UnityEngine;

namespace DraftsEditor.Editing {
	public class AxisRulers {
		public bool drawAxis = true, drawChunk = true;
		Line x, y, z;
		CubeLine point, chunk, selection;

		public void SetSize(Vector3Int chunkLenght, Vector3 tileSize) {
			var chunkSize = Vector3.Scale(chunkLenght, tileSize);

			point = new CubeLine() { Size = tileSize, Color = Color.white };
			chunk = new CubeLine() { Size = chunkSize, Color = Color.black };
			selection = new CubeLine() { Size = Vector3.zero, Color = Color.white };

			x = new Line() { size = chunkSize.x, color = Color.red, direction = Vector3.right, };
			y = new Line() { size = chunkSize.y, color = Color.green, direction = Vector3.up, };
			z = new Line() { size = chunkSize.z, color = Color.blue, direction = Vector3.forward, };
		}

		public void DrawAt(Vector3 position) {
			SetPointPosition(position);
			Draw();
		}

		public void SetSelection(Vector3 begin, Vector3 end) {
			throw new System.NotImplementedException();
		}

		public void SetPointPosition(Vector3 position) {
			var halfSize = chunk.Size / 2;
			var chunkPos = new Vector3(
				Mathf.Floor(position.x / chunk.Size.x) * chunk.Size.x,
				Mathf.Floor(position.y / chunk.Size.y) * chunk.Size.y,
				Mathf.Floor(position.z / chunk.Size.z) * chunk.Size.z)
				+ halfSize;

			point.Position = position;
			chunk.Position = chunkPos - point.Size / 2;
			x.Position = new Vector3(chunkPos.x, position.y, position.z);
			y.Position = new Vector3(position.x, chunkPos.y, position.z);
			z.Position = new Vector3(position.x, position.y, chunkPos.z);
			//x.Position = y.Position = z.Position = position;
		}

		public void Draw() {
			point.Draw();

			if(drawChunk)
				chunk.Draw();

			if(drawAxis) {
				x.Draw();
				y.Draw();
				z.Draw();
			}
		}
	}
}
