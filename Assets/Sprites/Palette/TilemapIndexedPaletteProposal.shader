Shader "Custom/TilemapIndexedPaletteProposal"
{
    // Proposal: requires an Editor baker and indexed sprites; original RGB sprites
    // cannot be rendered with this shader directly.
    //
    // _MainTex: packed RG8 / Unity TextureFormat.RG16 data, two bytes per pixel.
    // R byte = region * 8 + shadeSlot. Region 0 is fixed; regions 1..4 recolor.
    //          shadeSlot 0..7 supports eight shades (including the five-color region).
    // G byte = originalPaletteIndex + (opaque ? 64 : 0).
    //          Original palette index is 0..63. Transparency is binary.
    // Store byte values divided by 255, NOT divided by the field's maximum.
    // RGBA32 can carry the same encoding in RG as a format fallback; BA are unused.
    //
    // _RampTex: 8 x 256 linear data texture. X = shade slot, Y = ramp ID.
    // R byte = output palette index 0..63, encoded as index / 255.
    // Ramp ID 0 means use the pixel's original index; rows 1..255 are selectable.
    //
    // _PaletteTex: 64 x 1 color texture, one exact palette color per texel.
    // Import as sRGB color in a Linear project; the data textures must be linear.
    // ALL textures: Point, Clamp, no mipmaps, no lossy compression or resizing.
    //
    // Vertex RGBA carries four independent ramp IDs / 255 for regions 1..4.
    // Example: Color32(3, 14, 26, 12) selects four ramp ROWS, not palette colors.
    // Color.clear restores original indexed colors. Vertex alpha is NOT opacity.
    // Keep Tilemap color white, GameObject tint neutral, and LockColor disabled.
    // A preset stores four ramp IDs; each shared ramp stores eight palette indices.
    // Fill unused ramp slots explicitly when authoring a four-shade ramp.
    //
    // Generate Full Rect sprites with the original rectangles, pivots and PPU.
    // Automatic alpha mesh generation cannot infer opacity from the packed G bit.
    // Atlas packing must preserve data bytes and use a linear, uncompressed atlas.
    // Do not mix these sprites into an ordinary sRGB sprite atlas.
    // Defaults are placeholders until generated textures are assigned.
    // Palette indices/order must remain stable for saved ramp IDs to stay valid.
    // Blending/post-processing can produce screen colors outside the palette.
    Properties
    {
        [PerRendererData] _MainTex ("Packed Region / Shade / Original / Opacity", 2D) = "black" {}
        [NoScaleOffset] _RampTex ("Shade Ramps (8 x 256)", 2D) = "black" {}
        [NoScaleOffset] _PaletteTex ("Palette (64 x 1)", 2D) = "white" {}
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
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Tags { "LightMode" = "SRPDefaultUnlit" }

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_RampTex);
            TEXTURE2D(_PaletteTex);

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
                nointerpolation uint4 rampIds : TEXCOORD1;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.rampIds = (uint4)floor(saturate(v.color) * 255.0 + 0.5);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                uint2 data = (uint2)floor(
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).rg * 255.0 + 0.5);
                uint region = data.r >> 3;
                uint shadeSlot = data.r & 7u;
                uint paletteIndex = data.g & 63u;
                half opacity = (half)((data.g >> 6) & 1u);
                clip(opacity - 0.5h);

                // A fixed pixel or ramp 0 preserves its original palette index.
                if (region >= 1u && region <= 4u)
                {
                    uint rampId = i.rampIds[region - 1u];
                    if (rampId != 0u)
                    {
                        float encodedIndex = LOAD_TEXTURE2D(
                            _RampTex, int2(shadeSlot, rampId)).r;
                        paletteIndex = (uint)floor(encodedIndex * 255.0 + 0.5) & 63u;
                    }
                }

                // Integer texel loads avoid interpolation between palette entries.
                half3 rgb = LOAD_TEXTURE2D(_PaletteTex, int2(paletteIndex, 0)).rgb;
                return half4(rgb, opacity);
            }
            ENDHLSL
        }
    }
}
