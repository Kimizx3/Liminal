using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Umbra {

    [CustomEditor(typeof(UmbraSoftShadows))]
    public class UmbraSoftShadowsEditor : Editor {

        SerializedProperty profile, debugShadows;

        static UmbraPreset preset = UmbraPreset.None;
        static GUIStyle boxStyle;
        UmbraProfile cachedProfile;
        Editor cachedProfileEditor;

        private void OnEnable() {
            profile = serializedObject.FindProperty("profile");
            debugShadows = serializedObject.FindProperty("debugShadows");
        }

        public override void OnInspectorGUI() {

            if (boxStyle == null) {
                boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.padding = new RectOffset(15, 10, 5, 5);
            }

            UniversalRenderPipelineAsset pipe = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (pipe == null) {
                EditorGUILayout.HelpBox("Universal Rendering Pipeline asset is not set in 'Project Settings / Graphics' !", MessageType.Error);
                EditorGUILayout.Separator();
            } else if (!UmbraSoftShadows.installed) {
                EditorGUILayout.HelpBox("Umbra Render Feature must be added to the rendering pipeline renderer.", MessageType.Error);
                if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                    Selection.activeObject = pipe;
                }
                EditorGUILayout.Separator();
            }

            EditorGUILayout.BeginHorizontal();
            preset = (UmbraPreset)EditorGUILayout.EnumPopup(new GUIContent("Sample Preset"), preset);
            if (GUILayout.Button("Apply", GUILayout.Width(60))) {
                UmbraSoftShadows settings = (UmbraSoftShadows)target;
                settings.profile.ApplyPreset(preset);
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.Update();

            EditorGUILayout.PropertyField(profile);
            EditorGUILayout.PropertyField(debugShadows);

            if (profile.objectReferenceValue != null) {
                if (cachedProfile != profile.objectReferenceValue) {
                    cachedProfile = null;
                }
                if (cachedProfile == null) {
                    cachedProfile = (UmbraProfile)profile.objectReferenceValue;
                    cachedProfileEditor = CreateEditor(profile.objectReferenceValue);
                }

                // Drawing the profile editor
                EditorGUILayout.BeginVertical(boxStyle);
                cachedProfileEditor.OnInspectorGUI();

                EditorGUILayout.Separator();

                if (GUILayout.Button("Save As New Profile")) {
                    ExportProfile();
                }
                EditorGUILayout.EndVertical();
            } else {
                EditorGUILayout.HelpBox("Create or assign a profile.", MessageType.Info);
                if (GUILayout.Button("New Profile")) {
                    CreateProfile();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        void CreateProfile() {

            var fp = CreateInstance<UmbraProfile>();
            fp.name = "New Umbra Profile";

            string path = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets)) {
                path = AssetDatabase.GetAssetPath(obj);
                if (File.Exists(path)) {
                    path = Path.GetDirectoryName(path);
                }
                break;
            }

            string fullPath = path + "/" + fp.name + ".asset";
            fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

            AssetDatabase.CreateAsset(fp, fullPath);
            AssetDatabase.SaveAssets();
            profile.objectReferenceValue = fp;
            EditorGUIUtility.PingObject(fp);
        }

        void ExportProfile() {
            var fp = (UmbraProfile)profile.objectReferenceValue;
            var newProfile = Instantiate(fp);

            string path = AssetDatabase.GetAssetPath(fp);
            string fullPath = path;
            if (string.IsNullOrEmpty(path)) {
                path = "Assets";
                foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets)) {
                    path = AssetDatabase.GetAssetPath(obj);
                    if (File.Exists(path)) {
                        path = Path.GetDirectoryName(path);
                    }
                    break;
                }
                fullPath = path + "/" + fp.name + ".asset";
            }
            fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);
            AssetDatabase.CreateAsset(newProfile, fullPath);
            AssetDatabase.SaveAssets();
            profile.objectReferenceValue = newProfile;
            EditorGUIUtility.PingObject(fp);

        }

    }

}