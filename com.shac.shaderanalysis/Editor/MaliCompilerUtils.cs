using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.IO;
using System.Text;
public static class MaliCompilerUtils 
{
    private static int MAX_COMMAND_LENGTH = 8000;
    [MenuItem("SH_AC/Shader Analysis")]
    public static string ShaderMaliAnalysis(string fileName)
    {
        List<string> vertList = new List<string>();
        List<string> fragList = new List<string>();
        if (!File.Exists(fileName))
        {
            throw new Exception("文件不存在: " + fileName);
        } 
        
        StreamReader sr = new StreamReader(fileName);
        while (true)
        {
            string fileDataLine;
            fileDataLine = sr.ReadLine();
            if (fileDataLine == null)
            {
                break;
            }
            if(fileDataLine.Contains("#ifdef VERTEX"))
            {
                ReadShader(sr, vertList);

            }
            else if(fileDataLine.Contains("#ifdef FRAGMENT"))
            {
                ReadShader(sr, fragList);
            }
        }
        sr.Close();
        ClearFolderTempFile();
        var result = TempVertFragOutput(vertList, fragList);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return result;
    }

    private static void ClearFolderTempFile()
    {
        string outputDataPath = Application.dataPath.Replace("Assets", "Temp");
        if(!Directory.Exists(outputDataPath + "/ShaderAnalysis"))
        {
            Directory.CreateDirectory(outputDataPath + "/ShaderAnalysis");
        }
        if(!Directory.Exists(outputDataPath + "/ShaderAnalysis/tempOutput"))
        {
            Directory.CreateDirectory(outputDataPath + "/ShaderAnalysis/tempOutput");
        }
        string outputTempFolder = outputDataPath + "/ShaderAnalysis/tempOutput/";
        var fileList = Directory.GetFiles(outputTempFolder);
        foreach(var filePath in fileList)
        {
            File.Delete(filePath);
        }
    }

    private static void ReadShader(StreamReader sr, List<string> outlist)
    {
        int endFlag = 0;
        StringBuilder builder = new StringBuilder();
        string fileDataLine = sr.ReadLine();
        while (fileDataLine != null)
        {
            if (fileDataLine.Contains("#endif"))
            {
                endFlag--;
            }
            if (fileDataLine.Contains("#if"))
            {
                endFlag++;
            }
            if (endFlag < 0)
                break;
            builder.AppendLine(fileDataLine);
            fileDataLine = sr.ReadLine();
        }
        outlist.Add(builder.ToString());
      //  Debug.Log(outlist[outlist.Count-1]);
    }

    private static string TempVertFragOutput(List<string> vertList, List<string> fragList)
    {
        string outputDataPath = Application.dataPath.Replace("Assets", "Temp");
        string outputTempFolder = outputDataPath + "/ShaderAnalysis/tempOutput/";
        int minCount = Math.Min(vertList.Count, fragList.Count);
        string command = "";
        string fragCommand = "malioc -f ";
        string vertCommand = "malioc -v ";
        string vertFileName = "vertexVarying{0}.vert";
        string fragFileName = "fragmentVarying{0}.frag";
        StreamWriter sw;
        for (int i = 0; i< minCount; i++)
        {
            string tempVert = string.Format(vertFileName, i);
            string outputVertPath = outputTempFolder + tempVert;
            sw = new StreamWriter(outputVertPath, false);
            sw.Write(vertList[i]);
            sw.Close();
            command += vertCommand + "Temp/ShaderAnalysis/tempOutput/" + tempVert + " && ";

            string tempFrag = string.Format(fragFileName, i);
            string outputFragPath = outputTempFolder + tempFrag;
            sw = new StreamWriter(outputFragPath, false);
            sw.Write(fragList[i]);
            sw.Close();
            command += fragCommand + "Temp/ShaderAnalysis/tempOutput/" + tempFrag + " && ";
        }

        if(minCount < vertList.Count)
        {
            for(int i = minCount; i< vertList.Count; i++)
            {
                string tempVert = string.Format(vertFileName, i);
                string outputVertPath = outputTempFolder + tempVert;
                sw = new StreamWriter(outputVertPath, false);
                sw.Write(vertList[i]);
                sw.Close();
                command += vertCommand + "Temp/ShaderAnalysis/tempOutput/" + tempVert + " && ";
            }
        }

        if (minCount < fragList.Count)
        {
            for (int i = minCount; i < fragList.Count; i++)
            {
                string tempFrag = string.Format(fragFileName, i);
                string outputFragPath = outputTempFolder + tempFrag;
                sw = new StreamWriter(outputFragPath, false);
                sw.Write(fragList[i]);
                sw.Close();
                command += fragCommand + "Temp/ShaderAnalysis/tempOutput/" + tempFrag + " && ";
            }
        }
        
        command += "exit";
        //Debug.Log(command);

        string analysisOutput = MaliCompilerOutput(command);   
        File.WriteAllText(outputDataPath + "/ShaderAnalysis/FinalOutput.txt", analysisOutput);
        return analysisOutput;
    }




    public static string MaliCompilerOutput(string command)
    {

        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
        p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
        p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
        p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
        p.StartInfo.CreateNoWindow = true;//不显示程序窗口
        p.Start();//启动程序

        //向cmd窗口发送输入信息
        if (command.Length > MAX_COMMAND_LENGTH)
        {
            string commandSub = command.Substring(0, MAX_COMMAND_LENGTH);
            commandSub = commandSub.Substring(0, commandSub.LastIndexOf("&&"));
            commandSub += "&&exit";
            p.StandardInput.WriteLine(commandSub);
            Debug.LogError("vertexs & fragments varying are too much!!! so output the subset");
            //return commandSub;
        }
        else
        {
            p.StandardInput.WriteLine(command);     
        }
        p.StandardInput.AutoFlush = true;
        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();//等待程序执行完退出进程
        p.Close();
        return output;

    }

}
