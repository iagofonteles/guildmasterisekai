Shader "Custom/TilemapHueMask"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        [PerRendererData] _MaskTex ("Hue Mask", 2D) = "black" {}
        _MaskColorR ("Mask Color (Vertex R)", Color) = (1, 0, 0, 1)
        _MaskColorG ("Mask Color (Vertex G)", Color) = (0, 1, 0, 1)
        _MaskColorB ("Mask Color (Vertex B)", Color) = (0, 0, 1, 1)
        _MaskColorA ("Mask Color (Vertex A)", Color) = (1, 1, 1, 1)
        _MaskTolerance ("Mask Color Tolerance", Range(0.001, 1)) = 0.1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "CanUseSpriteAtlas" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Tags { "LightMode" = "SRPDefaultUnlit" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MaskColorR;
                float4 _MaskColorG;
                float4 _MaskColorB;
                float4 _MaskColorA;
                float _MaskTolerance;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 shiftSin : TEXCOORD1;
                float4 shiftCos : TEXCOORD2;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                // Each vertex channel controls one mask: 0 is neutral,
                // and 0..1 spans 0..360 degrees. Alpha is a shift, not opacity.
                sincos(v.color * TWO_PI, o.shiftSin, o.shiftCos);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float3 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv).rgb;
                float3 deltaR = mask - _MaskColorR.rgb;
                float3 deltaG = mask - _MaskColorG.rgb;
                float3 deltaB = mask - _MaskColorB.rgb;
                float3 deltaA = mask - _MaskColorA.rgb;
                float4 distanceSquared = float4(
                    dot(deltaR, deltaR), dot(deltaG, deltaG),
                    dot(deltaB, deltaB), dot(deltaA, deltaA));
                float4 regions = 1.0 - step(_MaskTolerance * _MaskTolerance, distanceSquared);
                // Normalize overlapping matches; unmatched pixels keep their original color.
                regions /= max(1.0, dot(regions, float4(1, 1, 1, 1)));
                float coverage = dot(regions, float4(1, 1, 1, 1));
                float shiftSin = dot(regions, i.shiftSin);
                float shiftCos = 1.0 - coverage + dot(regions, i.shiftCos);

                // Rodrigues hue rotation matrix around the neutral RGB axis.
                // Factored matrix multiplication avoids per-pixel trigonometry and HSV conversion.
                const float3 axis = float3(0.577350269, 0.577350269, 0.577350269);
                col.rgb = saturate(col.rgb * shiftCos
                    + cross(axis, col.rgb) * shiftSin
                    + axis * dot(axis, col.rgb) * (1.0 - shiftCos));
                return col;
            }
            ENDHLSL
        }
    }
}
