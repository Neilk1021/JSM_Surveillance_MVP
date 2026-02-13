using UnityEngine;
using System.Collections.Generic;

namespace Surveillance.TechTree
{
    public class TechTreeUI : MonoBehaviour
    {
        public TechTree techTree;
        public TechNodeUI nodePrefab;
        public RectTransform container;
        public float xSpacing = 300f;
        public float ySpacing = 200f;

        Dictionary<int, TechNodeUI> spawnedNodes = new();

        void Awake()
        {
            if (techTree.Nodes == null || techTree.Nodes.Length == 0)
                techTree.FillWithExampleData();

            techTree.BuildLookup();
            Build();
        }

        public void Build()
        {
            ClearVisualChildren();
            spawnedNodes.Clear();
            techTree.BuildLookup();

            foreach (var node in techTree.Nodes)
            {
                var ui = Instantiate(nodePrefab, container);
                ui.Initialize(node, techTree);

                spawnedNodes[node.ID] = ui;
            }

            LayoutNodes();
            var linesRoot = GetComponent<TechTreeLines>().Draw(spawnedNodes);
            linesRoot.SetSiblingIndex(0);
        }

        void LayoutNodes()
        {
            // Apply layouts for each depth layer
            foreach (var indexes in techTree.depthToIndexes.Values)
            {
                int count = indexes.Count;
                for (int i = 0; i < count; i++)
                {
                    int index = indexes[i];
                    var ui = spawnedNodes[index];
                    
                    float xPos = (i - (count % 2 == 0 ? count / 2 - 0.5f : count / 2)) * xSpacing;

                    float yPos = techTree.IDToDepth[index] * -ySpacing;
                    ui.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
                }
            }
        }

        public void ClearVisualChildren()
        {
            GetComponent<TechTreeLines>().Clear();
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = transform.GetChild(i).gameObject;
                DestroyImmediate(child);
            }
        }
    }
}
