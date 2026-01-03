using System;
using JSM.Surveillance.Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace JSM.Surveillance
{
    public class InputMachineObject : MachineObject
    {
        private Source _source;
        
        private int _peopleInRange;

        public Source Source => _source;

        protected override void Start()
        {
            _peopleInRange = _source.GetPeopleInRange();
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
            return new InputMachineInstance(_source, inventorySize);
        }
    }
}