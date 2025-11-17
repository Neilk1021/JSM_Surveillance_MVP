using System;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance.Surveillance
{
    [System.Serializable]
    public struct InformationRatio
    {
        public float consumer;
        public float government;
        public float corporate;
        public float crime;
    }
    
    [System.Serializable]
    public class HEHalf
    {
        public int origin;          
        public int twin = -1;    
        public int next = -1;      
        public int prev = -1;    
        public int face = -1;     
        public int dst;            
    }

    [System.Serializable]
    public class HEFace
    {
        public int halfEdge;
        public bool isExterior;
        public string label = "";
        public Color color = new(0.25f, 0.45f, 0.9f, 0.15f);
        [NonSerialized] public readonly Dictionary<string, string> meta = new();
        public List<int> loop = new();
        public float area;

        [NonSerialized] public HEFaceGameData data; 

        [SerializeField, HideInInspector] public string _dataJson;

        public void SyncDataJsonFromSO()
        {
            if (data != null)
                _dataJson = JsonUtility.ToJson(data);
            else
                _dataJson = null;
        }

        // Call this after loading the graph
        public void EnsureSOFromJson()
        {
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<HEFaceGameData>();

                if (!string.IsNullOrEmpty(_dataJson))
                    JsonUtility.FromJsonOverwrite(_dataJson, data);
            }
        }
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