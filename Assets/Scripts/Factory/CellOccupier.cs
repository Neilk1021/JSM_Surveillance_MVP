using System;
using System.Collections.Generic;
using cNel.DataStructures;
using UnityEngine;

namespace JSM.Surveillance
{
    public abstract class CellOccupier : MonoBehaviour
    {
        private Vector2Int[] _positions;
        protected FactoryGrid _grid;
        public int Rotation { get; protected set; } = 0;

        public virtual Vector2Int Size => new Vector2Int(1, 1);
        
        private PriorityQueue<Vector2Int, float> _pqPositions = new(PriorityQueueType.Min);
        public Vector2Int[] Positions => _positions;

        public FactoryGrid Grid => _grid;

        protected virtual void Start()
        {
            _grid = GetComponentInParent<FactoryGrid>();
        }

        public Vector2Int GetRootPosition()
        {
            return _pqPositions.IsEmpty() ? new Vector2Int(-1, -1) : _pqPositions.Peek();
        }
        protected void Initialize(List<Vector2Int> newPositions, FactoryGrid grid)
        {
            this._positions = newPositions.ToArray();
            this._grid = grid;

            
            foreach (var pos in newPositions)
            {
                int x = pos.x;
                int y = pos.y;
                _grid[x, y].SetOccupier(this);
            }

            _pqPositions = new PriorityQueue<Vector2Int, float>(PriorityQueueType.Min);
            foreach (var pos in newPositions)
            {
                _pqPositions.Push(pos, pos.magnitude);
            }
            
        }

        protected void ClearFromBoard()
        {
            foreach (var pos in _positions)
            {
                int x = pos.x;
                int y = pos.y;
                _grid[x, y].Clear();
            }
        }
        
        protected virtual void Remove()
        {
            Destroy(this);
            Destroy(gameObject);
            _grid.Modified();
        }
        
        public virtual void Place(List<Vector2Int> newPositions, Vector3 worldPos, FactoryGrid grid)
        {
            Initialize(newPositions, grid);
            transform.position = worldPos;
        }


        public abstract void Entered();

        public abstract void Exited();

    }
}