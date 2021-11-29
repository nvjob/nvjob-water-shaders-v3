// Copyright (c) 2016 Unity Technologies. MIT license - license_unity.txt
// #NVJOB Water Shaders. MIT license - license_nvjob.txt
// #NVJOB Water Shaders v3.0 (#NVJOB Advanced Water Shaders) - https://nvjob.github.io/
// #NVJOB Nicholas Veselov - https://nvjob.github.io


Shader "#NVJOB/Advanced Water Shaders/Surface" {


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


Properties{
//----------------------------------------------

[HideInInspector][NoScaleOffset]_AlbedoTex1("Albedo Texture 1", 2D) = "white" {}
[HideInInspector]_Albedo1Tiling("Albedo 1 Tiling", float) = 1
[HideInInspector][HDR]_AlbedoColorUp("Albedo Color Up", Color) = (1,0.8,0.56,1)
[HideInInspector][HDR]_AlbedoColorDown("Albedo Color Down", Color) = (1,0.8,0.56,1)
[HideInInspector]_AlbedoTessStrength("Albedo Tessellation Strength", float) = 1
[HideInInspector]_AlbedoIntensity("Brightness", Range(0.1, 5)) = 1
[HideInInspector]_AlbedoContrast("Contrast", Range(-0.5, 3)) = 1
[HideInInspector]_Glossiness("Glossiness", Range(0,1)) = 0.5
[HideInInspector]_Metallic("Metallic", Range(-1,2)) = 0.0
[HideInInspector]_SoftFactor("Soft Factor", Range(0.0001, 1)) = 0.5
[HideInInspector][NoScaleOffset]_NormalMap1("Normal Map 1", 2D) = "bump" {}
[HideInInspector]_NormalMap1Tiling("Normal Map 1 Tiling", float) = 1
[HideInInspector]_NormalMap1Strength("Normal Map 1 Strength", float) = 1
[HideInInspector][NoScaleOffset]_NormalMap2("Normal Map 2", 2D) = "bump" {}
[HideInInspector]_NormalMap2Tiling("Normal Map 2 Tiling", float) = 1.2
[HideInInspector]_NormalMap2Strength("Normal Map 2 Strength", float) = 1
[HideInInspector]_NormalMap2Flow("Normal Map 2 Flow", float) = 0.5
[HideInInspector]_MicrowaveScale("Micro Waves Scale", float) = 1
[HideInInspector]_MicrowaveStrength("Micro Waves Strength", float) = 0.5
[HideInInspector]_NormalTessStrength("Normal Tessellation Strength", float) = 1
[HideInInspector]_NormalTessOffset("Normal Tessellation Offset", float) = 0
[HideInInspector]_Tessellation("Tessellation", float) = 60
[HideInInspector]_TessStart("Start", float) = 1
[HideInInspector]_TessEnd("End", float) = 600
[HideInInspector]_TessNoise1Octaves("Noise 1 Octaves", float) = 4
[HideInInspector]_TessSetNoise1("Noise 1", Vector) = (40, 5, 1.5, 0.2)
[HideInInspector]_TessSetNoise2("Noise 2", Vector) = (1, 1, 15, 0.2)
[HideInInspector]_TessSetNoise3("Noise 3", Vector) = (1, 1, 25, 0.1)
[HideInInspector]_TessSetNoise4("Noise 4", Vector) = (1, 1, 35, 0.05)
[HideInInspector]_TessSetNoiseAngle2("Noise Angle 2", float) = 0
[HideInInspector]_TessSetNoiseAngle3("Noise Angle 3", float) = 0
[HideInInspector]_TessSetNoiseAngle4("Noise Angle 4", float) = 0
[HideInInspector]_ParallaxAmount("Parallax Amount", float) = 0.1
[HideInInspector]_ParallaxNormal2Offset("Parallax Normal Map 2 Offset", float) = 1
[HideInInspector]_ParallaxNoiseOctaves("Parallax Noise Octaves", float) = 4
[HideInInspector]_ParallaxNoise("Parallax Noise", Vector) = (100, 6, 2, 0.1)
[HideInInspector][HDR]_MirrorColor("Mirror Reflection Color", Color) = (1,1,1,0.5)
[HideInInspector]_MirrorDepthColor("Mirror Reflection Depth Color", Color) = (0,0,0,0.5)
[HideInInspector]_MirrorStrength("Reflection Strength", Range(0, 5)) = 1
[HideInInspector]_MirrorSaturation("Reflection Saturation", Range(0, 5)) = 1
[HideInInspector]_MirrorContrast("Reflection Contrast", Range(0, 5)) = 1
[HideInInspector]_MirrorFPOW("Mirror FPOW", Float) = 5.0
[HideInInspector]_MirrorR0("Mirror R0", Float) = 0.01
[HideInInspector]_MirrorWavePow("Reflections Wave Strength", Float) = 1
[HideInInspector]_MirrorReflectionTex("_MirrorReflectionTex", 2D) = "gray" {}
[HideInInspector][HDR]_FoamColor("Foam Color", Color) = (1, 1, 1, 1)
[HideInInspector]_FoamNoiseOctaves("Foam Noise Octaves", Range(1, 6)) = 3
[HideInInspector]_FoamNoise("Foam Noise", Vector) = (20, 1, 1000, 3)
[HideInInspector]_FoamShoreSoft("Foam Shore Soft", Range(0.0001, 1)) = 0.7
[HideInInspector]_FoamWavePow("Foam Wave Strength", Float) = 1
[HideInInspector]_FoamShorePow("Foam Shore Strength", Float) = 1
[HideInInspector]_AWSMovement("_AWSMovement", Vector) = (1, 1, 1, 1)
[HideInInspector]_AWSAngleTes("_AWSAngleTes", float) = 0

//----------------------------------------------
}



//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



SubShader{
///////////////////////////////////////////////////////////////////////////////////////////////////////////////

Tags{ "Queue" = "Geometry+800" "IgnoreProjector" = "True" "RenderType" = "Transparent" "ForceNoShadowCasting" = "True" }
LOD 200
Cull Off
ZWrite On
ColorMask 0

CGPROGRAM
#pragma shader_feature_local EFFECT_NORMALMAP2
#pragma shader_feature_local EFFECT_MICROWAVE
#pragma shader_feature_local EFFECT_PARALLAX
#pragma shader_feature_local EFFECT_MIRROR
#pragma shader_feature_local EFFECT_FOAM
#pragma surface surf Standard alpha:fade noshadowmask noshadow vertex:vert tessellate:tessWater
#pragma target 4.6

//----------------------------------------------

#include "AdvancedWaterShaders.cginc"

//----------------------------------------------

void surf(Input IN, inout SurfaceOutputStandard o) {

float2 uvworld = IN.worldPos.xz * 0.1 * _Albedo1Tiling;
float2 uvworldn = IN.worldPos.xz * 0.1 * _NormalMap1Tiling;

#ifdef EFFECT_PARALLAX
float2 offset = OffsetParallax(IN);
uvworld -= offset;
uvworldn += offset;
float2 uvn = uvworldn;
uvn.xy += float2(_AWSMovement.z, _AWSMovement.w);
#ifdef EFFECT_NORMALMAP2
float2 uvnd = uvworldn - (offset * _ParallaxNormal2Offset);
uvnd.xy += float2(_AWSMovement.z, _AWSMovement.w) * _NormalMap2Flow;
#endif
#else
float2 uvn = uvworldn;
uvn.xy += float2(_AWSMovement.z, _AWSMovement.w);
#ifdef EFFECT_NORMALMAP2
float2 uvnd = uvworldn;
uvnd.xy += float2(_AWSMovement.z, _AWSMovement.w) * _NormalMap2Flow;
#endif
#endif

float2 uv = uvworld;
uv.xy += float2(_AWSMovement.z, _AWSMovement.w);

float4 tex = tex2D(_AlbedoTex1, uv);
tex *= lerp(_AlbedoColorDown, _AlbedoColorUp, IN.color.x * _AlbedoTessStrength);

float colora = tex.a;

tex *= _AlbedoIntensity;
float3 albedo = ((tex - 0.5) * _AlbedoContrast + 0.5).rgb;

float3 normal = UnpackNormal(tex2D(_NormalMap1, uvn)) * _NormalMap1Strength;
#ifdef EFFECT_NORMALMAP2
normal += UnpackNormal(tex2D(_NormalMap2, uvnd * _NormalMap2Tiling)) * _NormalMap2Strength;
#ifdef EFFECT_MICROWAVE
normal -= UnpackNormal(tex2D(_NormalMap2, (uv + uvnd) * 2 * _MicrowaveScale)) * _MicrowaveStrength;
normal = normalize(normal / 3);
#else
normal = normalize(normal / 2);
#endif
#endif

normal += float3(IN.color.x, IN.color.x + _NormalTessOffset, IN.color.x) * _NormalTessStrength;
normal = normalize(normal / 2);

#ifdef EFFECT_MIRROR
o.Emission = (o.Emission + MirrorReflection(IN, normal)) * 0.6;
#endif

#ifdef EFFECT_FOAM
albedo = FoamFactor(IN, albedo);
#endif

o.Normal = normal;
o.Metallic = _Metallic;
o.Smoothness = _Glossiness;
o.Albedo.rgb = albedo;
o.Alpha = SoftFactor(IN, colora);

}

//----------------------------------------------

ENDCG

///////////////////////////////////////////////////////////////////////////////////////////////////////////////
}


FallBack "Legacy Shaders/Reflective/Bumped Diffuse"
CustomEditor "AWSEditor"


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
