using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    public enum NodeType
    {
        End,
        Start
    }

    public class ProcessorPortObject : MonoBehaviour
    {
        [SerializeField] private MachineObject owner;
        [SerializeField] private NodeType type;
        [SerializeField] private int nodeIndex;

        [Header("Position within machine's grid.")]
        [Tooltip("0 indexed position for the machine's subgrid. For example if a machine is 2x2, then 0 would be the left most cell.")]
        [HideInInspector][SerializeField] private int posX;
        [Tooltip("0 indexed position for the machine's subgrid. For example if a machine is 2x2, then 0 would be the bottom most cell.")]
        [HideInInspector][SerializeField] private int posY;
        
        private int _subcellX;
        private int _subcellY;

        public Vector2Int SubcellPosition
        {
            get
            {
                int x = _subcellX;
                int y = _subcellY;
                int w = owner.Size.x - 1;
                int h = owner.Size.y - 1;

                int angle = Mathf.RoundToInt(owner.Rotation) % 360;
                if (angle < 0) angle += 360;

                return angle switch
                {
                    90  => new Vector2Int(-y + h, x),      
                    180 => new Vector2Int(-x + w, -y + h), 
                    270 => new Vector2Int(y, -x + w),      
                    _   => new Vector2Int(x, y)           
                };
            }
        }

        public MachineObject Owner => owner;
        public NodeType Type => type;
        public int NodeIndex => nodeIndex;

        private ConnectionObject _connectionObject;
        public ConnectionObject ConnectionObject => _connectionObject;

        private void Awake()
        {
            _subcellX = posX;
            _subcellY = posY;
        }

        private void OnMouseDown()
        {
            if(!FactoryGrid.Editable)return;
            
            if (owner.Grid.ConnectionManager != null)
            {
                owner.Grid.ConnectionManager.OnNodeClicked(this);
            }
        }
        
        public void SetConnection(ConnectionObject newConnectionObject)
        {
            _connectionObject = newConnectionObject;
        }

        //public ProcessorPort BuildInstance()
        //{
            //return new ProcessorPort(connec)
        //}

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