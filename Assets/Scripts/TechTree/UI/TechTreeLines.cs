using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Surveillance.TechTree
{
    public class TechTreeLines : MonoBehaviour
    {
        public RectTransform container;
        public Image linePrefab;

        public void Draw(Dictionary<int, TechNodeUI> nodes)
        {
            foreach (var kvp in nodes)
            {
                var node = kvp.Value;
                var data = node.Data;

                if (data.ParentIDs == null) continue;

                foreach (var parentID in data.ParentIDs)
                {
                    var parent = nodes[parentID];
                    CreateLine(parent.transform as RectTransform,
                            node.transform as RectTransform);
                }
            }
        }

        void CreateLine(RectTransform a, RectTransform b)
        {
            var line = Instantiate(linePrefab, container);
            Vector2 dir = b.anchoredPosition - a.anchoredPosition;
            float dist = dir.magnitude;

            line.rectTransform.sizeDelta = new Vector2(dist, 4);
            line.rectTransform.anchoredPosition = a.anchoredPosition + dir * 0.5f;
            line.rectTransform.rotation =
                Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }
    }
}