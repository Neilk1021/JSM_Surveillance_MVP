using System;
using JSM.Surveillance.Game;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace JSM.Surveillance
{
    public class InputMachineObject : MachineObject
    {
        [SerializeField] private TextMeshProUGUI inputText;
        private Source _source;
        
        private int _peopleInRange;

        public Source Source => _source;

        protected override void Start()
        {
            _peopleInRange = _source.GetRawResourceRate();
            if(inputText != null)
                inputText.text = $"{_source.SourceName}\n[{_source.resource.name}]";
            base.Start();
        }

        
        protected override void OnMouseDown()
        {
            if (Placed) {
                Grid.UIManager.SwitchUI(this);
            }

            base.OnMouseDown();
        }

        public void Initialize(Source newSource)
        {
            _source = newSource;
        }

        public override MachineInstance BuildInstance()
        {
            return new InputMachineInstance(_source, inventorySize, GetRootPosition());
        }
    }
}