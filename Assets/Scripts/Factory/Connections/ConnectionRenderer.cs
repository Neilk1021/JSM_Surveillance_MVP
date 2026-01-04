using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace JSM.Surveillance
{
    [RequireComponent(typeof(LineRenderer))]
    public class ConnectionRenderer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LineRenderer lr;
        [FormerlySerializedAs("connection")] [SerializeField] private ConnectionObject connectionObject;

        [Header("Colors")] [SerializeField] private Color normalColor;
        [SerializeField] private Color highlightedColor;
        [SerializeField] private Color selectedColor;
        private Vector3[] worldPositions;
        
        private void Awake()
        {
            lr ??= GetComponent<LineRenderer>();
            connectionObject ??= GetComponent<ConnectionObject>();
        }

        public void PlaceConnection()
        {
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
   
            lr.positionCount = connectionObject.Positions.Length + 2;

            worldPositions = new Vector3[lr.positionCount];
            worldPositions[0] = transform.InverseTransformPoint(connectionObject.StartPortObject.transform.position);
                
            for (int i = 0; i < connectionObject.Positions.Length; i++)
            {
                worldPositions[i + 1] = transform.InverseTransformPoint((
                    connectionObject.Grid.GetWorldPosition(connectionObject.Positions[i]) + 
                    new Vector3(connectionObject.Grid.CellSize / 2, connectionObject.Grid.CellSize / 2, 0)
                ));
            }

            worldPositions[^1] = transform.InverseTransformPoint(connectionObject.EndPortObject.transform.position);
            lr.SetPositions(worldPositions);
        }

        public void FlipDirection()
        {
            worldPositions = worldPositions.Reverse().ToArray();
            lr.SetPositions(worldPositions);
        }
        
        public void Render(CellOccupierStatus status)
        {
            Color color;
            switch (status)
            {
                case CellOccupierStatus.NotHovering:
                    color = normalColor;
                    break;
                case CellOccupierStatus.Hovering:
                    color = highlightedColor;
                    break;
                case CellOccupierStatus.Selected:
                    color = selectedColor;
                    break;
                default:
                    color = normalColor;
                    break;
            }

            lr.startColor = color;
            lr.endColor = color;
        }
    }
}