using System;
using JSM.Surveillance.Data;

namespace JSM.Surveillance.Surveillance
{
    using System.Collections.Generic;
    using UnityEngine;
    
    
    [System.Serializable]
    public class HEGraphData
    {
        [System.Serializable]
        public struct Vertex
        {
            public Vector2 ij;

            public Vertex(float i, float j)
            {
                ij = new Vector2(i,j); 
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(ij.x, ij.y);
            }
        }
        
        [System.Serializable]
        public struct Edge   { public int a,b; public Edge(int a,int b){ this.a=a; this.b=b; } }

        private int _cols = 50;
        private int _rows = 50;
        public void MoveVertexBy(int vertex, Vector2 delta)
        {
            (float x, float y) = ((verts[vertex].ij + delta).x, (verts[vertex].ij + delta).y);

            x = Mathf.Clamp(x, 0, _cols);
            y = Mathf.Clamp(y, 0, _rows);
            verts[vertex] = new Vertex(x, y);
        }
        
        public List<Vertex> verts = new();
        public List<Edge>   edges = new();

        public List<HEHalf> halfEdges = new List<HEHalf>();
        public List<HEFace> faces = new List<HEFace>();
        
        public void Clear() { verts.Clear(); edges.Clear(); }

        private float EdgeEpsilon = 1e-4f;
        
        public HashSet<int> pinned = new();

        private int FindVertexByGrid(Vector2 ij)
        {
            for (int i = 0; i < verts.Count; i++)
                if (verts[i].ij == ij) return i;
            return -1;
        }

        bool PointInFace(HEFace face, Vector2 point)
        {
            return false;
        }
        
        int PickFace(List<HEFace> faces, Vector2 mousePos)
        {
            for (int i = 0; i < faces.Count; i++)
            {
                if (PointInFace(faces[i], mousePos))
                {
                    return i;
                }
            }
            
            return -1;
        }
        private int EnsureVertex(Vector2 ij, bool pin = false)
        {
            int i = FindVertexByGrid(ij);
            if (i < 0) { verts.Add(new Vertex(ij.x, ij.y)); i = verts.Count - 1; }
            if (pin) pinned.Add(i);
            return i;
        }

        private void EnsureEdge(int a, int b)
        {
            if (a == b) return;
            for (int i=0;i<edges.Count;i++)
            {
                var e = edges[i];
                if ((e.a == a && e.b == b) || (e.a == b && e.b == a)) return;
            }
            edges.Add(new Edge(a,b));
        }
        
        public void EnsureCornerVertices(int cols, int rows, bool pin = true)
        {
            _cols = cols;
            _rows = rows;
            
            var c0 = EnsureVertex(new Vector2Int(0, 0), pin);
            var c1 = EnsureVertex(new Vector2Int(cols - 1, 0), pin);
            var c2 = EnsureVertex(new Vector2Int(cols - 1, rows - 1), pin);
            var c3 = EnsureVertex(new Vector2Int(0, rows - 1), pin);
            
            EnsureEdge(c0, c1);
            EnsureEdge(c1, c2);
            EnsureEdge(c2, c3);
            EnsureEdge(c3, c0);
            
            if (pin) { pinned.Add(c0); pinned.Add(c1); pinned.Add(c2); pinned.Add(c3); }
            RebuildFaces();
        }

        public void RebuildFaces()
        {
            var graph = HEGraphDataUtil.RebuildFaces(verts, edges);
            halfEdges = graph.HalfEdges;
            faces = graph.Faces;
        }
        
        public void RemoveVertex(int v)
        {
            if (pinned.Contains(v)) return;  
            for (int i = edges.Count - 1; i >= 0; i--)
                if (edges[i].a == v || edges[i].b == v) edges.RemoveAt(i);

            verts.RemoveAt(v);

            var newPinned = new System.Collections.Generic.HashSet<int>();
            foreach (var p in pinned)
            {
                if      (p < v) newPinned.Add(p);
                else if (p > v) newPinned.Add(p - 1);
            }
            pinned.Clear(); foreach (var p in newPinned) pinned.Add(p);

            for (int i = 0; i < edges.Count; i++)
            {
                var e = edges[i];
                if (e.a > v) e.a--;
                if (e.b > v) e.b--;
                edges[i] = e;
            }
        }

        private int RemoveEdgesAlongSegment(Vector2 A, Vector2 B, float eps = 1e-4f)
        {
            int removed = 0;
            for (int i = edges.Count - 1; i >= 0; i--)
            {
                var e = edges[i];
                var P = verts[e.a].ij;
                var Q = verts[e.b].ij;
                if (PointOnSegment(P, A, B, eps) && PointOnSegment(Q, A, B, eps))
                {
                    edges.RemoveAt(i);
                    removed++;
                }
            }
            return removed;
        }

        static bool PointOnSegment(Vector2 P, Vector2 A, Vector2 B, float eps)
        {
            var AB = B - A;
            var AP = P - A;

            float area2 = Mathf.Abs(AB.x * AP.y - AB.y * AP.x);
            if (area2 > eps * (AB.magnitude + 1f)) return false;

            float t = Vector2.Dot(AP, AB) / (AB.sqrMagnitude + 1e-12f);
            return t >= -eps && t <= 1f + eps;
        }
        
