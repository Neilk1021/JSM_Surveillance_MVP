using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cNel.DataStructures;
using UnityEngine;

namespace JSM.Surveillance
{
    public class Draggable : CellOccupier 
    {
        [Range(1,10)]
        [SerializeField] private int width, height;

        public Vector2Int Size => new Vector2Int(width, height);
        
        private bool _isDragging = false;
        private ProcessorInstance _processor;

        private Camera _camera;
        private bool _draggable = true;
        
        private void Awake()
        {
            _camera = Camera.main;
        }

        protected override void Start()
        {
            _processor = GetComponent<ProcessorInstance>();
            base.Start();
        }

        private void OnMouseDown()
        {
            if(!_draggable) return;
            
            _isDragging = true;
            Debug.Log($"{_processor.name} is being dragged im killing myself");
        }

        private void OnMouseUp()
        {
            if (!_isDragging || !_draggable) return;
            _isDragging = false;

            Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);


            // Try placement on grid when let go snap to grid (i might need to change pivot of processors?)
            _draggable = !this.Grid.PlaceDraggable(this);
        }

        public virtual void Place(List<Vector2Int> newPositions, Vector2 worldPos, FactoryGrid grid)
        {
            base.Initialize(newPositions, grid);
            _draggable = false;
            transform.position = worldPos;
        }

        private void Update()
        {
            if (!_isDragging || !_draggable) return;

            Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            
            float objWidth = transform.lossyScale.x;
            float objHeight = transform.lossyScale.y; 
                    
            for (int i = 0; i < width; i++)
            {
                Vector2 hStart = new Vector2(
                    transform.position.x  - objWidth/2 + (objWidth/width)*i,
                    transform.position.y  - objHeight/2
                );
                Vector2 hEnd = new Vector2(
                    transform.position.x  - objWidth/2 + (objWidth/width)*i,
                    transform.position.y  + objHeight/2
                );
                
                Gizmos.DrawLine(hStart, hEnd);
            }
            
            
            for (int j = 0; j <  height; j++)
            {
        
                Vector2 vStart = new Vector2(
                    transform.position.x - objWidth/2,
                    transform.position.y - objHeight/2 + (objHeight/height) * j
                );
                Vector2 vEnd = new Vector2(
                    transform.position.x + objWidth/2,
                    transform.position.y - objHeight/2 + (objHeight/height) * j
                );
                    
                    
                Gizmos.DrawLine(vStart, vEnd);
            }
        }

        private bool InMachineRange(int x, int y)
        {
            return x < width && x >= 0 && y >= 0 && y < height;
        }

        private bool OnBorder(int x, int y)
        {
            return ((x==-1 || x==width) && (y==-1 | y==height));
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
        
        
        //TODO implement (this is you haiyi.)
        public override void Entered()
        {
           // haiyi this is functionally the same as OnMouseEntered, but its based on the cell in the grid.
        }

        public override void Exited()
        {
            // functionally the same as OnMouseExited, but its based on the cell in the grid. 
        }
    }
}