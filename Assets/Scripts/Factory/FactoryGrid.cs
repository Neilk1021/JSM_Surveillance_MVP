using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JSM.Surveillance.Game;
using JSM.Surveillance.UI;
using UnityEngine;

namespace JSM.Surveillance
{
    public class FactoryGrid : MonoBehaviour
    {
        [Header("Grid Input")]
        [SerializeField] private InputMachine gridInputPrefab;
        [SerializeField] private Vector2Int inputPosition;
        
        [Header("Grid Output")] 
        [SerializeField] private OutputMachine gridOutputPrefab;
        [SerializeField] private Vector2Int outputPosition;
        
        [Header("Size")]
        [SerializeField] private int gridWidth = 20;
        [SerializeField] private int gridHeight = 20;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private float gridLineThickness = 0.02f;

        private Vector2Int _hoveredCell = new Vector2Int(-1, -1);

        //Allows multiple grids where we just switch between different grids as needed.

        private readonly Dictionary<Vector2Int, ProcessorPort> _ports = new Dictionary<Vector2Int, ProcessorPort>(); 
        
        private FactoryCell[,] _grid; 
        
        private Camera _camera;

        private UIManager _uiManager;
        private InputMachine _gridInput;
        private OutputMachine _gridOutput;

        public float CellSize => cellSize;
        public int Width => gridWidth; 
        public int Height => gridHeight;
        public UIManager UIManager => _uiManager;
        private ConnectionManager _connectionManager;
        public  Source Source { get; private set; }
        
        public ConnectionManager ConnectionManager => _connectionManager;
        

        private void Awake()
        {
            _uiManager = GetComponent<UIManager>();
            
            _connectionManager = GetComponentInChildren<ConnectionManager>();
            _camera = GameObject.FindGameObjectWithTag("FactoryCam").GetComponent<Camera>();
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
            
            InitializeDraggable(_gridInput , inputPosition);
            InitializeDraggable(_gridOutput, outputPosition);
        }

        private void InitializeDraggable(Draggable draggable, Vector2Int leftCornerGridPos)
        {
            List<Vector2Int> draggablePositions = new();
            for (int i = 0; i < draggable.Size.x; i++)
            {
                for (int j = 0; j < draggable.Size.y; j++)
                {
                    draggablePositions.Add(new Vector2Int(i,j) + leftCornerGridPos);
                }
            }

            //Vector3 worldPosition = GetWorldPosition(leftCornerGridPos) + (Vector3)((Vector2)draggable.Size) * cellSize/2.0f;
            PlaceDraggable(draggable, draggablePositions);
        }
        
        private void Update()
        {
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

        public Vector2 GetMouseWorldPos2D()
        {
            Vector3 mousePos = Input.mousePosition;
            return _camera.ScreenToWorldPoint(mousePos);
        }
        
        public Vector3 GetMouseWorldPos3D()
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = _camera.ScreenPointToRay(mousePos);

            RaycastHit[] results = new RaycastHit[32];
            var size = Physics.RaycastNonAlloc(ray, results);
            Vector3 mouseWorldPos = Vector3.negativeInfinity;
            for (int i = 0; i < size; i++)
            {
                if (results[i].transform == transform)
                {
                    mouseWorldPos = results[i].point;
                }
            }

            return mouseWorldPos;
        }

        private bool VerifyPlacementValid(List<Vector2Int> positions)
        {
            if (positions == null) return false;
            
            return (!positions.Any(x => !IsValidGridPosition(x.x, x.y) || _grid[x.x, x.y].IsOccupied));
        }

        public bool PlaceConnection(Connection connection)
        {
            connection.transform.parent = transform;
            var positions = connection.Positions.ToList();
            if (positions.Contains(new Vector2Int(-1, -1))) {
                return false;
            }

            if (!VerifyPlacementValid(positions))
            {
                return false;
            }
            
            foreach (var pos in positions)
            {
                int x = pos.x;
                int y = pos.y;
                _grid[x, y].SetOccupier(connection);
            }

            return true;
        }


        public ProcessorPort GetPortAtCell(Vector2Int pos)
        {
            return _ports.GetValueOrDefault(pos, null);
        }

        public bool PlaceDraggableAtCurrentPosition(Draggable draggable)
        {
            return PlaceDraggable(draggable, GetDraggablePositions(draggable).Select(x=>GetGridPosition(x)).ToList());
        }

