using System;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    [System.Serializable]
    public class MachineInstance
    {
        private int _inventorySize; 
        protected readonly Dictionary<Resource, int> InputResources = new();
        protected ResourceVolume OutputResources;
        
        private readonly List<ProcessorPort> _iPorts = new();
        private readonly List<ProcessorPort> _oPorts = new();
        
        public IReadOnlyList<ProcessorPort> EndPorts => _iPorts;
        public IReadOnlyList<ProcessorPort> StartingPorts => _oPorts;
        
        public Dictionary<Resource, int> Inputs => InputResources;
        public ResourceVolume Output => OutputResources;

        public virtual event Action<Resource> OnResourceProduced;  
        
        public MachineInstance(int inventorySize)
        {
            _inventorySize = inventorySize;
        }

        /// <summary>
        /// Adds an input resource and a given amount.
        /// </summary>
        /// <param name="resource">The resource to add.</param>
        /// <param name="amount">The amount to add.</param>
        /// <returns>The overflow of the machine if too much was added.</returns>
        public int AddInput(Resource resource, int amount)
        {
            InputResources.TryAdd(resource, 0);
            InputResources[resource] += amount;
            if (InputResources[resource] > _inventorySize)
            {
                int delta = InputResources[resource] - _inventorySize;
                InputResources[resource] = _inventorySize;

                return delta;
            }

            return 0;
        }

        public virtual void ProcessTicks(int deltaTicks = 1){}
        
        public int RemoveOutput(int amnt)
        { 
            int val = Mathf.Min(amnt, OutputResources.amount);
            OutputResources.amount -= val;
            return val;
        }

        public void AddOutput(Resource resource, int amount)
        {
            if (OutputResources.resource != resource)
            {
                OutputResources.resource = resource;
                OutputResources.amount = 0;
            } 
            
            OutputResources.amount += amount;
            if (OutputResources.amount > _inventorySize)
            {
                OutputResources.amount = _inventorySize;
            }
        }

        public void AddOutputPort(ProcessorPort oP)
        {
            _oPorts.Add(oP);
        }

        public void AddInputPort(ProcessorPort iP)
        {
            _iPorts.Add(iP);
        }


    }
}