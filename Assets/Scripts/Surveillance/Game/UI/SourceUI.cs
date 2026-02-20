using System;
using System.Collections;
using System.Collections.Generic;
using JSM.Surveillance.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JSM.Surveillance.UI
{
    public class SourceUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI producingText;
        [SerializeField] private TextMeshProUGUI linkedToText;
        
        [SerializeField] private TextMeshProUGUI populationText;
        [SerializeField] private Canvas canvas;

        [SerializeField] private Button linkButton;
        [SerializeField] private Button sellButton;
        
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
            canvas.worldCamera = GameObject.FindGameObjectWithTag("UICam").GetComponent<Camera>();
        }

        private void OnEnable()
        {
            SurveillanceGameManager.instance.Simulator.OnStart += ReloadUI;
        }

        private void OnDisable()
        {
            SurveillanceGameManager.instance.Simulator.OnStart -= ReloadUI;
        }

        private void ReloadUI()
        {
            populationText.text = $"{_source.GetRawResourceRate()}";
            nameText.text = $"{_source.Data.ShopInfo.name}";
            descriptionText.text = $"{_source.Data.ShopInfo.desc}";

            linkButton.enabled = Source.Editable;
            sellButton.enabled = Source.Editable;
            
            var output = _source.GetOutputResourceType();
            producingText.text = output? $"[{output.ResourceName}] - ¥{output.Value}" : $"Nothing";
            linkedToText.text = _source.NextSource? $"{_source.NextSource.SourceName}" : $"Nothing [Selling]";
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

        public void CloseUI()
        {
            Destroy(gameObject);
        }
    }
}
