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
        
        private ProcessorInstance _processorInstance;
        private ComponentShopPreviewUI _previewInstance;
        private ComponentShopUI _shopUI;

        public void Load(ProcessorInstance processor, ComponentShopUI shop)
        {
            _shopUI = shop;
            LoadProcessor(processor);
        }
        
        private void LoadProcessor(ProcessorInstance processorInstance)
        {
            _processorInstance = processorInstance;
            RefreshUI();
        }

        private void RefreshUI()
        {
            if(_processorInstance == null) return;
            
            nameText.text = $"{_processorInstance.Data.ShopInfo.name}";
            costText.text = $"Cost: ${_processorInstance.Data.UpfrontCost}";
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(_previewInstance != null) return;

            _previewInstance = Instantiate(previewUIPrefab, transform);
            _previewInstance.LoadInformation(_processorInstance);
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(_previewInstance == null) return;
            
            Destroy(_previewInstance.gameObject);
            _previewInstance = null;
        }

        public void Buy()
        {
            _shopUI.Buy(_processorInstance);
        }
    }
}