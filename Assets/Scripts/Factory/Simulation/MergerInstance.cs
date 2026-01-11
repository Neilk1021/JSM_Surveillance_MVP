namespace JSM.Surveillance
{
    public class MergerInstance : MachineInstance
    {
        private Maintainable _data;
        public MergerInstance(int inventorySize, Maintainable data) : base(inventorySize)
        {
            _data = data;
        }
        
        
        public override void ProcessTicks(int deltaTicks = 1)
        {
            if (OutputResources.resource != null)
            {
                if (InputResources.ContainsKey(OutputResources.resource))
                {
                    OutputResources.amount += InputResources[OutputResources.resource];
                    InputResources[OutputResources.resource] = 0;
                    return;
                }
                
                
                if(OutputResources.amount >= 0 ) return;
            }
            
            
            foreach (var inputResource in InputResources)
            {
                OutputResources.resource = inputResource.Key;
                OutputResources.amount = inputResource.Value;
                break;
            }
        }
        
    }
}