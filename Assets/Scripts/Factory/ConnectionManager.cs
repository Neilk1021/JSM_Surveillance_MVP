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
            CellOccupier occupier = start.Owner.GetComponent<CellOccupier>();
            if(occupier == null) yield break;
            
            Vector2Int rootPos =  occupier.GetRootPosition();
            Vector2Int startingCell = start.SubcellPosition + rootPos;

            if (!grid.IsCellEmpty(startingCell)) {
                yield break; 
            }
            
            List<Vector2Int> connectionPositions = new List<Vector2Int> { startingCell };

            Vector2Int lastPos = startingCell;
            
            while (Input.GetMouseButton(0))
            {
                yield return null;
                
                var newPos = grid.GetGridPosition(_camera.ScreenToWorldPoint(Input.mousePosition));
                if (newPos == lastPos) continue;
                if (connectionPositions.Contains(newPos)) continue;
                if (!grid.IsCellEmpty(newPos)) continue;
                //TODO prevent gaps from forming and being valid.
                
                connectionPositions.Add(newPos);
                lastPos = newPos;
            }
            
            var connectionObj = Instantiate(connectionPrefab);
            connectionObj.InitializeConnection(null, null, connectionPositions);
            placeAction.Invoke(connectionObj);
        }

        
        private void RemoveConnection(Connection connection)
        {
            //remove connection
        }

    }
}