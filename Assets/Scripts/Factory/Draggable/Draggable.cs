using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    public partial class Draggable : CellOccupier 
    {
        [Range(1,10)]
        [SerializeField] private int width, height;

        public Vector2Int Size => new Vector2Int(width, height);
        
        private Camera _camera;
        
        private bool _isDragging = false;
        protected bool Placed = false;
        
        private void Awake()
        {
            _camera = Camera.main;
        }

        protected virtual void OnMouseDown()
        {
            if(Placed) return;
            _isDragging = true;
        }

        private void OnMouseUp()
        {
            if (!_isDragging || Placed) return;
            _isDragging = false;
            Placed = Grid.PlaceDraggable(this);
        }

        public virtual void Place(List<Vector2Int> newPositions, Vector3 worldPos, FactoryGrid grid)
        {
            Initialize(newPositions, grid);
            Placed = true;
            transform.position = worldPos;
        }

        private void Update()
        {
            if (!_isDragging || Placed) return;

            Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;
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