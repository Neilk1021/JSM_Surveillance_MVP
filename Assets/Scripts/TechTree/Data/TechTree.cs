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
        public IUnlockable[] Unlockables;
        public int[] ParentIDs;
    }

    [System.Serializable]
    public class TechTree
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

        public void FillWithExampleData()
        {
            Nodes = new Node[]
            {
                // ===== Layer 0 =====
                new Node
                {
                    ID = 0,
                    Name = "Animals",
                },

                // ===== Layer 1 =====
                new Node
                {
                    ID = 1,
                    Name = "Flying",
                    ParentIDs = new[] { 0 }
                },
                new Node
                {
                    ID = 2,
                    Name = "Walking",
                    ParentIDs = new[] { 0 }
                },

                // ===== Layer 2 =====
                new Node
                {
                    ID = 3,
                    Name = "Duck",
                    ParentIDs = new[] { 1, 2 }
                },
                new Node
                {
                    ID = 4,
                    Name = "Eagle",
                    ParentIDs = new[] { 1 }
                },
                new Node
                {
                    ID = 5,
                    Name = "Dog",
                    ParentIDs = new[] { 2 }
                },
                new Node
                {
                    ID = 6,
                    Name = "Cat",
                    ParentIDs = new[] { 2 }
                },
            };

            BuildLookup();
        }


        public void FillWithExampleData2()
        {
            Nodes = new Node[]
            {
                // ===== Layer 0 =====
                new Node
                {
                    ID = 0,
                    Name = "Animals",
                },

                // ===== Layer 1 =====
                new Node
                {
                    ID = 1,
                    Name = "Flying",
                    ParentIDs = new[] { 0 }
                },
                new Node
                {
                    ID = 2,
                    Name = "Walking",
                    ParentIDs = new[] { 0 }
                },
                new Node
                {
                    ID = 3,
                    Name = "Swimming",
                    ParentIDs = new[] { 0 }
                },

                // ===== Layer 2 =====
                new Node
                {
                    ID = 4,
                    Name = "Ducks",
                    ParentIDs = new[] { 1, 2, 3 }
                },
                new Node
                {
                    ID = 5,
                    Name = "Dogs",
                    ParentIDs = new[] { 2 }
                },
                new Node
                {
                    ID = 6,
                    Name = "Fish",
                    ParentIDs = new[] { 3 }
                },
                new Node
                {
                    ID = 7,
                    Name = "Platypus",
                    ParentIDs = new[] { 2, 3}
                },
            };

            BuildLookup();
        }

    }
}