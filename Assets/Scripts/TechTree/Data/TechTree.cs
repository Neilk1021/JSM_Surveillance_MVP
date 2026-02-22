using System.Collections.Generic;
using JSM.Surveillance;
using UnityEngine;
using UnityEngine.Serialization;

namespace Surveillance.TechTree
{
    [System.Serializable]
    public partial class TechTree
    {
        public Node[] Nodes;
        public Dictionary<int, Node> NodeByID = new();
        public Dictionary<int, int> IDToDepth = new();
        public Dictionary<int, List<int>> depthToIndexes = new();

        public void BuildLookup()
        {
            NodeByID.Clear();
            foreach (var node in Nodes)
                NodeByID[node.id] = node;

            IDToDepth.Clear();
            foreach (var node in Nodes)
                GetDepth(node, IDToDepth);

            depthToIndexes.Clear();
            foreach (var node in Nodes)
            {
                int depth = IDToDepth[node.id];
                if (!depthToIndexes.ContainsKey(depth))
                    depthToIndexes[depth] = new List<int>();
                depthToIndexes[depth].Add(node.id);
            }
        }


        // Returns the depth of the node in the tree, with root nodes having depth 0.
        int GetDepth(Node node, Dictionary<int, int> cache)
        {
            if (cache.TryGetValue(node.id, out int d))
                return d;

            if (node.parentIDs == null || node.parentIDs.Length == 0)
                return cache[node.id] = 0;

            int maxParentDepth = 0;
            foreach (var parentID in node.parentIDs)
            {
                maxParentDepth = Mathf.Max(
                    maxParentDepth,
                    GetDepth(NodeByID[parentID], cache)
                );
            }
            return cache[node.id] = maxParentDepth + 1;
        }

        
        
        public bool UnlockNode(Node node)
        {
            foreach (var parentID in node.parentIDs)
            {
                if (!NodeByID[parentID].isUnlocked)
                {
                    return false;
                }
            }
            
            return node.Purchase();
        }
    }
}