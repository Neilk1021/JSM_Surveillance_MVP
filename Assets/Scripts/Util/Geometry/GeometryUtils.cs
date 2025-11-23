using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance.Util
{
    public static class GeometryUtils 
    {
        public static float CalculateCirclePolygonOverlapPct(Vector2 circleCenter, float radius, IList<Vector2> polyPoints)
        {
            float polyArea = CalculatePolygonSignedArea(polyPoints);
            if (Mathf.Approximately(polyArea, 0f)) return 0f;

            float intersectionArea = 0f;
            int count = polyPoints.Count;

            for (int i = 0; i < count; i++)
            {
                Vector2 p1 = polyPoints[i];
                Vector2 p2 = polyPoints[(i + 1) % count];

                intersectionArea += GetSignedCircleTriangleIntersectionArea(circleCenter, radius, p1, p2);
            }

            return Mathf.Abs(intersectionArea / polyArea);
        }

        public static float CalculatePolygonSignedArea(IList<Vector2> points)
        {
            float area = 0f;
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 p1 = points[i];
                Vector2 p2 = points[(i + 1) % points.Count];
                area += (p1.x * p2.y) - (p1.y * p2.x);
            }
            return area * 0.5f;
        }
        
        private static float GetSignedCircleTriangleIntersectionArea(Vector2 center, float r, Vector2 p1, Vector2 p2)
        {
            Vector2 A = p1 - center;
            Vector2 B = p2 - center;

            float rSq = r * r;
            float magA_sq = A.sqrMagnitude;
            float magB_sq = B.sqrMagnitude;
            
            if (magA_sq <= rSq && magB_sq <= rSq)
            {
                return 0.5f * ((A.x * B.y) - (A.y * B.x));
            }

            Vector2 d = B - A;
            float a = d.sqrMagnitude;
            float b = 2 * Vector2.Dot(A, d);
            float c = magA_sq - rSq;
            
            float det = b * b - 4 * a * c;

            List<float> tValues = new List<float>(); 
            tValues.Add(0f);
            tValues.Add(1f);

            if (det > 0)
            {
                float sqrtDet = Mathf.Sqrt(det);
                float t1 = (-b - sqrtDet) / (2 * a);
                float t2 = (-b + sqrtDet) / (2 * a);

                if (t1 > 0f && t1 < 1f) tValues.Add(t1);
                if (t2 > 0f && t2 < 1f) tValues.Add(t2);
            }

            tValues.Sort();

            float totalSignedArea = 0f;

            for (int i = 0; i < tValues.Count - 1; i++)
            {
                float tStart = tValues[i];
                float tEnd = tValues[i + 1];
                
                float tMid = (tStart + tEnd) * 0.5f;
                Vector2 midPt = A + d * tMid;

                Vector2 segStart = A + d * tStart;
                Vector2 segEnd = A + d * tEnd;

                if (midPt.sqrMagnitude <= rSq + Mathf.Epsilon)
                {
                    totalSignedArea += 0.5f * ((segStart.x * segEnd.y) - (segStart.y * segEnd.x));
                }
                else 
                {
                    float angle1 = Mathf.Atan2(segStart.y, segStart.x);
                    float angle2 = Mathf.Atan2(segEnd.y, segEnd.x);
                    
                    float theta = angle2 - angle1;
                    
                    if (theta <= -Mathf.PI) theta += 2 * Mathf.PI;
                    else if (theta > Mathf.PI) theta -= 2 * Mathf.PI;

                    totalSignedArea += 0.5f * rSq * theta;
                }
            }

            return totalSignedArea;
        }
    }   
}

