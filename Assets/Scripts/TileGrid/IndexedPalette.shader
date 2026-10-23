Shader "Custom/IndexedPalette"
{
    // URP 2D lighting. Clear paint (< .001 per channel) draws the original sprite.
    // Otherwise RGBA selects four palette ramp IDs; no separate ramp texture.
    //
    // Encoded R byte = region * 8 + shade slot (0..4; slots 5..7 clamp to 4).
    // Encoded G byte = original palette index (0..63) + 64 when opaque.
    // Regions 1..4 recolor; region 0 and ramp ID 0 preserve the original index.
    // Default ramp IDs 1..13 select consecutive five-color groups in the 64-color palette.
    // ColorPaletteConfig sets _PaletteColorCount and _PaletteShades on shared materials.
    // The final ramp's fifth shade repeats index 63, since it has only four colors.
    //
    // Palette layout: 64x1, or 5x13 in texture row order (bottom row first).
    // Palette is sRGB; encoded sprites are linear, Point, uncompressed, no mipmaps.
    // Keep Tilemap tint white. Paint alpha selects region 4; it is not opacity.
    Properties
    {
        [PerRendererData] _MainTex ("Original or Encoded Sprite", 2D) = "white" {}
        [NoScaleOffset] _PaletteTex ("Palette (13 Ramps, 5 Shades)", 2D) = "white" {}
        [NoScaleOffset] _NormalMap ("Normal Map", 2D) = "bump" {}
        [HideInInspector] _PaletteColorCount ("Palette Color Count", Float) = 64
        [HideInInspector] _PaletteShades ("Palette Shades Per Ramp", Float) = 5
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
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_PaletteTex);
        TEXTURE2D(_NormalMap);
        SAMPLER(sampler_NormalMap);

        CBUFFER_START(UnityPerMaterial)
            float4 _PaletteTex_TexelSize;
            float _PaletteColorCount;
            float _PaletteShades;
        CBUFFER_END

        struct Attributes
        {
            float3 positionOS : POSITION;
            float2 uv : TEXCOORD0;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 color : COLOR;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            float2 lightingUV : TEXCOORD1;
            nointerpolation uint4 rampIds : TEXCOORD2;
            nointerpolation uint original : TEXCOORD3;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };

        float4 GetPaintColor(float4 vertexColor)
        {
            // BuildColor compensates before SetColor so Chunk mode can retain
            // exact byte IDs. Converting again here would apply it twice.
            return vertexColor * unity_SpriteColor;
        }

        Varyings vert(Attributes v)
        {
            Varyings o = (Varyings)0;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_TRANSFER_INSTANCE_ID(v, o);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            SetUpSpriteInstanceProperties();
            v.positionOS = UnityFlipSprite(v.positionOS, unity_SpriteProps.xy);
            o.positionCS = TransformObjectToHClip(v.positionOS);
            o.uv = v.uv;
            o.lightingUV = ComputeScreenPos(o.positionCS / o.positionCS.w).xy;
            float4 paint = GetPaintColor(v.color);
            o.original = all(abs(paint) < .001) ? 1u : 0u;
            o.rampIds = (uint4)floor(saturate(paint) * 255.0 + .5);
            return o;
        }

        half4 SamplePaint(float2 uv, uint4 rampIds, uint original)
        {
            float4 texel = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
            if (original != 0u) return texel;

            uint2 data = (uint2)floor(texel.rg * 255.0 + .5);
            uint region = data.r >> 3;
            uint shades = (uint)clamp(_PaletteShades, 1.0, 8.0);
            uint colorCount = (uint)clamp(_PaletteColorCount, 1.0, 64.0);
            uint shade = min(data.r & 7u, shades - 1u);
            uint paletteIndex = data.g & 63u;
            half opacity = (half)((data.g >> 6) & 1u);
            if (region >= 1u && region <= 4u)
            {
                uint ramp = min(rampIds[region - 1u], (colorCount + shades - 1u) / shades);
                if (ramp != 0u)
                    paletteIndex = min((ramp - 1u) * shades + shade, colorCount - 1u);
            }
            paletteIndex = min(paletteIndex, colorCount - 1u);
            uint width = (uint)_PaletteTex_TexelSize.z;
            half3 rgb = LOAD_TEXTURE2D(_PaletteTex,
                int2(paletteIndex % width, paletteIndex / width)).rgb;
            return half4(rgb, opacity);
        }
        ENDHLSL

        Pass
        {
            Name "Lit"
            Tags { "LightMode" = "Universal2D" }
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightShared.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            half4 frag(Varyings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                half4 color = SamplePaint(i.uv, i.rampIds, i.original);
                SurfaceData2D surfaceData;
                InputData2D inputData;
                InitializeSurfaceData(color.rgb, color.a, surfaceData);
                InitializeInputData(i.uv, i.lightingUV, inputData);
                return CombinedShapeLightShared(surfaceData, inputData);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Normals"
            Tags { "LightMode" = "NormalsRendering" }
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex normalsVert
            #pragma fragment normalsFrag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"

            struct NormalVaryings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
                half3 tangentWS : TEXCOORD2;
                half3 bitangentWS : TEXCOORD3;
                nointerpolation uint4 rampIds : TEXCOORD4;
                nointerpolation uint original : TEXCOORD5;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            NormalVaryings normalsVert(Attributes v)
            {
                NormalVaryings o = (NormalVaryings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                SetUpSpriteInstanceProperties();
                v.positionOS = UnityFlipSprite(v.positionOS, unity_SpriteProps.xy);
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                o.normalWS = TransformObjectToWorldDir(v.normalOS);
                o.tangentWS = TransformObjectToWorldDir(v.tangentOS.xyz);
                o.bitangentWS = cross(o.normalWS, o.tangentWS) * v.tangentOS.w;
                float4 paint = GetPaintColor(v.color);
                o.original = all(abs(paint) < .001) ? 1u : 0u;
                o.rampIds = (uint4)floor(saturate(paint) * 255.0 + .5);
                return o;
            }

            half4 normalsFrag(NormalVaryings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                SetUpSpriteInstanceProperties();
                half4 color = SamplePaint(i.uv, i.rampIds, i.original);
                half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv));
                return NormalsRenderingShared(color, normalTS, i.tangentWS, i.bitangentWS, i.normalWS);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment forwardFrag
            #pragma multi_compile_instancing

            half4 forwardFrag(Varyings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                return SamplePaint(i.uv, i.rampIds, i.original);
            }
            ENDHLSL
        }
    }
}
