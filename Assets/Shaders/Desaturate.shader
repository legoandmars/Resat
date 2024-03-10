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
        [Toggle(DISABLE_SHOW_OKHSL_VIEW)] _DisableShowOKHSLView("Disable Debug OKHSL View", Float) = 0 // should be 1 in prod
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

            #pragma shader_feature DISABLE_SHOW_OKHSL_VIEW 

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
            
            sampler2D _ForceSaturationMask;
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

                // inverted so rider doesn't throw a fit
#ifndef DISABLE_SHOW_OKHSL_VIEW
                    uint tempArrayIndex = ArrayIndexFromArrayCoordinates(ArrayCoordinatesFromOKHSL(RGBtoOKHSL(col), _OKHSLArrayResolution), _OKHSLArrayResolution);
                    float3 parsedOkhsl = OKHSLFromArrayIndex(tempArrayIndex, _OKHSLArrayResolution);
                    float3 parsedCol = OKHSLtoRGB(parsedOkhsl);

                    return float4(parsedCol.r, parsedCol.g, parsedCol.b, 1);
#endif
                // else is annoying but needed for rider to not black everything out
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

                // calculate cutout
                float2 center = float2(_CutoutOffsetAndScale.z, _CutoutOffsetAndScale.w);
                float2 pos = float2(newX, i.uv.y) - center;
                pos.x /= _CutoutOffsetAndScale.x;
                pos.y /= _CutoutOffsetAndScale.y;

                pos = abs(pos);
                float cut = 1 - step(0.5, max(pos.x, pos.y)); // Decides whether it's within the cutout or not

                // setup okhsl
                float3 okhsl = RGBtoOKHSL(col);
                uint2 arrayCoordinates = ArrayCoordinatesFromOKHSL(okhsl, _OKHSLArrayResolution);
                uint arrayIndex = ArrayIndexFromArrayCoordinates(arrayCoordinates, _OKHSLArrayResolution);
                
                // get desaturated color
                okhsl.g = 0;
                float4 desaturatedCol = float4(OKHSLtoRGB(okhsl), 0);
                
                // apply outside color, because we want a slight darkening outside of the viewport always
                desaturatedCol = lerp(desaturatedCol, desaturatedCol * _OutsideCutoutColor, 1 - cut); 

                // Create gradient to make animation smoother
                float scalingFactor = lerp(_GradientSettings.z, _GradientSettings.w, _AnimationPercent);
                float2 scaledCoordinates = float2(clamp(1 - pos.x * scalingFactor, 0, 1), clamp(1 - pos.y * scalingFactor, 0, 1));
                float gradient = clamp(pow(min(scaledCoordinates.x, scaledCoordinates.y) * _GradientSettings.x, _GradientSettings.y), 0, 1);

                // IMPORTANT: when camera ratio changes, so does the gradient!!
                // lerp between the last photo and the current photo, based on the animation speed
                float animationPercent = _AnimationPercent == 1 ? _AnimationPercent : gradient * _AnimationPercent; // temporary fix for gradient cancelling all colors when camera minimized
                float desaturationLerp = lerp(_PreviousGlobalOKHSLBuffer[arrayIndex] > 0, _GlobalOKHSLBuffer[arrayIndex] > 0, animationPercent);

                // adjusts all colors outside the camera viewport
                float4 multiplier = lerp(float4(1,1,1,1), _OutsideCutoutColor, 1 - cut);

                // lerp by force resaturation for objects like quest items
                float forceResaturate = tex2D(_ForceSaturationMask, i.uv);
                desaturationLerp = lerp(desaturationLerp, 1, step(0.6, forceResaturate));
                
                // TODO: Partial resaturation? Maybe you need to see a color 100 times to fully resat?
                return lerp(desaturatedCol, col, lerp(cut, 1, desaturationLerp)) * multiplier;
            }
            ENDCG
        }
    }
}
