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
            nameText.text = node.name;
        }

        public void Unlock()
        {
            if(!Tree.UnlockNode(Data)) return;
            unlockText.text = "Unlocked!";
        }
    }
}