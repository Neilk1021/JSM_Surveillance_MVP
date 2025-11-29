using UnityEngine;

namespace JSM.Surveillance.Game
{
    public class SeparatedAreaView : MonoBehaviour
    {
        [SerializeField] private float range = 1;

        private Vector3 _center = Vector3.zero;
        private const float SegmentsPerDegree = 0.5f;

        public float Range => range;
        public Vector3 Center => _center; 

        public void SetCenter(Vector3 newCenter)
        {
            _center = newCenter;
        }
        
        public Mesh GetMesh()
        {
            Mesh mesh = new Mesh();
            int segments = (int)(Mathf.Clamp(SegmentsPerDegree * 360f, 2, 1200));
            int vertexCount = segments + 2;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[vertexCount * 3];
            
            float halfAngle = 180f;
            float angleStep = 360f / segments;

            float currentAngle = -halfAngle;
            for (int i = 0; i < segments+1; i++)
            {
                float rad = currentAngle * Mathf.Deg2Rad;

                float x = Mathf.Sin(rad) * range;
                float y = Mathf.Cos(rad) * range;

                vertices[i] = new Vector3(x, y, 0) + _center - transform.position;
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
            Gizmos.DrawMesh(GetMesh(), transform.position, transform.rotation);
        }

    }
}