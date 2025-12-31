using System;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    public abstract class MachineObject : Draggable
    {
        [SerializeField] protected int inventorySize = 20;
        private readonly List<ProcessorPortObject> _iPorts = new();
        private readonly List<ProcessorPortObject> _oPorts = new();

        public IReadOnlyList<ProcessorPortObject> InputPorts => _iPorts;
        public IReadOnlyList<ProcessorPortObject> OutputPorts => _oPorts;

        
        public override void Place(List<Vector2Int> newPositions, Vector3 worldPos, FactoryGrid grid)
        {
            base.Place(newPositions, worldPos, grid);
            
            Vector2Int root = GetRootPosition();

            
            _iPorts.Clear();
            _oPorts.Clear();
            foreach (var port in GetComponentsInChildren<ProcessorPortObject>())
            {
                grid.RegisterPort(port.SubcellPosition + root, port);

                if (port.Type == NodeType.End) {
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

        }

        private void OnDisable()
        {
            if (Grid == null) {
                return;
            }
            
        }

        public virtual void Sell()
        {
            foreach (var port in GetComponentsInChildren<ProcessorPortObject>())
            {
                var connection = port.ConnectionObject;
                Grid.UnregisterPort(port.SubcellPosition + GetRootPosition());
                
                if(connection == null) continue;
                Destroy(connection.gameObject);
            }
            
            Remove();
        }
        
        public virtual void Move()
        {
            foreach (var port in GetComponentsInChildren<ProcessorPortObject>())
            {
                var connection = port.ConnectionObject;
                Grid.UnregisterPort(port.SubcellPosition + GetRootPosition());
                if(connection == null) continue;
                Destroy(connection.gameObject);
            }
            
            ClearFromBoard();
            StartPlacement();
        }
        
        public abstract MachineInstance BuildInstance();
    }
}