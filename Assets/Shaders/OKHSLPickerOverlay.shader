Shader "Unlit/OKHSLPickerOverlay"
{
    Properties
    {
        _Saturation ("Saturation", float) = 0.9
        _MainTex ("Overlay", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        ZTest Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

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
            float _Saturation;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 col = OKHSLtoRGB(float3(i.uv.x, _Saturation, i.uv.y));
                float3 overlayCol = tex2D(_MainTex, i.uv);
                float3 finalCol = lerp(col, overlayCol, overlayCol.r);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, finalCol);
                return fixed4(finalCol.r, finalCol.g, finalCol.b, 1);
            }
            ENDCG
        }
    }
}
