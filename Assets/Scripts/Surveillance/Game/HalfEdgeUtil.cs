using System.Collections.Generic;
using JSM.Surveillance.Surveillance;
using UnityEngine;

namespace JSM.Surveillance.Data
{
public static class HalfEdgeUtil
    {
        public static bool PointInPolygon(Vector2 p, List<int> loop, List<HEGraphData.Vertex> vertices ,bool includeBoundary = true)
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

        private static bool OnSegment(Vector2 a, Vector2 b, Vector2 p)
        {
            float cross = Cross(b - a, p - a);
            if (Mathf.Abs(cross) > Mathf.Epsilon) return false;

            float dot = Vector2.Dot(p - a, b - a);
            if (dot < -Mathf.Epsilon) return false;
            float len2 = (b - a).sqrMagnitude;
            if (dot - len2 > Mathf.Epsilon) return false;

            return true;
        }
        public static bool PointInFaceAABB(List<int> loop, List<HEGraphData.Vertex> vertices, Vector2 p)
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

        public static int PickFace(Vector2 worldPoint, List<HEFace> faces, List<HEGraphData.Vertex> vertices, bool onlyInterior = true, bool includeBoundary = true)
        {
            int best = -1;
            float bestAbsArea = float.NegativeInfinity;
            for (int i = faces.Count-1; i >= 0; i--)
            {
                var f = faces[i];
                if (f.loop == null || f.loop.Count < 3) continue;
                if (onlyInterior && f.isExterior) continue;

                if (!PointInFaceAABB(f.loop, vertices, worldPoint)) continue;

                if (HalfEdgeUtil.PointInPolygon(worldPoint, f.loop, vertices, includeBoundary))
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
        
        private static float Cross(Vector2 u, Vector2 v) => u.x * v.y - u.y * v.x;
    }
}