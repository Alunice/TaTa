using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
using System.IO;
using System.Linq;

namespace CSV2Mesh
{
    public class CSV2Mesh : EditorWindow
    {

        [SerializeField]
        protected List<TextAsset> textAssets = new List<TextAsset>();
        protected SerializedObject textAssetsObject;

        Vector2 scroll;
        private static Shader defaultShader;
        private static List<GameObject> goList = new List<GameObject>();
        private static List<Object> ModelAsset = new List<Object>();

        [SerializeField]
        static Material defaultMaterial;
        [SerializeField]
        static float scaleFactor = 0.01f;
        [SerializeField]
        static bool rotationN90 = false;
        [SerializeField]
        static bool reverseUvY = false;
        [SerializeField]
        static bool reverseTris = false;

        [SerializeField]
        static TextAsset dataTypes;
        static int data_pos_id = 3;
        static int data_tangent_id = 6;
        static int data_normal_id = 10;
        static int data_color_id = 14;
        static int data_uv_id = 18;

        [MenuItem("Tools/CSV2Mesh")]
        static void Open()
        {
            CSV2Mesh window = (CSV2Mesh)EditorWindow.GetWindow(typeof(CSV2Mesh));
        }

        private void OnGUI()
        {
            textAssetsObject = new SerializedObject(this);
            textAssetsObject.Update();

            EditorGUILayout.BeginVertical();

            

            dataTypes = EditorGUILayout.ObjectField("DataTypes:", dataTypes, typeof(TextAsset), true) as TextAsset;
        
            if (dataTypes)
            {
                var Head = ReadHeads(dataTypes);

                defaultMaterial = EditorGUILayout.ObjectField("DefaultMaterial:", defaultMaterial, typeof(Material), true) as Material;
                scaleFactor = EditorGUILayout.FloatField("ScaleFactor:", scaleFactor);

                data_pos_id = EditorGUILayout.Popup("Postion: ", data_pos_id, Head.ToArray(), EditorStyles.popup);
                data_tangent_id = EditorGUILayout.Popup("Tangent: ", data_tangent_id, Head.ToArray(), EditorStyles.popup);
                data_normal_id = EditorGUILayout.Popup("Normal: ", data_normal_id, Head.ToArray(), EditorStyles.popup);           
                data_color_id = EditorGUILayout.Popup("Color: ", data_color_id, Head.ToArray(), EditorStyles.popup);
                data_uv_id = EditorGUILayout.Popup("UV: ", data_uv_id, Head.ToArray(), EditorStyles.popup);           
            

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Options:");
                rotationN90 = GUILayout.Toggle(rotationN90, "rotationN90", GUILayout.Width(100));
                reverseUvY = GUILayout.Toggle(reverseUvY, "reverseUvY", GUILayout.Width(100));
                reverseTris = GUILayout.Toggle(reverseTris, "reverseTris", GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();

                scroll = EditorGUILayout.BeginScrollView(scroll);

                EditorGUILayout.PropertyField(textAssetsObject.FindProperty("textAssets"), true);
                textAssetsObject.ApplyModifiedProperties();

                EditorGUILayout.EndScrollView();

                Rect botton = EditorGUILayout.BeginHorizontal("Button");
                if (GUI.Button(botton, GUIContent.none))
                {
                    foreach (var item in textAssets)
                    {
                        string path = AssetDatabase.GetAssetPath(item);
                        ImportMeshFromRenderDoc(path, rotationN90, reverseUvY, reverseTris,Head);
                        Undo.RecordObject(item, "");
                    }

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                GUILayout.Label("Convert to Mesh", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
                EditorGUILayout.EndHorizontal();


                Rect botton3 = EditorGUILayout.BeginHorizontal("Button");
                if (GUI.Button(botton3, GUIContent.none))
                {
                    textAssets.Clear();
                    foreach (var go in goList)
                    {
                        DestroyImmediate(go);
                    }
                    goList.Clear();
                }
                GUILayout.Label("Clear List", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
                EditorGUILayout.EndHorizontal();
            }

            

            EditorGUILayout.EndVertical();
        }

        public static List<string> ReadHeads(TextAsset asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            if (!System.IO.File.Exists(assetPath))
                return null;
        

            string clipboard = System.IO.File.ReadAllText(assetPath);
            var allTexts = clipboard.Split('\n');

            var heads = allTexts[0].Trim().Replace(" ", "").Split(',');

            List<string> headlist = new List<string>();
            foreach (var key in heads)
            {
                if (key.Contains("."))
                {
                    headlist.Add(key.Split('.')[0]);
                }
                else
                {
                    headlist.Add(key);
                }
            }
            return headlist;
        }

        public static int ImportMeshFromRenderDoc(string assetPath, bool needRotationN90, bool reverseUvY, bool reverseTris, List<string> head)
        {
            if (!System.IO.File.Exists(assetPath))
            {
                return -1;
            }

            string clipboard = System.IO.File.ReadAllText(assetPath);
            var allTexts = clipboard.Split('\n');

            if (allTexts.Length <= 1)
            {
                return -2;
            }
            var heads = allTexts[0].Trim().Replace(" ", "").Split(',');
            List<float[]> allRows = new List<float[]>();
            ReadAllRows(allTexts, heads.Length, ref allRows);

            var IDX = GetColumnIndex(heads, "IDX");
            var position_x = GetColumnIndex(heads, head[data_pos_id] + ".x");
            var position_y = GetColumnIndex(heads, head[data_pos_id] + ".y");
            var position_z = GetColumnIndex(heads, head[data_pos_id] + ".z");
            var uv_x = GetColumnIndex(heads, head[data_uv_id] + ".x");
            var uv_y = GetColumnIndex(heads, head[data_uv_id] + ".y");
            var tangent_x = GetColumnIndex(heads, head[data_tangent_id] + ".x");
            var tangent_y = GetColumnIndex(heads, head[data_tangent_id] + ".y");
            var tangent_z = GetColumnIndex(heads, head[data_tangent_id] + ".z");
            var tangent_w = GetColumnIndex(heads, head[data_tangent_id] + ".w");
            var normal_x = GetColumnIndex(heads, head[data_normal_id] + ".x");
            var normal_y = GetColumnIndex(heads, head[data_normal_id] + ".y");
            var normal_z = GetColumnIndex(heads, head[data_normal_id] + ".z");
            var color_x = GetColumnIndex(heads, head[data_color_id] + ".x");
            var color_y = GetColumnIndex(heads, head[data_color_id] + ".y");
            var color_z = GetColumnIndex(heads, head[data_color_id] + ".z");
            var color_w = GetColumnIndex(heads, head[data_color_id] + ".w");
                    

            if (IDX < 0 || position_x < 0 || position_y < 0 || position_z < 0
                || uv_x < 0 || uv_y < 0 )
            {
                Debug.Log("Don't have position or UV data.");
                return -3;
            }
            bool hasNormalProp = (normal_x >= 0 && normal_y >= 0 && normal_z >= 0);
            bool hasTangentProp = (tangent_x >= 0 && tangent_y >= 0 && tangent_z >= 0 && tangent_w >=0);
            bool hasColorProp = (color_x >= 0 && color_y >= 0 && color_z >= 0 && color_w >= 0);

            int minIndex = 65535;
            int maxIndex = 0;
            for (int i = 0; i < allRows.Count; ++i)
            {
            
                int currIndex = (int)allRows[i][IDX];
                if (currIndex < minIndex)
                {
                    minIndex = currIndex;
                }
                else if (currIndex > maxIndex)
                {
                    maxIndex = currIndex;
                }
            }

            int vertexLength = maxIndex - minIndex + 1; // Container Self Index.
            int indexLen = allRows.Count;
            if (indexLen % 3 != 0)
            {
                Debug.Log("vertex Length is zero.");
                return -4;
            }

            Vector3[] outputVertexs = new Vector3[vertexLength];
            Vector3[] outputNormals = new Vector3[vertexLength];
            Vector4[] outputTangents = new Vector4[vertexLength];
            Color[] outputColors = new Color[vertexLength];
            Vector2[] outputUvs = new Vector2[vertexLength];
            int[] outputIndexBuff = new int[indexLen];
            var rotationN90 = needRotationN90 ? Quaternion.Euler(-90, 0, 0) : Quaternion.identity;
            for (int i = 0; i < allRows.Count; ++i)
            {
                var currLine = allRows[i];
                var realIndex = (int)currLine[IDX] - minIndex;
                outputIndexBuff[i] = realIndex;
                if (realIndex < outputVertexs.Length && realIndex >= 0)
                {
                    var p = new Vector3(currLine[position_x], currLine[position_y], currLine[position_z]);
                    outputVertexs[realIndex] = rotationN90 * p;

                    outputVertexs[realIndex].x *= scaleFactor;
                    outputVertexs[realIndex].y *= scaleFactor;
                    outputVertexs[realIndex].z *= scaleFactor;

                    if (hasNormalProp)
                    {
                        var nor = new Vector3(currLine[normal_x], currLine[normal_y], currLine[normal_z]);
                        outputNormals[realIndex] = rotationN90 * nor;
                    }

                    if (hasTangentProp)
                    {
                        outputTangents[realIndex] = new Vector4(currLine[tangent_x], currLine[tangent_y], currLine[tangent_z], currLine[tangent_w]);
                    }

                    if (hasColorProp)
                    {
                        outputColors[realIndex] = new Color(currLine[color_x], currLine[color_y], currLine[color_z], currLine[color_w]);
                    }
                    outputUvs[realIndex] = new Vector2(currLine[uv_x], reverseUvY ? 1 - currLine[uv_y] : currLine[uv_y]);
                }
                else
                {
                    return -5;
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = outputVertexs;
            mesh.SetTriangles(outputIndexBuff, 0);
            mesh.uv = outputUvs;
            if (hasNormalProp)
            {
                mesh.normals = outputNormals;
            }
            else
            {
                mesh.RecalculateNormals();
            }

            if (hasNormalProp)
            {
                mesh.tangents = outputTangents;
            }
            else
            {
                mesh.RecalculateTangents();
            }

            if (hasColorProp)
            {
                mesh.colors = outputColors;
            }

            if (reverseTris)
            {
                mesh.triangles = mesh.triangles.Reverse().ToArray();
            }
            CreateMeshAssetAndShow(mesh,assetPath);
            return 0;
        }

        private static void CreateMeshAssetAndShow(Mesh mesh,string assetPath)
        {
            string outputName = "_" + System.DateTime.Now.Ticks.ToString();
            var outpath = assetPath.Split(char.Parse("."));

            Material matRes;
            if (defaultMaterial == null)
            {
                matRes = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
            }
            else
            {
                matRes = defaultMaterial;
            }


            GameObject newObj = new GameObject();
            var mf = newObj.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;
                
            var mr = newObj.AddComponent<MeshRenderer>();
            mr.sharedMaterial = matRes;

            goList.Add(newObj);


            ModelExporter.ExportObject(outpath[outpath.Length - 2], newObj);
        }


        private static void ReadAllRows(string[] allTexts, int headsLength, ref List<float[]> allRows)
        {
            for (int lineIndex = 1; lineIndex < allTexts.Length; ++lineIndex)
            {
                var lineText = allTexts[lineIndex];
                if (lineText.Length <= 10)
                {
                    continue;
                }

                var cells = lineText.Trim().Replace(" ", "").Split(',');
                if (cells.Length != headsLength)
                {
                    continue;
                }

                float[] cellData = new float[cells.Length];
                for (int i = 0; i < cells.Length; ++i)
                {
                    float v = 0;
                    if (float.TryParse(cells[i], out v))
                    {
                        cellData[i] = v;
                    }
                    else
                    {
                        Debug.Log("Don't have csv data.");
                    }
                }

                allRows.Add(cellData);
            }
        }

        public static int GetColumnIndex(string[] input, string key)
        {
            for(int i=0; i<input.Length; ++i)
            {
                if (input[i] == key)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}