using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    public class Draggable : MonoBehaviour
    {
        private bool isDragging = false;
        private ProcessorInstance processor;

        private void Start()
        {
            processor = GetComponent<ProcessorInstance>();
        }

        private void OnMouseDown()
        {
            isDragging = true;
            Debug.Log($"{processor.name} is being dragged im killing myself");
        }

        private void OnMouseUp()
        {
            if (!isDragging) return;
            isDragging = false;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;


            // Try placement on grid when let go snap to grid (i might need to change pivot of processors?)
            
            
        }

        private void Update()
        {
            if (!isDragging) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = mousePos;
        }
    }
}