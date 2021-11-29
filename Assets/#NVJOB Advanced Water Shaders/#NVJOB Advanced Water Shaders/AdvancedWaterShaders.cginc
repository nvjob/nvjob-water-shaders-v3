// Copyright (c) 2016 Unity Technologies. MIT license - license_unity.txt
// #NVJOB Water Shaders. MIT license - license_nvjob.txt
// #NVJOB Water Shaders v3.0 (#NVJOB Advanced Water Shaders) - https://nvjob.github.io/
// #NVJOB Nicholas Veselov - https://nvjob.github.io


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


sampler2D_float _CameraDepthTexture;
float4 _CameraDepthTexture_TexelSize;

//----------------------------------------------

float4 _AWSMovement;
float _AWSAngleTes;

//----------------------------------------------

sampler2D _AlbedoTex1;
float _Albedo1Tiling;
float4 _AlbedoColorUp;
float4 _AlbedoColorDown;
half _AlbedoTessStrength;
half _AlbedoIntensity;
half _AlbedoContrast;
half _SoftFactor;

//----------------------------------------------

float _Shininess;
float _Glossiness;
float _Metallic;

//----------------------------------------------

sampler2D _NormalMap1;
float _NormalMap1Tiling;
float _NormalMap1Strength;
float _NormalTessStrength;
float _NormalTessOffset;

#ifdef EFFECT_NORMALMAP2
sampler2D _NormalMap2;
float _NormalMap2Tiling;
float _NormalMap2Strength;
float _NormalMap2Flow;
#endif

//----------------------------------------------

float _Tessellation;
float _TessStart;
float _TessEnd;
float _TessNoise1Octaves;
float4 _TessSetNoise1;
float4 _TessSetNoise2;
float4 _TessSetNoise3;
float4 _TessSetNoise4;
float _TessSetNoiseAngle2;
float _TessSetNoiseAngle3;
float _TessSetNoiseAngle4;

//----------------------------------------------

#ifdef EFFECT_MICROWAVE
float _MicrowaveScale;
float _MicrowaveStrength;
#endif

//----------------------------------------------

#ifdef EFFECT_PARALLAX
float _ParallaxAmount;
float _ParallaxNormal2Offset;
float _ParallaxNoiseOctaves;
float4 _ParallaxNoise;
#endif

//----------------------------------------------

#ifdef EFFECT_REFLECTION
samplerCUBE _ReflectionCube;
float4 _ReflectionColor;
float _ReflectionStrength;
float _ReflectionSaturation;
float _ReflectionContrast;
#endif

//----------------------------------------------

#ifdef EFFECT_MIRROR
sampler2D _GrabTexture : register(s0);
sampler2D _MirrorReflectionTex : register(s3);
float4 _MirrorColor;
float4 _MirrorDepthColor;
float _WeirdScale;
float _MirrorFPOW;
float _MirrorR0;
float _MirrorSaturation;
float _MirrorStrength;
float _MirrorContrast;
float _MirrorWavePow;
float4 _GrabTexture_TexelSize;
#endif

//----------------------------------------------

#ifdef EFFECT_FOAM
float4 _FoamColor;
float _FoamNoiseOctaves;
float4 _FoamNoise;
half _FoamShoreSoft;
half _FoamShorePow;
half _FoamWavePow;
#endif


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


struct Input {
float3 worldRefl;
float3 worldPos;
float4 screenPos;
float3 viewDir;
float4 color : COLOR;
INTERNAL_DATA
};


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


