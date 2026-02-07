using System;
using System.Collections.Generic;
using JSM.Surveillance.Saving;
using UnityEngine;

namespace JSM.Surveillance
{
    [System.Serializable]
    public class OutputMachineInstance : MachineInstance
    {
        private readonly Dictionary<Resource, int> _storedOutput =new Dictionary<Resource, int>();

        public OutputMachineInstance(int inventorySize, Vector2Int pos) : base(inventorySize, Vector2IntToGuid(pos))
        {
        }
        
        public OutputMachineInstance(int inventorySize, Guid guid) : base(inventorySize, guid)
        {
        }

        public  Dictionary<Resource, int> OutputResources => _storedOutput; 

        public void AddResource(Resource resource, int amnt)
        {
            _storedOutput.TryAdd(resource, 0);
            _storedOutput[resource] += amnt;
        }

        public override MachineStateDto BuildMachineDTO()
        {
            return new OutputInstanceDTO(this);
        }
    }
}