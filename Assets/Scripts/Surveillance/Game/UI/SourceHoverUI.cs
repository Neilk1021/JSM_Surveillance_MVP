using JSM.Surveillance.Game;
using TMPro;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class SourceHoverUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        
        public void Load(Source source)
        {
            if (Camera.main != null) transform.rotation = Camera.main.transform.rotation;
            text.text = source.SourceName;
        }
    }
}