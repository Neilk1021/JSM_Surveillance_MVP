using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JSM.Surveillance.UI
{
    public class ImageNameGroup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;

        public void Load(string textStr, Sprite img)
        {
            image.gameObject.SetActive(img != null);
            text.text = textStr;
            image.sprite = img;
        }
    }
}