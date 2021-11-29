// Copyright (c) 2016 Unity Technologies. MIT license - license_unity.txt
// #NVJOB Water Shaders. MIT license - license_nvjob.txt
// #NVJOB Water Shaders v3.0 (#NVJOB Advanced Water Shaders) - https://nvjob.github.io/
// #NVJOB Nicholas Veselov - https://nvjob.github.io


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


[CanEditMultipleObjects]
internal class AWSEditor : MaterialEditor
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    Color smLineColor = Color.HSVToRGB(0, 0, 0.55f), bgLineColor = Color.HSVToRGB(0, 0, 0.3f);
    int smLinePadding = 20, bgLinePadding = 35;

    static bool showAlbedo, showNormalMaps, showTessellation, showParallax, showReflection, showMirror, showFoam;


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public override void OnInspectorGUI()
    {
        //--------------

        SetDefaultGUIWidths();
        serializedObject.Update();
        SerializedProperty shaderFind = serializedObject.FindProperty("m_Shader");
        if (!isVisible || shaderFind.hasMultipleDifferentValues || shaderFind.objectReferenceValue == null) return;

        List<MaterialProperty> allProps = new List<MaterialProperty>(GetMaterialProperties(targets));

        //--------------

        GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        myFoldoutStyle.fontStyle = FontStyle.Bold;
        myFoldoutStyle.fontSize = 13;
        myFoldoutStyle.margin = new RectOffset(28, 0, 0, 0);

        //--------------

        EditorGUI.BeginChangeCheck();
        Header();
        DrawUILine(bgLineColor, 2, bgLinePadding);

        //--------------

        showAlbedo = EditorGUILayout.Foldout(showAlbedo, "Albedo Settings", true, myFoldoutStyle);
        if (showAlbedo) Albedo(allProps);
        DrawUILine(bgLineColor, 2, bgLinePadding);
        showNormalMaps = EditorGUILayout.Foldout(showNormalMaps, "Normal Map Settings", true, myFoldoutStyle);
        if (showNormalMaps) NormalMaps(allProps);
        DrawUILine(bgLineColor, 2, bgLinePadding);
        showTessellation = EditorGUILayout.Foldout(showTessellation, "Tessellation Settings", true, myFoldoutStyle);
        if (showTessellation) Tessellation(allProps);
        DrawUILine(bgLineColor, 2, bgLinePadding);
        showParallax = EditorGUILayout.Foldout(showParallax, "Parallax Settings", true, myFoldoutStyle);
        if (showParallax) Parallax(allProps);
        DrawUILine(bgLineColor, 2, bgLinePadding);
        if (allProps.Find(prop => prop.name == "_ReflectionCube") != null)
        {
            showReflection = EditorGUILayout.Foldout(showReflection, "Reflection Settings", true, myFoldoutStyle);
            if (showReflection) Reflection(allProps);
            DrawUILine(bgLineColor, 2, bgLinePadding);
        }
        showMirror = EditorGUILayout.Foldout(showMirror, "Mirror Reflection Settings", true, myFoldoutStyle);
        if (showMirror) MirrorReflection(allProps);
        DrawUILine(bgLineColor, 2, bgLinePadding);
        showFoam = EditorGUILayout.Foldout(showFoam, "Foam Settings", true, myFoldoutStyle);
        if (showFoam) Foam(allProps);

        //--------------

        DrawUILine(bgLineColor, 2, bgLinePadding);
        Information();
        DrawUILine(bgLineColor, 2, bgLinePadding);
        RenderQueueField();
        EnableInstancingField();
        DoubleSidedGIField();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //-------------- 
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Albedo(List<MaterialProperty> allProps)
    {
        //--------------

        DrawUILine(smLineColor, 1, smLinePadding);

        MaterialProperty albedoTex1 = allProps.Find(prop => prop.name == "_AlbedoTex1");
        MaterialProperty albedo1Tiling = allProps.Find(prop => prop.name == "_Albedo1Tiling");
        MaterialProperty albedoColorUp = allProps.Find(prop => prop.name == "_AlbedoColorUp");
        MaterialProperty albedoColorDown = allProps.Find(prop => prop.name == "_AlbedoColorDown");
        MaterialProperty albedoTessStrength = allProps.Find(prop => prop.name == "_AlbedoTessStrength");

        if (albedoTex1 != null && albedo1Tiling != null && albedoColorUp != null && albedoColorDown != null && albedoTessStrength != null)
        {
            allProps.Remove(albedoTex1);
            allProps.Remove(albedo1Tiling);
            allProps.Remove(albedoColorUp);
            allProps.Remove(albedoColorDown);
            allProps.Remove(albedoTessStrength);
            ShaderProperty(albedoTex1, albedoTex1.displayName);
            ShaderProperty(albedo1Tiling, albedo1Tiling.displayName);
            ShaderProperty(albedoColorUp, albedoColorUp.displayName);
            ShaderProperty(albedoColorDown, albedoColorDown.displayName);
            ShaderProperty(albedoTessStrength, albedoTessStrength.displayName);
        }

        DrawUILine(smLineColor, 1, smLinePadding);

        //--------------

        MaterialProperty albedoIntensity = allProps.Find(prop => prop.name == "_AlbedoIntensity");
        MaterialProperty albedoContrast = allProps.Find(prop => prop.name == "_AlbedoContrast");

        if (albedoIntensity != null && albedoContrast != null)
        {
            allProps.Remove(albedoIntensity);
            allProps.Remove(albedoContrast);
            ShaderProperty(albedoIntensity, albedoIntensity.displayName);
            ShaderProperty(albedoContrast, albedoContrast.displayName);
        }

        DrawUILine(smLineColor, 1, smLinePadding);

        //--------------

        MaterialProperty shininess = allProps.Find(prop => prop.name == "_Shininess");
        MaterialProperty specColor = allProps.Find(prop => prop.name == "_SpecColor");

        if (shininess != null && specColor != null)
        {
            allProps.Remove(shininess);
            allProps.Remove(specColor);
            ShaderProperty(shininess, shininess.displayName);
            ShaderProperty(specColor, specColor.displayName);
        }

        MaterialProperty glossiness = allProps.Find(prop => prop.name == "_Glossiness");
        MaterialProperty metallic = allProps.Find(prop => prop.name == "_Metallic");

        if (glossiness != null && metallic != null)
        {
            allProps.Remove(glossiness);
            allProps.Remove(metallic);
            ShaderProperty(glossiness, glossiness.displayName);
            ShaderProperty(metallic, metallic.displayName);
        }

        DrawUILine(smLineColor, 1, smLinePadding);

        //--------------

        MaterialProperty softFactor = allProps.Find(prop => prop.name == "_SoftFactor");

        if (softFactor != null)
        {
            allProps.Remove(softFactor);
            ShaderProperty(softFactor, softFactor.displayName);
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void NormalMaps(List<MaterialProperty> allProps)
    {
        //--------------   

        DrawUILine(smLineColor, 1, smLinePadding);

        MaterialProperty normalMap1 = allProps.Find(prop => prop.name == "_NormalMap1");
        MaterialProperty normalMap1Tiling = allProps.Find(prop => prop.name == "_NormalMap1Tiling");
        MaterialProperty normalMap1Strength = allProps.Find(prop => prop.name == "_NormalMap1Strength");

        if (normalMap1 != null && normalMap1Tiling != null && normalMap1Strength != null)
        {
            allProps.Remove(normalMap1);
            allProps.Remove(normalMap1Tiling);
            allProps.Remove(normalMap1Strength);
            ShaderProperty(normalMap1, normalMap1.displayName);
            ShaderProperty(normalMap1Tiling, normalMap1Tiling.displayName);
            ShaderProperty(normalMap1Strength, normalMap1Strength.displayName);
        }

        DrawUILine(smLineColor, 1, smLinePadding);

        //--------------

        MaterialProperty normalMap2 = allProps.Find(prop => prop.name == "_NormalMap2");
        MaterialProperty normalMap2Tiling = allProps.Find(prop => prop.name == "_NormalMap2Tiling");
        MaterialProperty normalMap2Strength = allProps.Find(prop => prop.name == "_NormalMap2Strength");
        MaterialProperty normalMap2Flow = allProps.Find(prop => prop.name == "_NormalMap2Flow");
        IEnumerable<bool> enableNormalMap2 = targets.Select(t => ((Material)t).shaderKeywords.Contains("EFFECT_NORMALMAP2"));

        if (enableNormalMap2 != null && normalMap2 != null && normalMap2Tiling != null && normalMap2Strength != null && normalMap2Flow != null)
        {
            allProps.Remove(normalMap2);
            allProps.Remove(normalMap2Tiling);
            allProps.Remove(normalMap2Strength);
            allProps.Remove(normalMap2Flow);

            bool? enable = EditorGUILayout.Toggle("Normal Map 2 Enable", enableNormalMap2.First());
            if (enable != null)
            {
                foreach (Material m in targets.Cast<Material>())
                {
                    if (enable.Value) m.EnableKeyword("EFFECT_NORMALMAP2");
                    else m.DisableKeyword("EFFECT_NORMALMAP2");
                }
            }
            if (enableNormalMap2.First())
            {
                ShaderProperty(normalMap2, normalMap2.displayName);
                ShaderProperty(normalMap2Tiling, normalMap2Tiling.displayName);
                ShaderProperty(normalMap2Strength, normalMap2Strength.displayName);
                ShaderProperty(normalMap2Flow, normalMap2Flow.displayName);
            }
        }

        //--------------

        if (enableNormalMap2 != null && enableNormalMap2.First())
        {
            DrawUILine(smLineColor, 1, smLinePadding);

            MaterialProperty microwaveScale = allProps.Find(prop => prop.name == "_MicrowaveScale");
            MaterialProperty microwaveStrength = allProps.Find(prop => prop.name == "_MicrowaveStrength");
            IEnumerable<bool> enableMicrowave = targets.Select(t => ((Material)t).shaderKeywords.Contains("EFFECT_MICROWAVE"));

            if (enableMicrowave != null && microwaveScale != null && microwaveStrength != null)
            {
                allProps.Remove(microwaveScale);
                allProps.Remove(microwaveStrength);

                bool? enable = EditorGUILayout.Toggle("Micro Waves Enable", enableMicrowave.First());
                if (enable != null)
                {
                    foreach (Material m in targets.Cast<Material>())
                    {
                        if (enable.Value) m.EnableKeyword("EFFECT_MICROWAVE");
                        else m.DisableKeyword("EFFECT_MICROWAVE");
                    }
                }
                if (enableMicrowave.First())
                {
                    ShaderProperty(microwaveScale, microwaveScale.displayName);
                    ShaderProperty(microwaveStrength, microwaveStrength.displayName);
                }
            }
        }
        else
        {
            foreach (Material m in targets.Cast<Material>()) m.DisableKeyword("EFFECT_MICROWAVE");
        }

        DrawUILine(smLineColor, 1, smLinePadding);

        //--------------

        MaterialProperty normalTessStrength = allProps.Find(prop => prop.name == "_NormalTessStrength");
        MaterialProperty normalTessOffset = allProps.Find(prop => prop.name == "_NormalTessOffset");

        if (normalTessStrength != null && normalTessOffset != null)
        {
            allProps.Remove(normalTessStrength);
            allProps.Remove(normalTessOffset);
            ShaderProperty(normalTessStrength, normalTessStrength.displayName);
            ShaderProperty(normalTessOffset, normalTessOffset.displayName);
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Tessellation(List<MaterialProperty> allProps)
    {
        //--------------

        DrawUILine(smLineColor, 1, smLinePadding);

        MaterialProperty tessellation = allProps.Find(prop => prop.name == "_Tessellation");
        MaterialProperty tessStart = allProps.Find(prop => prop.name == "_TessStart");
        MaterialProperty tessEnd = allProps.Find(prop => prop.name == "_TessEnd");

        if (tessellation != null && tessStart != null && tessEnd != null)
        {
            allProps.Remove(tessellation);
            allProps.Remove(tessStart);
            allProps.Remove(tessEnd);
            ShaderProperty(tessellation, tessellation.displayName);
            ShaderProperty(tessStart, tessStart.displayName);
            ShaderProperty(tessEnd, tessEnd.displayName);
        }

        //--------------

        DrawUILine(smLineColor, 1, smLinePadding);


        MaterialProperty tessNoise1Octaves = allProps.Find(prop => prop.name == "_TessNoise1Octaves");
        MaterialProperty tessSetNoise1 = allProps.Find(prop => prop.name == "_TessSetNoise1");

        if (tessSetNoise1 != null && tessNoise1Octaves != null)
        {
            allProps.Remove(tessNoise1Octaves);
            allProps.Remove(tessSetNoise1);
            ShaderProperty(tessNoise1Octaves, tessNoise1Octaves.displayName);
            ShaderProperty(tessSetNoise1, tessSetNoise1.displayName);
        }

        EditorGUILayout.HelpBox("X - Flow; Y - Gain; Z - Frequency; W - Amplitude", MessageType.None);

        //--------------

        DrawUILine(smLineColor, 1, smLinePadding);

        MaterialProperty tessSetNoise2 = allProps.Find(prop => prop.name == "_TessSetNoise2");
        MaterialProperty tessSetNoise3 = allProps.Find(prop => prop.name == "_TessSetNoise3");
        MaterialProperty tessSetNoise4 = allProps.Find(prop => prop.name == "_TessSetNoise4");
        MaterialProperty tessSetNoiseAngle2 = allProps.Find(prop => prop.name == "_TessSetNoiseAngle2");
        MaterialProperty tessSetNoiseAngle3 = allProps.Find(prop => prop.name == "_TessSetNoiseAngle3");
        MaterialProperty tessSetNoiseAngle4 = allProps.Find(prop => prop.name == "_TessSetNoiseAngle4");

        if (tessSetNoise2 != null && tessSetNoise3 != null && tessSetNoise4 != null && tessSetNoiseAngle2 != null && tessSetNoiseAngle3 != null && tessSetNoiseAngle4 != null)
        {
            allProps.Remove(tessSetNoise2);
            allProps.Remove(tessSetNoise3);
            allProps.Remove(tessSetNoise4);
            allProps.Remove(tessSetNoiseAngle2);
            allProps.Remove(tessSetNoiseAngle3);
            allProps.Remove(tessSetNoiseAngle4);
            ShaderProperty(tessSetNoiseAngle2, tessSetNoiseAngle2.displayName);
            ShaderProperty(tessSetNoise2, tessSetNoise2.displayName);
            ShaderProperty(tessSetNoiseAngle3, tessSetNoiseAngle3.displayName);
            ShaderProperty(tessSetNoise3, tessSetNoise3.displayName);
            ShaderProperty(tessSetNoiseAngle4, tessSetNoiseAngle4.displayName);
            ShaderProperty(tessSetNoise4, tessSetNoise4.displayName);
        }

        EditorGUILayout.HelpBox("X - Flow; Y - Gain; Z - Wavelength; W - Amplitude", MessageType.None);

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Parallax(List<MaterialProperty> allProps)
    {
        //--------------

        DrawUILine(smLineColor, 1, smLinePadding);

        MaterialProperty parallaxAmount = allProps.Find(prop => prop.name == "_ParallaxAmount");
        MaterialProperty parallaxNormal2Offset = allProps.Find(prop => prop.name == "_ParallaxNormal2Offset");
        MaterialProperty parallaxNoiseOctaves = allProps.Find(prop => prop.name == "_ParallaxNoiseOctaves");
        MaterialProperty parallaxNoise = allProps.Find(prop => prop.name == "_ParallaxNoise");
        IEnumerable<bool> enableParallax = targets.Select(t => ((Material)t).shaderKeywords.Contains("EFFECT_PARALLAX"));

        if (enableParallax != null && parallaxAmount != null && parallaxNormal2Offset != null && parallaxNoiseOctaves != null && parallaxNoise != null)
        {
            allProps.Remove(parallaxAmount);
            allProps.Remove(parallaxNormal2Offset);
            allProps.Remove(parallaxNoiseOctaves);
            allProps.Remove(parallaxNoise);

            bool? enable = EditorGUILayout.Toggle("Parallax Enable", enableParallax.First());
            if (enable != null)
            {
                foreach (Material m in targets.Cast<Material>())
                {
                    if (enable.Value) m.EnableKeyword("EFFECT_PARALLAX");
                    else m.DisableKeyword("EFFECT_PARALLAX");
                }
            }
            if (enableParallax.First())
            {
                ShaderProperty(parallaxAmount, parallaxAmount.displayName);
                ShaderProperty(parallaxNormal2Offset, parallaxNormal2Offset.displayName);
                ShaderProperty(parallaxNoiseOctaves, parallaxNoiseOctaves.displayName);
                ShaderProperty(parallaxNoise, parallaxNoise.displayName);
                EditorGUILayout.HelpBox("X - Flow; Y - Gain; Z - Frequency; W - Amplitude", MessageType.None);
            }
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Reflection(List<MaterialProperty> allProps)
    {
        //--------------

        DrawUILine(smLineColor, 1, smLinePadding);

        MaterialProperty reflectionCube = allProps.Find(prop => prop.name == "_ReflectionCube");
        MaterialProperty reflectionColor = allProps.Find(prop => prop.name == "_ReflectionColor");
        MaterialProperty reflectionStrength = allProps.Find(prop => prop.name == "_ReflectionStrength");
        MaterialProperty reflectionSaturation = allProps.Find(prop => prop.name == "_ReflectionSaturation");
        MaterialProperty reflectionContrast = allProps.Find(prop => prop.name == "_ReflectionContrast");
        IEnumerable<bool> enableReflectionCube = targets.Select(t => ((Material)t).shaderKeywords.Contains("EFFECT_REFLECTION"));

        if (enableReflectionCube != null && reflectionCube != null && reflectionColor != null && reflectionStrength != null && reflectionSaturation != null && reflectionContrast != null)
        {
            allProps.Remove(reflectionCube);
            allProps.Remove(reflectionColor);
            allProps.Remove(reflectionStrength);
            allProps.Remove(reflectionSaturation);
            allProps.Remove(reflectionContrast);

            bool? enable = EditorGUILayout.Toggle("Reflection Cubemap Enable", enableReflectionCube.First());
            if (enable != null)
            {
                foreach (Material m in targets.Cast<Material>())
                {
                    if (enable.Value) m.EnableKeyword("EFFECT_REFLECTION");
                    else m.DisableKeyword("EFFECT_REFLECTION");
                }
            }
            if (enableReflectionCube.First())
            {
                ShaderProperty(reflectionCube, reflectionCube.displayName);
                ShaderProperty(reflectionColor, reflectionColor.displayName);
                ShaderProperty(reflectionStrength, reflectionStrength.displayName);
                ShaderProperty(reflectionSaturation, reflectionSaturation.displayName);
                ShaderProperty(reflectionContrast, reflectionContrast.displayName);
            }
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void MirrorReflection(List<MaterialProperty> allProps)
    {
        //--------------

        DrawUILine(smLineColor, 1, smLinePadding);

        MaterialProperty mirrorColor = allProps.Find(prop => prop.name == "_MirrorColor");
        MaterialProperty mirrorDepthColor = allProps.Find(prop => prop.name == "_MirrorDepthColor");
        MaterialProperty mirrorFPOW = allProps.Find(prop => prop.name == "_MirrorFPOW");
        MaterialProperty mirrorR0 = allProps.Find(prop => prop.name == "_MirrorR0");
        MaterialProperty mirrorStrength = allProps.Find(prop => prop.name == "_MirrorStrength");
        MaterialProperty mirrorSaturation = allProps.Find(prop => prop.name == "_MirrorSaturation");
        MaterialProperty mirrorContrast = allProps.Find(prop => prop.name == "_MirrorContrast");
        MaterialProperty mirrorWavePow = allProps.Find(prop => prop.name == "_MirrorWavePow");
        IEnumerable<bool> enableMirrorReflection = targets.Select(t => ((Material)t).shaderKeywords.Contains("EFFECT_MIRROR"));

        if (enableMirrorReflection != null && mirrorColor != null && mirrorDepthColor != null && mirrorFPOW != null && mirrorR0 != null &&
            mirrorStrength != null && mirrorSaturation != null && mirrorContrast != null && mirrorWavePow != null)
        {
            allProps.Remove(mirrorColor);
            allProps.Remove(mirrorDepthColor);
            allProps.Remove(mirrorFPOW);
            allProps.Remove(mirrorR0);
            allProps.Remove(mirrorStrength);
            allProps.Remove(mirrorSaturation);
            allProps.Remove(mirrorContrast);
            allProps.Remove(mirrorWavePow);

            bool? enable = EditorGUILayout.Toggle("Mirror Reflection Enable", enableMirrorReflection.First());
            if (enable != null)
            {
                foreach (Material m in targets.Cast<Material>())
                {
                    if (enable.Value) m.EnableKeyword("EFFECT_MIRROR");
                    else m.DisableKeyword("EFFECT_MIRROR");
                }
            }
            if (enableMirrorReflection.First())
            {
                ShaderProperty(mirrorColor, mirrorColor.displayName);
                ShaderProperty(mirrorDepthColor, mirrorDepthColor.displayName);
                ShaderProperty(mirrorFPOW, mirrorFPOW.displayName);
                ShaderProperty(mirrorR0, mirrorR0.displayName);
                ShaderProperty(mirrorStrength, mirrorStrength.displayName);
                ShaderProperty(mirrorSaturation, mirrorSaturation.displayName);
                ShaderProperty(mirrorContrast, mirrorContrast.displayName);
                ShaderProperty(mirrorWavePow, mirrorWavePow.displayName);
            }
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    void Foam(List<MaterialProperty> allProps)
    {
        //--------------

        DrawUILine(smLineColor, 1, smLinePadding);

        MaterialProperty foamColor = allProps.Find(prop => prop.name == "_FoamColor");
        MaterialProperty foamNoiseOctaves = allProps.Find(prop => prop.name == "_FoamNoiseOctaves");
        MaterialProperty foamNoise = allProps.Find(prop => prop.name == "_FoamNoise");
        MaterialProperty foamSoft = allProps.Find(prop => prop.name == "_FoamShoreSoft");
        MaterialProperty foamWavePow = allProps.Find(prop => prop.name == "_FoamWavePow");
        MaterialProperty foamShorePow = allProps.Find(prop => prop.name == "_FoamShorePow");
        IEnumerable<bool> enableFoam = targets.Select(t => ((Material)t).shaderKeywords.Contains("EFFECT_FOAM"));

        if (enableFoam != null && foamColor != null && foamNoiseOctaves != null && foamNoise != null && foamSoft != null && foamWavePow != null && foamShorePow != null)
        {
            allProps.Remove(foamColor);
            allProps.Remove(foamNoiseOctaves);
            allProps.Remove(foamNoise);
            allProps.Remove(foamSoft);
            allProps.Remove(foamWavePow);
            allProps.Remove(foamShorePow);

            bool? enable = EditorGUILayout.Toggle("Foam Enable", enableFoam.First());
            if (enable != null)
            {
                foreach (Material m in targets.Cast<Material>())
                {
                    if (enable.Value) m.EnableKeyword("EFFECT_FOAM");
                    else m.DisableKeyword("EFFECT_FOAM");
                }
            }
            if (enableFoam.First())
            {
                ShaderProperty(foamColor, foamColor.displayName);
                ShaderProperty(foamWavePow, foamWavePow.displayName);
                ShaderProperty(foamShorePow, foamShorePow.displayName);
                ShaderProperty(foamSoft, foamSoft.displayName);
                DrawUILine(smLineColor, 1, smLinePadding);
                ShaderProperty(foamNoiseOctaves, foamNoiseOctaves.displayName);
                ShaderProperty(foamNoise, foamNoise.displayName);
                EditorGUILayout.HelpBox("For all the foam. X - Flow; Y - Gain; Z - Frequency; W - Amplitude", MessageType.None);
            }
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public static void Header()
    {
        //--------------

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUIStyle myLabelStyle = new GUIStyle();
        myLabelStyle.fontSize = 19;
        myLabelStyle.alignment = TextAnchor.MiddleCenter;
        EditorGUILayout.LabelField("#NVJOB Advanced Water Shaders", myLabelStyle);

        EditorGUILayout.Space();

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public static void Information()
    {
        //--------------

        if (GUILayout.Button("Description and Instructions")) Help.BrowseURL("https://nvjob.github.io/unity/nvjob-advanced-water-shaders");
        if (GUILayout.Button("#NVJOB Store")) Help.BrowseURL("https://nvjob.github.io/store/");

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        //--------------

        Rect line = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        line.height = thickness;
        line.y += padding / 2;
        line.x -= 2;
        EditorGUI.DrawRect(line, color);

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


[CustomEditor(typeof(AdvancedWaterShaders))]
[CanEditMultipleObjects]

public class NVWaterShaderEditor : Editor
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    Color smLineColor = Color.HSVToRGB(0, 0, 0.55f), bgLineColor = Color.HSVToRGB(0, 0, 0.3f);
    int smLinePadding = 20, bgLinePadding = 35;

    //--------------

    SerializedProperty rotateSpeed, rotateDistance, depthTextureModeOn, windZone, waterSyncWind;
    SerializedProperty mirrorOn, mirrorBackSide, textureSize, clipPlaneOffset, reflectLayers;
    SerializedProperty garbageCollection, waterMaterial;

    static bool showWaterMovement, showDepthMode, showWindZone, showMirror, showGarbage;


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    static void CreatePlane(Material material)
    {
        //--------------

        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        obj.name = "Advanced Water Shaders";
        obj.transform.localScale = new Vector3(100, 1, 100);
        obj.transform.position = Vector3.zero;
        Renderer rend = obj.GetComponent<Renderer>();
        rend.material = material;
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rend.receiveShadows = false;
        DestroyImmediate(obj.GetComponent<MeshCollider>());
        obj.AddComponent<AdvancedWaterShaders>().waterMaterial = material;

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    [MenuItem("#NVJOB/Create Advanced Water Shader (Surface)")]
    static void CreateAWSSurface()
    {
        //--------------

        Material material = new Material(Shader.Find("#NVJOB/Advanced Water Shaders/Surface"));
        CreatePlane(material);
        string folder = "#NVJOB Advanced Water Shaders/Materials";
        if (AssetDatabase.IsValidFolder("Assets/" + folder) == false)AssetDatabase.CreateFolder("Assets/#NVJOB Advanced Water Shaders", "Materials");
        string nameMat = "Water Shader Surface " + Random.Range(111111, 999999) + ".mat";
        AssetDatabase.CreateAsset(material, "Assets/"+ folder + "/" + nameMat);
        Debug.Log("Create Advanced Water Shaders. \n Material: Assets/" + folder + "/" + nameMat);

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    [MenuItem("#NVJOB/Create Advanced Water Shader (Specular)")]
    static void CreateAWSSpecular()
    {
        //--------------

        Material material = new Material(Shader.Find("#NVJOB/Advanced Water Shaders/Specular"));
        CreatePlane(material);
        string folder = "#NVJOB Advanced Water Shaders/Materials";
        if (AssetDatabase.IsValidFolder("Assets/" + folder) == false) AssetDatabase.CreateFolder("Assets/#NVJOB Advanced Water Shaders", "Materials");
        string nameMat = "Water Shader Specular " + Random.Range(111111, 999999) + ".mat";
        AssetDatabase.CreateAsset(material, "Assets/" + folder + "/" + nameMat);
        Debug.Log("Create Advanced Water Shaders. \n Material: Assets/" + folder + "/" + nameMat);

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void OnEnable()
    {
        //--------------

        waterMaterial = serializedObject.FindProperty("waterMaterial");
        rotateSpeed = serializedObject.FindProperty("rotateSpeed");
        rotateDistance = serializedObject.FindProperty("rotateDistance");
        depthTextureModeOn = serializedObject.FindProperty("depthTextureModeOn");
        waterSyncWind = serializedObject.FindProperty("waterSyncWind");
        windZone = serializedObject.FindProperty("windZone");
        mirrorOn = serializedObject.FindProperty("mirrorOn");
        mirrorBackSide = serializedObject.FindProperty("mirrorBackSide");
        textureSize = serializedObject.FindProperty("textureSize");
        clipPlaneOffset = serializedObject.FindProperty("clipPlaneOffset");
        reflectLayers = serializedObject.FindProperty("reflectLayers");
        garbageCollection = serializedObject.FindProperty("garbageCollection");

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public override void OnInspectorGUI()
    {
        //--------------

        serializedObject.Update();

        //--------------

        EditorGUI.BeginChangeCheck();
        AWSEditor.Header();

        GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        myFoldoutStyle.fontStyle = FontStyle.Bold;
        myFoldoutStyle.fontSize = 13;
        myFoldoutStyle.margin = new RectOffset(28, 0, 0, 0);

        //--------------

        AWSEditor.DrawUILine(bgLineColor, 2, bgLinePadding);
        showWaterMovement = EditorGUILayout.Foldout(showWaterMovement, "Water Global", true, myFoldoutStyle);
        if (showWaterMovement)
        {   
            AWSEditor.DrawUILine(smLineColor, 1, smLinePadding);
            EditorGUILayout.PropertyField(waterMaterial, new GUIContent("Water Material"));
            EditorGUILayout.PropertyField(rotateSpeed, new GUIContent("Rotate Speed"));
            EditorGUILayout.PropertyField(rotateDistance, new GUIContent("Movement Distance"));
        }

        //--------------

        AWSEditor.DrawUILine(bgLineColor, 2, bgLinePadding);
        showDepthMode = EditorGUILayout.Foldout(showDepthMode, "Depth Texture Mode", true, myFoldoutStyle);
        if (showDepthMode)
        {
            AWSEditor.DrawUILine(smLineColor, 1, smLinePadding);
            EditorGUILayout.PropertyField(depthTextureModeOn, new GUIContent("Depth Texture Mode"));
            EditorGUILayout.HelpBox("!!! For working shaders on mobile platforms with Forward Rendering. If you use mobile platforms, enable HDR for proper operation (Project Settings / Graphics).", MessageType.None);
        }

        //--------------

        AWSEditor.DrawUILine(bgLineColor, 2, bgLinePadding);
        showWindZone = EditorGUILayout.Foldout(showWindZone, "Wind Zone", true, myFoldoutStyle);
        if (showWindZone)
        {
            AWSEditor.DrawUILine(smLineColor, 1, smLinePadding);
            EditorGUILayout.PropertyField(waterSyncWind, new GUIContent("Water Sync With Wind"));
            EditorGUILayout.PropertyField(windZone, new GUIContent("Wind Zone Object"));
            EditorGUILayout.HelpBox("Optional. To synchronize the wind direction with the direction of water movement.", MessageType.None);
        }

        //--------------

        AWSEditor.DrawUILine(bgLineColor, 2, bgLinePadding);
        showMirror = EditorGUILayout.Foldout(showMirror, "Mirror Reflection", true, myFoldoutStyle);
        if (showMirror)
        {
            AWSEditor.DrawUILine(smLineColor, 1, smLinePadding);
            EditorGUILayout.PropertyField(mirrorOn, new GUIContent("Mirror Reflection Enable"));
            EditorGUILayout.PropertyField(mirrorBackSide, new GUIContent("Mirror Back Side"));
            EditorGUILayout.PropertyField(textureSize, new GUIContent("Mirror Texture Size"));
            EditorGUILayout.PropertyField(clipPlaneOffset, new GUIContent("Clipping plane offset"));
            EditorGUILayout.PropertyField(reflectLayers, new GUIContent("Reflection Layers"));
        }

        //--------------


        AWSEditor.DrawUILine(bgLineColor, 2, bgLinePadding);
        showGarbage = EditorGUILayout.Foldout(showGarbage, "Garbage Collection", true, myFoldoutStyle);
        if (showGarbage)
        {
            AWSEditor.DrawUILine(smLineColor, 1, smLinePadding);
            EditorGUILayout.PropertyField(garbageCollection, new GUIContent("Garbage Collection"));
        }

        //--------------

        serializedObject.ApplyModifiedProperties();
        AWSEditor.DrawUILine(bgLineColor, 2, bgLinePadding);
        AWSEditor.Information();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorUtility.SetDirty(target);

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}