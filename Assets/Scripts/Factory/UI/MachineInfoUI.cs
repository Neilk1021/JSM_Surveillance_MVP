using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JSM.Surveillance.UI
{
    public class MachineInfoUI : FactoryUI, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected TextMeshProUGUI machineNameText;
        [SerializeField] private Canvas canvas;
        
        private UIManager _uiManager;
        private bool _initialized = false;
        private MachineObject _machineObj;
        private Camera Camera => _uiManager.WorldCamera;

        private void Start()
        {
            canvas.worldCamera = _uiManager.WorldCamera;
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (MouseInside) return;
            if (!_initialized) {
                _initialized = true;
                return;
            }
            
            _uiManager.Close();
        }

        public void Close()
        {
            _uiManager.Close();
        }
        
        
        private void BoundCamera()
        {
            if(Camera == null) return;
            
            Vector3[] worldCorners = new Vector3[4];
            canvas.GetComponent<RectTransform>().GetWorldCorners(worldCorners);
            worldCorners = worldCorners.Select(x => Camera.WorldToViewportPoint(x)).ToArray();
            Vector3 currentCenter = Camera.WorldToViewportPoint(transform.position);
            
            if (worldCorners[2].x > 1 || worldCorners[2].y > 1)
            {
                Vector2 delta = worldCorners[2] - new Vector3(1,1,0);
                delta = new Vector2(
                    Mathf.Clamp(delta.x, 0, Mathf.Infinity), 
                    Mathf.Clamp(delta.y, 0,Mathf.Infinity)
                );

                currentCenter -= (Vector3)delta;
            }

            if (worldCorners[0].x < 0 || worldCorners[0].y < 0)
            {
                Vector2 delta = worldCorners[0];
                delta = new Vector2(
                    Mathf.Clamp(delta.x, -Mathf.Infinity, 0), 
                    Mathf.Clamp(delta.y, -Mathf.Infinity, 0)
                );

                currentCenter -= (Vector3)delta;
            }

            transform.position = Camera.ViewportToWorldPoint(currentCenter);
        }



        public override void Initialize(CellOccupier occupier, UIManager manager)
        {
            canvas.sortingOrder += SurveillanceWindow.GlobalSortOrder+1;
            if (occupier is MachineObject machineObject) {
                _machineObj = machineObject;
                machineNameText.text = $"{machineObject.GetMachineName()}";
            }
            
            
            _uiManager = manager;
            BoundCamera();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseInside = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseInside = false;
        }

        public void SetInside(bool inside)
        {
            MouseInside = inside;
        }
        
        
        public virtual void Sell()
        {
            _machineObj.Sell();
            Close();
        }

        public virtual void Move()
        {
            _machineObj.Move();
            Close();
        }
    }
}