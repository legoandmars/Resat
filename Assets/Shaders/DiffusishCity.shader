// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// This is kinda fucked

Shader "Custom/DiffusishCity" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _Emission ("Emission", Color) = (0,0,0,1) // not idea if this will ever be used
    _LightingColor ("Fake Lighting Color", Color) = (0.679,0.679,0.679,1)
    _MainTex ("Main Texture", 2D) = "white" {}
    _DetailTex ("Detail Texture", 2D) = "clear" {}
    _LightingStrength ("Lighting Strength", Range(0, 1)) = 1
    _ShadowBoostCutoff ("Shadow boost cutoff", Range(0, 2)) = 1
    _ShadowBoostStrength ("Shadow boost strength", Range(0, 2)) = 1
    [Toggle(DETAIL_TEXTURE)] _UseDetailTexture("Use detail texture", Float) = 0
    [Toggle(SHADOW_BOOST)] _BoostShadows("Use shadow boost", Float) = 0
    [Enum(UnityEngine.Rendering.CullMode)] _CullMode("Cull Mode", Int) = 2
    [IntRange] _StencilRef ("Stencil Reference Value", Range(0,255)) = 0
}
    
SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 200
    Cull [_CullMode]
    
    Stencil
    {
        Ref [_StencilRef]
        Comp Equal
    }
    
CGPROGRAM
#pragma surface surf LambertOverride

#pragma shader_feature DETAIL_TEXTURE
#pragma shader_feature SHADOW_BOOST

fixed _LightingStrength;
fixed4 _Color;
fixed4 _LightingColor; // TODO: remove!!
fixed4 _Emission;
sampler2D _MainTex;

#ifdef DETAIL_TEXTURE
sampler2D _DetailTex;
#endif

#ifdef SHADOW_BOOST
fixed _ShadowBoostCutoff;
fixed _ShadowBoostStrength;
#endif

inline fixed4 LightingLambertOverride (SurfaceOutput s, UnityGI gi)
{
    fixed diff = max (0, dot (s.Normal, gi.light.dir));
    
    fixed4 c;
    c.rgb = s.Albedo * gi.light.color * lerp(_LightingColor, diff, _LightingStrength); // multiply diff by lighting strength
    c.a = 1;

#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
        c.rgb += s.Albedo * gi.indirect.diffuse; // TODO: This part might need to be lerped, but it looks fine as-is IMO
#endif

#ifdef SHADOW_BOOST
    if(c.rgb.r < _ShadowBoostCutoff)
    {
        c.rgb *= c.rgb * _ShadowBoostStrength;
    }
#endif
    
    return c;
}

inline void LightingLambertOverride_GI (
    SurfaceOutput s,
    UnityGIInput data,
    inout UnityGI gi)
{
    gi = UnityGlobalIllumination (data, 1.0, s.Normal);
}

struct Input {
    float2 uv_MainTex;
    float2 uv_DetailTex;
};

void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
    
#ifdef DETAIL_TEXTURE
    fixed4 detail = tex2D (_DetailTex, IN.uv_DetailTex);
    c = lerp(c, detail, detail.a);
#endif
    
    o.Albedo = c.rgb;
    o.Alpha = c.a;
    o.Emission = _Emission;
}

ENDCG
}

Fallback "Legacy Shaders/VertexLit"
}