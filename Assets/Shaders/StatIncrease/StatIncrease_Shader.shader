Shader "Unlit/StatIncrease_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Width ("Width", Float) = 8
        _Height ("Height", Float) = 12
        _Speed ("Speed", Range(0,1)) = 0.3
        _TopColor ("TopColor", Color) = (1,1,0,1)
        _BottomColor ("BottomColor", Color) = (1,0,0,1)
        _Opacity ("Opacity", Range(0,1)) = 1
        _Direction ("Direction", Range(-1,1)) = -1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Assets/CustomShaderLibrary/CustomShaderLibrary.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _MainTex_ST, _TopColor, _BottomColor;
            float _Width, _Height, _Speed, _Opacity, _Direction;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float2 PixelSpaceScroll(v2f i)
            {
                float2 size = _MainTex_TexelSize.zw;
                float2 direction = i.uv * _Direction;
                direction.y -= _Time.y * _Speed;

                return size * direction;
            }

            float PixelSpaceTriangle(float2 pixelSpaceScroll)
            {
                float Width = pixelSpaceScroll.x / _Width;
                float TriangleWave = 2.0 * abs(2.0 * frac(Width + 0.5) - 1.0) - 1.0;
                
                float Rmap = remap(TriangleWave, float2(-1, 1), float2(0, 1));
                float TriangleMap = (Rmap * _Width) + pixelSpaceScroll.y;
                
                float Modulo = modulo(TriangleMap,_Height);
                return remap(Modulo, float2(0, _Height), float2(0, 1));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 pixelSpaceScroll = PixelSpaceScroll(i);
                float pixelSpaceTriangle = PixelSpaceTriangle(pixelSpaceScroll);
                
                float3 OverlayColor = lerp(_BottomColor,_TopColor,pixelSpaceTriangle) * _Opacity;
                float3 finalcolor = col.rgb + OverlayColor;
                
                return fixed4(finalcolor, col.a);
            }
            ENDCG
        }
    }
}