using System;
using System.Collections.Generic;
using cNel.DataStructures;
using UnityEngine;

namespace JSM.Surveillance
{
    public abstract class CellOccupier : MonoBehaviour
    {
        private Vector2Int[] _positions;
        private FactoryGrid _grid;
        
        private PriorityQueue<Vector2Int, float> _pqPositions = new(PriorityQueueType.Min);
        public Vector2Int[] Positions => _positions;

        public FactoryGrid Grid => _grid;

        protected virtual void Start()
        {
            _grid = FactoryGrid.ActiveGrid;
        }

        public Vector2Int GetRootPosition()
        {
            return _pqPositions.IsEmpty() ? new Vector2Int(-1, -1) : _pqPositions.Peek();
        }

        public void Clear()
        {
            Destroy(gameObject);
        }

        protected void Initialize(List<Vector2Int> newPositions, FactoryGrid grid)
        {
            this._positions = newPositions.ToArray();
            this._grid = grid;

            _pqPositions = new PriorityQueue<Vector2Int, float>(PriorityQueueType.Min);
            foreach (var pos in newPositions)
            {
                _pqPositions.Push(pos, pos.magnitude);
            }
            
        }

        public abstract void Entered();

        public abstract void Exited();

    }
}