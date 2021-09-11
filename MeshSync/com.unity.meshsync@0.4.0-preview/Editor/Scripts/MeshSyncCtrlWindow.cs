using UnityEditor;
using UnityEngine;
using Unity.AnimeToolbox;

namespace Unity.MeshSync.Editor
{
    public class MeshSyncCtrlWindow : EditorWindow
    {
		static MeshSyncCtrlWindow instance;

        enum Tab
        {
            MeshSyncCtrl,
            Installation
        }
        string[] tabNames =
        {
         "MeshSync控制面板",
         "DCC插件安装",
        };

        Tab tab;

        DCCToolInfo dccToolInfo;
        string m_lastOpenedFolder;

        string[] mayaValidVersion = { "2017", "2018", "2019", "2020" };
        string lastVersionStr = "";
        int versionIndex = 2;
        bool manualMayaVersion = false;


        [MenuItem("ArtTools/Scene Tools 场景工具/MeshSync控制面板")]
		static void AddWindow()
		{
			instance = GetWindowWithRect<MeshSyncCtrlWindow>(new Rect(0, 0, 600, 400));
            instance.Show();
		}

        MeshSyncServer m_server;
        Vector2 m_scrollPos;
        void OnGUI()
        {
            tab = (Tab)GUILayout.Toolbar((int)tab, tabNames);
            if (tab == Tab.MeshSyncCtrl)
            {
                DrawMeshSyncCtrlGUI();
                
            }
            else if (tab == Tab.Installation)
            {
                DrawInstallationGUI();
            }
        }
        void DrawMeshSyncCtrlGUI()
        {
            if (m_server == null)
            {
                m_server = FindObjectOfType<MeshSyncServer>();
                if (m_server != null)
                {
                    m_server.gameObject.transform.localPosition = new Vector3(0, 0, 0);
                    m_server.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                    m_server.gameObject.transform.localScale = new Vector3(1, 1, 1);
                    m_server.name = "MeshSyncServer";

                    if (!m_server.hasInit)
                    {
                        m_server.Init("Assets/MeshSyncAssets");
                        m_server.SetAutoStartServer(true);
                    }

                    DrawExportGUI();
                }
                else
                {
                    if (GUILayout.Button("创建MeshSync"))
                    {
                        m_server = MeshSyncMenu.CreateMeshSyncServer(true);
                    }
                }
            }
            else
            {
                DrawExportGUI();
            }
        }

        void DrawExportGUI()
        {
            MeshSyncServerInspector.DrawExport2Dcc(m_server);

            GUILayout.Space(10);

            if (GUILayout.Button("清除场景MeshSync"))
            {
                if (EditorUtility.DisplayDialog("清除操作", "该操作会清理所有DCC中修改的模型数据，删除Unity中的MeshSync，请确认是否执行该操作！", "确定"))
                {
                    if (MeshSyncServerInspector.CleanUpDCCData())
                        m_server.GetExportObj().SetActive(true);

                    DestroyImmediate(m_server.gameObject);
                    m_server = null;
                }
            }
        }

        #region Maya Integrator Part
        // Maya Installation
        void DrawInstallationGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Maya安装目录:" + m_lastOpenedFolder);
            if (GUILayout.Button("选择文件夹",GUILayout.Width(100)))
            {
                string folder = EditorUtility.OpenFolderPanel("Add DCC Tool", m_lastOpenedFolder, "");
                if (!string.IsNullOrEmpty(folder))
                {
                    m_lastOpenedFolder = folder;

                    //Find the path to the actual app,插件原方法根据路径查询版本有bug，这里手动修复一下
                    dccToolInfo = DCCFinderUtility.FindDCCToolInDirectory(DCCToolType.AUTODESK_MAYA, null, m_lastOpenedFolder);
                    //FixDccToolInfo(ref dccToolInfo);
                    if(dccToolInfo!= null)
                        lastVersionStr = dccToolInfo.DCCToolVersion;
                }
            }
            GUILayout.EndHorizontal();

            if (null == dccToolInfo)
            {
                GUILayout.Label("未能在该目录下找到Maya程序，请选择正确的Maya安装目录！或选择手动安装Maya插件。");
            }
            else
            {
                if (!isValidVersion(lastVersionStr))
                {
                    var areaStyle = new GUIStyle(GUI.skin.label);
                    areaStyle.richText = true;
                    GUILayout.Label("<b><color=red>未能成功匹配Maya版本，请手动确认Maya版本是否正确！</color></b>", areaStyle);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("maya版本：");
                    versionIndex = EditorGUILayout.Popup(versionIndex, mayaValidVersion,GUILayout.Width(80));
                    dccToolInfo.DCCToolVersion = mayaValidVersion[versionIndex];
                    GUILayout.Label("启动程序：" + dccToolInfo.AppPath);
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.Label("maya版本：" + dccToolInfo.DCCToolVersion + "，启动程序：" + dccToolInfo.AppPath);
                }

                if (GUILayout.Button("一键安装Maya插件"))
                {

                    if (EditorUtility.DisplayDialog("安装Maya插件", "请先确认Maya软件已关闭，否则插件可能安装失败！点击确定安装maya插件", "确定"))
                    {
                        var res = MeshSyncMayaIntegrator.InstallMeshSyncMayaTools(dccToolInfo, GetPackageFullPath());
                        if (res)
                        {
                            EditorUtility.DisplayDialog("安装成功", "maya插件已成功安装", "确定");
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("安装失败", "maya插件安装失败，请选择手动安装", "确定");
                        }
                    }  
                }   
            }
        }

        void FixDccToolInfo(ref DCCToolInfo info)
        {
            var appPath = info.AppPath;
            //2 levels up: "/bin/maya.exe";
            var productDir = PathUtility.GetDirectoryName(appPath, 2);
            var rootDir = PathUtility.GetDirectoryName(appPath, 3);
            productDir = productDir.Replace(rootDir, "");
            productDir = productDir.Replace("\\Maya", "");
            info.DCCToolVersion = productDir;
        }

        bool isValidVersion(string version)
        {
            for(int i = 0; i < mayaValidVersion.Length; i++)
            {
                if(mayaValidVersion[i] == version)
                {
                    versionIndex = i;
                    manualMayaVersion = false;
                    return true;
                }
            }
            manualMayaVersion = true;
            return false;
        }

        string GetPackageFullPath()
        {
            return Application.dataPath.Replace("Assets", "Packages");
        }

        #endregion
    }

}
