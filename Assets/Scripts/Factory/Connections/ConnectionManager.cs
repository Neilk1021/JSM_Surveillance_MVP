using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;

namespace JSM.Surveillance
{
    public class ConnectionManager : MonoBehaviour
    {
        public static ConnectionManager Instance { get; private set; }
        [SerializeField] FactoryGrid grid;
        [SerializeField] private Connection connectionPrefab;
        [SerializeField] private ConnectionPreviewRenderer connectionPreviewPreviewPrefab;
        
        private Camera _camera;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }
            
            Destroy(gameObject);
        }

        private void Start() {
            if (grid is null) {
                Debug.LogWarning($"Grid is NOT set, defaulting to searching scene for grid.");
            }

            _camera = Camera.main;
            grid ??= FindObjectOfType<FactoryGrid>();
        }

        public void OnNodeClicked(ProcessorPort port)
        {
            Debug.Log($"Node clicked: {port.Type} on processor {port.Owner.name}");

            // Check if node already has a connection, if so, should remove connection?

            //if node, start connection, take node of node type
            //if type input can only connect to output/base resource?
            //if output cannot connect to ohter output.
            //if node is input and pending an output node, connect and set pending null
            StartCoroutine(MakeConnection(port));
        }


        IEnumerator MakeConnection(ProcessorPort startPort)
        {
            CellOccupier occupier = startPort.Owner.GetComponent<CellOccupier>();
            if(occupier == null) yield break;
            
            Vector2Int rootPos =  occupier.GetRootPosition();
            Vector2Int startingCell = startPort.SubcellPosition + rootPos;

            if (!grid.IsCellEmpty(startingCell)) {
                yield break; 
            }
            
            List<Vector2Int> connectionPositions = new List<Vector2Int> { startingCell };

            Vector2Int lastPos = startingCell;
            
            ProcessorPort endPort = null;
            var connectionPreview = Instantiate(connectionPreviewPreviewPrefab);
            
            while (Input.GetMouseButton(0))
            {
                yield return null;
                
                var newPos = grid.GetGridPosition(grid.GetMouseWorldPos());
                
                if (newPos == lastPos) continue;
                if (connectionPositions.Contains(newPos))
                {
                    lastPos = SlicePositionList(newPos, ref connectionPositions);

                    endPort = RefreshPreview(startPort, connectionPositions, newPos, connectionPreview);
                    continue;
                }
                if (!grid.IsCellEmpty(newPos)) continue;
                if (!EnsurePathContinuity(newPos, lastPos, connectionPositions)) { continue; }

                connectionPositions.Add(newPos);
                lastPos = newPos;
                
                endPort = RefreshPreview(startPort, connectionPositions, newPos, connectionPreview);
            }
            
            Destroy(connectionPreview.gameObject);
            if (endPort == null || endPort == startPort || endPort.Type == startPort.Type || endPort.Owner == startPort.Owner) yield break;
            
            var connectionObj = Instantiate(connectionPrefab);
            connectionObj.InitializeConnection(startPort, endPort, grid, connectionPositions);
            grid.PlaceConnection(connectionObj);
        }

        private ProcessorPort RefreshPreview(ProcessorPort startPort, List<Vector2Int> connectionPositions, Vector2Int newPos,
            ConnectionPreviewRenderer connectionPreview)
        {
            var worldPos = connectionPositions.Select(x => 
                grid.GetWorldPosition(x) + 
                new Vector3(grid.CellSize / 2, grid.CellSize / 2, 0)
            ).ToList();
                    
            var endPort = grid.GetPortAtCell(newPos);
            connectionPreview.UpdateConnectionPath(worldPos, startPort, endPort);
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