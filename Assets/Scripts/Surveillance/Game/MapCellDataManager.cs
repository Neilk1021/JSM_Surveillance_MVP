using System;
using System.Collections.Generic;
using System.Linq;
using JSM.Surveillance.Data;
using JSM.Surveillance.Surveillance;
using UnityEngine;

namespace JSM.Surveillance.Game
{
    public class MapCellDataManager : MonoBehaviour
    {
        [SerializeField] [HideInInspector] private List<HEGraphData.Vertex> vertices;
        [SerializeField] [HideInInspector] private List<HEHalf> halfEdges;
        [SerializeField] [HideInInspector] private List<HEFace> faces;

        private Camera _camera;
        private List<MapEdgeVertex> _edgeVertices;
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
        public void Init(List<HEGraphData.Vertex> verts, List<HEHalf> halves, List<HEFace> faces)
        {
            vertices = verts;
            halfEdges = halves;
            this.faces = faces;  
        }

        
        private HEFace GetFaceAtPoint(Vector3 pos)
        {
            pos = ConvertWorldPosToDataPos(pos);
            int i = HalfEdgeUtil.PickFace(pos, faces, vertices);
            if (i == -1) {
                return null;
            }

            return faces[i];
        }
        

        public List<Vector2> GetFacePoints(HEFace face)
        {
            return face.loop.Select(x => new Vector2(vertices[x].ij.x, -vertices[x].ij.y)).ToList();
        } 
        
        private Vector3 ConvertWorldPosToDataPos(Vector3 input)
        {
            return new Vector3(input.x, -input.y, input.z);
        }
        
        public HashSet<HEFace> GetFacesAroundPoint(Vector3 pos, int depth = 2)
        {
            return GetFacesBFS(GetFaceAtPoint(pos), depth);
        }
        
        private HashSet<HEFace> GetFacesBFS(HEFace startingFace, int depth, HashSet<HEFace> visited = null)
        {
            if (depth == 0) {
                return null;
            }

            if (visited == null) {
                visited = new HashSet<HEFace>();
            }

            visited.Add(startingFace);
            for (int i = startingFace.halfEdge; halfEdges[i].next != startingFace.halfEdge ; i = halfEdges[i].next)
            {
                var face = faces[halfEdges[halfEdges[i].twin].face];
                if (visited.Contains(face)) {
                    continue;
                }
                
                GetFacesBFS(face, depth - 1, visited);
            }

            return visited;
        }

        public Vector3 GetMouseCurrentPosition()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
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