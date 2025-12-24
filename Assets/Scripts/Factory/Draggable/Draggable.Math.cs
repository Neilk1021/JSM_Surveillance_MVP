using UnityEngine;

namespace JSM.Surveillance
{
    public partial class Draggable : CellOccupier 
    {
        private bool InMachineRange(int x, int y)
        {
            return x < width && x >= 0 && y >= 0 && y < height;
        }
        
        /// <summary>
        /// Gets the position of a subcell on this draggable at position (posX, posY)
        /// </summary>
        /// <param name="posX">The X position of the cell.</param>
        /// <param name="posY">The Y position of the cell.</param>
        /// <returns>World Coordinates of where the cell is.</returns>

        public Vector2 GetSubcellPos(int posX, int posY)
        {
            float objWidth = transform.lossyScale.x;
            float objHeight = transform.lossyScale.y;
            float cellWidth = objWidth / width;
            float cellHeight = objHeight / height;

            return !InMachineRange(posX, posY)
                ? new Vector2(-1, -1)
                : new Vector2(
                    transform.position.x - objWidth / 2 + cellWidth * posX +cellWidth / 2,
                    transform.position.y - objHeight/2 + cellHeight*posY + cellHeight/2
                    );
        }

        /// <summary>
        /// Gets the position of a cell on the border of this draggable at position (cellX, cellY) 
        /// </summary>
        /// <param name="cellX">The X position of the border cell.</param>
        /// <param name="cellY">The Y position of the border cell</param>
        /// <returns>World Coordinates of where the cell is.</returns>
        public Vector2 GetBorderPosition(int cellX, int cellY)
        {
            float objWidth = transform.lossyScale.x;
            float objHeight = transform.lossyScale.y;
            float cellWidth = objWidth / width;
            float cellHeight = objHeight / height;

            float offsetX = 0;
            if (!InMachineRange(cellX, 0)) {
                offsetX = cellX < 0 ? cellWidth / 2 : -cellWidth / 2;
            }

            float offsetY = 0; 
            if (!InMachineRange(0, cellY))
            {
                offsetY = cellY < 0 ? cellHeight / 2 : -cellHeight / 2;
            }

            return new Vector2(
                transform.position.x - objWidth / 2 + cellWidth*cellX + cellWidth / 2 + offsetX,
                transform.position.y - objHeight/2 + cellHeight*cellY + cellHeight/ 2 + offsetY
            );
        }

        private bool IsBorderCell(int posX, int posY)
        {
            return ((posX == -1 || posX == width) ^ (posY == -1 || posY == height));
        }
        
        /// <summary>
        /// Projects a cell to the border of this draggable.
        /// </summary>
        /// <param name="posX">X pos of Cell, Grid is local to the draggable.</param>
        /// <param name="posY">Y pos of Cell, Grid is local to the draggable.</param>
        /// <returns>Associated Border Cell.</returns>
        public Vector2Int GetBorderCell(int prevX, int prevY, int posX, int posY)
        {
            if (IsBorderCell(posX, posY)) return new Vector2Int(posX, posY);
            
            int magX = Mathf.Abs(posX - width / 2);
            int magY = Mathf.Abs(posY - height / 2);
            
            if (posX < -1) posX = -1;
            if (posY < -1) posY = -1; 
            if (posX >= width) posX = width;
            if (posY >= height) posY = height;

            if (prevX != posX)
            {
                var x = Mathf.RoundToInt(((float)posX) / width) * width;
                x = x == 0 ? -1 : x;
                
                if (posY >= height) posY = height-1;
                if (posY < 0) posY = 0; 
                
                return new Vector2Int(x, posY);
            }

            if (prevY != posY)
            {
                var y = Mathf.RoundToInt((float)posY / height) * height;
                y = y == 0 ? -1 : y;
                if (posX >= width) posX = width-1;
                if (posX < 0) posX = 0; 
            
                return new Vector2Int(posX, y);
            }

            return new Vector2Int(posX, posY);
        }
    }
}