Shader "Custom/TilemapHueMask"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        [PerRendererData] _MaskTex ("Hue Mask", 2D) = "black" {}
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

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

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
                float4 color : COLOR;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float3 RGBToHSV(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

                float d = q.x - min(q.w, q.y);
                float e = 1e-10;

                return float3(
                    abs(q.z + (q.w - q.y) / (6.0 * d + e)),
                    d / (q.x + e),
                    q.x
                );
            }

            float3 HSVToRGB(float3 c)
            {
                float3 p = abs(frac(c.xxx + float3(0.0, 2.0 / 3.0, 1.0 / 3.0)) * 6.0 - 3.0);
                return c.z * lerp(1.0.xxx, saturate(p - 1.0), c.y);
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half3 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv).rgb;

                // Decode mask regions
                float3 m = step(0.5, mask);

                float4 regions = float4(
                    m.r * (1.0 - m.g) * (1.0 - m.b), // Red
                    m.g * (1.0 - m.r) * (1.0 - m.b), // Green
                    m.b * (1.0 - m.r) * (1.0 - m.g), // Blue
                    m.r * m.g * m.b                 // White
                );

                // Vertex color: 0.5 = neutral
                float4 shifts = i.color - 0.5;

                float hueShift = dot(regions, shifts);

                float3 hsv = RGBToHSV(col.rgb);
                hsv.x = frac(hsv.x + hueShift);

                col.rgb = HSVToRGB(hsv);

                return col;
            }
            ENDHLSL
        }
    }
}
