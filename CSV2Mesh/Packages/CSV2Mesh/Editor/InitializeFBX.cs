using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
using System.IO;
using System.Linq;

namespace InitializeFBX
{
    public class InitializeFBX : EditorWindow
    {

        [SerializeField]
        protected List<Object> ModelAsset = new List<Object>();
        protected SerializedObject ModelsObject;

        Vector2 scroll;

        [MenuItem("Tools/InitializeFBX")]
        static void Open()
        {
            InitializeFBX window = (InitializeFBX)EditorWindow.GetWindow(typeof(InitializeFBX));
        }

        private void OnGUI()
        {
            ModelsObject = new SerializedObject(this);
            ModelsObject.Update();

            EditorGUILayout.BeginVertical();

            scroll = EditorGUILayout.BeginScrollView(scroll);

            EditorGUILayout.PropertyField(ModelsObject.FindProperty("ModelAsset"), true);
            ModelsObject.ApplyModifiedProperties();

            EditorGUILayout.EndScrollView();

            Rect botton = EditorGUILayout.BeginHorizontal("Button");
            if (GUI.Button(botton, GUIContent.none))
            {
                foreach (var item in ModelAsset)
                {
                    string newName = item.name.Substring(9, item.name.Length-9);
                    item.name = newName;
                    string assetPath = AssetDatabase.GetAssetPath(item);
                    AssetDatabase.RenameAsset(assetPath, newName);

                    Undo.RecordObject(item, "");
                }
            }
            GUILayout.Label("ReName Assets", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            EditorGUILayout.EndHorizontal();

            Rect botton2 = EditorGUILayout.BeginHorizontal("Button");
            if (GUI.Button(botton2, GUIContent.none))
            {
                foreach (var item in ModelAsset)
                {
                    string assetPath = AssetDatabase.GetAssetPath(item);
                    ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                    importer.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnMaterialName, ModelImporterMaterialSearch.Everywhere);
                    importer.SaveAndReimport();

                    Undo.RecordObject(item, "");
                }
            }
            GUILayout.Label("Remap Materials", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            EditorGUILayout.EndHorizontal();

            Rect botton3 = EditorGUILayout.BeginHorizontal("Button");
            if (GUI.Button(botton3, GUIContent.none))
            {
                ModelAsset.Clear();
            }
            GUILayout.Label("Clear List", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.EndVertical();
        }

        
    }
}