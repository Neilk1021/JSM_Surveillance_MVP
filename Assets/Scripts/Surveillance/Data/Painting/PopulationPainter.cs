using System;
using System.Collections.Generic;
using JSM.Surveillance.Surveillance;

namespace JSM.Surveillance.Data
{
    using UnityEditor;
    using UnityEngine;
    
    public static class PopulationPainter
    {
        public enum  PaintType
        {
            Population,
            Risk
        }

        const int PopulationDelta = 1;
        private const float RiskDelta = 0.01f;
        
        public static void PaintPopulation(HEGraphData data, Vector2 worldPoint, float radius, PaintType type, bool remove = false)
        {
            float radiusSqr = radius * radius;

            foreach (var face in data.faces)
            {
                if (face == null || face.loop == null || face.loop.Count < 3) 
                    continue;
                
                if(!face.data.isStreet || face.isExterior) continue;
                
                float distSqr = DistancePointToPolygonSqr(data, worldPoint, face.loop);
                if (distSqr <= radiusSqr)
                {
                    if (face.data == null) {
                        face.EnsureSOFromJson();
                    }

                    if (face.data == null) {
                        return;
                    }

                    switch (type)
                    {
                        case PaintType.Population:
                            face.data.dailyPopulation += PopulationDelta * (remove ? -1 : 1);
                            face.data.dailyPopulation = (int)Mathf.Clamp(face.data.dailyPopulation, 0, 100000000000000);
                            break;
                        case PaintType.Risk:
                            face.data.riskFactor += RiskDelta * (remove ? -1 : 1);
                            face.data.riskFactor = Mathf.Clamp01(face.data.riskFactor);
                            break;
                    }
                    
                    face.SyncDataJsonFromSO();
                    EditorUtility.SetDirty(face.data);
                    
                }
            }

            //EditorUtility.SetDirty(data);
        }

        static float DistancePointToSegmentSqr(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float denom = ab.sqrMagnitude;
            if (denom < Mathf.Epsilon)
                return (p - a).sqrMagnitude;

            float t = Vector2.Dot(p - a, ab) / denom;
            t = Mathf.Clamp01(t);
            Vector2 proj = a + t * ab;
            return (p - proj).sqrMagnitude;
        }

        static float DistancePointToPolygonSqr(HEGraphData data, Vector2 p, List<int> loop)
        {
            if (HEGraphDataUtil.PointInPolygon(data, p, loop)) 
                return 0f;

            float best = float.PositiveInfinity;

            for (int i = 0, j = loop.Count - 1; i < loop.Count; j = i++)
            {
                Vector2 a = data.verts[loop[j]].ij;
                Vector2 b = data.verts[loop[i]].ij;
                float d2 = DistancePointToSegmentSqr(p, a, b);
                if (d2 < best) best = d2;
            }

            return best;
        }
    }

}