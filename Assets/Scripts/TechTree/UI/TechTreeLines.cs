using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Surveillance.TechTree
{
    public class TechTreeLines : MonoBehaviour
    {
        public RectTransform container;
        public Image linePrefab;
        [HideInInspector] [SerializeField] private Transform _linesRoot; 
        
        
        public Transform Draw(Dictionary<int, TechNodeUI> nodes)
        {
            VerifyLinesRoot();
            foreach (var kvp in nodes)
            {
                var node = kvp.Value;
                var data = node.Data;

                if (data.parentIDs == null) continue;

                foreach (var parentID in data.parentIDs)
                {
                    var parent = nodes[parentID];
                    CreateLine(parent.transform as RectTransform,
                            node.transform as RectTransform);
                }
            }

            return _linesRoot;
        }

        private void VerifyLinesRoot()
        {
            if(_linesRoot != null) DestroyImmediate(_linesRoot.gameObject);
            _linesRoot = new GameObject("lines").transform;
            _linesRoot.parent = container;
            _linesRoot.localScale = Vector3.one;
            _linesRoot.localPosition = Vector3.zero;
        }

        void CreateLine(RectTransform a, RectTransform b)
        {
            var line = Instantiate(linePrefab, _linesRoot);
            Vector2 dir = b.anchoredPosition - a.anchoredPosition;
            float dist = dir.magnitude;

            line.rectTransform.sizeDelta = new Vector2(dist, 4);
            line.rectTransform.anchoredPosition = a.anchoredPosition + dir * 0.5f;
            line.rectTransform.rotation =
                Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }

        public void Clear()
        {
            if (_linesRoot != null)
            {
                DestroyImmediate(_linesRoot.gameObject);
            }
        }
    }
}