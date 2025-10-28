using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance.Surveillance.Data
{
    public class HEGraphAsset : ScriptableObject
    {
        private List<VertexData> _verticies;
        private List<HalfEdgeData> _halfEdges;
        private List<CellData> _cells;
    }   
}

