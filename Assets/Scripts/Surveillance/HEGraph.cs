using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance.Surveillance
{
    
    public class HEHalf
    {
        public int origin;          
        public int twin = -1;    
        public int next = -1;      
        public int prev = -1;    
        public int face = -1;     
        public int dst;            
    }

    public class HEFace
    {
        public int halfEdge;             
        public bool isExterior;           
        public string label = "";
        public Color color = new(0.25f, 0.45f, 0.9f, 0.15f);
        public readonly Dictionary<string, string> meta = new();
        public List<int> loop = new();
        public float area;                
    }
    
    public class HEGraph
    {
        public readonly List<HEHalf> HalfEdges;
        public readonly List<HEFace> Faces; 

        public HEGraph(List<HEHalf> halfEdges, List<HEFace> faces) 
        {
            this.Faces = faces;
            this.HalfEdges = halfEdges;
        }

    }
}