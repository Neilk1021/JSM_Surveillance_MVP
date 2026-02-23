using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Surveillance.MoneyGraph
{
    public class MoneyGraphUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MoneyGraphLines lineUI;
        [SerializeField] private RectTransform graphBackground;
        [SerializeField] private RectTransform coordsOrigin;
        [SerializeField] private RectTransform linesRoot;
        [SerializeField] private GameObject pointPrefab;
        [SerializeField] private GameObject axisPointPrefab;

        [Header("Settings")]
        [SerializeField] private Vector2 startXRange; // Range of X in the beginning
        [SerializeField] private Vector2 startYRange; // Range of Y in the beginning
        [SerializeField] private string xAxisLabel; // Label for X axis
        [SerializeField] private string yAxisLabel; // Label for Y axis
        [SerializeField] private float animationDuration = 3f;
        
        [HideInInspector] [SerializeField] private List<RectTransform> pointsRef = new List<RectTransform>();
        [HideInInspector] [SerializeField] private List<RectTransform> axisPoints = new List<RectTransform>();
        [HideInInspector] [SerializeField] private MoneyGraph _moneyGraph;
        private float _xIncrement;
        private float _yIncrement;
        private RectMask2D _mask;

        // Reset the graph, should be called at beginning
        public void ResetGraph()
        {   
            _moneyGraph = new MoneyGraph(startXRange, startYRange);
            ClearPoints();
            UpdateIncrements();

            ResizeMask();
        }
        
        private void ResizeMask()
        {
            if(_mask == null) _mask = linesRoot.GetComponent<RectMask2D>();
            linesRoot.sizeDelta = new Vector2(graphBackground.rect.width * 2, graphBackground.rect.height * 2);

            _mask.padding = new Vector4(0, 0, graphBackground.rect.width, 0);
        }

        // Incase lists are lost, re-initialize them, should only be used in editor
        public void DebugInitializeLists()
        {
            pointsRef = new List<RectTransform>();
            axisPoints = new List<RectTransform>();
        }

        // Draw lines between all points
        public void DrawLines()
        {
            lineUI.ReDraw(pointsRef, linesRoot);
        }

        // Play mask animation
        public void PlayAnimation()
        {
            StartCoroutine(AnimateMask());
        }

        IEnumerator AnimateMask()
        {
            float startTime = Time.time;
            float endTime = startTime + animationDuration;

            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / (endTime - startTime);
                float padding = Mathf.Lerp(graphBackground.rect.width, 0f, t);

                if (_mask == null) _mask = linesRoot.GetComponent<RectMask2D>();
                _mask.padding = new Vector4(0, 0, padding, 0);
                yield return null;
            }
        }

        // Draw a single point on the graph
        // Also draw
        public void AddPoint(Vector2 position)
        {
            if (_moneyGraph.GetPoints().Contains(position))
            {
                return;
            }

            _moneyGraph.AddPoint(position);

            Vector2 scaledPosition = new Vector2(position.x * _xIncrement, position.y * _yIncrement);
            
            GameObject pointInstance = Instantiate(pointPrefab, linesRoot);
            // GameObject pointInstance = Instantiate(pointPrefab, coordsOrigin);

            RectTransform rectTransform = pointInstance.transform as RectTransform;
            rectTransform.anchoredPosition = scaledPosition;
            pointsRef.Add(rectTransform);

            UpdateIncrements();
        }

        // Draw a single axis point on the graph
        private void AddAxisPoint(Vector2 position)
        {
            Vector2 scaledPosition = new Vector2(position.x * _xIncrement, position.y * _yIncrement);
            
            GameObject pointInstance = Instantiate(axisPointPrefab, coordsOrigin);

            RectTransform rectTransform = pointInstance.transform as RectTransform;
            rectTransform.anchoredPosition = scaledPosition;
            axisPoints.Add(rectTransform);
        }

        // Update increments for points and axis points
        // Also rescales points if Y range changes
        private void UpdateIncrements()
        {
            _xIncrement = graphBackground.rect.width / (_moneyGraph.XRange.y - _moneyGraph.XRange.x);
            _yIncrement = graphBackground.rect.height / (_moneyGraph.YRange.y - _moneyGraph.YRange.x);

            UpdateAxisIncrements();
            UpdatePointsPositions();
        }

        private void UpdateAxisIncrements()
        {
            foreach (RectTransform point in axisPoints)
            {
                DestroyImmediate(point.gameObject);
            }
            axisPoints.Clear();

            for (int i = 0; i <= _moneyGraph.XRange.y; i++)
            {
                AddAxisPoint(new Vector2(i, 0));
            }

            for (int i = 0; i <= _moneyGraph.YRange.y; i++)
            {
                AddAxisPoint(new Vector2(0, i));
            }
        }

        private void UpdatePointsPositions()
        {
            List<Vector2> pointsPos = _moneyGraph.GetPoints();
            for (int i = 0; i < pointsPos.Count; i++)
            {
                pointsRef[i].anchoredPosition = new Vector2(pointsPos[i].x * _xIncrement, pointsPos[i].y * _yIncrement);
            }
        }

        // Clear all points and axis points
        public void ClearPoints()
        {
            foreach (RectTransform point in pointsRef)
            {
                #if UNITY_EDITOR
                DestroyImmediate(point.gameObject);
                #else
                Destroy(point.gameObject);
                #endif
            }
            pointsRef.Clear();

            foreach (RectTransform point in axisPoints)
            {
                #if UNITY_EDITOR
                DestroyImmediate(point.gameObject);
                #else
                Destroy(point.gameObject);
                #endif
            }
            axisPoints.Clear();

            _moneyGraph.ClearPoints();

            lineUI.DeleteLines();
        }
    }
}