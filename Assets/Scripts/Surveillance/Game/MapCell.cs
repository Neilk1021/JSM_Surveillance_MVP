using System;
using System.Collections.Generic;
using System.Linq;
using JSM.Surveillance.Surveillance;
using JSM.Surveillance.UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Surveillance.Game
{
    public struct CellData
    {
        public int DailyPopulation;
        public float RiskFactor;
    }
    
    public class MapCell : MonoBehaviour
    {
        private MapCellManager _manager;
        [SerializeField] private HEFace face;

        public HEFace Face => face;
        private HEFaceGameData data;
        private MeshFilter _filter; 
        [HideInInspector] [SerializeField] private Vector3 center = Vector3.negativeInfinity;
        public bool IsStreet
        {
            get
            {
                face.EnsureSOFromJson();
                return data.isStreet;
            }
        }


        /// <summary>
        /// Initializes a map cell based on a given face.
        /// </summary>
        /// <param name="face">Face to draw from</param>
        public void Init(HEFace face) {
            this.face = face;
            face.EnsureSOFromJson();
            data = face.data;
            center = GetMeshCenter(GetComponent<MeshFilter>());
        }

        public CellData GetData()
        {
            return new CellData() { DailyPopulation = face.data.dailyPopulation, RiskFactor = face.data.riskFactor };
        }
        
        private void Awake()
        {
            _manager = FindObjectOfType<MapCellManager>();
            _filter = GetComponent<MeshFilter>();
            if (center == Vector3.negativeInfinity)
            {
                _filter = GetComponent<MeshFilter>();
                GetMeshCenter(_filter);
            }
        }

        private void Start()
        {
            if (_manager == null) {
                return;
            }
            face.EnsureSOFromJson();
            data = face.data;
            
            if (data.companyId == "") {
                return;
            }
            var obj = Instantiate(_manager.CompanyTextPrefab, center, quaternion.identity);
            obj.transform.parent = transform;
            var nameUI = obj.GetComponent<CompanyNameUI>();
            nameUI.SetCompanyName(data.companyId);
            nameUI.Resize(_filter.sharedMesh.bounds);
        }

        public Vector3 GetCenter()
        {
            return GetMeshCenter(_filter);
        }
        public static Vector3 GetMeshCenter(MeshFilter meshFilter)
        {
            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null) return meshFilter.transform.position;

            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            if (vertices == null || vertices.Length == 0 || triangles == null || triangles.Length == 0)
                return meshFilter.transform.position;

            Vector3 accumulatedCenter = Vector3.zero;
            double totalArea = 0.0;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 a = vertices[triangles[i]];
                Vector3 b = vertices[triangles[i + 1]];
                Vector3 c = vertices[triangles[i + 2]];

                Vector3 triCenter = (a + b + c) / 3f;

                float area = Vector3.Cross(b - a, c - a).magnitude * 0.5f;

                accumulatedCenter += triCenter * area;
                totalArea += area;
            }

            if (totalArea <= 0.0)
                return meshFilter.transform.position;

            Vector3 localCenter = accumulatedCenter / (float)totalArea;

            return meshFilter.transform.TransformPoint(localCenter);
        }
    }
}