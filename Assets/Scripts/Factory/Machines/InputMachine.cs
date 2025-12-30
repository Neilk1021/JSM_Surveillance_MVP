using System;
using JSM.Surveillance.Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace JSM.Surveillance
{
    public class InputMachine : MachineInstance
    {
        [SerializeField] private SourceData sourceData;
        [SerializeField] private Source source;
        
        //TODO REMOVE TS
        [SerializeField] private Resource testInputResource;
        private readonly float _cycleTime = 1;
        private float _timeSinceLastCycle = 0;
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
            OutputResource.amount = 0;
            OutputResource.resource = testInputResource;
        }

        /*private void Update()
        {
            _timeSinceLastCycle += Time.deltaTime;

            //TODO REMOVE REMOVE REMOVE
            if (_timeSinceLastCycle > _cycleTime)
            {
                OutputResource.amount += _peopleInRange;
                _timeSinceLastCycle = 0;
                MachineStateUpdated();
            }
        }*/


    }
}