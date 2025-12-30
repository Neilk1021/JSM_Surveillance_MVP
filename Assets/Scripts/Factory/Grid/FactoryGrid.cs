using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JSM.Surveillance.Game;
using JSM.Surveillance.UI;
using UnityEngine;

namespace JSM.Surveillance
{
    public partial class FactoryGrid : MonoBehaviour
    {
        [Header("Grid Input")]
        [SerializeField] private InputMachine gridInputPrefab;
        [SerializeField] private Vector2Int inputPosition;
        
        [Header("Grid Output")] 
        [SerializeField] private OutputMachine gridOutputPrefab;
        [SerializeField] private Vector2Int outputPosition;
        
        private Vector2Int _hoveredCell = new Vector2Int(-1, -1);

        //Allows multiple grids where we just switch between different grids as needed.

        private readonly Dictionary<Vector2Int, ProcessorPort> _ports = new Dictionary<Vector2Int, ProcessorPort>(); 
        
        private FactoryCell[,] _grid; 
        
        private Camera _camera;

        private UIManager _uiManager;
        private InputMachine _gridInput;
        private OutputMachine _gridOutput;

        public UIManager UIManager => _uiManager;
        private ConnectionManager _connectionManager;
        public  Source Source { get; private set; }
        
        public ConnectionManager ConnectionManager => _connectionManager;
        

        private void Awake()
        {
            _uiManager = GetComponent<UIManager>();
            
            _connectionManager = GetComponentInChildren<ConnectionManager>();
            _camera = GetComponentInParent<Camera>();
            _camera ??= Camera.main;
            
            InitializeGrid();
        }

        private void Start()
        {
            InitializeInputOutput();
        }

        private void InitializeGrid()
        {
            _grid = new FactoryCell[gridWidth, gridHeight];
            for (int i = 0; i <  gridWidth; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    _grid[i, j] = new FactoryCell(i,j);
                }
            }
        }

        private void InitializeInputOutput()
        {
            _gridInput = Instantiate(gridInputPrefab, transform);
            _gridOutput = Instantiate(gridOutputPrefab, transform);
            
            _gridInput.Initialize(Source);
            
            InitializeCellOccupier(_gridInput , inputPosition);
            InitializeCellOccupier(_gridOutput, outputPosition);
        }

        private void InitializeCellOccupier(CellOccupier occupier, Vector2Int leftCornerGridPos)
        {
            List<Vector2Int> occupierPositions = new();
            for (int i = 0; i < occupier.Size.x; i++)
            {
                for (int j = 0; j < occupier.Size.y; j++)
                {
                    occupierPositions.Add(new Vector2Int(i,j) + leftCornerGridPos);
                }
            }

            PlaceCellOccupier(occupier, occupierPositions);
        }
        
