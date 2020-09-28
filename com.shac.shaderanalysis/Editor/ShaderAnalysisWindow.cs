using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class ShaderAnalysisWindow : EditorWindow
{
    ShaderAnalysisWindow()
    {
        this.titleContent = new GUIContent("Shader Analysis");
    }

    private string _analysisResult = "";
    private Vector2 scrollPos;
    private Vector2 scrollPos2;
    private bool showLastFiles = false;

    [MenuItem("SH_AC/Shadow Analysis Window")]
    static void showWindow()
    {
        var window = EditorWindow.GetWindow(typeof(ShaderAnalysisWindow));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();

        

        ShaderListArea();

        GUILayout.Space(5);
        GUI.skin.label.fontSize = 18;
        GUILayout.Label("Analysis Shader Result");
        scrollPos2 = GUILayout.BeginScrollView(scrollPos2,GUILayout.MaxHeight(350));
        if (_analysisResult == "")
        {
            GUILayout.TextArea(_analysisResult,GUILayout.Height(340));
        }
        else
        {
            GUILayout.TextArea(_analysisResult);
        }
        EditorGUILayout.EndScrollView();
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUI.skin.label.fontSize = 11;
        GUI.skin.label.fontStyle = FontStyle.BoldAndItalic;
        GUILayout.Label("Output file path: Temp/ShaderAnalysis/FinalOutput.txt");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Clear Output", GUILayout.Width(120)))
        {
            OnClickResetButton();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void ShaderListArea()
    {
        GUILayout.Space(10);
        GUI.skin.label.fontSize = 18;
        GUILayout.Label("Compiled Shader List");
        GUILayout.BeginHorizontal();
        showLastFiles = GUILayout.Toggle(showLastFiles, "Show last modified files");
      //  GUILayout.Space(15);
        if (GUILayout.Button("Delete All", GUILayout.Width(115)))
        {
            OnClickClearButton();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        var folderPath = Application.dataPath;
        folderPath = folderPath.Replace("Assets", "Temp");

        var fileList = Directory.GetFiles(folderPath);
        List<string> shaderList = new List<string>();
        foreach(var fileName in fileList)
        {
            if (fileName.Contains(".shader") && fileName.Contains("Compiled"))
            {
                shaderList.Add(fileName);
            }
        }

        var _shaderList = shaderList.ToArray();
        int minCount = _shaderList.Length;
        if (showLastFiles)
        {
            Array.Sort(_shaderList, (s1, s2) =>
            {
                var info1 = new FileInfo(s1);
                var info2 = new FileInfo(s2);
                if (info1.LastWriteTime < info2.LastWriteTime)
                    return 1;
                else if (info1.LastWriteTime > info2.LastWriteTime)
                    return -1;
                else return 0;
            });
            minCount = Math.Min(5, _shaderList.Length);
        }else if(minCount > 0)
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight(125));

        for (int i = 0;i < minCount; i++)
        {
            var shaderPath = _shaderList[i];
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(shaderPath.Replace(folderPath + "\\",""));
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("Analysis",GUILayout.Width(100)))
            {
                OnClickAnalysisButton(shaderPath);

            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3);
        }
        if (!showLastFiles && minCount > 0)
            GUILayout.EndScrollView();
        GUI.skin.label.fontSize = 11;
        GUI.skin.label.fontStyle = FontStyle.BoldAndItalic;
        if(minCount == 0)
            GUILayout.Label("There is no COMPILED shader file.");
        else
        {
            GUILayout.Space(5);
            if (showLastFiles)
                GUILayout.Label("Show the last 5 modified files. Sorted by last write time.");
        }
        
    }
    private void OnClickAnalysisButton(string shaderPath)
    {

        _analysisResult = MaliCompilerUtils.ShaderMaliAnalysis(shaderPath);
        Debug.Log(_analysisResult);
    }

    private void OnClickResetButton()
    {
        _analysisResult = "";
    }

    private void OnClickClearButton()
    {
        if (EditorUtility.DisplayDialog("Clear File", "Delete ALL compiled shader file?", "Delete", "Cancle"))
        {
            var folderPath = Application.dataPath;
            folderPath = folderPath.Replace("Assets", "Temp");

            var fileList = Directory.GetFiles(folderPath);
            foreach (var fileName in fileList)
            {
                if (fileName.Contains(".shader") && fileName.Contains("Compiled"))
                {
                    File.Delete(fileName);
                }
            }
        }
        _analysisResult = "";
    }

}
