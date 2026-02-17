using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace JSM.Surveillance.UI
{
    public class ComponentShopPreviewUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private TextMeshProUGUI costText;

        [SerializeField] private VideoPlayer videoPlayer;
        
        public void LoadInformation(ProcessorObject processor)
        {
            if(nameText != null)
                nameText.text = $"{processor.Data.ShopInfo.name}";
            
            if(descText != null)
                descText.text = $"{processor.Data.ShopInfo.desc}";
            
            if(costText != null)
                costText.text = $"{processor.Data.UpfrontCost} ¥";

            if(videoPlayer != null)
                videoPlayer.clip = processor.Data.ShopInfo.videoClip;

            //nameText.text = $"{machineObject.}";
        }


        public void LoadInformation(MachineObject machineObject)
        {
            if (nameText != null)
                nameText.text = $"{machineObject.GetMachineName()}";

            if (descText != null)
                descText.text = $"{machineObject.GetMachineDesc()}";


            if (costText != null)
                costText.text = $"${machineObject.GetMachineCost()} ¥";
            
            
            if(videoPlayer != null)
                videoPlayer.clip = machineObject.GetVideoClip();


            //nameText.text = $"{machineObject.}";
        }

    }
}