Shader "Unlit/Desaturate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScreenResolution ("Screen Resolution (W, H)", Vector) = (1,1,1,1)
        _CutoutOffsetAndCenter ("Cutout Offset and Center (W, H, X, Y)", Vector) = (0,0,0,0)
        _OutsideCutoutColor ("Outside Cutout Color", Color) = (1,1,1,1)
        _AnimationPercent ("Post-photo animation timer", Range(0.0, 1.0)) = 1.0
        _GradientSettings ("Gradient (Mult, Power, MinScale, MaxScale)", Vector) = (2,2,2,1)
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
            #include "ColorArrays.cginc"

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
            float4 _OutsideCutoutColor;
            float _AnimationPercent;
            float4 _GradientSettings;
            
            StructuredBuffer<int> _GlobalOKHSLBuffer;
            StructuredBuffer<int> _PreviousGlobalOKHSLBuffer;
            float2 _OKHSLArrayResolution;
            
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

                // rescale space to clamp like centered UI
                float newX = i.uv.x;
                float ratioAdjustment = 1.77777778f / (_ScreenResolution.x / _ScreenResolution.y);

                // 0 when low aspect ratio
                // up to 0.5 when high aspect ratio
                // this is the left edge
                float idealWidth = (1.7777778f * _ScreenResolution.y);

                // clamp higher aspect ratios to 16:9 to match canvas scaling
                float width = ((_ScreenResolution.x - idealWidth) / _ScreenResolution.x) / 2;
                newX -= width;
                newX /= ratioAdjustment;
                
                float2 center = float2(_CutoutOffsetAndScale.z, _CutoutOffsetAndScale.w);
                float2 pos = float2(newX, i.uv.y) - center;
                pos.x /= _CutoutOffsetAndScale.x;
                pos.y /= _CutoutOffsetAndScale.y;

                pos = abs(pos);
                float cut = 1 - step(0.5, max(pos.x, pos.y)); // Decides whether it's within the cutout or not

                // apply outside color first, because we want a slight darkening outside of the viewport always
                col = lerp(col, col * _OutsideCutoutColor, 1 - cut); 

                // get desaturated color
                float3 okhsl = RGBtoOKHSL(col);
                okhsl.g = 0;
                float4 desaturatedCol = float4(OKHSLtoRGB(okhsl), 0);

                uint2 arrayCoordinates = ArrayCoordinatesFromOKHSL(okhsl, _OKHSLArrayResolution);
                uint arrayIndex = ArrayIndexFromArrayCoordinates(arrayCoordinates, _OKHSLArrayResolution);

                // Create gradient to make animation smoother
                float scalingFactor = lerp(_GradientSettings.z, _GradientSettings.w, _AnimationPercent);
                float2 scaledCoordinates = float2(clamp(1 - pos.x * scalingFactor, 0, 1), clamp(1 - pos.y * scalingFactor, 0, 1));
                float gradient = clamp(pow(min(scaledCoordinates.x, scaledCoordinates.y) * _GradientSettings.x, _GradientSettings.y), 0, 1);

                // IMPORTANT: when camera ratio changes, so does the gradient!!
                // lerp between the last photo and the current photo, based on the animation speed
                float animationPercent = _AnimationPercent == 1 ? _AnimationPercent : gradient * _AnimationPercent;
                float desaturationLerp = lerp(_PreviousGlobalOKHSLBuffer[arrayIndex] > 0, _GlobalOKHSLBuffer[arrayIndex] > 0, animationPercent);

                // TODO: Partial resaturation? Maybe you need to see a color 100 times to fully resat?
                cut = lerp(cut, 1, desaturationLerp);
                return lerp(desaturatedCol, col, cut);
            }
            ENDCG
        }
    }
}
