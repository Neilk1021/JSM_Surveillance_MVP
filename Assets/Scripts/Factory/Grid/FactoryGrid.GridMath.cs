using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JSM.Surveillance
{
    public partial class FactoryGrid
    {
        [Header("Size")]
        [SerializeField] private int gridWidth = 20;
        [SerializeField] private int gridHeight = 20;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private float gridLineThickness = 0.02f; 
        

        public float CellSize => cellSize;
        public int Width => gridWidth; 
        public int Height => gridHeight;
        
        public Vector3 GetWorldPosition(Vector2Int gridPosition)
        {
            // grid start at (0,0)
            return transform.rotation * new Vector3(
                gridPosition.x * cellSize,
                gridPosition.y * cellSize,
                0
            ) + transform.position;
            
        }
        
        public ProcessorPort GetPortAtCell(Vector2Int pos)
        {
            return _ports.GetValueOrDefault(pos, null);
        }

        private bool VerifyPlacementValid(List<Vector2Int> positions)
        {
            if (positions == null) return false;
            return (!positions.Any(x => !IsValidGridPosition(x.x, x.y) || _grid[x.x, x.y].IsOccupied));
        }
        
        public Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            var localizedPosition = transform.InverseTransformPoint(worldPosition);
            
            return new Vector2Int(
                Mathf.FloorToInt((localizedPosition.x ) / cellSize),
                Mathf.FloorToInt((localizedPosition.y ) / cellSize)
            );
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
        
        private List<Vector2> GetOccupierPositions(CellOccupier occupier)
        {
            List<Vector2> positions = new List<Vector2>();

            int width = occupier.Size.x;
            int height = occupier.Size.y;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j <  height; j++)
                {
                    Vector2 newPos = new Vector2(
                        occupier.transform.position.x + (cellSize * (Mathf.FloorToInt(-width / 2) + i)),
                        occupier.transform.position.y + (cellSize * (Mathf.FloorToInt(-width / 2) + j))
                    );
                    
                    positions.Add(newPos);
                }
            }

            return positions;
        }
        
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
        
        public bool MouseOverGrid()
        {
            return IsValidGridPosition(GetGridPosition(GetMouseWorldPos3D()));
        }

        //get what current cell is hovered
        private Vector2Int GetHoveredCell() 
        {
            return _hoveredCell;
        }
    }
}