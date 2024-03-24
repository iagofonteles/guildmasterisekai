using UnityEngine;

namespace GuildMasterIsekai {
	public interface IVisitor {
		GameObject Prefab => Resources.Load<GameObject>("Visitor/Default");
	}
}