        public bool IsDraggableValid(Draggable draggable)
        {
            return VerifyPlacementValid(GetDraggablePositions(draggable).Select(x=>GetGridPosition(x)).ToList());
        }
        
        public bool PlaceDraggable(Draggable draggable, List<Vector2Int> gridPositions)
        {
            draggable.transform.parent = transform;

            if (gridPositions.Contains(new Vector2Int(-1, -1))) {
                return false;
            }

            if (!VerifyPlacementValid(gridPositions)) {
                return false;
            }
            
            foreach (var pos in gridPositions)
            {
                int x = pos.x;
                int y = pos.y;
                _grid[x, y].SetOccupier(draggable);
            }

            Vector3 worldPos = new Vector3(
                gridPositions.Average(x => GetWorldPosition(x).x) + cellSize / 2,
                gridPositions.Average(x => GetWorldPosition(x).y) + cellSize / 2,
                transform.position.z
            ); 
            
            draggable.Place(gridPositions, worldPos, this);
            return true;
        }

        private List<Vector2> GetDraggablePositions(Draggable draggable)
        {
            List<Vector2> positions = new List<Vector2>();

            int width = draggable.Size.x;
            int height = draggable.Size.y;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j <  height; j++)
                {
                    Vector2 newPos = new Vector2(
                        draggable.transform.position.x + (cellSize * (Mathf.FloorToInt(-width / 2) + i)),
                        draggable.transform.position.y + (cellSize * (Mathf.FloorToInt(-width / 2) + j))
                    );
                    
                    positions.Add(newPos);
                }
            }

            return positions;
        }

        public Vector3 GetWorldPosition(Vector2Int gridPosition)
        {
            // grid start at (0,0)
            return transform.rotation * new Vector3(
                gridPosition.x * cellSize,
                gridPosition.y * cellSize,
                0
            ) + transform.position;
            
        }

        public Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            var localizedPosition = transform.InverseTransformPoint(worldPosition);
            
            return new Vector2Int(
                Mathf.FloorToInt((localizedPosition.x ) / cellSize),
                Mathf.FloorToInt((localizedPosition.y ) / cellSize)
            );
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.gray;

            // Draw vertical lines
            for (int x = 0; x <= gridWidth; x++)
            {
                Vector3 center = new Vector3(x * cellSize, (gridHeight * cellSize) / 2f, 0) + transform.position;
                Vector3 size = new Vector3(gridLineThickness, gridHeight * cellSize, 0.1f);
                Gizmos.DrawCube(center, size);
            }

            // Draw horizontal lines
            for (int y = 0; y <= gridHeight; y++)
            {
                Vector3 center = new Vector3((gridWidth * cellSize) / 2f, y * cellSize, 0) + transform.position;
                Vector3 size = new Vector3(gridWidth * cellSize, gridLineThickness, 0.1f);
                Gizmos.DrawCube(center, size);
            }

            // Draw hover highlight
            if (IsValidGridPosition(_hoveredCell.x, _hoveredCell.y))
            {
                Vector3 cellCenter = GetWorldPosition(_hoveredCell) + new Vector3(cellSize / 2f, cellSize / 2f, 0);
                Gizmos.color = new Color(0.5f, 0.8f, 1f, 0.3f);
                Gizmos.DrawCube(cellCenter, new Vector3(cellSize, cellSize, 0.1f));
            }
        }

        //machine placement
        //tryPlaceMachine: check if space is occupied
        //placeMachine

        private bool IsValidGridPosition(Vector2Int pos)
        {
            return IsValidGridPosition(pos.x, pos.y);
        }

        private bool IsValidGridPosition(int x, int y)
        {
            return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
        }

        public bool IsCellEmpty(Vector2 pos) {
            return IsCellEmpty((int)pos.x, (int)pos.y);
        }
        
        public bool IsCellEmpty(Vector2Int pos) { 
            return IsCellEmpty(pos.x, pos.y);
        }
        
        public bool IsCellEmpty(int x, int y)
        {
            if (!IsValidGridPosition(x, y)) return false;
            
            return !_grid[x, y].IsOccupied;
        }
        
        //get what current cell is hovered
        private Vector2Int GetHoveredCell() 
        {
            return _hoveredCell;
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

        public bool MouseOverGrid()
        {
            return IsValidGridPosition(GetGridPosition(GetMouseWorldPos3D()));
        }

    }
}