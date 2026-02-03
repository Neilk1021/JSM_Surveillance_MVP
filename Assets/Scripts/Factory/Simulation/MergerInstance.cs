using JSM.Surveillance.Saving;
using UnityEngine;

namespace JSM.Surveillance
{
    public class MergerInstance : MachineInstance
    {
        private Maintainable _data;
        public MergerInstance(int inventorySize, Maintainable data, Vector2Int pos) : base(inventorySize, Vector2IntToGuid(pos))
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

        public override MachineStateDto BuildMachineDTO()
        {
            //TODO implement
            throw new System.NotImplementedException();
        }
    }
}