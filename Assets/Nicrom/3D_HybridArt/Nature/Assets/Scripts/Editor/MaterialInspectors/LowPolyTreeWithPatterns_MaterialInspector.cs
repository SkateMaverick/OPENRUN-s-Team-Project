using UnityEditor;
using UnityEngine;

namespace Nicrom
{
    public class LowPolyTreeWithPatterns_MaterialInspector : ShaderGUI {

        MaterialProperty metallic = null;
        MaterialProperty smoothness = null;

        MaterialProperty colorOne = null;
        MaterialProperty colorTwo = null;

        MaterialProperty mainTex = null;
        MaterialProperty patternTex = null;
        MaterialProperty noiseTexture = null;
        MaterialProperty noiseTextureTiling = null;
        MaterialProperty noisePannerSpeed = null;

        MaterialProperty mbDefaultBending = null;
        MaterialProperty mbAmplitude = null;
        MaterialProperty mbAmplitudeOffset = null;
        MaterialProperty mbFrequency = null;
        MaterialProperty mbFrequencyOffset = null;
        MaterialProperty mbPhase = null;
        MaterialProperty mbWindDirection = null;
        MaterialProperty mbWindDirectionOffset = null;
        MaterialProperty mbWindDirBlend = null;
        MaterialProperty mbMaxHeight = null;

        MaterialProperty dbVerticalBendingToggle = null;
        MaterialProperty dbVerticalAmplitude = null;
        MaterialProperty dbVerticalAmplitudeOffset = null;
        MaterialProperty dbVerticalFrequency = null;
        MaterialProperty dbVerticalPhase = null;
        MaterialProperty dbVerticalMaxLength = null;

        MaterialProperty dbHorizontalBendingToggle = null;
        MaterialProperty dbHorizontalAmplitude = null;
        MaterialProperty dbHorizontalFrequency = null;
        MaterialProperty dbHorizontalPhase = null;
        MaterialProperty dbHorizontalMaxRadius = null;

        MaterialProperty unitScale = null;

        MaterialEditor matEditor;

        public void FindProperties(MaterialProperty[] mProps)
        {
            metallic                    = FindProperty("_Metallic", mProps);
            smoothness                  = FindProperty("_Smoothness", mProps);

            colorOne                    = FindProperty("_PatternColor1", mProps);
            colorTwo                    = FindProperty("_PatternColor2", mProps);

            mainTex                     = FindProperty("_MainTex", mProps);
            patternTex                  = FindProperty("_PatternTex", mProps);
            noiseTexture                = FindProperty("_NoiseTexture", mProps);
            noiseTextureTiling          = FindProperty("_NoiseTextureTilling", mProps);
            noisePannerSpeed            = FindProperty("_NoisePannerSpeed", mProps);

            mbDefaultBending            = FindProperty("_MBDefaultBending", mProps);
            mbAmplitude                 = FindProperty("_MBAmplitude", mProps);
            mbAmplitudeOffset           = FindProperty("_MBAmplitudeOffset", mProps);
            mbFrequency                 = FindProperty("_MBFrequency", mProps);
            mbFrequencyOffset           = FindProperty("_MBFrequencyOffset", mProps);
            mbPhase                     = FindProperty("_MBPhase", mProps);
            mbWindDirection             = FindProperty("_MBWindDir", mProps);
            mbWindDirectionOffset       = FindProperty("_MBWindDirOffset", mProps);
            mbWindDirBlend              = FindProperty("_MBWindDirBlend", mProps);
            mbMaxHeight                 = FindProperty("_MBMaxHeight", mProps);

            dbVerticalBendingToggle     = FindProperty("_EnableVerticalBending", mProps);
            dbVerticalAmplitude         = FindProperty("_DBVerticalAmplitude", mProps);
            dbVerticalAmplitudeOffset   = FindProperty("_DBVerticalAmplitudeOffset", mProps);
            dbVerticalFrequency         = FindProperty("_DBVerticalFrequency", mProps);
            dbVerticalPhase             = FindProperty("_DBVerticalPhase", mProps);
            dbVerticalMaxLength         = FindProperty("_DBVerticalMaxLength", mProps);

            dbHorizontalBendingToggle   = FindProperty("_EnableHorizontalBending", mProps);
            dbHorizontalAmplitude       = FindProperty("_DBHorizontalAmplitude", mProps);
            dbHorizontalFrequency       = FindProperty("_DBHorizontalFrequency", mProps);
            dbHorizontalPhase           = FindProperty("_DBHorizontalPhase", mProps);
            dbHorizontalMaxRadius       = FindProperty("_DBHorizontalMaxRadius", mProps);

            unitScale                   = FindProperty("_UnitScale", mProps);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] mProps)
        {
            matEditor = materialEditor;
            Material material = materialEditor.target as Material;

            FindProperties(mProps);
            ShaderPropertiesGUI(material);
        }

