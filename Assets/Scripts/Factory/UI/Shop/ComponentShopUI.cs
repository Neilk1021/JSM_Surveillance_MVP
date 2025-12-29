using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSM.Surveillance.UI
{
    public class ComponentShopUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ComponentShopElementUI componentUIElementPrefab;
        [SerializeField] private List<ProcessorInstance> buyableProcessors;
        [Tooltip("The transform that all the UI prefabs should be spawned under.")]
        [SerializeField] private Transform contentView;
        
        private void Awake()
        {
            if (Camera.main != null) _cc = Camera.main.GetComponent<CameraController>();
        }

        private void Start()
        {
            foreach (var processor in buyableProcessors)
            {
                var element = Instantiate(componentUIElementPrefab, contentView);
                element.LoadProcessor(processor);
            }
        }

        private CameraController _cc;

        public void OnPointerEnter(PointerEventData eventData)
        {
            _cc?.SetScrollActive(false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _cc?.SetScrollActive(true);
        }
    }
}
