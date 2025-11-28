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
        private HashSet<MapEdgeVertex> _edgeVertices;
        
        private void Awake()
        {
            _camera = Camera.main;
            _edgeVertices = FindObjectsOfType<MapEdgeVertex>().ToHashSet();
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
        
        /// <summary>
        /// Gets all the points in a face. 
        /// </summary>
        /// <param name="face">Face whose vertices you'd like to get.</param>
        /// <returns>Face's vertices.</returns>
        public List<Vector2> GetFacePoints(HEFace face)
        {
            return face.loop.Select(x => new Vector2(vertices[x].ij.x, -vertices[x].ij.y)).ToList();
        } 
        
        private Vector3 ConvertWorldPosToDataPos(Vector3 input)
        {
            return new Vector3(input.x, -input.y, input.z);
        }
        
        /// <summary>
        /// Get's all the faces around a given point using DFS.
        /// </summary>
        /// <param name="pos">The point for where to look.</param>
        /// <param name="depth">How many faces to search from the starting face.</param>
        /// <returns>All the faces around a given point.</returns>
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
            HashSet<HEFace> combined = new HashSet<HEFace>(visited);
            
            bool firstOver = false;
            for (int i = startingFace.halfEdge; i != startingFace.halfEdge || !firstOver; i = halfEdges[i].next)
            {
                firstOver = true;
                var face = faces[halfEdges[halfEdges[i].twin].face];
                if (visited.Contains(face)) {
                    continue;
                }

                var visitedClone = new HashSet<HEFace>(visited);
                GetFacesBFS(face, depth - 1, visitedClone);
                combined.UnionWith(visitedClone);
            }

            return combined;
        }

        /// <summary>
        /// Gets mouse's position on the current map. 
        /// </summary>
        /// <returns>Current mouse position.</returns>
        public Vector3 GetMouseCurrentPosition()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] results = new RaycastHit[32];
            var size = Physics.RaycastNonAlloc(ray, results, 100);

            for (int i = 0; i < size && i < 32; ++i)
            {
                if (results[i].transform.CompareTag("MapCell"))
                {
                    return results[i].point;
                }
            }

            return Vector3.negativeInfinity;
        }

        /// <summary>
        /// Returns the map edge closest to a given point.
        /// </summary>
        /// <param name="pos">Position of where to look.</param>
        /// <param name="threshold">How far to check for the nearest vertex.</param>
        /// <returns>MapVertex that the point is closest to.</returns>
        /// <exception cref="ArgumentException">Thrown if the provided position is not valid.</exception>
        public MapEdgeVertex GetVertexClosetTo(Vector3 pos, float threshold = Mathf.Infinity)
        {
            if (_edgeVertices.Count <= 0) {
                throw new ArgumentException("No vertices in the scene!");
            }

            return _edgeVertices
                .Where(x=> Vector2.Distance(pos,x.transform.position) < threshold)
                .OrderBy(x=> Vector2.Distance(pos, x.transform.position))
                .FirstOrDefault();
        }
    }
}