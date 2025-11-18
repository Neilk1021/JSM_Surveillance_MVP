Shader "Custom/FovShader"
{
Properties
    {
        _Color ("Color", Color) = (1, 0, 1, 1) // bright magenta
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        Pass
        {
            Name "ForwardUnlit"
            // "SRPDefaultUnlit" works both in URP 3D and 2D renderers
            Tags { "LightMode"="SRPDefaultUnlit" }
            
            Stencil
            {
                Ref 1
                Comp NotEqual // <- ALWAYS passes (no masking yet)
                Pass Keep
            }

            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                return _Color;
            }

            ENDHLSL
        }
    }
}
