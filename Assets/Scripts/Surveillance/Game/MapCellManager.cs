using System;
using System.Collections.Generic;
using System.Linq;
using JSM.Surveillance.Surveillance;
using UnityEngine;
using UnityEngine.Serialization;

namespace Surveillance.Game
{
    public class MapCellManager : MonoBehaviour
    {
        [Header("Prefabs")] [SerializeField] GameObject companyTextPrefab;
        
        public GameObject CompanyTextPrefab => companyTextPrefab;

        [SerializeField] [HideInInspector] private List<HEGraphData.Vertex> vertices;
        [SerializeField] [HideInInspector] private List<HEHalf> halfEdges;

        private List<MapEdgeVertex> _edgeVertices;
        
        private Camera _mainCamera;
        private Camera _camera;

        public IReadOnlyList<MapEdgeVertex> EdgeVertices => _edgeVertices;

        private void Awake()
        {
            _camera = Camera.main;
            _edgeVertices = FindObjectsOfType<MapEdgeVertex>().ToList();
        }
        
        /// <summary>
        /// Gets a given vertex.;
        /// </summary>
        /// <param name="pos">Vertex to selct</param>
        /// <returns>Vertex at position pos</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public HEGraphData.Vertex GetVertex(int pos)
        {
            if (pos >= vertices.Count || pos < 0)
            {
                throw new ArgumentOutOfRangeException($"{pos} out of range of verticies");
            }
            
            return vertices[pos];
        }
        
        /// <summary>
        /// Initializes the MapCellManager;
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="halves"></param>
        public void Init(List<HEGraphData.Vertex> verts, List<HEHalf> halves)
        {
            vertices = verts;
            halfEdges = halves;
        }


        public Vector3 GetMouseCurrentPosition()
        {
            _mainCamera ??= _camera;
            if (_mainCamera is null) {
                return Vector3.negativeInfinity;
            }
            
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray, 100);

            foreach (var hit in hits)
            {
                if (hit.transform.CompareTag("MapCell"))
                {
                    return hit.point;
                }
            }

            return Vector3.negativeInfinity;
        }

        public MapEdgeVertex GetVertexClosetTo(Vector3 pos, float threshold = Mathf.Infinity)
        {
            if (_edgeVertices.Count <= 0) {
                throw new ArgumentException("No vertices in the scene!");
            }
            
            int best = -1;
            float distance = threshold; 
            for (int i = 0; i < _edgeVertices.Count; i++)
            {
                var currentDistance = Vector2.Distance(pos, _edgeVertices[i].transform.position);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    best = i;
                }
            }

            if (best == -1) {
                return null;
            }

            return _edgeVertices[best];
        }
    }
}