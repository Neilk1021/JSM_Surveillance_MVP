using System.Collections;
using System.Collections.Generic;

namespace JSM.Surveillance
{
    public class OutputMachine : MachineInstance
    {
        private readonly Dictionary<Resource, int> _storedOutput =new Dictionary<Resource, int>();
        public  Dictionary<Resource, int> OutputResources => _storedOutput; 

        public void AddResource(Resource resource, int amnt)
        {
            _storedOutput.TryAdd(resource, 0);
            _storedOutput[resource] += amnt;
        }
    }
}