        public int[] SubdivideQuadReplacePerimeter(
            HEGraphData g,
            int i0, int i1, int i2, int i3,      // indices of the 4 selected vertices (any order)
            bool addInteriorEdges = true, bool pinNew = false)
        {
            // 1) Order the 4 points CCW as p00, p10, p11, p01
            var p0 = g.verts[i0]; var p1 = g.verts[i1];
            var p2 = g.verts[i2]; var p3 = g.verts[i3];

            OrderQuadCCW(p0.ij, p1.ij, p2.ij, p3.ij, out var p00, out var p10, out var p11, out var p01);

            int idx00 = NearestIndex(g, p00);
            int idx10 = NearestIndex(g, p10);
            int idx11 = NearestIndex(g, p11);
            int idx01 = NearestIndex(g, p01);

            g.RemoveEdgesAlongSegment(p00, p10, g.EdgeEpsilon);
            g.RemoveEdgesAlongSegment(p10, p11, g.EdgeEpsilon);
            g.RemoveEdgesAlongSegment(p11, p01, g.EdgeEpsilon);
            g.RemoveEdgesAlongSegment(p01, p00, g.EdgeEpsilon);

            // (optional) remove diagonals if your mesh stored them
            g.RemoveEdgesAlongSegment(p00, p11, g.EdgeEpsilon);
            g.RemoveEdgesAlongSegment(p10, p01, g.EdgeEpsilon);

            int[] nine = SubdivideQuadBilinear(g, p00, p10, p11, p01, addInteriorEdges, pinNew);


            return nine;
        }


        static int NearestIndex(HEGraphData g, Vector2 p)
        {
            int best = -1; float bestD2 = float.PositiveInfinity;
            for (int i = 0; i < g.verts.Count; i++)
            {
                float d2 = (g.verts[i].ij - p).sqrMagnitude;
                if (d2 < bestD2) { bestD2 = d2; best = i; }
            }
            return best;
        }

        private static void OrderQuadCCW(
            Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3,
            out Vector2 p00, out Vector2 p10, out Vector2 p11, out Vector2 p01)
        {
            // centroid + angle sort
            var arr = new System.Collections.Generic.List<Vector2> { v0, v1, v2, v3 };
            Vector2 c = (v0 + v1 + v2 + v3) * 0.25f;
            arr.Sort((a,b) =>
            {
                float aa = Mathf.Atan2(a.y - c.y, a.x - c.x);
                float bb = Mathf.Atan2(b.y - c.y, b.x - c.x);
                return aa.CompareTo(bb);
            });
            // ensure CCW by signed area
            float area2 = 0f;
            for (int i = 0; i < 4; i++)
            {
                var a = arr[i]; var b = arr[(i + 1) % 4];
                area2 += a.x * b.y - b.x * a.y;
            }
            if (area2 < 0f) arr.Reverse();

            // start from “bottom-left” (min x+y)
            int start = 0; float best = float.PositiveInfinity;
            for (int i = 0; i < 4; i++)
            {
                float s = arr[i].x + arr[i].y;
                if (s < best) { best = s; start = i; }
            }
            p00 = arr[(start + 0) % 4];
            p10 = arr[(start + 1) % 4];
            p11 = arr[(start + 2) % 4];
            p01 = arr[(start + 3) % 4];
        }

        // your bilinear subdivision from earlier (unchanged):
        private static int[] SubdivideQuadBilinear(HEGraphData g, Vector2 p00, Vector2 p10, Vector2 p11, Vector2 p01,
                                                  bool addInteriorEdges = true, bool pinNew = false)
        {
            static Vector2 BL(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float u, float v)
            {
                float omu = 1f - u, omv = 1f - v;
                return (omu*omv)*a + (u*omv)*b + (u*v)*c + (omu*v)*d;
            }
            float[] t = { 0f, 0.5f, 1f };
            var pts = new Vector2[9];
            int k = 0;
            for (int j=0;j<3;j++)
                for (int i=0;i<3;i++)
                    pts[k++] = BL(p00, p10, p11, p01, t[i], t[j]);

            var idx = new int[9];
            for (int i=0;i<9;i++) idx[i] = g.EnsureVertex(pts[i], pinNew);

            if (addInteriorEdges)
            {
                for (int row=0; row<3; row++) { int r=row*3; g.EnsureEdge(idx[r+0], idx[r+1]); g.EnsureEdge(idx[r+1], idx[r+2]); }
                for (int col=0; col<3; col++) { g.EnsureEdge(idx[0+col], idx[3+col]); g.EnsureEdge(idx[3+col], idx[6+col]); }
            }
            else
            {
                g.EnsureEdge(idx[0], idx[2]); g.EnsureEdge(idx[6], idx[8]); g.EnsureEdge(idx[0], idx[6]); g.EnsureEdge(idx[2], idx[8]);
            }
            return idx;
        }

        public void SetFaceLabel(int faceIndex, string label) { if (faceIndex >= 0 && faceIndex < faces.Count) faces[faceIndex].label = label; }
        public void SetFaceColor(int faceIndex, Color c)     { if (faceIndex >= 0 && faceIndex < faces.Count) faces[faceIndex].color = c; }
        public void SetFaceMeta(int faceIndex, string k, string v)
        {
            if (faceIndex >= 0 && faceIndex < faces.Count) faces[faceIndex].meta[k] = v;
        } 
    }
}