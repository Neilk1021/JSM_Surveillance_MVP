using System;
using JSM.Surveillance.Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace JSM.Surveillance
{
    public class InputMachineObject : MachineObject
    {
        [SerializeField] private SourceData sourceData;
        [SerializeField] private Source source;
        
        private int _peopleInRange;

        public SourceData Data => sourceData;
        public Source Source => source;

        private void Start()
        {
            _peopleInRange = source.GetPeopleInRange();
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
            source = newSource;
            sourceData = newSource.Data;
        }

        public override MachineInstance BuildInstance()
        {
            return new InputMachineInstance(source, inventorySize);
        }
    }
}