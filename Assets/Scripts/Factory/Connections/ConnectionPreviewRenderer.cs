using System;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    [RequireComponent(typeof(LineRenderer))]
    public class ConnectionPreviewRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer lr;

        private void Awake() {
            lr ??= GetComponent<LineRenderer>();
        }

        private void SwitchLRColor(ProcessorPort start, ProcessorPort endPort)
        {
            if (endPort != null &&  endPort != start && endPort.Type != start.Type && endPort.Owner != start.Owner) {
                lr.startColor = Color.green;
                lr.endColor = Color.green;
            }
            else {
                lr.startColor = Color.red;
                lr.endColor = Color.red;
            }
        }

        public void UpdateConnectionPath(
            List<Vector3> worldPositionsList, 
            ProcessorPort start, 
            ProcessorPort end= null)
        {
            SwitchLRColor(start, end);
            
            lr.positionCount = worldPositionsList.Count + 2;

            Vector3[] worldPositions = new Vector3[lr.positionCount];
            worldPositions[0] = transform.InverseTransformPoint(start.transform.position);
                
            for (int i = 0; i < worldPositionsList.Count; i++)
            {
                worldPositions[i + 1] = transform.InverseTransformPoint(worldPositionsList[i]);
            }

            worldPositions[^1] = end == null ? worldPositions[^2] : transform.InverseTransformPoint(end.transform.position);
            lr.SetPositions(worldPositions);
        }
        
    }
}