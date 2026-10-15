using System;
using Drafts;
using UnityEngine;

namespace GuildMasterIsekai.Visitor.Spot
{
    [Serializable]
    public class Spot : MonoBehaviour
    {
        [ReadOnly] public Vector3 lookTarget;
        [SerializeReference] private IVisitorAI owner;
        public Vector3 Position => transform.position;

        public IVisitorAI Owner
        {
            get => owner;
            set {
                owner = value;
                OnOwnerChanged?.Invoke(this, value);
            }
        }

        public event Action<Spot, IVisitorAI> OnOwnerChanged;
    }
}