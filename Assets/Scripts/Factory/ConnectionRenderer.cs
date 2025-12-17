using System;
using UnityEngine;

namespace JSM.Surveillance
{
    [RequireComponent(typeof(LineRenderer))]
    public class ConnectionRenderer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LineRenderer lr;
        [SerializeField] private Connection connection;

        [Header("Colors")] [SerializeField] private Color normalColor;
        [SerializeField] private Color highlightedColor;
        [SerializeField] private Color selectedColor;
        
        private void Awake()
        {
            lr ??= GetComponent<LineRenderer>();
            connection ??= GetComponent<Connection>();
        }

        public void PlaceConnection()
        {
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
   
            lr.positionCount = connection.Positions.Length + 2;

            Vector3[] worldPositions = new Vector3[lr.positionCount];
            worldPositions[0] = connection.StartPort.transform.position;
                
            for (int i = 0; i < connection.Positions.Length; i++)
            {
                worldPositions[i + 1] = (
                    connection.Grid.GetWorldPosition(connection.Positions[i]) + 
                    new Vector3(connection.Grid.CellSize / 2, connection.Grid.CellSize / 2, 0)
                );
            }

            worldPositions[^1] = connection.EndPort.transform.position;
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