        private void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.P))
            {
                StartSimulation();
            }*/
            
            UpdateHoveredCell();
        }

        private void UpdateHoveredCell()
        {
            if (_camera == null) return;

            var mouseWorldPos = GetMouseWorldPos3D();
            if(mouseWorldPos == Vector3.negativeInfinity) return;
            Vector2Int newHoveredCell = GetGridPosition(mouseWorldPos);

            if (_hoveredCell == newHoveredCell) return;
            
            if(_hoveredCell != new Vector2Int(-1,-1)) 
                _grid[_hoveredCell.x, _hoveredCell.y].ExitHover();
                
            _hoveredCell = IsValidGridPosition(newHoveredCell.x, newHoveredCell.y) ? newHoveredCell : new Vector2Int(-1, -1);
            
            if (_hoveredCell != new Vector2Int(-1, -1))
                _grid[_hoveredCell.x, _hoveredCell.y].EnterHover();
        }
        

        public bool PlaceConnection(Connection connectionPrefab, ProcessorPort startPort, ProcessorPort endPort, List<Vector2Int> connectionPositions)
        {
            if (connectionPositions.Contains(new Vector2Int(-1, -1))) {
                return false;
            }
            if (!VerifyPlacementValid(connectionPositions)) {
                return false;
            }
            
            var connectionObj = Instantiate(connectionPrefab, transform);
            connectionObj.InitializeConnection(startPort, endPort, this, connectionPositions);
            connectionObj.transform.parent = transform;

            return true;
        }

        public bool PlaceCellOccupierAtCurrentPosition(CellOccupier cellOccupier)
        {
            return PlaceCellOccupier(cellOccupier, GetOccupierPositions(cellOccupier).Select(x=>GetGridPosition(x)).ToList());
        }

        public bool IsCellOccupierValid(CellOccupier occupier)
        {
            return VerifyPlacementValid(GetOccupierPositions(occupier).Select(x=>GetGridPosition(x)).ToList());
        }

        private bool PlaceCellOccupier(CellOccupier cellOccupier, List<Vector2Int> gridPositions)
        {
            cellOccupier.transform.parent = transform;

            if (gridPositions.Contains(new Vector2Int(-1, -1))) {
                return false;
            }

            if (!VerifyPlacementValid(gridPositions)) {
                return false;
            }
            Vector3 worldPos = new Vector3(
                gridPositions.Average(x => GetWorldPosition(x).x) + cellSize / 2,
                gridPositions.Average(x => GetWorldPosition(x).y) + cellSize / 2,
                transform.position.z
            ); 
            
            cellOccupier.Place(gridPositions, worldPos, this);
            return true;
        }

        public void RegisterPort(Vector2Int pos, ProcessorPort port )
        {
            _ports.TryAdd(pos, port);
        }
        public void UnregisterPort(Vector2Int pos)
        {
            _ports.Remove(pos);
        }
        
        public FactoryCell GetCell(int x, int y)
        {
            if (!IsValidGridPosition(x, y)) 
                throw new ArgumentException($"{x}, {y} is not in {gameObject.name}'s grid!");
            
            return _grid[x, y];
        }


        public void SetSource(Source source)
        {
            Source = source;
        }

         public FactoryCell this[int i, int i1] => _grid[i, i1];

         // ReSharper disable Unity.PerformanceAnalysis
         private void StartSimulation()
         {
             StartChildren(_gridInput);
             FeedAllMachines(_gridInput);
         }

         private void StartChildren(MachineInstance machineInstance)
         {
             if(machineInstance == null) return;
             
             if (machineInstance is ProcessorInstance processorInstance)
             {
                 if(processorInstance.IsRunning) return;
                 processorInstance.StartSimulation();
             }

             foreach (var port in machineInstance.OutputPorts)
             {
                 if (port.Connection != null)
                 {
                     StartChildren(port.Connection.OutputMachine);
                 }
             }
         }
         
         // ReSharper disable Unity.PerformanceAnalysis
         public void ReloadMachineResources()
         {
            FeedAllMachines(_gridInput);
         }

        
         private void FeedAllMachines(MachineInstance currentMachine)
         {
             if (currentMachine == null) return;
             if (currentMachine is OutputMachine outputMachine)
             {
                 Source.HandleOutputResourceVolume(outputMachine);
                 return;
             }
             
             
             List<MachineInstance> nextMachines = new List<MachineInstance>();
             var splitAmnt = 0;
             
             foreach (var port in currentMachine.OutputPorts)
             {
                 if (port.Connection == null) continue; 
                 MachineInstance nextMachine = port.Connection.OutputMachine;
                 nextMachines.Add(nextMachine);
                 switch (nextMachine)
                 {
                     case ProcessorInstance nextProcessor:
                         if (nextProcessor.Recipe != null &&
                             nextProcessor.Recipe.RequiresInput(currentMachine.Output.resource))
                         {
                             ++splitAmnt;
                         }
                         break;
                     case OutputMachine:
                         ++splitAmnt;
                         break;
                     default: 
                         break;
                 }
             }

             if (currentMachine.Output.amount > 0)
             {
                 FeedChildMachines(currentMachine, nextMachines, splitAmnt);
             }

             foreach (var machine in nextMachines)
             {
                 FeedAllMachines(machine);
             }
         }

         private void FeedChildMachines(MachineInstance currentMachine, List<MachineInstance> nextMachines, int splitAmnt)
         {
             foreach (var machine in nextMachines.TakeWhile(machine => splitAmnt != 0))
             {
                 switch (machine)
                 {
                     case ProcessorInstance processor when processor.Recipe == null || !processor.Recipe.RequiresInput(currentMachine.Output.resource):
                         break;
                     case ProcessorInstance processor:
                         FeedProcessorInputs(currentMachine, processor, splitAmnt);
                         splitAmnt -= 1;
                         break;
                     case OutputMachine output:
                         FeedOutputResources(currentMachine, output, splitAmnt);
                         splitAmnt -= 1;
                         break;
                 }
             }
         }

         private void FeedOutputResources(MachineInstance previous, OutputMachine outputMachine, int splitAmnt)
         {
             int amnt = previous.RemoveOutput(previous.Output.amount / splitAmnt);
             outputMachine.AddResource(previous.Output.resource, amnt);
         }
         
         private void FeedProcessorInputs(MachineInstance previous, ProcessorInstance current, int splitAmnt)
         {
             int amnt = previous.RemoveOutput( previous.Output.amount / splitAmnt);
             int excess = current.AddInput(previous.Output.resource, amnt);
             previous.AddOutput(previous.Output.resource, excess);
         }
         private bool IsValidTarget(MachineInstance m, Resource res) => m switch
         {
             ProcessorInstance p => p.Recipe?.RequiresInput(res) ?? false,
             OutputMachine => true,
             _ => false
         };
    }
}