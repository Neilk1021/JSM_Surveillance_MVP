Shader "Hidden/Custom/PixelateURP"
{
    Properties
    {
        _PixelSize("Pixel Size", Float) = 4
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalRenderPipeline"
        }

        Pass
        {
            Name "Pixelate"
            ZTest Always
            ZWrite Off
            Cull Off
            Blend One Zero

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // For cmd.Blit-style blits, URP binds source as _MainTex
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _PixelSize;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            Varyings Vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 Frag (Varyings i) : SV_Target
            {
                float2 uv = i.uv;

                float2 screenPixels = _ScreenParams.xy;

                float2 blocks = screenPixels / _PixelSize;

                uv = floor(uv * blocks) / blocks;

                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                return col;
            }
            ENDHLSL
        }
    }
}
