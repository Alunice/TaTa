using System;
using System.IO;
using Unity.AnimeToolbox;
using UnityEditor;
using UnityEngine;

namespace Unity.MeshSync.Editor
{
    public class MeshSyncMayaIntegrator 
    {
        
        private const string Folder_Prefix = "com.unity.meshsync@0.4.0-preview/DCCTools~/maya/";
      
        internal static bool InstallMeshSyncMayaTools(DCCToolInfo dccToolInfo, string extractedTempPath)
        {
            string srcRoot = Path.Combine(extractedTempPath, Folder_Prefix);
            if (!Directory.Exists(srcRoot))
            {
                return false;
            }
            string configFolder = FindConfigFolder();

            const string AUTOLOAD_SETUP = "pluginInfo -edit -autoload true MeshSyncClientMaya;";
            const string SHELF_SETUP = "UnityMeshSync_Shelf;";
            //const string MAYA_CLOSE_COMMAND = "scriptJob -idleEvent quit;";
            const string FINALIZE_SETUP = AUTOLOAD_SETUP + SHELF_SETUP;

            string copySrcFolder = srcRoot;
            string copyDestFolder = configFolder;
            string argFormat = null;
            string loadPluginCmd = null;

            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    {
                        //C:\Users\Unity\Documents\maya\modules
                        const string FOLDER_PREFIX = "modules";
                        copySrcFolder = Path.Combine(srcRoot, FOLDER_PREFIX);
                        copyDestFolder = Path.Combine(configFolder, FOLDER_PREFIX);

                        argFormat = "-command \"{0}\"";

                        //Maya script only supports '/' as PathSeparator
                        //Example: loadPlugin """C:/Users/Unity/Documents/maya/modules/UnityMeshSync/2019/plug-ins/MeshSyncClientMaya.mll""";
                        string mayaPluginPath = Path.Combine(copyDestFolder, "UnityMeshSync", dccToolInfo.DCCToolVersion,
                            @"plug-ins\MeshSyncClientMaya.mll").Replace('\\', '/');
                        loadPluginCmd = "loadPlugin \"\"\"" + mayaPluginPath + "\"\"\";";
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }

            //Copy files to config folder
            const string MOD_FILE = "UnityMeshSync.mod";
            string scriptFolder = Path.Combine("UnityMeshSync", dccToolInfo.DCCToolVersion);
            string srcModFile = Path.Combine(copySrcFolder, MOD_FILE);
            if (!File.Exists(srcModFile))
            {
                return false;
            }
            try
            {
                Directory.CreateDirectory(copyDestFolder);
                File.Copy(srcModFile, Path.Combine(copyDestFolder, MOD_FILE), true);
                FileUtility.CopyRecursive(Path.Combine(copySrcFolder, scriptFolder),
                    Path.Combine(copyDestFolder, scriptFolder),
                    true);
                CopyShelvesFile(Path.Combine(copySrcFolder, "shelf_UnityMeshSync.mel"), copyDestFolder, dccToolInfo.DCCToolVersion);
            }
            catch
            {
                return false;
            }

            //Auto Load
            string arg = string.Format(argFormat, loadPluginCmd + FINALIZE_SETUP);
            bool setupSuccessful = SetupAutoLoadPlugin(dccToolInfo.AppPath, arg);

            return setupSuccessful;
        }

        private static void CopyShelvesFile(string srcpath, string destfolder, string version)
        {
            var destpathfolder = Path.Combine(destfolder, version) + "/prefs/shelves/";
            if (Directory.Exists(destpathfolder))
            {
                var destpath = destpathfolder + "shelf_UnityMeshSync.mel";
                if (!File.Exists(destpath))
                    File.Copy(srcpath, destpath);
            }
        }

        private static string FindConfigFolder()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    {

                        //If MAYA_APP_DIR environment variable is setup, use that config folder
                        //If not, use %USERPROFILE%\Documents\maya 
                        string path = Environment.GetEnvironmentVariable("MAYA_APP_DIR");
                        if (!string.IsNullOrEmpty(path))
                            return path;

                        path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                        if (Environment.OSVersion.Version.Major >= 6)
                        {
                            path = Directory.GetParent(path).ToString();
                        }
                        path += @"\Documents\maya";
                        return path;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }

        }

        static bool SetupAutoLoadPlugin(string mayaPath, string startArgument)
        {

            try
            {
                if (!File.Exists(mayaPath))
                {
                    Debug.LogError("[MeshSync] No maya installation found at " + mayaPath);
                    return false;
                }

                //[note-sin: 2020-5-12] WindowStyle=Hidden (requires UseShellExecute=true and RedirectStandardError=false),
                //seems to be able to hide only the splash screen, but not the Maya window.
                System.Diagnostics.Process mayaProcess = new System.Diagnostics.Process
                {
                    StartInfo = {
                    FileName = mayaPath,
                    UseShellExecute = true,
                    RedirectStandardError = false,
                    Arguments = startArgument
                },
                    EnableRaisingEvents = true
                };
                mayaProcess.Start();


            }
            catch (Exception e)
            {
                Debug.LogError("[MeshSync] Failed to start Maya. Exception: " + e.Message);
                return false;
            }

            return true;
        }


    }
}

