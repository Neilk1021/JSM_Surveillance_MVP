using System.Collections.Generic;

namespace JSM.Surveillance
{
    [System.Serializable]
    public class OutputMachineInstance : MachineInstance
    {
        private readonly Dictionary<Resource, int> _storedOutput =new Dictionary<Resource, int>();

        public OutputMachineInstance(int inventorySize) : base(inventorySize)
        {
        }

        public  Dictionary<Resource, int> OutputResources => _storedOutput; 

        public void AddResource(Resource resource, int amnt)
        {
            _storedOutput.TryAdd(resource, 0);
            _storedOutput[resource] += amnt;
        }
    }
}