using System;
using System.Collections;
using System.Collections.Generic;
using JSM.Surveillance.Game;
using Surveillance.Game;
using UnityEngine;

namespace  JSM.Surveillance.UI
{
    public class ShopBuyButton : MonoBehaviour
    {
        [SerializeField] private SurveillanceShop surveillanceShop;
        
        [SerializeField] private Maintainable itemToBuy;
        [SerializeField] private GameObject itemPreviewUIPrefab;
        [SerializeField] private Vector2 previewOffset = new Vector2(-300,0);
        private GameObject itemPreviewUI = null;

        private void Awake()
        {
            surveillanceShop = GetComponentInParent<SurveillanceShop>();
        }

        public void Buy()
        {
            if (itemToBuy is SourceData sourceData) {
                surveillanceShop.BuySource(sourceData);
            }
        }
        
        public void OnPointerEnter()
        {
            if (itemPreviewUI != null)
            {
                Destroy(itemPreviewUI);
            }

            itemPreviewUI = Instantiate(itemPreviewUIPrefab, transform);
            
            itemPreviewUI.transform.position = new Vector3(
                itemPreviewUI.transform.position.x + previewOffset.x, 
                itemPreviewUI.transform.position.y + previewOffset.y, 
                itemPreviewUI.transform.position.z 

                );
            
            itemPreviewUI.GetComponent<ShopPreviewItem>().Init(
                itemToBuy.ShopInfo.itemModelPrefab, 
                itemToBuy.ShopInfo,
                itemToBuy.UpfrontCost
                );
        }

        public void OnPointerExit()
        {
            Destroy(itemPreviewUI);
            itemPreviewUI = null;
        }
    }
}
