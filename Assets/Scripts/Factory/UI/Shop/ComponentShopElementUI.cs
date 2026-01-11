using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSM.Surveillance.UI
{
    public class ComponentShopElementUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private ComponentShopPreviewUI previewUIPrefab;
        
        private MachineObject _machineObject;
        private ComponentShopPreviewUI _previewInstance;
        private ComponentShopUI _shopUI;

        public void Load(MachineObject processor, ComponentShopUI shop)
        {
            _shopUI = shop;
            LoadProcessor(processor);
        }
        
        private void LoadProcessor(MachineObject processorObject)
        {
            _machineObject = processorObject;
            RefreshUI();
        }

        private void RefreshUI()
        {
            if(_machineObject == null) return;

            switch (_machineObject)
            {
                case ProcessorObject p0:
                    nameText.text = $"{p0.Data.ShopInfo.name}";
                    costText.text = $"Cost: ${p0.Data.UpfrontCost}";
                    break;
                case SplitterObject s0:
                    nameText.text = $"{s0.Data.ShopInfo.name}";
                    costText.text = $"Cost: ${s0.Data.UpfrontCost}";
                    break;
                case MergerObject m0:
                    nameText.text = $"{m0.Data.ShopInfo.name}";
                    costText.text = $"Cost: ${m0.Data.UpfrontCost}";
                    break;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(_previewInstance != null) return;

            _previewInstance = Instantiate(previewUIPrefab, transform);
            _previewInstance.transform.parent = _shopUI.transform;

            if (_machineObject is ProcessorObject p0)
            {
                _previewInstance.LoadInformation(p0);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(_previewInstance == null) return;
            
            Destroy(_previewInstance.gameObject);
            _previewInstance = null;
        }

        public void Buy()
        {
            _shopUI.Buy(_machineObject);
        }
    }
}