using System.Collections;
using System.Collections.Generic;
using JSM.Surveillance.Game;
using TMPro;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class SourceUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI populationText;
        
        private Source _source;
        //TODO make it actually load the needed data.
        public void Init(Source source)
        {
            _source = source;
            if (Camera.main != null) transform.rotation = Camera.main.transform.rotation;
            populationText.text = $"Daily people watched: {source.GetPeopleInRange()}";
        }
    }
}
