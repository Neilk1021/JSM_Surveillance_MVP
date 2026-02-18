using System.Collections.Generic;
using UnityEngine;

namespace Surveillance.TechTree
{
    [System.Serializable]
    public class Node
    {
        public string Name;
        public int ID;
        public bool IsUnlocked = false;
        public string Description;
        [SerializeReference] public IUnlockable[] Unlockables;
        public int[] ParentIDs;
    }

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
                NodeByID[node.ID] = node;

            IDToDepth.Clear();
            foreach (var node in Nodes)
                GetDepth(node, IDToDepth);

            depthToIndexes.Clear();
            foreach (var node in Nodes)
            {
                int depth = IDToDepth[node.ID];
                if (!depthToIndexes.ContainsKey(depth))
                    depthToIndexes[depth] = new List<int>();
                depthToIndexes[depth].Add(node.ID);
            }
        }


        // Returns the depth of the node in the tree, with root nodes having depth 0.
        int GetDepth(Node node, Dictionary<int, int> cache)
        {
            if (cache.TryGetValue(node.ID, out int d))
                return d;

            if (node.ParentIDs == null || node.ParentIDs.Length == 0)
                return cache[node.ID] = 0;

            int maxParentDepth = 0;
            foreach (var parentID in node.ParentIDs)
            {
                maxParentDepth = Mathf.Max(
                    maxParentDepth,
                    GetDepth(NodeByID[parentID], cache)
                );
            }
            return cache[node.ID] = maxParentDepth + 1;
        }
    }
}