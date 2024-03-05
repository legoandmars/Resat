Shader "Custom/ColorVertexBase" {
    Properties{ }
 
        SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
        LOD 200
 
        Pass
        {
        Name "DEFERRED"
        Tags { "LightMode" = "Deferred"    }
 
        CGPROGRAM
        // compile directives
        #pragma vertex vert_surf
        #pragma fragment frag_surf
        #pragma target 2.0
        #pragma addshadows
        #pragma multi_compile_instancing
        #pragma exclude_renderers nomrt
        #pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
        #pragma multi_compile_prepassfinal
        #include "HLSLSupport.cginc"
        #define UNITY_INSTANCED_LOD_FADE
        #define UNITY_INSTANCED_SH
        #define UNITY_INSTANCED_LIGHTMAPSTS
        #include "UnityShaderVariables.cginc"
        #include "UnityShaderUtilities.cginc"
        // -------- variant for: <when no other keywords are defined>
        #if !defined(INSTANCING_ON)
        // Surface shader code generated based on:
        // writes to per-pixel normal: no
        // writes to emission: no
        // writes to occlusion: no
        // needs world space reflection vector: no
        // needs world space normal vector: no
        // needs screen space position: no
        // needs world space position: no
        // needs view direction: no
        // needs world space view direction: no
        // needs world space position for lighting: YES
        // needs world space view direction for lighting: YES
        // needs world space view direction for lightmaps: no
        // needs vertex color: YES
        // needs VFACE: no
        // needs SV_IsFrontFace: no
        // passes tangent-to-world matrix to pixel shader: no
        // reads from normal: YES
        // 0 texcoords actually used
        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "UnityPBSLighting.cginc"
 
        #define INTERNAL_DATA
        #define WorldReflectionVector(data,normal) data.worldRefl
        #define WorldNormalVector(data,normal) normal
 
        // Original surface shader snippet:
        #line 5 ""
        #ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
        #endif
        /* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
        //#pragma surface surf Standard /*exclude_path:forwardadd*/ halfasview /*fullforwardshadows*/
 
        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 2.0
        struct Input {
            float4 color: COLOR;
        };
 
        void surf(Input IN, inout SurfaceOutputStandard o) {
            o.Albedo = IN.color;
            o.Metallic = 0.145;
            o.Smoothness = 0.32;// _Glossiness;
            //o.Alpha = IN.color.a;
        }
 
 
        // vertex-to-fragment interpolation data
        struct v2f_surf {
            UNITY_POSITION(pos);
            nointerpolation float3 worldNormal : TEXCOORD0;
            float3 worldPos : TEXCOORD1;
            fixed4 color : COLOR0;
        #ifndef DIRLIGHTMAP_OFF
            float3 viewDir : TEXCOORD2;
        #endif
            float4 lmap : TEXCOORD3;
        #ifndef LIGHTMAP_ON
            #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
            nointerpolation half3 sh : TEXCOORD4; // SH
            #endif
        #else
            #ifdef DIRLIGHTMAP_OFF
            float4 lmapFadePos : TEXCOORD4;
            #endif
        #endif
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };
 
        struct appdata_full_my {
            float4 vertex : POSITION;
            nointerpolation float4 tangent : TANGENT;
            nointerpolation float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
            float4 texcoord1 : TEXCOORD1;
            float4 texcoord2 : TEXCOORD2;
            float4 texcoord3 : TEXCOORD3;
            fixed4 color : COLOR;
            // and additional texture coordinates only on XBOX360
        };
 
        // vertex shader
        v2f_surf vert_surf(appdata_full_my v)
        {
            UNITY_SETUP_INSTANCE_ID(v);
            v2f_surf o;
            UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
            UNITY_TRANSFER_INSTANCE_ID(v,o);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            o.pos = UnityObjectToClipPos(v.vertex);
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            float3 worldNormal = UnityObjectToWorldNormal(v.normal);
            o.worldPos.xyz = worldPos;
            o.worldNormal = worldNormal;
            float3 viewDirForLight = UnityWorldSpaceViewDir(worldPos);
            float3 worldLightDir = UnityWorldSpaceLightDir(worldPos);
            viewDirForLight = normalize(normalize(viewDirForLight) + normalize(worldLightDir));
            #ifndef DIRLIGHTMAP_OFF
            o.viewDir = viewDirForLight;
            #endif
            o.color = v.color;
        #ifdef DYNAMICLIGHTMAP_ON
            o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
        #else
            o.lmap.zw = 0;
        #endif
        #ifdef LIGHTMAP_ON
            o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
            #ifdef DIRLIGHTMAP_OFF
            o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
            o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
            #endif
        #else
            o.lmap.xy = 0;
            #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
                o.sh = 0;
                o.sh = ShadeSHPerVertex(worldNormal, o.sh);
            #endif
        #endif
            return o;
        }
        #ifdef LIGHTMAP_ON
        float4 unity_LightmapFade;
        #endif
        fixed4 unity_Ambient;
 
        // fragment shader
        void frag_surf(v2f_surf IN,
            out half4 outGBuffer0 : SV_Target0,
            out half4 outGBuffer1 : SV_Target1,
            out half4 outGBuffer2 : SV_Target2,
            out half4 outEmission : SV_Target3
        #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
            , out half4 outShadowMask : SV_Target4
        #endif
        ) {
            UNITY_SETUP_INSTANCE_ID(IN);
            // prepare and unpack data
            Input surfIN;
            #ifdef FOG_COMBINED_WITH_TSPACE
            UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
            #elif defined (FOG_COMBINED_WITH_WORLD_POS)
            UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
            #else
            UNITY_EXTRACT_FOG(IN);
            #endif
            UNITY_INITIALIZE_OUTPUT(Input,surfIN);
            surfIN.color.x = 1.0;
            float3 worldPos = IN.worldPos.xyz;
            #ifndef USING_DIRECTIONAL_LIGHT
            fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
            #else
            fixed3 lightDir = _WorldSpaceLightPos0.xyz;
            #endif
            float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
            worldViewDir = normalize(worldViewDir + lightDir);
            surfIN.color = IN.color;
            #ifdef UNITY_COMPILER_HLSL
            SurfaceOutputStandard o = (SurfaceOutputStandard)0;
            #else
            SurfaceOutputStandard o;
            #endif
            o.Albedo = 0.0;
            o.Emission = 0.0;
            o.Alpha = 0.0;
            o.Occlusion = 1.0;
            fixed3 normalWorldVertex = fixed3(0,0,1);
            o.Normal = IN.worldNormal;
            normalWorldVertex = IN.worldNormal;
 
            // call surface function
            surf(surfIN, o);
            fixed3 originalNormal = o.Normal;
            half atten = 1;
 
            // Setup lighting environment
            UnityGI gi;
            UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
            gi.indirect.diffuse = 0;
            gi.indirect.specular = 0;
            gi.light.color = 0;
            gi.light.dir = half3(0,1,0);
            // Call GI (lightmaps/SH/reflections) lighting function
            UnityGIInput giInput;
            UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
            giInput.light = gi.light;
            giInput.worldPos = worldPos;
            giInput.worldViewDir = worldViewDir;
            giInput.atten = atten;
            #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
            giInput.lightmapUV = IN.lmap;
            #else
            giInput.lightmapUV = 0.0;
            #endif
            #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
            giInput.ambient = IN.sh;
            #else
            giInput.ambient.rgb = 0.0;
            #endif
            giInput.probeHDR[0] = unity_SpecCube0_HDR;
            giInput.probeHDR[1] = unity_SpecCube1_HDR;
            #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
            giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
            #endif
            #ifdef UNITY_SPECCUBE_BOX_PROJECTION
            giInput.boxMax[0] = unity_SpecCube0_BoxMax;
            giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
            giInput.boxMax[1] = unity_SpecCube1_BoxMax;
            giInput.boxMin[1] = unity_SpecCube1_BoxMin;
            giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
            #endif
            LightingStandard_GI(o, giInput, gi);
 
            // call lighting function to output g-buffer
            outEmission = LightingStandard_Deferred(o, worldViewDir, gi, outGBuffer0, outGBuffer1, outGBuffer2);
            #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
            outShadowMask = UnityGetRawBakedOcclusions(IN.lmap.xy, worldPos);
            #endif
            #ifndef UNITY_HDR_ON
            outEmission.rgb = exp2(-outEmission.rgb);
            #endif
        }
 
 
        #endif
 
        ENDCG
        }
 
 
        Pass{
        Name "ShadowCaster"
        Tags { "LightMode" = "ShadowCaster" }
        ZWrite On ZTest LEqual
 
        CGPROGRAM
            // compile directives
            #pragma vertex vert_surf
            #pragma fragment frag_surf
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_shadowcaster
            #include "HLSLSupport.cginc"
            #define UNITY_INSTANCED_LOD_FADE
            #define UNITY_INSTANCED_SH
            #define UNITY_INSTANCED_LIGHTMAPSTS
            #include "UnityShaderVariables.cginc"
            #include "UnityShaderUtilities.cginc"
            // -------- variant for: <when no other keywords are defined>
            #if !defined(INSTANCING_ON)
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
 
            #define INTERNAL_DATA
            #define WorldReflectionVector(data,normal) data.worldRefl
            #define WorldNormalVector(data,normal) normal
 
            // Original surface shader snippet:
            #line 7 ""
            #ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
            #endif
            /* UNITY: Original start of shader */
            // Physically based Standard lighting model, and enable shadows on all light types
            //#pragma surface surf Standard halfasview addshadow
 
            // Use shader model 3.0 target, to get nicer looking lighting
            //#pragma target 2.0
            struct Input {
                float4 color: COLOR;
            };
 
        // vertex-to-fragment interpolation data
        struct v2f_surf {
              V2F_SHADOW_CASTER;
              float3 worldPos : TEXCOORD1;
              UNITY_VERTEX_INPUT_INSTANCE_ID
              UNITY_VERTEX_OUTPUT_STEREO
        };
 
        // vertex shader
        v2f_surf vert_surf(appdata_full v)
        {
              UNITY_SETUP_INSTANCE_ID(v);
              v2f_surf o;
              UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
              UNITY_TRANSFER_INSTANCE_ID(v,o);
              UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
              float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
              //float3 worldNormal = UnityObjectToWorldNormal(v.normal);
              o.worldPos.xyz = worldPos;
              TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
              return o;
        }
 
        // fragment shader
        fixed4 frag_surf(v2f_surf IN) : SV_Target
        {
            UNITY_SETUP_INSTANCE_ID(IN);
        // prepare and unpack data
        #ifdef FOG_COMBINED_WITH_TSPACE
          UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
        #elif defined (FOG_COMBINED_WITH_WORLD_POS)
          UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
        #else
          UNITY_EXTRACT_FOG(IN);
        #endif
        SHADOW_CASTER_FRAGMENT(IN)
    }
 
 
    #endif
    ENDCG
 
    }
 
    }
 
}
 
 