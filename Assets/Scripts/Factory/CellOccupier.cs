using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    public abstract class CellOccupier : MonoBehaviour
    {
        protected Vector2Int[] positions;

        public CellOccupier(List<Vector2Int> positions) {
            this.positions = positions.ToArray();
        }

        protected CellOccupier() {
            this.positions = null;
        }

        public void Clear()
        {
            Destroy(gameObject);
        }
    }
}