using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    public enum NodeType
    {
        Input,
        Output
    }
    public class ProcessorNode : MonoBehaviour
    {
        [SerializeField] private ProcessorInstance owner;
        [SerializeField] private NodeType type;
        [SerializeField] private int nodeIndex;

        public ProcessorInstance Owner => owner;
        public NodeType Type => type;
        public int NodeIndex => nodeIndex;
        private void OnMouseDown()
        {
            Debug.Log("node clicked: " + name);

            if (ConnectionManager.Instance != null)
            {
                ConnectionManager.Instance.OnNodeClicked(this);
            }
        }
    }
}