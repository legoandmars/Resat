Shader "Unlit/Desaturate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScreenResolution ("Screen Resolution (W, H)", Vector) = (1,1,1,1)
        _CutoutOffsetAndCenter ("Cutout Offset and Center (W, H, X, Y)", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "ColorSpaces.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ScreenResolution;
            float4 _CutoutOffsetAndScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 hsl = RGBtoOKHSL(col);
                hsl.g = 0;
                float4 rgb = float4(OKHSLtoRGB(hsl), 0);
                
                float ratioAdjustment = 1.77777778 / (_ScreenResolution.x / _ScreenResolution.y);
                float2 center = float2(_CutoutOffsetAndScale.z, _CutoutOffsetAndScale.w);
                float2 pos = i.uv - center;
                pos.x /= _CutoutOffsetAndScale.x * ratioAdjustment;
                pos.y /= _CutoutOffsetAndScale.y;
                pos = abs(pos);
                float cut = 1 - step(0.5, max(pos.x, pos.y)); // Decides whether it's within the cutout or not

                return lerp(rgb, col, cut);
            }
            ENDCG
        }
    }
}
