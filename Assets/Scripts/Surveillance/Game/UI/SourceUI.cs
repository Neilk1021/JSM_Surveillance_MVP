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

        private MapCellManager _manager;
        //TODO make it actually load the needed data.
        public void Init(Source source, MapCellManager manager)
        {
            _manager = manager;
            _source = source;
            if (Camera.main != null) transform.rotation = Camera.main.transform.rotation;
            populationText.text = $"Daily people watched: {source.GetPeopleInRange()}";
        }

        public Source GetSource()
        {
            return _source;
        }

        public void SellSource()
        {
            _source.CloseUI();
            SurveillanceGameManager.instance.SellSource(_source);
        }

        private void DestroySource()
        {
            _source.CloseUI();
            Destroy(_source.gameObject);
        }
    }
}
