using System;
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
    public class ProcessorPort : MonoBehaviour
    {
        [SerializeField] private ProcessorInstance owner;
        [SerializeField] private NodeType type;
        [SerializeField] private int nodeIndex;

        [Header("Position within machine's grid.")]
        [Tooltip("0 indexed position for the machine's subgrid. For example if a machine is 2x2, then 0 would be the left most cell.")]
        [HideInInspector][SerializeField] private int posX;
        [Tooltip("0 indexed position for the machine's subgrid. For example if a machine is 2x2, then 0 would be the bottom most cell.")]
        [HideInInspector][SerializeField] private int posY;
        
        private int _subcellX;
        private int _subcellY;

        public Vector2Int SubcellPosition => new Vector2Int(_subcellX, _subcellY);
        
        public ProcessorInstance Owner => owner;
        public NodeType Type => type;
        public int NodeIndex => nodeIndex;

        private Connection _connection;
        public Connection Connection => _connection;

        private void OnMouseDown()
        {
            Debug.Log("node clicked: " + name);

            if (ConnectionManager.Instance != null)
            {
                ConnectionManager.Instance.OnNodeClicked(this);
            }
        }
        
        public void SetConnection(Connection newConnection)
        {
            _connection = newConnection;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_subcellX == posX && _subcellY == posY) return;
            
            var borderCell = owner.GetComponent<Draggable>().GetBorderCell(_subcellX, _subcellY, posX,posY);
            posX = borderCell.x;
            posY = borderCell.y;
            _subcellX = borderCell.x;
            _subcellY = borderCell.y;
            var newPos = owner.GetComponent<Draggable>().GetBorderPosition(borderCell.x, borderCell.y);
            

            transform.position = newPos;
        }
        #endif
    }
}