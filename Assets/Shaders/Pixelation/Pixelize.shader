Shader "Hidden/Pixelize"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Keep your structs exactly as they were
            struct Attributes
            {
                float4 PositionOS : POSITION;
                float2 uv : TEXCOORD0;
                uint vertexID : SV_VertexID; // Added this to handle procedural blitting
            };

            struct Varyings
            {
                float4 PositionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Unity 6 Blitter sends data to _BlitTexture
            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            uniform float2 _BlockCount;
            uniform float2 _BlockSize;
            uniform float2 _HalfBlockSize;

            Varyings vert(Attributes i)
            {
                Varyings o;
                
                // 1. Generate procedural triangle
                float2 uv = float2((i.vertexID << 1) & 2, i.vertexID & 2);
                o.PositionHCS = float4(uv * 2.0 - 1.0, 0.0, 1.0);
                o.uv = uv;

                // 2. MANUAL FLIP FIX:
                // Unity flips the projection matrix when rendering to a texture on certain APIs.
                // _ProjectionParams.x will be -1.0 when the image is inverted.
                if (_ProjectionParams.x < 0.0)
                {
                    o.uv.y = 1.0 - o.uv.y;
                }
                
                return o;
            }

            half4 frag(Varyings i) : SV_TARGET
            {
                float2 blockPos = floor(i.uv * _BlockCount);
                float2 blockCenter = blockPos * _BlockSize + _HalfBlockSize;

                // Sample from the correct Blit texture
                return SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, blockCenter);
            }
            ENDHLSL
        }
        }
        
    }
