using JSM.Surveillance.Game;

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
        
        public ExternalInputInstance(Source parent, int index , int inventorySize) : base(inventorySize)
        {
            _incomingSourceIndex = index;
            _source = parent;
        }
    }
}