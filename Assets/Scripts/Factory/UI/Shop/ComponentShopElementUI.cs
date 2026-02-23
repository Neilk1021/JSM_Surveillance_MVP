using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JSM.Surveillance.UI
{
    public class ComponentShopElementUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private ComponentShopPreviewUI previewUIPrefab;
        [SerializeField] private Image previewImage;
        
        private MachineObject _machineObject;
        private ComponentShopPreviewUI _previewInstance;
        private ComponentShopUI _shopUI;

        private RectTransform _shopRect;
        
        public void Load(MachineObject processor, ComponentShopUI shop)
        {
            _shopUI = shop;
            _shopRect = shop.GetComponent<RectTransform>();
            LoadProcessor(processor);
        }
        
        private void LoadProcessor(MachineObject processorObject)
        {
            _machineObject = processorObject;
            RefreshUI();
        }
        private static string ConvertMoneyToYenString(int amount)
        {
            return $"¥{amount:N0}";
        }

        private void RefreshUI()
        {
            if(_machineObject == null) return;

            string nameStr = "";
            string costStr = "";
            Sprite previewSprite = null;
            
            switch (_machineObject)
            {
                case ProcessorObject p0:
                    nameStr = $"{p0.Data.ShopInfo.name}";
                    costStr = ConvertMoneyToYenString(p0.Data.UpfrontCost);
                    previewSprite = p0.Data.ShopInfo.sprite;
                    break;
                case SplitterObject s0:
                    nameStr = $"{s0.Data.ShopInfo.name}";
                    costStr = ConvertMoneyToYenString(s0.Data.UpfrontCost); 
                    previewSprite = s0.Data.ShopInfo.sprite;
                    break;
                case MergerObject m0:
                    nameStr = $"{m0.Data.ShopInfo.name}";
                    costStr = ConvertMoneyToYenString(m0.Data.UpfrontCost); 
                    previewSprite = m0.Data.ShopInfo.sprite;
                    break;
                default:
                    return;
            }

            if (nameText != null) nameText.text = nameStr;
            if (costText != null) costText.text = costStr;
            if (previewImage != null) previewImage.sprite = previewSprite;

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(_previewInstance != null) return;

            _previewInstance = Instantiate(previewUIPrefab, transform);
            _previewInstance.transform.parent = _shopUI.transform;
            ClampRect();
            if (_machineObject is ProcessorObject p0)
            {
                _previewInstance.LoadInformation(p0);
            }
            else
            {
                _previewInstance.LoadInformation(_machineObject);
            }
        }

        private void ClampRect()
        {
            RectTransform previewRect = _previewInstance.GetComponent<RectTransform>();
            Vector3 pos = previewRect.localPosition; 
            
            float containerHalfHeight = _shopRect.rect.height * 0.5f;
            float childHalfHeight = previewRect.rect.height * 0.5f;

            float maxY = containerHalfHeight - childHalfHeight;
            
            pos.y = Mathf.Clamp(pos.y, -maxY, maxY);
            previewRect.localPosition = pos;
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