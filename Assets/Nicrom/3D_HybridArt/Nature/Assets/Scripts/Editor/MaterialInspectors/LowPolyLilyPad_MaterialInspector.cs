using UnityEditor;
using UnityEngine;

namespace Nicrom {
    public class LowPolyLilyPad_MaterialInspector : ShaderGUI {

        MaterialProperty metallic = null;
        MaterialProperty smoothness = null;

        MaterialProperty mainTex = null;
        MaterialProperty noiseTexture = null;
        MaterialProperty noiseTextureTiling = null;
        MaterialProperty noisePannerSpeed = null;

        MaterialProperty horizontalAmplitude = null;
        MaterialProperty horizontalAmplitudeOffset = null;
        MaterialProperty horizontalFrequency = null;
        MaterialProperty horizontalFrequencyOffset = null;
        MaterialProperty horizontalPhase = null;
        MaterialProperty horizontalWindDirection = null;
        MaterialProperty horizontalWindDirectionOffset = null;
        MaterialProperty horizontalGlobalWind = null;

        MaterialProperty rotationToggle = null;
        MaterialProperty rotationAmplitude = null;
        MaterialProperty rotationAmplitudeOffset = null;
        MaterialProperty rotationFrequency = null;
        MaterialProperty rotationFrequencyOffset = null;
        MaterialProperty rotationPhase = null;

        MaterialEditor matEditor;

        public void FindProperties(MaterialProperty[] mProps)
        {
            metallic                            = FindProperty("_Metallic", mProps);
            smoothness                          = FindProperty("_Smoothness", mProps);

            mainTex                             = FindProperty("_MainTex", mProps);
            noiseTexture                        = FindProperty("_NoiseTexture", mProps);
            noiseTextureTiling                  = FindProperty("_NoiseTextureTilling", mProps);
            noisePannerSpeed                    = FindProperty("_NoisePannerSpeed", mProps);

            horizontalAmplitude                 = FindProperty("_HorizontalAmplitude", mProps);
            horizontalAmplitudeOffset           = FindProperty("_HorizontalAmplitudeOffset", mProps);
            horizontalFrequency                 = FindProperty("_HorizontalFrequency", mProps);
            horizontalFrequencyOffset           = FindProperty("_HorizontalFrequencyOffset", mProps);
            horizontalPhase                     = FindProperty("_HorizontalPhase", mProps);
            horizontalWindDirection             = FindProperty("_HorizontalWindDir", mProps);
            horizontalWindDirectionOffset       = FindProperty("_HorizontalWindDirOffset", mProps);
            horizontalGlobalWind                = FindProperty("_HorizontalWindDirBlend", mProps);
         
            rotationToggle                      = FindProperty("_EnableRotation", mProps);
            rotationAmplitude                   = FindProperty("_RotationAmplitude", mProps);
            rotationAmplitudeOffset             = FindProperty("_RotationAmplitudeOffset", mProps);
            rotationFrequency                   = FindProperty("_RotationFrequency", mProps);
            rotationFrequencyOffset             = FindProperty("_RotationFrequencyOffset", mProps);
            rotationPhase                       = FindProperty("_RotationPhase", mProps);
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
                    matEditor.TexturePropertySingleLine(new GUIContent("Albedo"), mainTex);

                    GUILayout.Space(5);
                    matEditor.ShaderProperty(metallic, "Metallic");
                    matEditor.ShaderProperty(smoothness, "Smoothness");
                });

                EditorGUILayout.Separator();
                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Horizontal Movement"), EditorStyles.boldLabel);

                    GUILayout.Space(5);
                    matEditor.ShaderProperty(horizontalAmplitude, new GUIContent("Amplitude", "The amplitude of the horizontal movement."));
                    matEditor.ShaderProperty(horizontalAmplitudeOffset, new GUIContent("Amplitude Offset", "The amplitude offset of the horizontal movement. The value of this field is multiplied with a static noise value and added to the horizontal amplitude"));

                    GUILayout.Space(5);
                    matEditor.ShaderProperty(horizontalFrequency, new GUIContent("Frequency", "The frequency of the horizontal movement."));
                    matEditor.ShaderProperty(horizontalFrequencyOffset, new GUIContent("Frequency Offset", "The frequency offset of the horizontal movement. The value of this field is multiplied with a static noise value and added to the horizontal frequency"));

                    GUILayout.Space(5);
                    matEditor.ShaderProperty(horizontalPhase, new GUIContent("Phase", "The phase of the horizontal movement. A phase shift is applied based on the position the game object has on the XZ axis."));

                    GUILayout.Space(5);
                    matEditor.ShaderProperty(horizontalWindDirection, new GUIContent("Wind Dir", "The direction of the wind"));
                    matEditor.ShaderProperty(horizontalWindDirectionOffset, new GUIContent("Wind Dir Offset", "The wind direction offset. "
                        + "This value is multiplied with an animated noise value and added to the wind direction to create wind direction variation over time."));
                    matEditor.ShaderProperty(horizontalGlobalWind, new GUIContent("Wind Dir Blend", "Determines the blending between the local direction of the wind and the global direction of the wind. "
                        + "When set to 0, the local wind direction will be used in the shader. When set to 1, the global wind direction will be used in the shader."));
                    
                });

                EditorGUILayout.Separator();
                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Rotation Around Local Pivot"), EditorStyles.boldLabel);
                    matEditor.ShaderProperty(rotationToggle, new GUIContent("Enable", "Enables/Disables the rotation of the lily pads around their local pivots."));

                    if (rotationToggle.floatValue == 1)
                    {
                        GUILayout.Space(5);
                        matEditor.ShaderProperty(rotationAmplitude, new GUIContent("Amplitude", "The amplitude of the rotation."));
                        matEditor.ShaderProperty(rotationAmplitudeOffset, new GUIContent("Amplitude Offset", "The amplitude offset of the rotation."));

                        GUILayout.Space(5);
                        matEditor.ShaderProperty(rotationFrequency, new GUIContent("Frequency", "The frequency of the rotation."));
                        matEditor.ShaderProperty(rotationFrequencyOffset, new GUIContent("Frequency Offset", "The frequency offset of the rotation."));

                        GUILayout.Space(5);
                        matEditor.ShaderProperty(rotationPhase, new GUIContent("Phase", "The phase of the rotation. A phase shift is applied based on the position the game object has on the XZ axis."));
                    }
                });

                EditorGUILayout.Separator();
                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("World Space Noise"), EditorStyles.boldLabel);

                    GUILayout.Space(5);
                    matEditor.TexturePropertySingleLine(new GUIContent("Noise Texture"), noiseTexture);

                    matEditor.ShaderProperty(noiseTextureTiling, new GUIContent("Noise Tilling: Static (XY), Animated (ZW)", "Noise texture tiling. "
                        + "XY values are used for static noise and ZW for animated noise. The static noise is used to calculate the final amplitude offset and frequency offset values. "
                        + "The animated noise is used to calculate the final wind direction offset. The wind direction offset changes based on the noise texture used and the values of the field Noise Panner Speed."));
                    matEditor.ShaderProperty(noisePannerSpeed, new GUIContent("Noise Panner Speed", "Noise texture panner speed."));
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
