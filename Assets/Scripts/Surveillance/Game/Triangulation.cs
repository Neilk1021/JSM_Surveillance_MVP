namespace Surveillance.Game
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public static class Triangulation
    {
        public static List<int> Triangulate(IReadOnlyList<Vector2> pts)
        {
            int n = pts.Count;
            var indices = new List<int>(n);
            for (int i = 0; i < n; i++) indices.Add(i);

            if (SignedArea(pts) < 0f) indices.Reverse();

            var triangles = new List<int>((n - 2) * 3);
            int guard = 0;

            while (indices.Count > 3 && guard++ < 100000)
            {
                bool cutEar = false;

                for (int i = 0; i < indices.Count; i++)
                {
                    int i0 = indices[(i - 1 + indices.Count) % indices.Count];
                    int i1 = indices[i];
                    int i2 = indices[(i + 1) % indices.Count];

                    Vector2 a = pts[i0], b = pts[i1], c = pts[i2];

                    if (!IsConvex(a, b, c)) continue;
                    if (TriangleArea2(a, b, c) <= 1e-10f) continue; 

                    bool contains = false;
                    for (int k = 0; k < indices.Count; k++)
                    {
                        int idx = indices[k];
                        if (idx == i0 || idx == i1 || idx == i2) continue;
                        if (PointInTriangle(pts[idx], a, b, c))
                        {
                            contains = true;
                            break;
                        }
                    }
                    if (contains) continue;

                    // It's an ear — clip it.
                    triangles.Add(i0);
                    triangles.Add(i1);
                    triangles.Add(i2);
                    indices.RemoveAt(i);
                    cutEar = true;
                    break;
                }

                if (!cutEar)
                    break; 
            }

            if (indices.Count == 3)
            {
                triangles.Add(indices[0]);
                triangles.Add(indices[1]);
                triangles.Add(indices[2]);
            }

            return triangles;
        }
        
        public static int[] FlipWindingToUnity(List<int> tris)
        {
            for (int i = 0; i < tris.Count; i += 3)
            {
                // (a, b, c) -> (a, c, b)
                (tris[i + 1], tris[i + 2]) = (tris[i + 2], tris[i + 1]);
            }
            return tris.ToArray();
        }
        static float SignedArea(IReadOnlyList<Vector2> p)
        {
            double a = 0;
            for (int i = 0, j = p.Count - 1; i < p.Count; j = i++)
                a += (double)(p[j].x * p[i].y - p[i].x * p[j].y);
            return (float)(0.5 * a);
        }

        static bool IsConvex(Vector2 a, Vector2 b, Vector2 c)
        {
            float cross = (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
            return cross > 0f;
        }

        static double TriangleArea2(Vector2 a, Vector2 b, Vector2 c)
        {
            return System.Math.Abs((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x));
        }

        static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2 v0 = c - a, v1 = b - a, v2 = p - a;
            float d00 = Vector2.Dot(v0, v0);
            float d01 = Vector2.Dot(v0, v1);
            float d11 = Vector2.Dot(v1, v1);
            float d20 = Vector2.Dot(v2, v0);
            float d21 = Vector2.Dot(v2, v1);
            float denom = d00 * d11 - d01 * d01;
            if (Mathf.Abs(denom) < 1e-12f) return false;
            float v = (d11 * d20 - d01 * d21) / denom;
            float w = (d00 * d21 - d01 * d20) / denom;
            float u = 1f - v - w;
            return (u >= -1e-6f && v >= -1e-6f && w >= -1e-6f);
        }
    }

}