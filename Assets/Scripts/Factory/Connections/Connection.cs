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
    public class Connection : CellOccupier
    {
        private ProcessorPort _startPort;
        private ProcessorPort _endPort;
        
        private MachineInstance _inputMachine; 
        private MachineInstance _outputMachine;
        private ConnectionRenderer _renderer;

        public MachineInstance InputMachine => _inputMachine;
        public MachineInstance OutputMachine => _outputMachine;

        public ProcessorPort StartPort => _startPort;
        public ProcessorPort EndPort => _endPort;

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
                if (Input.GetMouseButtonDown(0))
                {
                    Remove();
                }
            }
        }

        public void InitializeConnection(ProcessorPort start, ProcessorPort end, FactoryGrid grid, List<Vector2Int> path)
        {
            base.Initialize(path, grid);
            _startPort = start;
            _endPort = end;

            _startPort.SetConnection(this);
            _endPort.SetConnection(this);
            
            _inputMachine = start.Type == NodeType.Output ? start.Owner : end.Owner;
            _outputMachine = start.Type == NodeType.Input ? start.Owner : end.Owner;
            
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