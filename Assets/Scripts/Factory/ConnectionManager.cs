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

        [SerializeField] private GameObject connectionVisualPrefab;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }
            
            Destroy(gameObject);
        }

        public void OnNodeClicked(ProcessorNode node)
        {
            Debug.Log($"Node clicked: {node.Type} on processor {node.Owner.name}");

            // Check if node already has a connection, if so, should remove connection?

            //if node, start connection, take node of node type
            //if type input can only connect to output/base resource?
            //if output cannot connect to ohter output.
            //if node is input and pending an output node, connect and set pending null
        }

        
        private void RemoveConnection(Connection connection)
        {
            //remove connection
        }

    }
}