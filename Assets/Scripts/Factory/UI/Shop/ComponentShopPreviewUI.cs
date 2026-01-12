using TMPro;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class ComponentShopPreviewUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private TextMeshProUGUI costText;

        public void LoadInformation(ProcessorObject processor)
        {
            nameText.text = $"{processor.Data.ShopInfo.name}";
            descText.text = $"{processor.Data.ShopInfo.desc}";
            costText.text = $"<i>Cost: ${processor.Data.UpfrontCost}</i>";

            //nameText.text = $"{machineObject.}";
        }
        
        
        public void LoadInformation(MachineObject machineObject)
        {
            nameText.text = $"{machineObject.GetMachineName()}";
            descText.text = $"{machineObject.GetMachineDesc()}";
            costText.text = $"<i>Cost: ${machineObject.GetMachineCost()}</i>";

            //nameText.text = $"{machineObject.}";
        }

    }
}