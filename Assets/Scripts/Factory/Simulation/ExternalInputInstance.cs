using System;
using JSM.Surveillance.Game;
using JSM.Surveillance.Saving;
using UnityEngine;

namespace JSM.Surveillance
{
    public partial class ExternalInputInstance : MachineInstance
    {
        private readonly int _incomingSourceIndex = 0;
        private readonly Source _source;
        public Source Source => _source;
        public int IncomingSourceIndex => _incomingSourceIndex;

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
        
        
        public ExternalInputInstance(Source parent, int index , int inventorySize, Guid id) : base(inventorySize, id)
        {
            _incomingSourceIndex = index;
            _source = parent;
        }
    }
}