using System;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai.Visitor
{
    public interface IVisitorAI : IDisposable
    {
        bool ReadyToAct { get; }
        void Act();
    }

    public class VisitorAI : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;

        private IVisitorAI ai;
        public NavMeshAgent Agent => agent;

        private void Awake()
        {
            enabled = false;
        }

        private void Update()
        {
            if (ai != null && ai.ReadyToAct) ai.Act();
        }

        private void OnDestroy()
        {
            ai?.Dispose();
        }

        public void SetVisitor(IVisitorAI ai)
        {
            if (ReferenceEquals(this.ai, ai)) return;
            this.ai?.Dispose();
            this.ai = ai;
            enabled = ai != null;
        }
    }
}