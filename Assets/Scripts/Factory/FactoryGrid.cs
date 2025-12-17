using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JSM.Surveillance
{
    public class FactoryGrid : MonoBehaviour
    {
        [SerializeField] private int gridWidth = 20;
        [SerializeField] private int gridHeight = 20;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private float gridLineThickness = 0.02f;

        private Vector2Int _hoveredCell = new Vector2Int(-1, -1);

        private FactoryCell[,] _grid; 
        
        private Camera _camera;

        public float CellSize => cellSize;
        public static FactoryGrid Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }
            
            Destroy(gameObject);
        }

        private void Start()
        {
            InitializeGrid();
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

        private void Update()
        {
            UpdateHoveredCell();
        }

        private void UpdateHoveredCell()
        {
            if (_camera == null) return;

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(_camera.transform.position.z);
            Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(mousePos);
            mouseWorldPos.z = 0;

            Vector2Int newHoveredCell = GetGridPosition(mouseWorldPos);
            _hoveredCell = IsValidGridPosition(newHoveredCell.x, newHoveredCell.y) ? newHoveredCell : new Vector2Int(-1, -1);
        }

        private bool VerifyPlacementValid(List<Vector2Int> positions)
        {
            return (!positions.Any(x => _grid[x.x, x.y].IsOccupied));
        }

        public bool PlaceConnection(Connection connection)
        {
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
        
        public bool PlaceDraggable(Draggable draggable)
        {
            var positions = GetDraggablePositions(draggable);
            List<Vector2Int> gridPositions = positions.Select(x => GetGridPosition(x)).ToList();

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
            
            Vector2 worldPos = new Vector2(
                gridPositions.Average(x => GetWorldPosition(x).x ) + cellSize/2, 
                gridPositions.Average(x =>GetWorldPosition(x).y )+cellSize/2
            ); 
            
            draggable.Place(gridPositions, worldPos);
            return true;

            //_hoveredCell = IsValidGridPosition(newHoveredCell.x, newHoveredCell.y) ? newHoveredCell : new Vector2Int(-1, -1);
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
            return new Vector3(
                gridPosition.x * cellSize,
                gridPosition.y * cellSize,
                0
            );
        }

        public Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPosition.x / cellSize),
                Mathf.FloorToInt(worldPosition.y / cellSize)
            );
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.gray;

            // Draw vertical lines
            for (int x = 0; x <= gridWidth; x++)
            {
                Vector3 center = new Vector3(x * cellSize, (gridHeight * cellSize) / 2f, 0);
                Vector3 size = new Vector3(gridLineThickness, gridHeight * cellSize, 0.1f);
                Gizmos.DrawCube(center, size);
            }

            // Draw horizontal lines
            for (int y = 0; y <= gridHeight; y++)
            {
                Vector3 center = new Vector3((gridWidth * cellSize) / 2f, y * cellSize, 0);
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

    }
}