using TMPro;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class ShopPreviewItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemDesc;
        [SerializeField] private TextMeshProUGUI itemCost;
        
        public void Init(GameObject newPreview, ShopInfo shopInfo, int upfrontCost)
        {
            ItemPreviewer.LoadPreview(newPreview);
            itemName.text = shopInfo.name;
            itemDesc.text = shopInfo.desc;
            itemCost.text = $"Cost: {upfrontCost}¥";
        }
    }
}