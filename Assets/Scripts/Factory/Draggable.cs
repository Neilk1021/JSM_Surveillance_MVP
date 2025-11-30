using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private void Start()
        {
            _draggable = true;
            _processor = GetComponent<ProcessorInstance>();
        }

        private void OnMouseDown()
        {
            if(!_draggable) return;
            
            _isDragging = true;
            Debug.Log($"{_processor.name} is being dragged im killing myself");
        }

        private void OnMouseUp()
        {
            if (!_isDragging) return;
            _isDragging = false;

            Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);


            // Try placement on grid when let go snap to grid (i might need to change pivot of processors?)
            _draggable = !FactoryGrid.Instance.PlaceDraggable(this);
        }

        public void Place(List<Vector2Int> newPositions, Vector2 worldPos) {
            this.positions = newPositions.ToArray();
            transform.position = worldPos;
        }

        private void Update()
        {
            if (!_isDragging || !_draggable) return;

            Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;
        }

    }
}