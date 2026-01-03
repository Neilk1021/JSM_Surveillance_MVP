using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JSM.Surveillance.Game;
using JSM.Surveillance.Saving;
using JSM.Surveillance.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace JSM.Surveillance
{
    [System.Serializable]
    public struct ExternalInputMachinePlacement
    {
        public ExternalInputObject machineObject;
        public Vector2Int machinePosition;
    }
    
    public partial class FactoryGrid : MonoBehaviour
    {
        [Header("Grid End")]
        [SerializeField] private InputMachineObject gridInputPrefab;
        [SerializeField] private Vector2Int inputPosition;
        
        [Header("Grid Start")] 
        [SerializeField] private OutputMachine gridOutputPrefab;
        [SerializeField] private Vector2Int outputPosition;

        [Header("External Inputs")] [SerializeField]
        private List<ExternalInputMachinePlacement> externalMachinePlacements;
        
        
        private Vector2Int _hoveredCell = new Vector2Int(-1, -1);

        [SerializeField] private MachineBank machineBank;
        
        //Allows multiple grids where we just switch between different grids as needed.

        private readonly Dictionary<Vector2Int, ProcessorPortObject> _ports = new Dictionary<Vector2Int, ProcessorPortObject>(); 
        
        private FactoryCell[,] _grid; 
        
        private Camera _camera;

        private UIManager _uiManager;
        private InputMachineObject _gridInput;
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
        }

        private void Start()
        {
        }

        public void Initialize(FactoryBlueprint blueprint = null)
        {
            _uiManager ??= GetComponent<UIManager>();
            _connectionManager ??= GetComponentInChildren<ConnectionManager>();
            _camera ??= GetComponentInParent<Camera>();
            _camera ??= Camera.main;
            
            InitializeGrid();
            InitializeInputOutput();
            InitializeExternalMachines();
            
            if(blueprint == null) return;
            
            LoadFactoryLayout(blueprint);
        }

        private void InitializeExternalMachines()
        {
            for(int i = 0; i < externalMachinePlacements.Capacity && i < Source.MaxIncomingSourceLinks; ++i)
            {
                var newMachine = Instantiate(externalMachinePlacements[i].machineObject, transform);
                newMachine.Initialize(Source, i);
                InitializeCellOccupier(newMachine, externalMachinePlacements[i].machinePosition);
            }
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
            /*if (End.GetKeyDown(KeyCode.P))
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
        

        public bool PlaceConnection(ConnectionObject connectionObjectPrefab, ProcessorPortObject startPortObject, ProcessorPortObject endPortObject, List<Vector2Int> connectionPositions)
        {
            if (connectionPositions.Contains(new Vector2Int(-1, -1))) {
                return false;
            }
            if (!VerifyPlacementValid(connectionPositions)) {
                return false;
            }
            
            var connectionObj = Instantiate(connectionObjectPrefab, transform);
            connectionObj.InitializeConnection(startPortObject, endPortObject, this, connectionPositions);
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

        public void RegisterPort(Vector2Int pos, ProcessorPortObject portObject )
        {
            _ports.TryAdd(pos, portObject);
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

        public FactoryGridSimulation BuildSimulator()
        {
            return new FactoryGridSimulation(GetComponentsInChildren<MachineObject>(),Source);
        }

        public void SaveGridToSource()
        {
            //todo save position stuff

            Source.SetLastLayout(SaveCurrentLayout());
            Source.SetSimulation(BuildSimulator());
        }

        public void CloseGrid()
        {
            SaveGridToSource();
            Destroy(gameObject);
        }

        public FactoryBlueprint SaveCurrentLayout()
        {
            var machineObjects = GetComponentsInChildren<MachineObject>();
            var connectionObjects = GetComponentsInChildren<ConnectionObject>();

            List<MachineNode> machineNodes = machineObjects.Select(
                machineObject => new MachineNode()
                {
                    positions = machineObject.Positions.ToList(),
                    prefabId = machineObject.GetPrefabId(), 
                    recipeId = machineObject is ProcessorObject pO && pO.Recipe != null ? pO.Recipe.Guid : "",
                    localPos = machineObject.transform.localPosition
                }).ToList();

            List<ConnectionEdge> connectionEdges = connectionObjects.Select(connectionObject => new ConnectionEdge()
            {
                fromPos = connectionObject.StartPortObject.SubcellPosition +
                          connectionObject.StartMachine.GetRootPosition(),
                toPos = connectionObject.EndPortObject.SubcellPosition +
                        connectionObject.EndMachine.GetRootPosition(),

                outputPortIndex =
                    connectionObject.StartMachine.GetOutputPortIndex(connectionObject.StartPortObject),
                inputPortIndex = connectionObject.EndMachine.GetInputPortIndex(connectionObject.EndPortObject),

                positions = connectionObject.Positions
            }).ToList(); 
            
            return new FactoryBlueprint()
            {
                machines = machineNodes,
                connections =  connectionEdges
            };
        }

        private void LoadFactoryLayout(FactoryBlueprint blueprint)
        {
            foreach (var machine in blueprint.machines)
            {
                var machinePrefab = machineBank.GetMachine(machine.prefabId);
                if (machinePrefab is InputMachineObject or OutputMachine or ExternalInputObject) {
                    continue;
                }
                
                var machineObj = Instantiate(machinePrefab, transform);
                PlaceCellOccupier(machineObj, machine.positions);
                machineObj.transform.localPosition = machine.localPos;

                if (machineObj is ProcessorObject pO)
                {
                    pO.LoadRecipeById(machine.recipeId);
                }
            }

            foreach (var connection in blueprint.connections)
            {
                _connectionManager.PlaceConnection(connection);
            }
        }
    }
}