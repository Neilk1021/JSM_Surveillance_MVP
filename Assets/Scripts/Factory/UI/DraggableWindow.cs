using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSM.Surveillance.UI
{
    public class DraggableWindow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Tooltip("Set this to the topmost Transform of the window.")]
        [SerializeField] private Transform root;

        private bool _clicked = false;
        private Camera _camera;
        private Vector3 _startingMousePos;
        private Vector3 _startingRootPos;

        private RectTransform _rectTransform;

        private void Start()
        {
            _rectTransform = transform.GetComponent<RectTransform>();
        }

        private void Awake()
        {
            _camera ??= GetComponentInParent<Camera>();
        }


        private void Update()
        {
            if(_camera == null) return;
            BoundCamera();
            
            if (!_clicked) return;
            var delta = Input.mousePosition - _startingMousePos;
            var current = _camera.WorldToScreenPoint(_startingRootPos);
            var newPos = _camera.ScreenToWorldPoint(current + delta);
            root.transform.position = newPos;
            

        }

        private void BoundCamera()
        {
            
            if(_camera == null) return;
            Vector3[] worldCorners = new Vector3[4];
            _rectTransform.GetWorldCorners(worldCorners);
            worldCorners = worldCorners.Select(x => _camera.WorldToViewportPoint(x)).ToArray();
            if (worldCorners[2].x > 1 || worldCorners[2].y > 1)
            {
                Vector2 delta = worldCorners[2] - new Vector3(1,1,0);
                delta = new Vector2(
                    Mathf.Clamp(delta.x, 0, Mathf.Infinity), 
                    Mathf.Clamp(delta.y, 0,Mathf.Infinity)
                    );

                Vector3 deltaWorld = _camera.transform.TransformDirection(delta);
                root.position -= deltaWorld;
            }

            if (worldCorners[0].x < 0 || worldCorners[0].y < 0)
            {
                Vector2 delta = worldCorners[0];
                delta = new Vector2(
                    Mathf.Clamp(delta.x, -Mathf.Infinity, 0), 
                    Mathf.Clamp(delta.y, -Mathf.Infinity, 0)
                );

                Vector3 deltaWorld = _camera.transform.TransformDirection(delta);
                root.position -= deltaWorld;
            }
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            _startingMousePos = Input.mousePosition;
            _startingRootPos = root.transform.position;
            root.GetComponent<SurveillanceWindow>()?.BringToFront();
            _clicked = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _clicked = false;
        }
    }
}