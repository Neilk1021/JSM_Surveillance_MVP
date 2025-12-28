using System;
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
        
        private void Awake()
        {
            _camera ??= GetComponentInParent<Camera>();
        }


        private void Update()
        {
            if (!_clicked || _camera == null) return;
            var delta = Input.mousePosition - _startingMousePos;
            var current = _camera.WorldToScreenPoint(_startingRootPos);
            var newPos = _camera.ScreenToWorldPoint(current + delta);
            root.transform.position = newPos;
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