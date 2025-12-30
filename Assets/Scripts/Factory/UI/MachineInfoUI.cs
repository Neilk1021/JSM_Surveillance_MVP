using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JSM.Surveillance.UI
{
    public abstract class MachineInfoUI : FactoryUI, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected TextMeshProUGUI machineNameText;
        [SerializeField] private Canvas canvas;
        
        private UIManager _uiManager;
        private bool _initialized = false;

        private void Start()
        {
            canvas.worldCamera = _uiManager.WorldCamera;
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (MouseInside) return;
            if (!_initialized) {
                _initialized = true;
                return;
            }
            
            _uiManager.Close();
        }

        public void Close()
        {
            _uiManager.Close();
        }


        public override void Initialize(CellOccupier occupier, UIManager manager)
        {
            canvas.sortingOrder += SurveillanceWindow.GlobalSortOrder+1;
            _uiManager = manager;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseInside = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseInside = false;
        }

        public void SetInside(bool inside)
        {
            MouseInside = inside;
        }
    }
}