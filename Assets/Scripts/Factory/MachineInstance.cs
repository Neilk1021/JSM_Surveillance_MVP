using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    public abstract class MachineInstance : Draggable
    {
        protected readonly Dictionary<Resource, int> InputResources = new();
        protected readonly Dictionary<Resource, int> OutputResources = new();
        
        public Dictionary<Resource, int> Inputs => InputResources;
        public Dictionary<Resource, int> Outputs => OutputResources;
        
        public override void Place(List<Vector2Int> newPositions, Vector3 worldPos, FactoryGrid grid)
        {
            base.Place(newPositions, worldPos, grid);
            
            Vector2Int root = GetRootPosition();
            foreach (var port in GetComponentsInChildren<ProcessorPort>())
            {
                grid.RegisterPort(port.SubcellPosition + root, port);
            }
        }
        

    }
}