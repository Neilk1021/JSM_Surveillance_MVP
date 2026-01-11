using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance.Saving
{
    [System.Serializable]
    public class FactoryBlueprint {
        public List<MachineNode> machines = new();
        public List<ConnectionEdge> connections = new();
    }

    [System.Serializable]
    public class MachineNode {
        public List<Vector2Int> positions;
        public string prefabId; 
        public string recipeId;
        public Vector3 localPos;
        public int rotation;
    }

    [System.Serializable]
    public class ConnectionEdge
    {
        public Vector2Int fromPos;
        public Vector2Int toPos;
        public Vector2Int toRootPos;
        public Vector2Int fromRootPos;
        public Vector2Int[] positions;
        public int inputPortIndex;
    }
}