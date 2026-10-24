using System.Collections.Generic;
using Drafts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.Interactables
{
    public class InteractableComponent : MonoBehaviour, IPointerClickHandler
    {
        private static readonly HashSet<IInteractable> _instances = new();
        public static IReadOnlyCollection<IInteractable> Instances => _instances;

        [SerializeReference, TypeInstance] private IInteractable interactable;
        [SerializeField] private PanelRenderer ui;
        
        private void Awake()
        {
            ui.enabled = false;
            if (!ui) return;
            ui.RegisterUIReloadCallback(interactable.BindUI);
        }

        private void OnEnable()
        {
            interactable.__setTransform(transform);
            _instances.Add(interactable);
            IInteractable.ClearCache();
        }

        private void OnDisable()
        {
            _instances.Remove(interactable);
            IInteractable.ClearCache();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            ui.enabled = !ui.enabled;
        }
    }
}