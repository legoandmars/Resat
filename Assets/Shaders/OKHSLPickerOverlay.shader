Shader "Unlit/OKHSLPickerOverlay"
{
    Properties
    {
        _Saturation ("Saturation", float) = 0.9
        _MainTex ("Overlay", 2D) = "white" {}
        _PreviewTex ("Preview Tex", 2D) = "white" {}
        _AnimationPercent ("Post-photo animation timer", Range(0.0, 1.0)) = 1.0
        _AnimationState ("Animating", Range(0.0, 1.0)) = 1.0
        _Speed ("UV speed", Float) = 1.0
        _BaseSaturation ("Base Saturation", Float) = 1.0
        _PreviewColor ("Preview Color", Color) = (1,1,1,1)
        _GlobalColor ("Global Color", Color) = (1,1,1,1)
        _OutsideColor ("Outside Color", Color) = (1,1,1,1)
        _NoiseScale ("Noise Scale", Vector) = (1,1,1,1)
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
            sampler2D _PreviewTex;
            float4 _PreviewTex_ST;
            float4 _NoiseScale;
            float _Saturation;
            float _BaseSaturation;
            float _Speed;
            float _AnimationPercent;
            float _AnimationState;
            fixed4 _PreviewColor;
            fixed4 _GlobalColor;
            fixed4 _OutsideColor;
            
            float2 unity_gradientNoise_dir(float2 p)
            {
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float unity_gradientNoise(float2 p)
            {
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(unity_gradientNoise_dir(ip), fp);
                float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
            }

            
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
                float3 tempCol = OKHSLtoRGB(float3(i.uv.x, _Saturation, i.uv.y));
                float3 desatTempCol = OKHSLtoRGB(float3(i.uv.x, _BaseSaturation, i.uv.y + 0.05f)); // TODO: remove hard lightness cap
/*                if (i.uv.y < 1.0f / 16.0f)
                {
                    desatTempCol = OKHSLtoRGB(float3(i.uv.x, _BaseSaturation, i.uv.y + 0.03f));
                }*/
                float3 fullyDesatTempCol = OKHSLtoRGB(float3(i.uv.x, 0, i.uv.y + 0.2f));
                
                float4 col = float4(tempCol.r, tempCol.g, tempCol.b, 1);
                float4 desatCol = float4(desatTempCol.r, desatTempCol.g, desatTempCol.b, 1);
                float4 fullyDesatCol = float4(fullyDesatTempCol.r, fullyDesatTempCol.g, fullyDesatTempCol.b, 1);

                // float2 noiseScale = float2(_NoiseScale.x, _NoiseScale.x / 4);
                // i.uv.x += unity_gradientNoise(float2(32902, 320) + i.uv * noiseScale + _Time.y * _NoiseScale.z) * _NoiseScale.w;
                // i.uv.y += unity_gradientNoise(i.uv * noiseScale + _Time.y * _NoiseScale.z) * _NoiseScale.w;
                float4 overlayCol = tex2D(_MainTex, i.uv);
                float4 previewCol = tex2D(_PreviewTex, i.uv * _PreviewTex_ST.xy + _PreviewTex_ST.zw + float2(_Time.y * _Speed, 0)) * fullyDesatCol;

                float4 finalCol = lerp(col, desatCol, desatCol.a);
                if (overlayCol.r < 0.8f && overlayCol.r > 0.5f)
                {
                    // gobal
                    finalCol = lerp(col, _GlobalColor, _GlobalColor.a);;
                }
                else if (overlayCol.r > 0.9f)
                {
                    // works assuming we only need alpha
                    previewCol.a = lerp(previewCol.a, _GlobalColor.a, lerp(0, _AnimationPercent, _AnimationState));
                    // float4 interpolatedCol = lerp(previewCol * _PreviewColor, _GlobalColor, lerp(0, _AnimationPercent, _AnimationState));
                    finalCol = lerp(col, previewCol, previewCol.a);
                }
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, finalCol);
                return fixed4(finalCol.r, finalCol.g, finalCol.b, 1);
            }
            ENDCG
        }
    }
}
