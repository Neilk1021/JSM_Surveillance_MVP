using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JSM.Surveillance
{
    [System.Serializable]
    public class Connection : CellOccupier
    {
        [SerializeField] private GameObject connectionVisual;
        
        private ProcessorInstance _inputMachine; 
        private ProcessorInstance _outputMachine;
        private LineRenderer _lineRenderer;

        public LineRenderer LineRenderer => _lineRenderer;
        public ProcessorInstance InputMachine => _inputMachine;
        public ProcessorInstance OutputMachine => _outputMachine;
        

        public void InitializeConnection(ProcessorInstance output, ProcessorInstance input, List<Vector2Int> path)
        {
            base.Initialize(path); 
            _outputMachine = output;
            _inputMachine = input;
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public void RemoveConnection()
        {
            //remove connection
        }
    }
}