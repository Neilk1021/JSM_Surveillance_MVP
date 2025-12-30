using System;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    public abstract class MachineInstance : Draggable
    {
        [SerializeField] private int inventorySize = 20; 
        protected readonly Dictionary<Resource, int> InputResources = new();
        protected ResourceVolume OutputResource;
        
        public Dictionary<Resource, int> Inputs => InputResources;
        public ResourceVolume Output => OutputResource;

        private readonly List<ProcessorPort> _iPorts = new();
        private readonly List<ProcessorPort> _oPorts = new();

        public IReadOnlyList<ProcessorPort> InputPorts => _iPorts;
        public IReadOnlyList<ProcessorPort> OutputPorts => _oPorts;

        public event Action machineStateUpdated; 
        
        public override void Place(List<Vector2Int> newPositions, Vector3 worldPos, FactoryGrid grid)
        {
            base.Place(newPositions, worldPos, grid);
            
            Vector2Int root = GetRootPosition();

            machineStateUpdated += grid.ReloadMachineResources;
            
            _iPorts.Clear();
            _oPorts.Clear();
            foreach (var port in GetComponentsInChildren<ProcessorPort>())
            {
                grid.RegisterPort(port.SubcellPosition + root, port);

                if (port.Type == NodeType.Input) {
                    _iPorts.Add(port);
                }
                else {
                    _oPorts.Add(port);
                }
            }
        }

        private void OnEnable()
        {
            if (Grid == null) {
                return;
            }

            machineStateUpdated += Grid.ReloadMachineResources;
        }

        private void OnDisable()
        {
            if (Grid == null) {
                return;
            }
            
            machineStateUpdated -= Grid.ReloadMachineResources;
        }

        public virtual void Sell()
        {
            foreach (var port in GetComponentsInChildren<ProcessorPort>())
            {
                var connection = port.Connection;
                Grid.UnregisterPort(port.SubcellPosition + GetRootPosition());
                
                if(connection == null) continue;
                Destroy(connection.gameObject);
            }
            
            Remove();
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
            if (InputResources[resource] > inventorySize)
            {
                int delta = InputResources[resource] - inventorySize;
                InputResources[resource] = inventorySize;

                return delta;
            }

            return 0;
        }

        public int RemoveOutput(int amnt)
        { 
            int val = Mathf.Min(amnt, OutputResource.amount);
            OutputResource.amount -= val;
            return val;
        }
        
        public virtual void Move()
        {
            foreach (var port in GetComponentsInChildren<ProcessorPort>())
            {
                var connection = port.Connection;
                Grid.UnregisterPort(port.SubcellPosition + GetRootPosition());
                if(connection == null) continue;
                Destroy(connection.gameObject);
            }
            
            ClearFromBoard();
            StartPlacement();
        }

        public void AddOutput(Resource resource, int amount)
        {
            if(OutputResource.resource != resource) return;
            
            OutputResource.amount += amount;
            if (OutputResource.amount > inventorySize)
            {
                InputResources[resource] = inventorySize;
            }
        }

        protected void MachineStateUpdated()
        {
            machineStateUpdated?.Invoke();
        }
    }
}