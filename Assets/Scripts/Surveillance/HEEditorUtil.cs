using UnityEngine;

namespace JSM.Surveillance.Surveillance
{
    public class HEEditorUtil
    {
        static float PolygonAreaCCW(in System.ReadOnlySpan<Vector2> poly)
        {
            double a = 0;
            for (int i=0;i<poly.Length;i++)
            {
                var p = poly[i];
                var q = poly[(i+1)%poly.Length];
                a += p.x * q.y - q.x * p.y;
            }
            return (float)(0.5 * a); // >0 means CCW
        }

        public static void OrderQuadCCW(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3,
            out Vector2 p00, out Vector2 p10, out Vector2 p11, out Vector2 p01)
        {
            var arr = new System.Collections.Generic.List<Vector2> { v0, v1, v2, v3 };

            // centroid
            Vector2 c = (v0 + v1 + v2 + v3) * 0.25f;
            arr.Sort((a,b) =>
            {
                float aa = Mathf.Atan2(a.y - c.y, a.x - c.x);
                float bb = Mathf.Atan2(b.y - c.y, b.x - c.x);
                return aa.CompareTo(bb);
            });

            // ensure CCW
            var span = arr.ToArray(); 
            if (PolygonAreaCCW(span) < 0f) arr.Reverse();

            // rotate so we start at the visual "bottom-left" (min x+y)
            int start = 0; float best = float.PositiveInfinity;
            for (int i=0;i<4;i++)
            {
                float s = arr[i].x + arr[i].y;
                if (s < best) { best = s; start = i; }
            }
            Vector2 q0 = arr[(start+0)%4];
            Vector2 q1 = arr[(start+1)%4];
            Vector2 q2 = arr[(start+2)%4];
            Vector2 q3 = arr[(start+3)%4];

            // map CCW: q0->p00, then around
            p00 = q0; p10 = q1; p11 = q2; p01 = q3;
        } 
    }
}