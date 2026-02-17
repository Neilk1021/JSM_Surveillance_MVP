using System;
using JSM.Surveillance.Game;
using JSM.Surveillance.Saving;
using UnityEngine;

namespace JSM.Surveillance
{
    [System.Serializable]
    public partial class InputMachineInstance : MachineInstance
    {
        private readonly SourceData _sourceData;
        private readonly Source _source;
        
        private int _peopleInRange = -1;
        public SourceData Data => _sourceData;
        public Source Source => _source;
        private int _currentTicks = 0;
        
        
        public override void ProcessTicks(int deltaTicks = 1)
        {
            if (_peopleInRange == -1)
            {
                _peopleInRange = _source.GetRawResourceRate();
            }
            
            var ticksPerHr = SurveillanceGameManager.instance.Simulator.TicksPerHR;
            _currentTicks += deltaTicks;

            if (_currentTicks < ticksPerHr) return;
            
            int mult = _currentTicks / ticksPerHr;
            _currentTicks %= ticksPerHr;
                
            AddOutput(_source.resource, mult * _peopleInRange);
        }


        public InputMachineInstance(Source newSource, int inventorySize, Guid guid) : base(inventorySize, guid)
        {
            _source = newSource;
            _sourceData = newSource.Data;
        }
        
        public InputMachineInstance(Source newSource, int inventorySize, Vector2Int pos) : base(inventorySize, Vector2IntToGuid(pos))
        {
            _source = newSource;
            _sourceData = newSource.Data;
        }
    }
}