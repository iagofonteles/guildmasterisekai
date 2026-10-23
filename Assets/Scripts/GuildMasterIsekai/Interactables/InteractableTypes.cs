using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.Interactables
{
    // clickable, open close the guild for the day

    // rest when idle
    public class RestPlace : IInteractable
    {
        void IInteractable.__setTransform(Transform t) => Transform = t;
        public Transform Transform { get; private set; }
        public void SubscribeUI(PanelRenderer ui) => throw new InvalidOperationException();
    }
}