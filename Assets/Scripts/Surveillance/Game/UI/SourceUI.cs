using System.Collections;
using System.Collections.Generic;
using JSM.Surveillance.Game;
using TMPro;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class SourceUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI producingText;
        [SerializeField] private TextMeshProUGUI linkedToText;
        
        [SerializeField] private TextMeshProUGUI populationText;
        
        private Source _source;

        private MapCellManager _manager;
        //TODO make it actually load the needed data.
        public void Init(Source source, MapCellManager manager)
        {
            _manager = manager;
            _source = source;
            _source.OnModified.AddListener(ReloadUI);
            if (Camera.main != null) transform.rotation = Camera.main.transform.rotation;
            ItemPreviewer.LoadPreview(source.Data.ShopInfo.itemModelPrefab);
            ReloadUI();
        }

        private void ReloadUI()
        {
            populationText.text = $"Daily people watched: {_source.GetPeopleInRange()}";
            nameText.text = $"{_source.Data.ShopInfo.name}";
            descriptionText.text = $"{_source.Data.ShopInfo.desc}";
            
            //TODO replace this with an actual check to see what its producing
            producingText.text = $"Producing: [Nothing] Must Modify";
            linkedToText.text = $"Sending to: [Nothing] Selling Data";
        }

        public Source GetSource()
        {
            return _source;
        }

        public void LinkSource()
        {
            if(_source.Grid != null && _source.Grid.MouseOverGrid()) return;
            
            _source.StartLinking();
            _source.CloseUI();
        }
        
        public void ModifySource()
        {
            _source.SpawnGrid();
        }

        public void SellSource()
        {
            if(_source.Grid != null && _source.Grid.MouseOverGrid()) return;
            
            _source.CloseUI();
            SurveillanceGameManager.instance.SellSource(_source);
        }

        private void DestroySource()
        {
            _source.CloseUI();
            _source.Destroy();
        }
    }
}
