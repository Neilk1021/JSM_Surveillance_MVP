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
        [SerializeField] private List<ProcessorObject> buyableProcessors;
        [Tooltip("The transform that all the UI prefabs should be spawned under.")]
        [SerializeField] private Transform contentView;

        private FactoryGrid _grid;
        
        private void Awake()
        {
            if (Camera.main != null) _cc = Camera.main.GetComponent<CameraController>();
            _grid = GetComponentInParent<FactoryGrid>();
        }

        private void Start()
        {
            foreach (var processor in buyableProcessors)
            {
                var element = Instantiate(componentUIElementPrefab, contentView);
                element.Load(processor, this);
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

        public void Buy(ProcessorObject processorPrefab)
        {
            if (!SurveillanceGameManager.instance.MoneyManager.ChangeMoneyBy(-processorPrefab.Data.UpfrontCost)) {
                return;
            }
            
            var processorObj = Instantiate(processorPrefab, _grid.transform);
            processorObj.StartPlacement();
        }
    }
}
