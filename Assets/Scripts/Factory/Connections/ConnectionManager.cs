using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JSM.Surveillance.Saving;
using UnityEngine.Serialization;

namespace JSM.Surveillance
{
    public class ConnectionManager : MonoBehaviour
    {
        [SerializeField] FactoryGrid grid;
        [FormerlySerializedAs("connectionPrefab")] [SerializeField] private ConnectionObject connectionObjectPrefab;
        [SerializeField] private ConnectionPreviewRenderer connectionPreviewPreviewPrefab;
        
        private Camera _camera;

        private void Start() {
            if (grid is null) {
                Debug.LogWarning($"Grid is NOT set, defaulting to searching scene for grid.");
            }

            _camera = Camera.main;
            grid ??= FindObjectOfType<FactoryGrid>();
        }

        public void OnNodeClicked(ProcessorPortObject portObject)
        {
            Debug.Log($"Node clicked: {portObject.Type} on processor {portObject.Owner.name}");

            // Check if node already has a connection, if so, should remove connection?

            //if node, start connection, take node of node type
            //if type input can only connect to output/base resource?
            //if output cannot connect to ohter output.
            //if node is input and pending an output node, connect and set pending null
            StartCoroutine(MakeConnection(portObject));
        }

        public void PlaceConnection(ConnectionEdge edge)
        {
            grid.PlaceConnection(
                connectionObjectPrefab, 
                grid.GetPortAtCell(edge.fromPos),
                grid.GetPortAtCell(edge.toPos),
                edge.positions.ToList()
            );
        }
        
        
        IEnumerator MakeConnection(ProcessorPortObject startPortObject)
        {
            CellOccupier occupier = startPortObject.Owner.GetComponent<CellOccupier>();
            if(occupier == null) yield break;
            
            Vector2Int rootPos =  occupier.GetRootPosition();
            Vector2Int startingCell = startPortObject.SubcellPosition + rootPos;
            
            Debug.Log($"{rootPos}, {startingCell}");
            
            if (!grid.IsCellEmpty(startingCell)) {
                yield break; 
            }
            
            List<Vector2Int> connectionPositions = new List<Vector2Int> { startingCell };

            Vector2Int lastPos = startingCell;
            
            ProcessorPortObject endPortObject = null;
            var connectionPreview = Instantiate(connectionPreviewPreviewPrefab, transform);
            
            while (Input.GetMouseButton(0))
            {
                yield return null;
                
                var newPos = grid.GetGridPosition(grid.GetMouseWorldPos3D());
                
                if (newPos == lastPos) continue;
                if (connectionPositions.Contains(newPos))
                {
                    lastPos = SlicePositionList(newPos, ref connectionPositions);

                    endPortObject = RefreshPreview(startPortObject, connectionPositions, newPos, connectionPreview);
                    continue;
                }
                if (!grid.IsCellEmpty(newPos)) continue;
                if (!EnsurePathContinuity(newPos, lastPos, connectionPositions)) { continue; }

                connectionPositions.Add(newPos);
                lastPos = newPos;
                
                endPortObject = RefreshPreview(startPortObject, connectionPositions, newPos, connectionPreview);
            }
            
            Destroy(connectionPreview.gameObject);
            if (endPortObject == null || endPortObject == startPortObject || endPortObject.Type == startPortObject.Type || endPortObject.Owner == startPortObject.Owner) yield break;
            

            grid.PlaceConnection(connectionObjectPrefab, startPortObject, endPortObject, connectionPositions);

        }

        private ProcessorPortObject RefreshPreview(ProcessorPortObject startPortObject, List<Vector2Int> connectionPositions, Vector2Int newPos,
            ConnectionPreviewRenderer connectionPreview)
        {
            var worldPos = connectionPositions.Select(x => 
                grid.GetWorldPosition(x) + 
                new Vector3(grid.CellSize / 2, grid.CellSize / 2, 0)
            ).ToList();
                    
            var endPort = grid.GetPortAtCell(newPos);
            connectionPreview.UpdateConnectionPath(worldPos, startPortObject, endPort);
            return endPort;
        }

        private bool EnsurePathContinuity(Vector2Int newPos, Vector2Int lastPos, List<Vector2Int> connectionPositions)
        {
            while (Vector2Int.Distance(newPos, lastPos) > 1)
            {
                Vector2Int dir = newPos - lastPos;
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                {
                    var tempXPos = lastPos + new Vector2Int(Math.Sign(dir.x), 0);
                    if (connectionPositions.Contains(tempXPos) || !grid.IsCellEmpty(tempXPos)) {
                        return false;
                    }
                        
                    lastPos = tempXPos;
                    connectionPositions.Add(lastPos);
                    continue;
                }
                
                var tempYPos = lastPos + new Vector2Int(0, Math.Sign(dir.y));
                if (connectionPositions.Contains(tempYPos) || !grid.IsCellEmpty(tempYPos)) {
                    return false;
                }

                lastPos = tempYPos; 
                connectionPositions.Add(lastPos);
            }

            return true;
        }

        private static Vector2Int SlicePositionList(Vector2Int newPos, ref List<Vector2Int> connectionPositions)
        {
            Vector2Int lastPos;
            bool found = false;
            connectionPositions = connectionPositions.TakeWhile(x =>
            {
                if (found) return false;
                if (x.Equals(newPos))
                {
                    found = true;
                }
                return true;
            }).ToList();
            lastPos = newPos;
            return lastPos;
        }
    }
}