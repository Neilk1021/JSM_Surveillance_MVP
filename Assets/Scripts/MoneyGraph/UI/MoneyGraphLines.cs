using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Surveillance.MoneyGraph
{
    public class MoneyGraphLines : MonoBehaviour
    {
        public Image linePrefab;
        [HideInInspector] [SerializeField] private List<RectTransform> lines = new();
        
        public Transform ReDraw(List<RectTransform> points, RectTransform linesRoot)
        {
            DeleteLines();
            for (int i = 0; i < points.Count - 1; i++)
            {
                CreateLine(points[i], points[i + 1], linesRoot);
            }
            return linesRoot;
        }

        public void CreateLine(RectTransform a, RectTransform b, RectTransform parent)
        {
            var line = Instantiate(linePrefab, parent);
            Vector2 dir = b.anchoredPosition - a.anchoredPosition;
            float dist = dir.magnitude;

            line.rectTransform.sizeDelta = new Vector2(dist, 4);
            line.rectTransform.anchoredPosition = a.anchoredPosition + dir * 0.5f;
            line.rectTransform.rotation =
                Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

            lines.Add(line.rectTransform);
        }

        public void DeleteLines()
        {
            foreach (RectTransform line in lines)
            {
                #if UNITY_EDITOR
                DestroyImmediate(line.gameObject);
                #else
                Destroy(line.gameObject);
                #endif
            }
            lines.Clear();
        }
    }
}