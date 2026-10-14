using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai {
	public interface IVisitor {
		IVisitorAI GetAI(NavMeshAgent agent, HallSpots spots);
		GameObject Prefab => Resources.Load<GameObject>("Visitor/Default");
	}
}
