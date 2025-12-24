Shader "Unlit/PolygonEdgeFactor"
{
 Properties
    {
        [Header(Gradient Settings)]
        _EdgeColor ("Edge Color", Color) = (0.1,0.2,0.5,1)
        _CenterColor ("Center Color", Color) = (0.00, 0.00, .02, 1)
        _GradientPower ("Gradient Power", Range(0.1, 5.0)) = 3.0

        [Header(Border Settings)]
        _BorderColor ("Border Color", Color) = (0.05,0.25,0.35,1)
        _BorderSize ("Border Size (World Units)", Range(0.0, 2.0)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100
        
        Pass
        {
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }
            
            ZWrite On
            ZTest LEqual
            
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 vertex : SV_POSITION; float3 worldPos : TEXCOORD0; };

            int _PointsCount;
            float4 _Points[100];
            float _MaxDist;
            
            fixed4 _EdgeColor, _CenterColor, _BorderColor;
            float _GradientPower, _BorderSize;

            float DistPointToSeg(float2 p, float2 a, float2 b)
            {
                float2 ab = b - a;
                float t = dot(p - a, ab) / max(dot(ab, ab), 1e-12);
                t = saturate(t);
                return distance(a + t * ab, p);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float min_dist = 100000.0;
                float2 p = i.worldPos.xy;

                for (int j = 0; j < _PointsCount; j++)
                {
                    float2 a = _Points[j].xy;
                    float2 b = _Points[(j + 1) % _PointsCount].xy;
                    min_dist = min(min_dist, DistPointToSeg(p, a, b));
                }

                // Gradient calculation remains the same
                float normalized_dist = saturate(min_dist / _MaxDist);
                float gradientFactor = pow(normalized_dist, _GradientPower);
                fixed4 gradientColor = lerp(_EdgeColor, _CenterColor, gradientFactor);

                float border_thickness_in_world = _BorderSize;

                float anti_alias_width = fwidth(min_dist);
                
                float borderFactor = smoothstep(
                    border_thickness_in_world - anti_alias_width, 
                    border_thickness_in_world, 
                    min_dist
                );


                fixed4 finalColor = lerp(_BorderColor, gradientColor, borderFactor);

                return finalColor;
            }
            ENDCG
        }
    }
}