float2 RotationVector(float2 uv, float angle) {
float sina, cosa;
sincos(angle * UNITY_PI / 180, sina, cosa);
uv = float2(mul(float2x2(cosa, -sina, sina, cosa), uv));
return uv;
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


float Random(float2 uv) {
return frac(sin(dot(uv, float2(12.9898, 4.1414))) * 43758.5453);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


float Noise1(float2 uv, float gain, float amplitude, float octaves) {
float result;
float2 uvt = uv * 0.1;
for (int i = 0; i < octaves; i++) {
uvt *= (i + 1) * 0.43;
const float2 d = float2(0.0, 1.0);
float2 b = floor(uvt);
float2 f = smoothstep(0, 1, frac(uvt));
float lp1 = lerp(Random(b), Random(b + d.yx), f.x);
float lp2 = lerp(Random(b + d.xy), Random(b + d.yy), f.x);
float endCalc = lerp(lp1, lp2, f.y);
endCalc = 0.5 - (abs(sin(endCalc * gain)) / 2);
endCalc *= amplitude;
result += endCalc;
}
return result - amplitude * 0.5;
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


float Noise2(float2 uv, half2 direction, float angle, float speed, float wavelength, float amplitude, float gain) {
amplitude *= 0.1;
half2 ndir = normalize(direction);
ndir = RotationVector(ndir, angle);
float piw = 2 * UNITY_PI / wavelength;
float spd = piw * (dot(uv, ndir) - sqrt(9.8 / piw) * _Time.y * speed);
float step = amplitude / piw;
float spdsin = pow(abs(sin(spd)), gain);
float endCalc = 0.5 - (spdsin / 2);
endCalc *= step;
return endCalc - amplitude * 0.5;
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


float CalcTess(float4 vertex, float minDist, float maxDist, float tess) {
float3 wpos = mul(unity_ObjectToWorld, vertex).xyz;
float dist = distance(wpos, _WorldSpaceCameraPos);
float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
return f;
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


float4 tessWater(appdata_full v0, appdata_full v1, appdata_full v2) {
float3 f;
float4 tess;
f.x = CalcTess(v0.vertex, _TessStart, _TessEnd, _Tessellation);
f.y = CalcTess(v1.vertex, _TessStart, _TessEnd, _Tessellation);
f.z = CalcTess(v2.vertex, _TessStart, _TessEnd, _Tessellation);
tess.x = 0.5 * (f.y + f.z);
tess.y = 0.5 * (f.x + f.z);
tess.z = 0.5 * (f.x + f.y);
tess.w = (f.x + f.y + f.z) / 3.0f;
return tess;
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


void vert(inout appdata_full v) {
float2 uvw = mul(unity_ObjectToWorld, v.vertex).xz;
float2 uvmn1 = uvw;
uvmn1 += _AWSMovement.zw * _TessSetNoise1.x;
float nt1 = Noise1(uvmn1 * _TessSetNoise1.z, _TessSetNoise1.y, _TessSetNoise1.w, _TessNoise1Octaves);
float nt2 = Noise2(uvw, _AWSMovement.zw, _TessSetNoiseAngle2 - _AWSAngleTes, _TessSetNoise2.x, _TessSetNoise2.z, _TessSetNoise2.w, _TessSetNoise2.y);
float nt3 = Noise2(uvw, _AWSMovement.zw, _TessSetNoiseAngle3 - _AWSAngleTes, _TessSetNoise3.x, _TessSetNoise3.z, _TessSetNoise3.w, _TessSetNoise3.y);
float nt4 = Noise2(uvw, _AWSMovement.zw, _TessSetNoiseAngle4 - _AWSAngleTes, _TessSetNoise4.x, _TessSetNoise4.z, _TessSetNoise4.w, _TessSetNoise4.y);
float dis = nt1 + nt2 + nt3 + nt4;
v.color.x = dis;
dis *= 5;
v.vertex.xyz += v.normal * dis;
v.vertex.y -= 2.5;
COMPUTE_EYEDEPTH(v.color.w);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


#ifdef EFFECT_PARALLAX
float2 OffsetParallax(Input IN) {
float2 uvnh = IN.worldPos.xz;
uvnh += _AWSMovement.zw * _ParallaxNoise.x;
float nh = Noise1(uvnh * _ParallaxNoise.z, _ParallaxNoise.y, _ParallaxNoise.w, _ParallaxNoiseOctaves);
return ParallaxOffset(nh, _ParallaxAmount, float3(uvnh, IN.viewDir.z));
}
#endif


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


#ifdef EFFECT_REFLECTION
float3 SpecularReflection(Input IN, float4 albedo, float3 normal) {
float4 reflcol = texCUBE(_ReflectionCube, WorldReflectionVector(IN, normal));
reflcol *= albedo.a;
reflcol *= _ReflectionStrength;
float LumRef = dot(reflcol, float3(0.2126, 0.7152, 0.0722));
float3 reflcolL = lerp(LumRef.xxx, reflcol, _ReflectionSaturation);
reflcolL = ((reflcolL - 0.5) * _ReflectionContrast + 0.5);
return reflcolL * _ReflectionColor.rgb;
}
#endif


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


#ifdef EFFECT_MIRROR
float4 MirrorReflection(Input IN, float3 normal) {
IN.screenPos.xy = normal * _GrabTexture_TexelSize.xy * IN.screenPos.z + IN.screenPos.xy;
IN.screenPos += IN.color.xxxx * _MirrorWavePow;
half4 reflcol = tex2Dproj(_MirrorReflectionTex, IN.screenPos);
reflcol *= _MirrorStrength;
float LumRef = dot(reflcol, float3(0.2126, 0.7152, 0.0722));
reflcol.rgb = lerp(LumRef.xxx, reflcol, _MirrorSaturation);
reflcol.rgb = ((reflcol.rgb - 0.5) * _MirrorContrast + 0.5);
reflcol *= _MirrorColor;
float3 refrColor = tex2Dproj(_GrabTexture, IN.screenPos);
refrColor = _MirrorDepthColor * refrColor;
half fresnel = saturate(1.0 - dot(normal, normalize(IN.viewDir)));
fresnel = pow(fresnel, _MirrorFPOW);
fresnel = _MirrorR0 + (1.0 - _MirrorR0) * fresnel;
return reflcol * fresnel + half4(refrColor.xyz, 1.0) * (1.0 - fresnel);
}
#endif


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



float SoftFade(Input IN, float value, float softf) {
float rawZ = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos + value));
return saturate(softf * (LinearEyeDepth(rawZ) - IN.color.w));
}



//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



float SoftFactor(Input IN, float colora) {
return colora * SoftFade(IN, 0.0001, _SoftFactor);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


#ifdef EFFECT_FOAM
float3 FoamFactor(Input IN, float3 albedo) {
float2 foamuv = IN.worldPos.xz;
foamuv += _AWSMovement.zw * -_FoamNoise.x;
float foamuvnoi = Noise1((foamuv * _FoamNoise.z) * 10, _FoamNoise.y, _FoamNoise.w, _FoamNoiseOctaves);
float3 fcolor = _FoamColor * foamuvnoi;
if (_FoamWavePow != 0) albedo = lerp(albedo, fcolor, IN.color.x * _FoamWavePow);
if (_FoamShorePow != 0) {
float fade = SoftFade(IN, 0.0001, 1 - _FoamShoreSoft);
albedo = lerp(fcolor * _FoamShorePow, albedo, fade);
}
return albedo;
}
#endif



//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////