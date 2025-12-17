using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace JSM.Surveillance
{
    public class ConnectionManager : MonoBehaviour
    {
        public static ConnectionManager Instance { get; private set; }
        [SerializeField] FactoryGrid grid;
        [SerializeField] private Connection connectionPrefab;
        [SerializeField] private LineRenderer connectionPreviewPrefab;
        
        private Camera _camera;

        private ProcessorNode _currentNode = null;
        
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

        public void OnNodeClicked(ProcessorNode node)
        {
            Debug.Log($"Node clicked: {node.Type} on processor {node.Owner.name}");

            // Check if node already has a connection, if so, should remove connection?

            //if node, start connection, take node of node type
            //if type input can only connect to output/base resource?
            //if output cannot connect to ohter output.
            //if node is input and pending an output node, connect and set pending null
            StartCoroutine(MakeConnection(node, PlaceConnection));
        }

        private void PlaceConnection(Connection connection)
        {
            LineRenderer lr = connection.LineRenderer; 
            lr.positionCount = connection.Positions.Length;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.SetPositions(
                connection.Positions.Select(x=>
                        grid.GetWorldPosition(x) + 
                        new Vector3(grid.CellSize/2, grid.CellSize/2, 0)
                    ).ToArray()
                );
        }
        

        IEnumerator MakeConnection(ProcessorNode start, Action<Connection> placeAction)
        {
            bool valid = false;
            CellOccupier occupier = start.Owner.GetComponent<CellOccupier>();
            if(occupier == null) yield break;
            
            Vector2Int rootPos =  occupier.GetRootPosition();
            Vector2Int startingCell = start.SubcellPosition + rootPos;

            if (!grid.IsCellEmpty(startingCell)) {
                yield break; 
            }
            
            List<Vector2Int> connectionPositions = new List<Vector2Int> { startingCell };

            Vector2Int lastPos = startingCell;
            
            //TODO refactor into its own class.
            var lr = Instantiate(connectionPreviewPrefab);
            lr.startColor = Color.red;
            lr.endColor = Color.red;
            
            while (Input.GetMouseButton(0))
            {
                yield return null;
                
                var newPos = grid.GetGridPosition(_camera.ScreenToWorldPoint(Input.mousePosition));
                if (newPos == lastPos) continue;
                
                if (connectionPositions.Contains(newPos))
                {
                    lastPos = SlicePositionList(newPos, ref connectionPositions);
                    UpdateLineRendererPath(lr, connectionPositions);
                    continue;
                }
                if (!grid.IsCellEmpty(newPos)) continue;
                if (!EnsurePathContinuity(newPos, lastPos, connectionPositions)) {
                    continue;
                }

                connectionPositions.Add(newPos);
                lastPos = newPos;

                if (_currentNode != null && _currentNode != start) {
                    valid = true;
                    lr.startColor = Color.green;
                    lr.endColor = Color.green;
                }
                else {
                    valid = false;
                    lr.startColor = Color.red;
                    lr.endColor = Color.red;
                }
                
                UpdateLineRendererPath(lr, connectionPositions);
            }
            
            Destroy(lr.gameObject);
            if (!valid) yield break;
            
            var connectionObj = Instantiate(connectionPrefab);
            connectionObj.InitializeConnection(_currentNode, start, connectionPositions);
            grid.PlaceConnection(connectionObj);
            placeAction.Invoke(connectionObj);
        }

        private void UpdateLineRendererPath(LineRenderer lr, List<Vector2Int> connectionPositions)
        {
            lr.positionCount = connectionPositions.Count;
            lr.SetPositions(connectionPositions.Select(x =>
                grid.GetWorldPosition(x) +
                new Vector3(grid.CellSize / 2, grid.CellSize / 2, 0)
            ).ToArray());
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


        private void RemoveConnection(Connection connection)
        {
            //remove connection
        }

        //TODO fix these so they work.
        public void OnNodeEntered(ProcessorNode processorNode)
        {
            _currentNode = processorNode;
        }

        public void OnNodeExited(ProcessorNode processorNode)
        {
            if(_currentNode == processorNode)
                _currentNode = null;
        }

    }
}