        public void ShaderPropertiesGUI(Material material)
        {
            EditorGUI.BeginChangeCheck();
            {
                EditorGUIUtility.fieldWidth = 64f;

                EditorGUILayout.Separator();
                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Surface"), EditorStyles.boldLabel);

                    GUILayout.Space(5);
                    matEditor.TexturePropertySingleLine(new GUIContent("Leaves Texture"), mainTex);
                    matEditor.TexturePropertySingleLine(new GUIContent("Bark Texture"), patternTex);
                    GUILayout.Space(5);
                    matEditor.ShaderProperty(colorOne, "Bark Color 1");
                    matEditor.ShaderProperty(colorTwo, "Bark Color 2");

                    GUILayout.Space(5);
                    matEditor.ShaderProperty(metallic, "Metallic");
                    matEditor.ShaderProperty(smoothness, "Smoothness");
                });

                EditorGUILayout.Separator();
                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Main Bending"), EditorStyles.boldLabel);
                    
                    GUILayout.Space(5);
                    matEditor.ShaderProperty(mbDefaultBending, new GUIContent("Default Bending", "The base bending applied to the model."));

                    GUILayout.Space(5);
                    matEditor.ShaderProperty(mbAmplitude, new GUIContent("Amplitude", "The amplitude of the main bending."));
                    matEditor.ShaderProperty(mbAmplitudeOffset, new GUIContent("Amplitude Offset", "The amplitude offset of the main bending. The value of this field is multiplied with a static noise value and added to the main bending amplitude."));

                    GUILayout.Space(5);
                    matEditor.ShaderProperty(mbFrequency, new GUIContent("Frequency", "The frequency of the main bending."));
                    matEditor.ShaderProperty(mbFrequencyOffset, new GUIContent("Frequency Offset", "The frequency offset of the main bending. The value of this field is multiplied with a static noise value and added to the main bending frequency"));

                    GUILayout.Space(5);
                    matEditor.ShaderProperty(mbPhase, new GUIContent("Phase", "The phase of the main bending. A phase shift is applied based on the position of the game object on the XZ axis. "
                        + "If the bending of the models that are close to each other is synchronous, try to increase the value of this field."));

                    GUILayout.Space(5);
                    matEditor.ShaderProperty(mbWindDirection, new GUIContent("Wind Dir", "The direction of the wind."));
                    matEditor.ShaderProperty(mbWindDirectionOffset, new GUIContent("Wind Dir Offset", "The wind direction offset. "
                        + "This value is multiplied with an animated noise value and added to the wind direction to create wind direction variation over time."));
                    matEditor.ShaderProperty(mbWindDirBlend, new GUIContent("Wind Dir Blend", "Determines the blending between the local direction of the wind and the global direction of the wind. "
                        + "When set to 0, the local wind direction will be used in the shader. When set to 1, the global wind direction will be used in the shader."));

