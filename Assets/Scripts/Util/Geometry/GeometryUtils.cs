using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance.Util
{
    public static class GeometryUtils 
    {
        private static Vector2[] _clipBufferA = new Vector2[64];
        private static Vector2[] _clipBufferB = new Vector2[64];

        /// <summary>
        /// Calculates area % of a polygon overlapped by a Circular Sector (Pie Slice).
        /// </summary>
        /// <param name="center">Origin of the sector</param>
        /// <param name="radius">Radius of the arc</param>
        /// <param name="polyPoints">The polygon vertices</param>
        /// <param name="facingDir">Normalized direction the sector is facing</param>
        /// <param name="fovDegrees">Total field of view angle in degrees</param>
        public static float CalculateSectorPolygonOverlapPct(
            Vector2 center, 
            float radius, 
            IList<Vector2> polyPoints, 
            Vector2 facingDir, 
            float fovDegrees)
        {
            float polyArea = CalculatePolygonSignedArea(polyPoints);
            if (Mathf.Approximately(polyArea, 0f)) return 0f;
            polyArea = Mathf.Abs(polyArea); // Ensure positive for percentage calc

            float halfAngle = fovDegrees * 0.5f * Mathf.Deg2Rad;
            
           
            Vector2 leftNormal = RotateVector(facingDir, -halfAngle + (Mathf.PI / 2f));

            Vector2 rightNormal = RotateVector(facingDir, halfAngle - (Mathf.PI / 2f));

            int countA = ClipPolygonToPlane(polyPoints, polyPoints.Count, center, leftNormal, _clipBufferA);
            if (countA == 0) return 0f;

            int countB = ClipPolygonToPlane(_clipBufferA, countA, center, rightNormal, _clipBufferB);
            if (countB == 0) return 0f;

          
            
            float intersectionArea = CalculateCirclePolygonOverlapArea(center, radius, _clipBufferB, countB);

            return intersectionArea / polyArea;
        }


        private static Vector2 RotateVector(Vector2 v, float radians)
        {
            float c = Mathf.Cos(radians);
            float s = Mathf.Sin(radians);
            return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
        }

        private static int ClipPolygonToPlane(IList<Vector2> inputPoly, int inputCount, Vector2 planeOrigin, Vector2 planeNormal, Vector2[] outputPoly)
        {
            int outputCount = 0;
            
            if (inputCount == 0) return 0;

            Vector2 prevPoint = inputPoly[inputCount - 1];
            // Dot > 0 means "inside" (in front of normal)
            bool prevInside = Vector2.Dot(prevPoint - planeOrigin, planeNormal) >= 0;

            for (int i = 0; i < inputCount; i++)
            {
                Vector2 currPoint = inputPoly[i];
                bool currInside = (Vector2.Dot(currPoint - planeOrigin, planeNormal) >= 0);

                if (prevInside && currInside)
                {
                    outputPoly[outputCount++] = currPoint;
                }
                else if (prevInside && !currInside)
                {
                    outputPoly[outputCount++] = LinePlaneIntersection(prevPoint, currPoint, planeOrigin, planeNormal);
                }
                else if (!prevInside && currInside)
                {
                    outputPoly[outputCount++] = LinePlaneIntersection(prevPoint, currPoint, planeOrigin, planeNormal);
                    outputPoly[outputCount++] = currPoint;
                }

                prevPoint = currPoint;
                prevInside = currInside;
            }

            return outputCount;
        }

        private static Vector2 LinePlaneIntersection(Vector2 p1, Vector2 p2, Vector2 planeOrigin, Vector2 planeNormal)
        {
            Vector2 lineDir = p2 - p1;
            float denom = Vector2.Dot(lineDir, planeNormal);
            
            if (Mathf.Abs(denom) < Mathf.Epsilon) return p1; 

            Vector2 originToP1 = planeOrigin - p1;
            float t = Vector2.Dot(originToP1, planeNormal) / denom;
            
            return p1 + lineDir * t;
        }

        public static float CalculateCirclePolygonOverlapArea(Vector2 circleCenter, float radius, IList<Vector2> polyPoints, int pointCount)
        {
            float intersectionArea = 0f;
            
            for (int i = 0; i < pointCount; i++)
            {
                Vector2 p1 = polyPoints[i];
                Vector2 p2 = polyPoints[(i + 1) % pointCount]; // Wrap around using count
                intersectionArea += GetSignedCircleTriangleIntersectionArea(circleCenter, radius, p1, p2);
            }

            return Mathf.Abs(intersectionArea); // Return Area, not Pct
        }
        
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

