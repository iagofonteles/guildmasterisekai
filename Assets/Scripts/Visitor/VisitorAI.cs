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
        private IVisitorAI _ai;

        public NavMeshAgent Agent => agent;

        private void Awake()
        {
            enabled = false;
        }

        private void Update()
        {
            if (_ai != null && _ai.ReadyToAct) _ai.Act();
        }

        private void OnDestroy()
        {
            _ai?.Dispose();
        }

        public void SetVisitor(IVisitorAI ai)
        {
            if (ReferenceEquals(_ai, ai)) return;
            _ai?.Dispose();
            _ai = ai;
            enabled = ai != null;
        }
    }
}