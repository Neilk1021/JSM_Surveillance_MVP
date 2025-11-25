using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    public class FactoryGrid : MonoBehaviour
    {
        [SerializeField] private int gridWidth = 20;
        [SerializeField] private int gridHeight = 20;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private float gridLineThickness = 0.02f;

        private Vector2Int hoveredCell = new Vector2Int(-1, -1);
        private Dictionary<Maintainable, List<Vector2Int>> machineOccupancies = new Dictionary<Maintainable, List<Vector2Int>>();
        private Dictionary<Connection, List<Vector2Int>> connectionPaths = new Dictionary<Connection, List<Vector2Int>>();

        public static FactoryGrid Instance { get; private set; }

        private void Update()
        {
            UpdateHoveredCell();
        }

        private void UpdateHoveredCell()
        {
            if (Camera.main == null) return;

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
            mouseWorldPos.z = 0;

            Vector2Int newHoveredCell = GetGridPosition(mouseWorldPos);

            if (IsValidGridPosition(newHoveredCell.x, newHoveredCell.y))
            {
                if (newHoveredCell != hoveredCell)
                {
                    hoveredCell = newHoveredCell;
                }
            }
            else
            {
                hoveredCell = new Vector2Int(-1, -1);
            }
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
            if (IsValidGridPosition(hoveredCell.x, hoveredCell.y))
            {
                Vector3 cellCenter = GetWorldPosition(hoveredCell) + new Vector3(cellSize / 2f, cellSize / 2f, 0);
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

        //get what current cell is hovered
        private Vector2Int GetHoveredCell() 
        {
            return hoveredCell;
        }

    }
}