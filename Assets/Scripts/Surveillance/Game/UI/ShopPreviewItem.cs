using TMPro;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class ShopPreviewItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemDesc;

        
        public void Init(GameObject newPreview, string newItemName, string newItemDesc)
        {
            ItemPreviewer.LoadPreview(newPreview);
            itemName.text = newItemName;
            itemDesc.text = newItemDesc;
        }
    }
}