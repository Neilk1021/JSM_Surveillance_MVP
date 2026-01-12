using JSM.Surveillance.Game;
using UnityEngine;

namespace JSM.Surveillance
{
    public class ExternalInputInstance : MachineInstance
    {
        private readonly int _incomingSourceIndex = 0;
        private readonly Source _source;
        
        
        public override void ProcessTicks(int deltaTicks = 1)
        {
            Source prev = _source.IncomingSourceLinks[_incomingSourceIndex];
            if(!prev) return;

            var vol = prev.TakeResources();
            AddOutput(vol.resource, vol.amount);
        }
        
        public ExternalInputInstance(Source parent, int index , int inventorySize, Vector2Int pos) : base(inventorySize, Vector2IntToGuid(pos))
        {
            _incomingSourceIndex = index;
            _source = parent;
        }
    }
}