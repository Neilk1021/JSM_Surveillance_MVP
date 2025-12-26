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
        [SerializeField] protected Image previewImage;

        private UIManager _uiManager;
        private bool _inside = false;
        private bool _initialized = false;
        
        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (_inside) return;
            if (!_initialized) {
                _initialized = true;
                return;
            }
            
            _uiManager.Close();
        }


        public override void Initialize(CellOccupier occupier, UIManager manager)
        {
            _uiManager = manager;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _inside = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _inside = false;
        }
    }
}