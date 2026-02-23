// Scale X and Y size based on the max/min points
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

namespace Surveillance.MoneyGraph
{
    [System.Serializable]
    public class MoneyGraph
    {
        private Vector2 _xRange;
        private Vector2 _yRange;
        public Vector2 XRange => _xRange;
        public Vector2 YRange => _yRange;

        // privates
        private List<Vector2> points;

        public MoneyGraph(Vector2 startXRange, Vector2 startYRange)
        {
            _xRange = startXRange;
            _yRange = startYRange;
            points = new List<Vector2>();
        }

        public void AddPoint(Vector2 point)
        {
            points.Add(point);

            if (point.y > _yRange.y)
            {
                _yRange.y = point.y;
            }
        }

        public void ClearPoints()
        {
            points.Clear();
        }

        public List<Vector2> GetPoints()
        {
            return points;
        }
    }
}