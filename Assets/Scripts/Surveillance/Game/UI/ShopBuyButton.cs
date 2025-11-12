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
        [SerializeField] private Maintainable itemToBuy;
        [SerializeField] private GameObject itemPreviewUIPrefab;
        [SerializeField] private Vector2 previewOffset = new Vector2(-300,0);
        private GameObject itemPreviewUI = null;
        
        public void Buy()
        {
            if (itemToBuy.GetType() == typeof(SourceData))
            {
                var obj = Instantiate(((SourceData)itemToBuy).Source.gameObject);
                obj.GetComponent<Source>().Init(FindObjectOfType<MapCellManager>());
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
                itemToBuy.ShopInfo.name, 
                itemToBuy.ShopInfo.desc
                );
        }

        public void OnPointerExit()
        {
            Destroy(itemPreviewUI);
            itemPreviewUI = null;
        }
    }
}
