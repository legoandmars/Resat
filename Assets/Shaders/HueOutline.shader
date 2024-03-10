// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/HueOutline" {
Properties {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _MainTexOutline ("Base (Outline) Trans (A)", 2D) = "white" {}
    _InitialHueColor ("Initial OKHSL Color", Color) = (1,0,0,0)
    _Speed ("Hueshift speed", float) = 1
}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100

    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "ColorSpaces.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 outlineColor : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MainTexOutline;
            float4 _MainTexOutline_ST;
            float _Speed;
            fixed4 _InitialHueColor;
        
            v2f vert (appdata_t v)
            {
                
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

                // calculate color in vert to save perf
                float3 okhsl = RGBtoOKHSL(_InitialHueColor.rgb);
                okhsl.r = (_Time.y * _Speed) % 1;
                float3 rgb = OKHSLtoRGB(okhsl);
                float4 outlineColor = float4(rgb.r, rgb.g, rgb.b, 1);
                o.outlineColor = outlineColor;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                fixed4 outlineCol = tex2D(_MainTexOutline, i.texcoord) * i.outlineColor;
                // calculate outline
                // ideally this would be done using UV offset in shader, but i am on a time crunch
                col = lerp(col, outlineCol, 1 - col.a);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
        ENDCG
    }
}

}