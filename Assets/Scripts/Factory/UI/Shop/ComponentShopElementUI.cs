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
        
        private ProcessorObject _processorObject;
        private ComponentShopPreviewUI _previewInstance;
        private ComponentShopUI _shopUI;

        public void Load(ProcessorObject processor, ComponentShopUI shop)
        {
            _shopUI = shop;
            LoadProcessor(processor);
        }
        
        private void LoadProcessor(ProcessorObject processorObject)
        {
            _processorObject = processorObject;
            RefreshUI();
        }

        private void RefreshUI()
        {
            if(_processorObject == null) return;
            
            nameText.text = $"{_processorObject.Data.ShopInfo.name}";
            costText.text = $"Cost: ${_processorObject.Data.UpfrontCost}";
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(_previewInstance != null) return;

            _previewInstance = Instantiate(previewUIPrefab, transform);
            _previewInstance.transform.parent = _shopUI.transform;
            _previewInstance.LoadInformation(_processorObject);
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(_previewInstance == null) return;
            
            Destroy(_previewInstance.gameObject);
            _previewInstance = null;
        }

        public void Buy()
        {
            _shopUI.Buy(_processorObject);
        }
    }
}