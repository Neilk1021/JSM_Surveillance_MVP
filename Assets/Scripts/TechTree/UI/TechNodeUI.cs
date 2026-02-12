using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace Surveillance.TechTree
{
    public class TechNodeUI : MonoBehaviour
    {
        public TMP_Text nameText;
        public TMP_Text unlockText;
        public Node Data;
        public TechTree Tree;

        public void Initialize(Node node, TechTree tree)
        {
            Data = node;
            Tree = tree;
            nameText.text = node.Name;
        }

        public void Unlock()
        {
            foreach (var parentID in Data.ParentIDs)
            {
                if (!Tree.NodeByID[parentID].IsUnlocked)
                {
                    Debug.LogError($"Cannot unlock {Data.Name} because parent {Tree.NodeByID[parentID].Name} is locked");
                    return;
                }
            }
            Data.IsUnlocked = true;
            Debug.Log($"Unlocked {Data.Name}.");
            unlockText.text = "Unlocked!";
        }
    }
}