using System.Collections.Generic;
using System.Linq;
using JSM.Surveillance.Game;
using JSM.Surveillance.Surveillance;
using JSM.Surveillance.UI;
using TMPro;
using UnityEngine;

namespace Surveillance.Game
{

    public class MapLoader : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textPrefab;
        [SerializeField] private GameObject vertexPrefab;
        [SerializeField] private TextAsset map;

        [SerializeField] private MapCellDataManager manager;
        
        private List<HEGraphData.Vertex> _vertices;
        private List<HEHalf> _halfEdges;

        private HashSet<HEGraphData.Vertex> streetVertices = new HashSet<HEGraphData.Vertex>();
        private HashSet<HEGraphData.Vertex> buildingVertices = new HashSet<HEGraphData.Vertex>();
        
        static float DistPointToSeg(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float t = Vector2.Dot(p - a, ab) / Mathf.Max(ab.sqrMagnitude, 1e-12f);
            t = Mathf.Clamp01(t);
            return Vector2.Distance(a + t * ab, p);
        }

        static float MinDistToPolygonEdges(Vector2 p, List<Vector2> ring)
        {
            float d = float.PositiveInfinity;
            int n = ring.Count;
            for (int i = 0; i < n; i++)
            {
                Vector2 a = ring[i];
                Vector2 b = ring[(i + 1) % n];
                d = Mathf.Min(d, DistPointToSeg(p, a, b));
            }
            return d;
        }

        private GameObject GenerateAndPreparePolygonForShader(HEFace face)
        {
            face.EnsureSOFromJson();
            if (face.Data == null) {
                return null;
            }
            
            var loopIdx = face.loop;
            var verts2D = loopIdx.Select(i => new Vector2(_vertices[i].ij.x, -_vertices[i].ij.y)).ToList();
            var mesh = GenerateMeshFromVertices(verts2D);
            StoreVertexInSet(face);
            return CreateCell(face, mesh, verts2D);
        }

        private static Mesh GenerateMeshFromVertices(List<Vector2> verts2D)
        {
            var tris = Triangulation.Triangulate(verts2D);
    
            var pos3 = verts2D.Select(v => new Vector3(v.x, v.y, 0f)).ToArray();
            var mesh = new Mesh { vertices = pos3, triangles = Triangulation.FlipWindingToUnity(tris)};
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }



        private GameObject CreateCell(HEFace face, Mesh mesh, List<Vector2> verts2D)
        {
            var go = new GameObject("Filled Face Shader");
            go.transform.parent = transform;
            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, face.Data.isStreet ? 0.0001f :  0);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>();

            
            Material material = new Material(Shader.Find( face.Data.isStreet ?  "Unlit/PolygonEdgeFactorStreet":  "Unlit/PolygonEdgeFactor")); 

            var points = GeneratePointsFromFace(verts2D);

            Vector2 centroid = Vector2.zero;
            foreach(var v in verts2D) centroid += v;
            centroid /= verts2D.Count;
            float maxDist = MinDistToPolygonEdges(centroid, verts2D);


            var cell = go.AddComponent<MapCellRendering>();
            go.AddComponent<MeshCollider>();
            go.AddComponent<MapCell>().Init(face);
            
            cell.SetShaderInfo(verts2D, points, maxDist, 0.01f);
            cell.SetFace(face, material);
            
  
            go.tag = "MapCell";

            if (face.Data.isStreet)
            {
                var text = Instantiate(textPrefab, go.transform);
                text.transform.position = centroid;
                cell.SetTextObject(text);
            }

            return go;
        }

        private void StoreVertexInSet(HEFace face)
        {
            var loopIdx = face.loop;
            var verts2D = loopIdx.Select(i => _vertices[i]).ToList();
            
            for (int i = 0; i < verts2D.Count; i++)
            {
                if (face.Data.isStreet) {
                    streetVertices.Add(verts2D[i]);
                }
                else {
                    buildingVertices.Add(verts2D[i]);
                }
            }
        }

        public void LoadVertices()
        {
            HashSet<HEGraphData.Vertex> intersection = new HashSet<HEGraphData.Vertex>(streetVertices); // Create a copy of set1
            intersection.IntersectWith(buildingVertices);
            GameObject vertParents = new GameObject("Vertices")
            {
                transform =
                {
                    parent = transform
                }
            };

            if (vertexPrefab == null) {
                Debug.LogError("No vertex prefab assigned, aborting.");
            }
            
            foreach (var vert in intersection)
            {
                GameObject vertObj = Instantiate(vertexPrefab, vertParents.transform); 
                vertObj.transform.position = new Vector3(vert.ij.x, -vert.ij.y, -0.025f);
            }
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public void LoadMap()
        {
            var data = JsonUtility.FromJson<HEGraphData>(map.text);
            if (data == null)
            {
                return;
            }

            if (manager == null) {
                manager = GetComponent<MapCellDataManager>();
            }
            
            streetVertices.Clear();
            buildingVertices.Clear();

            _vertices = data.verts;
            _halfEdges = data.halfEdges;

            manager.Init(_vertices, _halfEdges, data.faces);

            GameObject cellParent = new GameObject("Cells")
            {
                transform =
                {
                    parent = transform
                }
            };
            

            List<MapCellRendering> renderers = new List<MapCellRendering>();
            for (int i = 1; i < data.faces.Count; i++)
            {
               var obj = GenerateAndPreparePolygonForShader(data.faces[i]);
               if (obj is null) continue;
               
               renderers.Add(obj.GetComponent<MapCellRendering>());
               obj.transform.parent = cellParent.transform;
            }
            LoadVertices();
            FindObjectOfType<MapRenderingManager>()?.Init(renderers.ToArray());
        }
        
        
        private static Vector4[] GeneratePointsFromFace(List<Vector2> verts2D)
        {
            var points = new Vector4[verts2D.Count];
            for (int i = 0; i < verts2D.Count; i++)
            {
                points[i] = new Vector4(verts2D[i].x, verts2D[i].y, 0, 0);
            }

            return points;
        }

    }
}