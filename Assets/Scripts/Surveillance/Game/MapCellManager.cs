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
        [SerializeField] [HideInInspector] private List<HEFace> faces;
        
        private Dictionary<HEFace, MapCell> _cells;
        private List<MapEdgeVertex> _edgeVertices;
        
        private Camera _mainCamera;
        private Camera _camera;

        public IReadOnlyList<MapEdgeVertex> EdgeVertices => _edgeVertices;

        private void Awake()
        {
            _camera = Camera.main;
            _cells = FindObjectsOfType<MapCell>()
                .GroupBy(cell => cell.Face)
                .ToDictionary(g => g.Key, g => g.First());
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

        public int GetPopulationInFace(HEFace face)
        {
            if (!_cells.ContainsKey(face)) {
                return 0;
            }
            
            return _cells[face].GetData().DailyPopulation;
        }

        public void PrintFace(HEFace face)
        {
            Debug.Log(_cells[face].GetCenter());
        }

        public MapCell GetCell(HEFace face)
        {
            if (!_cells.TryGetValue(face, out var cell))
            {
                throw new ArgumentException("Face not in map.");
            }
            
            return cell;
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

        public List<Vector2> GetFacePoints(HEFace face)
        {
            return face.loop.Select(x => new Vector2(vertices[x].ij.x, -vertices[x].ij.y)).ToList();
        } 
        
        bool PointInFaceAABB(List<int> loop, Vector2 p)
        {
            float minX = float.PositiveInfinity, minY = float.PositiveInfinity;
            float maxX = float.NegativeInfinity, maxY = float.NegativeInfinity;

            for (int i = 0; i < loop.Count; i++)
            {
                var v = vertices[loop[i]].ij;
                if (v.x < minX) minX = v.x;
                if (v.y < minY) minY = v.y;
                if (v.x > maxX) maxX = v.x;
                if (v.y > maxY) maxY = v.y;
            }

            return p.x >= minX - Mathf.Epsilon && p.x <= maxX + Mathf.Epsilon && p.y >= minY - Mathf.Epsilon && p.y <= maxY + Mathf.Epsilon;
        }

        private Vector3 ConvertWorldPosToDataPos(Vector3 input)
        {
            return new Vector3(input.x, -input.y, input.z);
        }
        
        private HEFace GetFaceAtPoint(Vector3 pos)
        {
            pos = ConvertWorldPosToDataPos(pos);
            int i = PickFace(pos);
            if (i == -1) {
                return null;
            }

            return faces[i];
        }

        public HashSet<HEFace> GetFacesAroundPoint(Vector3 pos, int depth = 3)
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
        
        
        
        public int PickFace(Vector2 worldPoint, bool onlyInterior = true, bool includeBoundary = true)
        {
            int best = -1;
            float bestAbsArea = float.NegativeInfinity;
            for (int i = faces.Count-1; i >= 0; i--)
            {
                var f = faces[i];
                if (f.loop == null || f.loop.Count < 3) continue;
                if (onlyInterior && f.isExterior) continue;

                if (!PointInFaceAABB(f.loop, worldPoint)) continue;

                if (PointInPolygon(worldPoint, f.loop, includeBoundary))
                {
                    float a = Mathf.Abs(f.area);
                    if (a > bestAbsArea)
                    {
                        bestAbsArea = a;
                        best = i;
                    }
                }
            }
            return best;
        }
        public bool PointInPolygon(Vector2 p, List<int> loop, bool includeBoundary = true)
        {
            bool inside = false;

            for (int i = 0, j = loop.Count - 1; i < loop.Count; j = i++)
            {
                Vector2 a = vertices[loop[j]].ij;
                Vector2 b = vertices[loop[i]].ij;

                if (includeBoundary && OnSegment(a, b, p)) return true;

                bool condY = ((a.y > p.y) != (b.y > p.y));
                if (condY)
                {
                    float xCross = a.x + (b.x - a.x) * ((p.y - a.y) / (b.y - a.y));
                    if (xCross > p.x) inside = !inside;
                }
            }

            return inside;
        }

        static bool OnSegment(Vector2 a, Vector2 b, Vector2 p)
        {
            float cross = Cross(b - a, p - a);
            if (Mathf.Abs(cross) > Mathf.Epsilon) return false;

            float dot = Vector2.Dot(p - a, b - a);
            if (dot < -Mathf.Epsilon) return false;
            float len2 = (b - a).sqrMagnitude;
            if (dot - len2 > Mathf.Epsilon) return false;

            return true;
        }

        static float Cross(Vector2 u, Vector2 v) => u.x * v.y - u.y * v.x;

    }
}