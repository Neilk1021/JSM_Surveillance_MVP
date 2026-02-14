using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JSM.Surveillance.Util;
using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections.Generic;
using UnityEngine;

public static class MeshExtensions
{
    public static void WeldVertices(this Mesh mesh, float threshold = 0.001f)
    {
        Vector3[] oldVertices = mesh.vertices;
        int[] oldTriangles = mesh.triangles;
        List<Vector3> newVertices = new List<Vector3>();
        int[] newTriangles = new int[oldTriangles.Length];
        
        // Maps old vertex index to new vertex index
        Dictionary<Vector3, int> vertexMap = new Dictionary<Vector3, int>();

        for (int i = 0; i < oldVertices.Length; i++)
        {
            Vector3 v = oldVertices[i];
            // Rounding to threshold to handle floating point precision
            Vector3 key = new Vector3(
                Mathf.Round(v.x / threshold) * threshold,
                Mathf.Round(v.y / threshold) * threshold,
                Mathf.Round(v.z / threshold) * threshold
            );

            if (!vertexMap.ContainsKey(key))
            {
                vertexMap.Add(key, newVertices.Count);
                newVertices.Add(v);
            }
        }

        for (int i = 0; i < oldTriangles.Length; i++)
        {
            Vector3 v = oldVertices[oldTriangles[i]];
            Vector3 key = new Vector3(
                Mathf.Round(v.x / threshold) * threshold,
                Mathf.Round(v.y / threshold) * threshold,
                Mathf.Round(v.z / threshold) * threshold
            );
            newTriangles[i] = vertexMap[key];
        }

        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles;
    }
}

public class MapCellCollider : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshCollider meshCollider;

    public void Init(MeshFilter mf, MeshCollider mc)
    {
        meshCollider = mc;
        meshFilter = mf;
    }
    
    [ContextMenu("Generate Mesh")]
    public void GenerateAndReplaceMeshCollider()
    {
        if(meshCollider == null) return;
        var mesh = meshFilter.sharedMesh;

        
        Vector3[] vertices = mesh.vertices.Select(x=>(Vector3)((Vector2)x)).ToArray();
        int[] tris = mesh.triangles;
        for (int i = 0; i < tris.Length; i += 3)
        {
            (tris[i], tris[i + 1]) = (tris[i + 1], tris[i]);
        }  

        
        var replacement = Extrude2DShape.ExtrudeLoop(vertices, 0.5f, -0.25f, tris);
        replacement.WeldVertices();

        tris = replacement.triangles;
        for (int i = 0; i < tris.Length; i += 3)
        {
            (tris[i], tris[i + 1]) = (tris[i + 1], tris[i]);
        } 
        
        replacement.triangles = tris;
        
        replacement.RecalculateNormals();
        replacement.RecalculateBounds();
        
        meshCollider.cookingOptions = MeshColliderCookingOptions.UseFastMidphase | 
                                      MeshColliderCookingOptions.WeldColocatedVertices;
                
        Physics.BakeMesh(replacement.GetInstanceID(), false);
        
        meshCollider.sharedMesh = null; 
        meshCollider.sharedMesh = replacement;
    }

    void OnDrawGizmosSelected() {
        Mesh mesh = GetComponent<MeshCollider>().sharedMesh;
        if (mesh == null) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < mesh.vertices.Length; i++) {
            Gizmos.DrawRay(transform.TransformPoint(mesh.vertices[i]), 
                transform.TransformDirection(mesh.normals[i]) * 0.2f);
        }
    } 
    public void OnPointerEnter(PointerEventData eventData)
    {
       
        Debug.Log("ENTERED BITCH");

    }
}