                    GUILayout.Space(5);
                    matEditor.ShaderProperty(mbMaxHeight, new GUIContent("Max Height", "The height of the tallest model that uses this material. "
                        + "This value is used to calculate the final main bending amplitude of a vertex."));                 
                });

                EditorGUILayout.Separator();
                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Vertical Detail Bending"), EditorStyles.boldLabel);
                    matEditor.ShaderProperty(dbVerticalBendingToggle, new GUIContent("Enable", "Enables/Disables the detail vertical bending. This bending is applied only to the branches/leaves of a model."));

                    if (dbVerticalBendingToggle.floatValue == 1)
                    {
                        GUILayout.Space(5);
                        matEditor.ShaderProperty(dbVerticalAmplitude, new GUIContent("Amplitude", "The amplitude of the detail vertical bending."));
                        matEditor.ShaderProperty(dbVerticalAmplitudeOffset, new GUIContent("Amplitude Offset", "The amplitude offset of the detail vertical bending."));

                        GUILayout.Space(5);
                        matEditor.ShaderProperty(dbVerticalFrequency, new GUIContent("Frequency", "The frequency of the detail vertical bending."));
                        matEditor.ShaderProperty(dbVerticalPhase, new GUIContent("Phase", "The phase of the detail vertical bending. A phase shift is applied based on the position that the game object has on the XZ axis. "
                            + "If the vertical bending of the branches/leaves of the models that are close to each other, are synchronous, try to increase the value of this field."));

                        GUILayout.Space(5);
                        matEditor.ShaderProperty(dbVerticalMaxLength, new GUIContent("Max Length", "The length of the longest branch/leaf. " + "The value of this field is used to determine the final detail vertical bending amplitude of a vertex. "));
                    }
                });

                EditorGUILayout.Separator();
                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Horizontal Detail Bending"), EditorStyles.boldLabel);
                    matEditor.ShaderProperty(dbHorizontalBendingToggle, new GUIContent("Enable", "Enables/Disables the detail horizontal bending. This bending is applied only to the branches/leaves of a model."));

                    if (dbHorizontalBendingToggle.floatValue == 1)
                    {
                        GUILayout.Space(5);
                        matEditor.ShaderProperty(dbHorizontalAmplitude, new GUIContent("Amplitude", "The amplitude of the detail horizontal bending."));
                        matEditor.ShaderProperty(dbHorizontalFrequency, new GUIContent("Frequency", "The frequency of the detail horizontal bending."));
                        matEditor.ShaderProperty(dbHorizontalPhase, new GUIContent("Phase", "The phase of the detail horizontal bending. A phase shift is applied based on the position that the game object has on the XZ axis. "
                            + "If the horizontal bending of the branches/leaves of the models that are close to each other, are synchronous, try to increase the value of this field."));

                        GUILayout.Space(5);
                        matEditor.ShaderProperty(dbHorizontalMaxRadius, new GUIContent("Max Radius", "The radius on the XZ axis, of the biggest branch/leaf."));
                    }
                });

                EditorGUILayout.Separator();
                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("World Space Noise"), EditorStyles.boldLabel);

                    GUILayout.Space(5);
                    matEditor.TexturePropertySingleLine(new GUIContent("Noise Texture"), noiseTexture);
                    matEditor.ShaderProperty(noiseTextureTiling, new GUIContent("Noise Tiling: Static (XY), Animated (ZW)", "Noise texture tiling. "
                        + "The XY values are used for static noise tiling and ZW for animated noise tiling."));
                    matEditor.ShaderProperty(noisePannerSpeed, new GUIContent("Noise Panning Speed", "The panning speed of the noise texture."));
                });

                EditorGUILayout.Separator();
                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Misc"), EditorStyles.boldLabel);

                    matEditor.ShaderProperty(unitScale, new GUIContent("Unit Scale", "The unit scale is used to convert data from the 0-1 range to world space values. "
                        + "Placing an incorrect value in this field will result in the wind simulation not working correctly. "
                        + "Please consult the documentation to find what values should be placed in this field."));
                });

                EditorGUILayout.Separator();
                InspectorBox(10, () =>
                {
                    GUILayout.Label("Advanced Options", EditorStyles.boldLabel);
                    matEditor.EnableInstancingField();
                });
            }
        }

        public void InspectorBox(int aBorder, System.Action inside)
        {
            Rect r = EditorGUILayout.BeginHorizontal();

            GUI.Box(r, GUIContent.none);
            GUILayout.Space(aBorder);
            EditorGUILayout.BeginVertical();
            GUILayout.Space(aBorder);
            inside();
            GUILayout.Space(aBorder);
            EditorGUILayout.EndVertical();
            GUILayout.Space(aBorder);
            EditorGUILayout.EndHorizontal();
        }
    }
}
