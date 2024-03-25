using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai {

	public class AnimatorParameter : MonoBehaviour {
		//static int? speed, horizontal, vertical;

		public float walkSpeed = 3.5f;
		Animator animator;
		NavMeshAgent agent;

		void Start() {
			animator = GetComponent<Animator>();
			agent = GetComponentInParent<NavMeshAgent>();

			//speed ??= animator.parameters.IndexOf(p => p.name == "Speed");
			//horizontal ??= animator.parameters.IndexOf(p => p.name == "Horizontal");
			//vertical ??= animator.parameters.IndexOf(p => p.name == "Vertical");
		}

		void FixedUpdate() {
			var f = transform.forward;
			animator.SetFloat("Speed", agent.velocity.magnitude / walkSpeed);
			animator.SetFloat("Horizontal", f.x);
			animator.SetFloat("Vertical", f.y);
		}
	}

}