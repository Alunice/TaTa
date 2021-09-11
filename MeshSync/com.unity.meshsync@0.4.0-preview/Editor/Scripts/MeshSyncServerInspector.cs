using UnityEditor;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace Unity.MeshSync.Editor  {
    [CustomEditor(typeof(MeshSyncServer))]
    internal class MeshSyncServerInspector : MeshSyncPlayerInspector   {
        
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------

        public override void OnEnable() {
            base.OnEnable();
        }

//----------------------------------------------------------------------------------------------------------------------
        public override void OnInspectorGUI()
        {
            var so = serializedObject;
            var t = target as MeshSyncServer;

            EditorGUILayout.Space();
            DrawServerSettings(t, so);
            DrawExport2Dcc(t);
            DrawPlayerSettings(t, so);
            DrawMaterialList(t);
            DrawTextureList(t);
            DrawAnimationTweak(t);
            DrawExportAssets(t);
            DrawPluginVersion();

            so.ApplyModifiedProperties();
        }

        public static void DrawServerSettings(MeshSyncServer t, SerializedObject so)
        {
            var styleFold = EditorStyles.foldout;
            styleFold.fontStyle = FontStyle.Bold;

            bool isServerStarted = t.IsServerStarted();
            string serverStatus = isServerStarted ? "Server (Status: Started)" : "Server (Status: Stopped)";
            t.foldServerSettings= EditorGUILayout.Foldout(t.foldServerSettings, serverStatus, true, styleFold);
            if (t.foldServerSettings) {
                
                bool autoStart = EditorGUILayout.Toggle("Auto Start", t.IsAutoStart());
                t.SetAutoStartServer(autoStart);

                //Draw GUI that are disabled when autoStart is true
                EditorGUI.BeginDisabledGroup(autoStart);
                int serverPort = EditorGUILayout.IntField("Server Port:", (int) t.GetServerPort());
                t.SetServerPort((ushort) serverPort);
                GUILayout.BeginHorizontal();
                if (isServerStarted) {
                    if (GUILayout.Button("Stop", GUILayout.Width(110.0f))) {
                        t.StopServer();
                    }
                } else {
                    if (GUILayout.Button("Start", GUILayout.Width(110.0f))) {
                        t.StartServer();
                    }
 
                }
                GUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
                
                EditorGUILayout.PropertyField(so.FindProperty("m_assetDir"));
                EditorGUILayout.PropertyField(so.FindProperty("m_rootObject"));
                EditorGUILayout.Space();
            }
        }

        //----------------------------------------------------------------------------------------------------------------------                
        private enum DccMode { Maya, Max };

        static DccMode currentDccMode = DccMode.Maya;

        private static string[] dccNames = {"maya","3dsmax"};

        private static int selectDccMode = 0;
        public static void DrawExport2Dcc(MeshSyncServer t)
        {

            var styleFold = EditorStyles.foldout;
            styleFold.fontStyle = FontStyle.Bold;

            // Export To DCC
            t.foldExportDCC = EditorGUILayout.Foldout(t.foldExportDCC, "Export2DCC", true, styleFold);

            if (t.foldExportDCC)
            {
                EditorGUI.BeginChangeCheck();
                GameObject rootObject = (GameObject)EditorGUILayout.ObjectField("导出的Prefab", t.GetExportObj(),
                   typeof(GameObject), allowSceneObjects: true);
                if (EditorGUI.EndChangeCheck())
                {
                    var lastobj = t.GetExportObj();
                    if (lastobj && lastobj.activeInHierarchy == false)
                        lastobj.SetActive(true);
                    if (rootObject != null)
                        t.SetExportObj(rootObject);
                }

                if (rootObject != null)
                {
                    t.SetExportObj(rootObject);

                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("DCC软件：");
                    selectDccMode = EditorGUILayout.Popup(selectDccMode, dccNames, GUILayout.Width(100));
                    currentDccMode = (DccMode)selectDccMode;
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);

                    t.autoHideObj = EditorGUILayout.Toggle("导出后自动隐藏源文件", t.autoHideObj);
                    t.autoUpdatePosDCC = EditorGUILayout.Toggle("自动同步坐标材质信息", t.autoUpdatePosDCC);
                    if (GUILayout.Button("导出到" + dccNames[selectDccMode]))
                    {
                        ExportSelect(t.GetExportObj().GetComponentsInChildren<MeshFilter>(),t);
                    }
                    string showSourceObj = t.GetExportObj().activeSelf ? "隐藏导出的原场景Prefab" : "显示导出的原场景Prefab";
                    if (GUILayout.Button(showSourceObj))
                    {
                        t.GetExportObj().SetActive(!t.GetExportObj().activeSelf);
                    }
                    /*
                    if (GUILayout.Button("同步Lightmap数据"))
                    {
                        t.UpdateLightMapData();
                    }*/
                    if (!t.autoUpdatePosDCC)
                    {
                        if (GUILayout.Button("手动同步坐标材质"))
                        {
                            t.UpdateServerMeshTransform();
                        }
                    }
                    if (GUILayout.Button("一键还原场景"))
                    {
                        if (EditorUtility.DisplayDialog("还原场景", "该操作会清理所有" + dccNames[selectDccMode] + "中修改的模型数据，还原Unity场景，请确认是否执行还原操作！", "确定"))
                        {
                            if(CleanUpDCCData())
                                t.GetExportObj().SetActive(true);
                        }
                    }
                }

            }
        }

        private MeshSyncServer m_meshSyncServer = null;

        //----------------------------------------------------------------------------------------------------------------------
        #region DCC Script Sockets

        static string commandLineMaya = "import maya.cmds as cmds;import maya.mel as mel;pathoffiles = [{0}]; namelist = [{1}];\n" +
@"
cmds.select(ado = True)
slist = cmds.ls(sl = True)
for sel in slist:
	cmds.delete(sel)
for filepath in pathoffiles:
	cmds.file(filepath, i = True, type = 'fbx')
	cmds.select(ado = True)
	selected = cmds.ls(sl = True)
	for sel in selected:
		if(not sel in namelist):
		    cmds.hide(sel)
" +
"mel.eval(\"global string $OutPutFilePath = \\\"{2}\\\"\")";


        static string commandLineMax = "global pathoffiles = #({0});global namelist = #({1});\n" +
@"
for o in objects do delete o
for filepath in pathoffiles do ImportFile filepath #noPrompt
for o in objects do
(
    if (for name in namelist where name == o.name collect name).count == 0 do hide o
)
" +
"global string OutPutFilePath = \"{2}\"";

        static int mayaPort = 7001;
        static int maxPort = 7500;

        static void Export2DCC(List<string> filepathList,List<string> meshNameList)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            int port = mayaPort;
            string Command = commandLineMaya;

            if (currentDccMode == DccMode.Maya)
            {
                port = mayaPort;
                Command = commandLineMaya;
            }
            else if (currentDccMode == DccMode.Max)
            {
                port = maxPort;
                Command = commandLineMax;
            }

            try
            {
                clientSocket.Connect(new IPEndPoint(ip, port)); //配置服务器IP与端口  
            }
            catch
            {
                Debug.LogError("can't connect " + dccNames[selectDccMode] + " server !");
                return;
            }
            string pathArray = "";
            string namelist = "";
            foreach (var path in filepathList)
            {
                Debug.Log(path);
                pathArray += "\"" + path + "\",";
            }
            pathArray = pathArray.Substring(0, pathArray.Length - 1);
            foreach (var name in meshNameList)
            {
                Debug.Log(name);
                namelist += "\"" + name + "\",";
            }
            namelist = namelist.Substring(0, namelist.Length - 1);
            Debug.Log(string.Format(Command, pathArray, namelist, filepathList[0]));
            //clientSocket.Send(Encoding.UTF8.GetBytes("print \"hello\" "));
            clientSocket.Send(Encoding.UTF8.GetBytes(string.Format(Command, pathArray, namelist, filepathList[0])));
            clientSocket.Close();

        }

        public static void ExportSelect(MeshFilter[] msfs, MeshSyncServer t)
        {
            List<string> filepathList = new List<string>();
            List<string> meshnameList = new List<string>();
            foreach (var msf in msfs)
            {
                var path = AssetDatabase.GetAssetPath(msf.sharedMesh);
                if (path != null && path != "")
                {
                    path = Application.dataPath.Replace("Assets", "") + path;
                    if (!meshnameList.Contains(msf.sharedMesh.name))
                        meshnameList.Add(msf.sharedMesh.name);
                    if (!filepathList.Contains(path))
                        filepathList.Add(path);
                }
            }
            if(filepathList.Count < 1)
            {
                EditorUtility.DisplayDialog("prefab引用错误", "找不到prefab所引用的fbx源文件，请确认prefab结构是否正确！", "确定");
                return;
            }

            if(filepathList.Count > 1)
            {
                if(!EditorUtility.DisplayDialog("prefab fbx引用数量", "prefab所引用的fbx源文件数量为"+ filepathList.Count + "，Maya中完成修改后请手动导出，使用工具一键导出功能可能发生错误！", "确定"))
                    return;
            }
                
            Export2DCC(filepathList, meshnameList);
            if (t.autoHideObj)
                t.GetExportObj().SetActive(false);
        }

        static string cleanCommandLineMaya = @"
import maya.cmds as cmds
import maya.mel as mel

cmds.select(ado = True)
slist = cmds.ls(sl = True)
for sel in slist:
	cmds.delete(sel)
";
        static string cleanCommandLineMax = @"for o in objects do delete o";
        public static bool CleanUpDCCData()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            int port = mayaPort;
            string cleanCommand = cleanCommandLineMaya;

            if(currentDccMode == DccMode.Maya)
            {
                port = mayaPort;
                cleanCommand = cleanCommandLineMaya;
            }
            else if(currentDccMode == DccMode.Max)
            {
                port = maxPort;
                cleanCommand = cleanCommandLineMax;
            }

            try
            {
                clientSocket.Connect(new IPEndPoint(ip, port)); //配置服务器IP与端口  
            }
            catch
            {
                Debug.LogError("can't connect " + dccNames[selectDccMode] + "server !");
                return false;
            }
            clientSocket.Send(Encoding.UTF8.GetBytes(cleanCommand));
            clientSocket.Close();
            return true;
        }


        #endregion
    }
}
