using UnityEngine;

namespace JSM.Surveillance
{
    public partial class FactoryGrid
    {
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

    }
}