using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
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
        [SerializeField] public int halfEdge;
        public bool isExterior;
        public string label = "";
        public Color color = new(0.25f, 0.45f, 0.9f, 0.15f);
        [NonSerialized] public readonly Dictionary<string, string> meta = new();
        public List<int> loop = new();
        public float area;
        [NonSerialized] HEFaceGameData _data;
        public HEFaceGameData Data
        {
            get
            {
                EnsureSOFromJson();
                return _data;
            }
            set => _data = value;
        }

        [SerializeField, HideInInspector] public string _dataJson;

        
        //TODO FIND BETTER WAY TO CHECK IF TWO FACES ARE THE FACE FACE
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(HEFace))
            {
                return false;
            }

            return ((HEFace)obj).GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return halfEdge;
        }

        
        public void SyncDataJsonFromSO()
        {
            _dataJson = _data != null ? JsonUtility.ToJson(_data) : null;
        }

        // Call this after loading the graph
        public void EnsureSOFromJson()
        {
            if (_data == null)
            {
                _data = ScriptableObject.CreateInstance<HEFaceGameData>();

                if (!string.IsNullOrEmpty(_dataJson))
                    JsonUtility.FromJsonOverwrite(_dataJson, _data);
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