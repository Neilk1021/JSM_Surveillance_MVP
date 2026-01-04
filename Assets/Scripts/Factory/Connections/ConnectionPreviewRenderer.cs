using System;
using System.Collections.Generic;
using System.Linq;
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

        private void SwitchLRColor(ProcessorPortObject start, ProcessorPortObject endPortObject)
        {
            if (endPortObject != null &&  endPortObject != start && endPortObject.Type != start.Type && endPortObject.Owner != start.Owner) {
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
            ProcessorPortObject start, 
            ProcessorPortObject end= null)
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
            if(start.Type == NodeType.End) worldPositions = worldPositions.Reverse().ToArray();
            lr.SetPositions(worldPositions);
        }
        
    }
}