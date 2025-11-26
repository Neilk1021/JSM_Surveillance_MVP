using System;
using System.Collections.Generic;
using Surveillance.Game;
using UnityEngine;

namespace JSM.Surveillance.Game
{
    public class FovView : MonoBehaviour
    {
        [SerializeField] private float range = 1;
        
        [Range(1,359)]
        [SerializeField] private float fov = 90;

        private const float SegmentsPerDegree = 0.5f;

        public float FOV => fov;
        public float Range => range;
        
        public Mesh GetFOVMesh()
        {
            Mesh mesh = new Mesh();
            int segments = (int)(Mathf.Clamp(SegmentsPerDegree * fov, 2,1200));
            int vertexCount = segments + 2;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[vertexCount * 3];


            vertices[0] = Vector3.zero;

            float halfAngle = fov / 2;
            float angleStep = fov / segments;

            float currentAngle = -halfAngle;
            for (int i = 0; i <  segments; i++)
            {
                float rad = currentAngle * Mathf.Deg2Rad;

                float x = Mathf.Sin(rad) * range;
                float y = Mathf.Cos(rad) * range;

                vertices[i + 1] = new Vector3(x, y, 0);
                currentAngle += angleStep;
            }

            for (int i = 0; i < segments; i++)
            {
                int triIndex = i * 3;

                triangles[triIndex + 0] = 0;       
                triangles[triIndex + 1] = i + 1;   
                triangles[triIndex + 2] = i + 2;   
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            return mesh;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 1, 1, 0.1f);
            Gizmos.DrawMesh(GetFOVMesh(), transform.position, transform.rotation);
        }

        public Vector2 GetDirection()
        {
            float theta = (transform.rotation.eulerAngles.z+90) * Mathf.Deg2Rad;  
            return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
        }
    }
}