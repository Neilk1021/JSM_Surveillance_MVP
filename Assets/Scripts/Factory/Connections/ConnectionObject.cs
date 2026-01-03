using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JSM.Surveillance
{
    public enum CellOccupierStatus
    {
        NotHovering,
        Hovering,
        Selected
    }
    
    [System.Serializable]
    public class ConnectionObject : CellOccupier
    {
        private ProcessorPortObject _startPortObject;
        private ProcessorPortObject _endPortObject;
        
        private MachineObject _endMachine; 
        private MachineObject _startMachine;
        private ConnectionRenderer _renderer;

        public MachineObject EndMachine => _endMachine;
        public MachineObject StartMachine => _startMachine;

        public ProcessorPortObject StartPortObject => _startPortObject;
        public ProcessorPortObject EndPortObject => _endPortObject;

        private CellOccupierStatus _status;  
        
        private void Awake()
        {
            _renderer = GetComponent<ConnectionRenderer>();
        }

        private void Update()
        {
            _renderer.Render(_status);
            if (_status == CellOccupierStatus.Hovering)
            {
                if (Input.GetMouseButtonDown(0) && !Grid.UIManager.IsUIOpen())
                {
                    Remove();
                }
            }
        }

        public void InitializeConnection(ProcessorPortObject start, ProcessorPortObject end, FactoryGrid grid, List<Vector2Int> path)
        {
            base.Initialize(path, grid);
            _startPortObject = start;
            _endPortObject = end;
            
            _startPortObject.SetConnection(this);
            _endPortObject.SetConnection(this);
            
            _endMachine = start.Type == NodeType.End ? start.Owner : end.Owner;
            _startMachine = start.Type == NodeType.Start ? start.Owner : end.Owner;
            
            _renderer.PlaceConnection();
        }


        public override void Entered()
        {
            if(_status != CellOccupierStatus.Selected)
                _status = CellOccupierStatus.Hovering;
        }

        public override void Exited()
        {
            
            if(_status != CellOccupierStatus.Selected)
                _status = CellOccupierStatus.NotHovering;

        }
    }
}