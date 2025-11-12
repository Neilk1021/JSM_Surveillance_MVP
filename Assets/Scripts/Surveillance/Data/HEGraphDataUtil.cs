using System;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance.Surveillance
{
    public class HEGraphDataUtil
    {
        public static int PickFace(HEGraphData data, Vector2 worldPoint, bool onlyInterior = true, bool includeBoundary = true)
        {
            int best = -1;
            float bestAbsArea = float.NegativeInfinity;
            for (int i = data.faces.Count-1; i >= 0; i--)
            {
                var f = data.faces[i];
                if (f.loop == null || f.loop.Count < 3) continue;
                if (onlyInterior && f.isExterior) continue;

                if (!PointInFaceAABB(data, f.loop, worldPoint)) continue;

                if (PointInPolygon(data, worldPoint, f.loop, includeBoundary))
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

        public static bool PointInPolygon(HEGraphData data, Vector2 p, List<int> loop, bool includeBoundary = true)
        {
            bool inside = false;

            for (int i = 0, j = loop.Count - 1; i < loop.Count; j = i++)
            {
                Vector2 a = data.verts[loop[j]].ij;
                Vector2 b = data.verts[loop[i]].ij;

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

        static bool PointInFaceAABB(HEGraphData data, List<int> loop, Vector2 p)
        {
            float minX = float.PositiveInfinity, minY = float.PositiveInfinity;
            float maxX = float.NegativeInfinity, maxY = float.NegativeInfinity;

            for (int i = 0; i < loop.Count; i++)
            {
                var v = data.verts[loop[i]].ij;
                if (v.x < minX) minX = v.x;
                if (v.y < minY) minY = v.y;
                if (v.x > maxX) maxX = v.x;
                if (v.y > maxY) maxY = v.y;
            }

            return p.x >= minX - Mathf.Epsilon && p.x <= maxX + Mathf.Epsilon && p.y >= minY - Mathf.Epsilon && p.y <= maxY + Mathf.Epsilon;
        }

        public static float SignedArea(HEGraphData data, List<int> loop)
        {
            double sum = 0;
            for (int i = 0, j = loop.Count - 1; i < loop.Count; j = i++)
            {
                var a = data.verts[loop[j]].ij;
                var b = data.verts[loop[i]].ij;
                sum += (double)a.x * b.y - (double)a.y * b.x;
            }
            return (float)(0.5 * sum);
        }
        
        static float AngleCCW(Vector2 from, Vector2 to)
        {
            if (from.sqrMagnitude == 0 || to.sqrMagnitude == 0) return float.NaN;

            float cross = from.x * to.y - from.y * to.x;
            float dot   = Vector2.Dot(from, to);
            float a = Mathf.Atan2(cross, dot);     // [-π, π]
            if (a < 0) a += 2f * Mathf.PI;         // [0, 2π)
            return a;
        }

        public static HEGraph RebuildFaces(List<HEGraphData.Vertex> verts, List<HEGraphData.Edge> edges)
        {
            var halfEdges = new List<HEHalf>();
            var faces     = new List<HEFace>();

            if (verts == null || edges == null || verts.Count == 0 || edges.Count == 0)
                return new HEGraph(halfEdges, faces);

            var outgoing = new List<int>[verts.Count];
            for (int v = 0; v < verts.Count; v++) outgoing[v] = new List<int>();

            for (int i = 0; i < edges.Count; i++)
            {
                var e = edges[i];
                if (e.a == e.b) continue;

                int hAB = halfEdges.Count;
                halfEdges.Add(new HEHalf { origin = e.a, dst = e.b, twin = hAB + 1, next = -1, prev = -1, face = -1 });

                int hBA = halfEdges.Count;
                halfEdges.Add(new HEHalf { origin = e.b, dst = e.a, twin = hAB,     next = -1, prev = -1, face = -1 });

                outgoing[e.a].Add(hAB);
                outgoing[e.b].Add(hBA);
            }

        
            for (int h = 0; h < halfEdges.Count; h++)
            {
                int v = halfEdges[h].dst;
                int u = halfEdges[h].origin;

                Vector2 incoming = verts[v].ij - verts[u].ij;      
                Vector2 from     = -incoming;                        

                float bestAngle = float.PositiveInfinity;
                int bestEdge = -1;

                var atV = outgoing[v];
                for (int k = 0; k < atV.Count; k++)
                {
                    int e = atV[k];
                    if (e == halfEdges[h].twin) continue;           

                    Vector2 to = verts[halfEdges[e].dst].ij - verts[v].ij;
                    float a = AngleCCW(from, to);                     
                    if (a < bestAngle)
                    {
                        bestAngle = a;
                        bestEdge = e;
                    }
                }

                halfEdges[h].next = bestEdge;                       
                if (bestEdge != -1) halfEdges[bestEdge].prev = h;
            }

            var visited = new HashSet<int>();

            for (int hStart = 0; hStart < halfEdges.Count; hStart++)
            {
                if (visited.Contains(hStart)) continue;
                if (halfEdges[hStart].next == -1) continue;

                int h = hStart;
                var cycle = new List<int>();          // half-edge indices in the loop
                int guard = 0;

                while (h != -1 && !visited.Contains(h) && guard < halfEdges.Count * 2)
                {
                    cycle.Add(h);
                    visited.Add(h);
                    h = halfEdges[h].next;
                }

                if (h != hStart) continue;
                if (cycle.Count < 3) continue;

                var loopVerts = new List<int>();
                for (int i = 0; i < cycle.Count; i++)
                    loopVerts.Add(halfEdges[cycle[i]].origin);

                float area2 = 0f;
                for (int i = 0; i < loopVerts.Count; i++)
                {
                    var a = verts[loopVerts[i]].ij;
                    var b = verts[loopVerts[(i + 1) % loopVerts.Count]].ij;
                    area2 += a.x * b.y - a.y * b.x;
                }
                float area = 0.5f * area2;

                bool isExterior = area < 0f;

                int faceIndex = faces.Count;
                faces.Add(new HEFace
                {
                    halfEdge  = hStart,
                    color     = new Color(0.2f, 0.2f, 1, 0.4f),
                    area      = Mathf.Abs(area),
                    isExterior= !isExterior,
                    loop      = loopVerts
                });

                foreach (int he in cycle)
                    halfEdges[he].face = faceIndex;
            }
            return new HEGraph(halfEdges, faces);
        }
    }
}
