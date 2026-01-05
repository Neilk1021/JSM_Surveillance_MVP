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
        
        private HashSet<HEFace> GetFacesBFS(HEFace startingFace, int maxDepth)
        {
            var visited = new HashSet<HEFace>();
            var queue = new Queue<(HEFace face, int currentDepth)>();

            if (startingFace == null) return visited;

            visited.Add(startingFace);
            queue.Enqueue((startingFace, 0));

            while (queue.Count > 0)
            {
                var (currentFace, currentDepth) = queue.Dequeue();

                if (currentDepth >= maxDepth) 
                    continue;

                bool firstOver = false;
                int startHe = currentFace.halfEdge;
                for (int i = startHe; i != startHe || !firstOver; i = halfEdges[i].next)
                {
                    firstOver = true;
            
                    int twinIndex = halfEdges[i].twin;
                    int neighborFaceIndex = halfEdges[twinIndex].face;
            
                    HEFace neighborFace = faces[neighborFaceIndex]; 

                    if (neighborFace != null && !visited.Contains(neighborFace))
                    {
                        visited.Add(neighborFace);
                        queue.Enqueue((neighborFace, currentDepth + 1));
                    }
                }
            }

            return visited;
        }

        /// <summary>
        /// Gets mouse's position on the current map. 
        /// </summary>
        /// <returns>Current mouse position.</returns>
        public Vector3 GetMouseCurrentPosition()
        {
            if (!EfficientResScaler.instance || !_camera.targetTexture)
                return GetMouseCurrentPositionOld();
            
            var displayRect = EfficientResScaler.instance.DisplayUI.rectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                displayRect, 
                Input.mousePosition, 
                EfficientResScaler.instance.rtCam, // Use null if Canvas is Screen Space - Overlay
                out var localPoint
            );

            Vector2 viewportPoint = new Vector2(
                (localPoint.x / displayRect.rect.width) + 0.5f,
                (localPoint.y / displayRect.rect.height) + 0.5f
            );

            Ray ray = _camera.ViewportPointToRay(viewportPoint);

            RaycastHit[] results = new RaycastHit[32];
            int size = Physics.RaycastNonAlloc(ray, results, 100);

            for (int i = 0; i < size; ++i)
            {
                if (results[i].transform.CompareTag("MapCell"))
                {
                    return results[i].point;
                }
            }

            return Vector3.negativeInfinity;
        }

        private Vector3 GetMouseCurrentPositionOld()
        {
            float ratio = 1;
            if (_camera.targetTexture)
            {
                ratio = (float)Screen.currentResolution.width / _camera.targetTexture.width;
            }

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition / ratio);
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