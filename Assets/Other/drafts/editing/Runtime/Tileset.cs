using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Drafts.Editing {
	[CreateAssetMenu(menuName = "Drafts/Tilemap3D/Tileset")]
	public class Tileset : ScriptableObject {
		[SerializeField, SO(typeof(ITile))] ScriptableObject[] tiles;
		public virtual IEnumerable<ITile> Tiles => tiles.Select(t => t is ITile p ? p : Throw(t));

		ITile Throw(ScriptableObject so) {
			Debug.LogError($"{so.name} is not ITilePrefab", this);
			return null;
		}
	}
}
