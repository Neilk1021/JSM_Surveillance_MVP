using System.Collections.Generic;
using cNel.DataStructures;
using UnityEngine;

namespace JSM.Surveillance
{
    public abstract class CellOccupier : MonoBehaviour
    {
        protected Vector2Int[] positions;

        private PriorityQueue<Vector2Int, float> pqPositions;
        public Vector2Int[] Positions => positions;
        public PriorityQueue<Vector2Int, float> OrderedPositions => pqPositions; 

        public Vector2Int GetRootPosition()
        {
            return pqPositions.IsEmpty() ? new Vector2Int(-1, -1) : pqPositions.Peek();
        }

        public void Clear()
        {
            Destroy(gameObject);
        }

        protected void Initialize(List<Vector2Int> newPositions)
        {
            this.positions = newPositions.ToArray();

            pqPositions = new PriorityQueue<Vector2Int, float>(PriorityQueueType.Min);
            foreach (var pos in newPositions)
            {
                pqPositions.Push(pos, pos.magnitude);
            }
            
        }
    }
}