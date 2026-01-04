using JSM.Surveillance.Game;
using UnityEngine;

namespace JSM.Surveillance
{
    [System.Serializable]
    public class InputMachineInstance : MachineInstance
    {
        private readonly SourceData _sourceData;
        private readonly Source _source;
        
        private int _peopleInRange;
        public SourceData Data => _sourceData;
        public Source Source => _source;
        
        
        public override void ProcessTicks(int deltaTicks = 1)
        {
            AddOutput(_source.resource, _peopleInRange);
        }
        
        public InputMachineInstance(Source newSource, int inventorySize) : base(inventorySize)
        {
            _source = newSource;
            _sourceData = newSource.Data;
            _peopleInRange = _source.GetPeopleInRange();
        }
    }
}