using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    [System.Serializable]
    public class Connection
    {
        [SerializeField] private ProcessorInstance inputMachine;
        [SerializeField] private ProcessorInstance outputMachine;

        public ProcessorInstance InputMachine => inputMachine;
        public ProcessorInstance OutputMachine => outputMachine;

        [SerializeField] private List<Vector2Int> occupiedPathCells = new List<Vector2Int>();
        public IReadOnlyList<Vector2Int> OccupiedPathCells => occupiedPathCells;

        [SerializeField] private GameObject connectionVisual;

        public void InitializeConnection(ProcessorInstance output, ProcessorInstance input, List<Vector2Int> path, GameObject visual = null)
        {
            outputMachine = output;
            inputMachine = input;
            occupiedPathCells = path;
            connectionVisual = visual;
        }

        public void RemoveConnection()
        {
            //remove connection
        }
    }
}