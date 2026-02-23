using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;

namespace JSM.Surveillance
{
    public abstract class MachineObject : Draggable, ISerializationCallbackReceiver
    {
        [Header("GUID")]
        [FormerlySerializedAs("_guid")] [SerializeField] private string guid;
        [Header("InventorySize")]
        [SerializeField] protected int inventorySize = 20;

        private readonly List<ProcessorPortObject> _iPorts = new();
        private readonly List<ProcessorPortObject> _oPorts = new();
        
        public int InventorySize => inventorySize;
        public IReadOnlyList<ProcessorPortObject> InputPorts => _iPorts;
        public IReadOnlyList<ProcessorPortObject> OutputPorts => _oPorts;

        public string Guid => guid;

        public event Action OnModify; 
        
        public override void Place(List<Vector2Int> newPositions, Vector3 worldPos, FactoryGrid grid)
        {
            base.Place(newPositions, worldPos, grid);
            
            Vector2Int root = GetRootPosition();


            grid.OnModify.AddListener(OnModifyFunc);
            _iPorts.Clear();
            _oPorts.Clear();
            foreach (var port in GetComponentsInChildren<ProcessorPortObject>())
            {
                grid.RegisterPort(port.SubcellPosition + root, port);

                if (port.Type == NodeType.End) {
                    _iPorts.Add(port);
                }
                else {
                    _oPorts.Add(port);
                }
            }
        }

        private void OnModifyFunc()
        {
            OnModify?.Invoke();
        }

        private void OnEnable()
        {
            if (Grid == null) {
                return;
            }

        }

        private void OnDisable()
        {
            if (Grid == null) {
                return;
            }
            _grid.OnModify.RemoveListener(OnModifyFunc);
        }

        public virtual void Sell()
        {
            foreach (var port in GetComponentsInChildren<ProcessorPortObject>())
            {
                var connection = port.ConnectionObject;
                Grid.UnregisterPort(port.SubcellPosition + GetRootPosition(),port);
                
                if(connection == null) continue;
                Destroy(connection.gameObject);
            }
            
            Remove();
        }
        
        public virtual void Move()
        {
            foreach (var port in GetComponentsInChildren<ProcessorPortObject>())
            {
                var connection = port.ConnectionObject;
                Grid.UnregisterPort(port.SubcellPosition + GetRootPosition(), port);
                if(connection == null) continue;
                Destroy(connection.gameObject);
            }
            
            ClearFromBoard();
            StartPlacement();
        }

        protected override IEnumerator FollowMouseUntilPlace()
        {
            Color startingColor = spriteRenderer.color;
            yield return null;
            SortingGroup.sortAtRoot = true;
            while (!Placed)
            {
                yield return new WaitUntil(() =>
                {
                    if (Grid.MouseOverGrid())
                    {
                        spriteRenderer.color = (Grid.IsCellOccupierValid(this) ? validColor : invalidColor);
                        var gridPos = Grid.GetGridPosition(Grid.GetMouseWorldPos3D());

                        Vector2Int rotatedSize = Rotation % 180 == 0 ? Size : new Vector2Int(Size.y, Size.x);
                        
                        Vector3 offset = new Vector2(
                            rotatedSize.x  % 2 == 0 ? 0 : Grid.CellSize.x / 2.0f,
                            rotatedSize.y % 2 == 0 ? 0 : Grid.CellSize.y / 2.0f
                            );

                        transform.position = Grid.GetWorldPosition(gridPos) + offset;

                    }
                    else
                    {
                        spriteRenderer.color = invalidColor;
                        Vector2 mousePos = Grid.GetMouseWorldPos2D();
                        transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
                    }

                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        Rotate();
                    }

                    if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) {
                        Sell();
                        return true;
                    }

                    //TODO allow cancels
                    return Input.GetMouseButtonDown(0);
                });
                Placed = Grid.PlaceCellOccupierAtCurrentPosition(this);
                yield return new WaitForEndOfFrame();
            }
            
            spriteRenderer.color = startingColor;
            SortingGroup.sortAtRoot = false;
        }

        public abstract MachineInstance BuildInstance();

        public string GetPrefabId()
        {
            return guid.ToString();
        }

        public int GetInputPortIndex(ProcessorPortObject port)
        {
            return _iPorts.IndexOf(port);
        }

        public int GetOutputPortIndex(ProcessorPortObject port)
        {
            return _oPorts.IndexOf(port);
        }

        public void SetRotation(int machineRotation)
        {
            Rotation = machineRotation;
            transform.rotation = Quaternion.Euler(0, 0, machineRotation);
        }

        public virtual string GetMachineName()
        {
            return name;
        }

        public virtual int GetMachineCost() {
            return 0;
        }

        public Guid GetGuid() {
            return MachineInstance.Vector2IntToGuid(GetRootPosition());
        }
        
        public virtual string GetMachineDesc()
        {
            return "";
        }

        private void OnMouseEnter()
        {
            if(spriteRenderer != null)
                spriteRenderer.color = defaultColor * new Color(0.8f, 0.8f, 0.8f, 1);
            else if(spriteImage != null)
                spriteImage.color = defaultColor * new Color(0.8f,0.8f, 0.8f, 1);
        }

        private void OnMouseExit()
        {
            if(spriteRenderer != null)
                spriteRenderer.color = defaultColor;
            else if (spriteImage != null )
                spriteImage.color = defaultColor;
        }

        public void OnBeforeSerialize()
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = System.Guid.NewGuid().ToString();
            }
        }
        public void OnAfterDeserialize()
        {
            return;
        }

        public virtual VideoClip GetVideoClip()
        {
            return null;
        }

        public virtual Resource GetResource() {
            return null;
        }
    }
}