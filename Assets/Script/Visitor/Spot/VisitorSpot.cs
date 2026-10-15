using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GuildMasterIsekai.Visitor.Spot
{
    [Serializable]
    public class VisitorSpot : MonoBehaviour
    {
        [SerializeField] protected List<Spot> spots = new();

        public Vector3 Position => transform.position;

        protected virtual void Start()
        {
            foreach (var spot in spots)
                spot.lookTarget = Position;
        }

        public Spot GetFreeSpot() => spots.FirstOrDefault(s => s.isActiveAndEnabled && s.Owner == null);
    }
}