using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Data;
using System.CodeDom;
using System.Windows.Forms;

namespace WinformTest
{
    public class CSVUtils
    {

        List<string[]> _datas = new List<string[]>();

        string COL_POS_NAME = "in_POSITION0.x";
        string COL_NM_NAME = "in_NORMAL0.x";
        string COL_UV0_NAME = "in_TEXCOORD0.x";
        string COL_UV1_NAME = "TEXCOORD1.x";
        string COL_UV2_NAME = "TEXCOORD2.x";
        string COL_UV3_NAME = "TEXCOORD3.x";
        string COL_VETCOLOR_NAME = "COLOR.x";

        CSVToObj _formObj = null;

        public bool hasNormal = true;
        public bool hasUV1 = true;
        public bool hasUV2 = false;
        public bool hasUV3 = false;
        public bool hasUV4 = false;


        public void LoadCsvFile(string fileName)
        {
            //对数据的有效性进行验证

            if (fileName == null)
            {
                throw new Exception("请指定要载入的CSV文件名");
            }
            else if (!File.Exists(fileName))
            {
                throw new Exception("指定的CSV文件不存在");
            }

            StreamReader sr = new StreamReader(fileName);

            _datas.Clear();
            _formObj = null;

            string csvDataLine;

            csvDataLine = "";
            while (true)
            {
                string fileDataLine;

                fileDataLine = sr.ReadLine();
                if (fileDataLine == null)
                {
                    break;
                }
                if (csvDataLine == "")
                {
                    csvDataLine = fileDataLine;
                }
                else
                {
                    csvDataLine += "/r/n" + fileDataLine;//GetDeleteQuotaDataLine(fileDataLine);
                }
                //如果包含偶数个引号，说明该行数据中出现回车符或包含逗号
                if (!IfOddQuota(csvDataLine))
                {
                    //  Debug.WriteLine(csvDataLine);
                    var linedatas = csvDataLine.Split(',');
                    _datas.Add(linedatas);
                    csvDataLine = "";
                }
            }
            sr.Close();
            //数据行出现奇数个引号
            if (csvDataLine.Length > 0)
            {
                MessageBox.Show("CSV文件的格式有错误");
                _datas.Clear();
                throw new Exception("CSV文件的格式有错误");
            }

            //BuildObjData(fileName.Replace(".csv", ".obj"));
        }

        public bool LoadDataSuccess()
        {
            return _datas.Count > 0;
        }

        public string[] GetDataFirstLine()
        {
            if (_datas.Count > 0)
            {
                return (string[])_datas[0].Clone();
            }
            return new string[0];
        }

        public void SetColName(string posName, string uvName, string normalName)
        {
            COL_NM_NAME = normalName;
            COL_POS_NAME = posName;
            COL_UV0_NAME = uvName;
        }

        public void SetColName(string posName, string nmName, string uvName,
            string uv1Name, string uv2Name, string uv3Name, string vertexColorName)
        {
            COL_POS_NAME = posName;
            COL_NM_NAME = nmName;
            COL_UV0_NAME = uvName;

            COL_UV1_NAME = uv1Name;
            COL_UV2_NAME = uv2Name;
            COL_UV3_NAME = uv3Name;
            COL_VETCOLOR_NAME = vertexColorName;
        }

            public void SetFormOBJ(CSVToObj obj)
        {
            _formObj = obj;
        }

        /// <summary>
        /// 判断字符串是否包含奇数个引号
        /// </summary>
        /// <param name="dataLine">数据行</param>
        /// <returns>为奇数时，返回为真；否则返回为假</returns>
        public bool IfOddQuota(string dataLine)
        {
            int quotaCount;
            bool oddQuota;

            quotaCount = 0;
            for (int i = 0; i < dataLine.Length; i++)
            {
                if (dataLine[i] == '\"')
                {
                    quotaCount++;
                }
            }

            oddQuota = false;
            if (quotaCount % 2 == 1)
            {
                oddQuota = true;
            }

            return oddQuota;
        }

        public struct VertexData
        {
            public float posX;
            public float posY;
            public float posZ;
            public float posW;

            public float normalX;
            public float normalY;
            public float normalZ;

            public float uvX;
            public float uvY;

            public VertexData(float posX, float posY, float posZ, float posW, float normalX, float normalY, float normalZ, float uvX, float uvY)
            {
                this.posX = posX;
                this.posY = posY;
                this.posZ = posZ;
                this.posW = posW;
                this.normalX = normalX;
                this.normalY = normalY;
                this.normalZ = normalZ;
                this.uvX = uvX;
                this.uvY = uvY;
            }


        }

        public void BuildObjData(string outputPath)
        {
            if (_datas.Count < 1)
            {
                MessageBox.Show("CSV文件的数据有错误");
                throw new Exception("CSV文件的数据有错误");
            }
            var titleDatas = _datas[0];
            int dataLenth = titleDatas.Length;
            int posXColumn = 0, normalXColumn = 0, uvXColumn = 0;

            List<VertexData> indexDataList = new List<VertexData>();
            Dictionary<int, int> _indexPair = new Dictionary<int, int>();

            for (int i = 0; i < dataLenth; i++)
            {
                string title = titleDatas[i];
                if (title.Contains(COL_POS_NAME))
                {
                    posXColumn = i;
                }
                else if (title.Contains(COL_UV0_NAME))
                {
                    uvXColumn = i;
                }
                else if (title.Contains(COL_NM_NAME))
                {
                    normalXColumn = i;
                }
            }

            _formObj.AddListViewItems("clean up scientific notation and build index buffer data...(1/4)");
            //clean up scientific notation and build index buffer data
            for (int i = 1; i < _datas.Count; i++)
            {
                var tempDatas = _datas[i];
                for (int ii = 0; ii < dataLenth; ii++)
                {
                    if (tempDatas[ii].Contains("E"))
                    {
                        tempDatas[ii] = "0";
                    }
                }

                // build index buffer data
                int id = int.Parse(tempDatas[1]);
                if (!_indexPair.ContainsKey(id))
                {
                    //normal and pos multiply -1 since MAYA and RenderDoc coord diffs
                    VertexData newData = new VertexData(
                        float.Parse(tempDatas[posXColumn]) * -1f,     //pos.x
                        float.Parse(tempDatas[posXColumn + 1]),     //pos.y
                        float.Parse(tempDatas[posXColumn + 2]),     //pos.z
                        float.Parse(tempDatas[posXColumn + 3]),     //pos.w
                        float.Parse(tempDatas[normalXColumn]) * -1f,     //nor.x
                        float.Parse(tempDatas[normalXColumn + 1]),     //nor.y
                        float.Parse(tempDatas[normalXColumn + 2]),     //nor.z
                        float.Parse(tempDatas[uvXColumn]),     //u
                        float.Parse(tempDatas[uvXColumn + 1])     //v
                        );
                    indexDataList.Add(newData);
                    _indexPair.Add(id, indexDataList.Count);
                }
            }


            _formObj.AddListViewItems("write obj vtx...(2/4)");
            //write obj string
            StringBuilder builder = new StringBuilder();

            if (posXColumn != 0)
            {
                for (int i = 0; i < indexDataList.Count; i++)
                {
                    VertexData tmpData = indexDataList[i];
                    builder.Append("v ");
                    builder.Append(tmpData.posX.ToString("F5"));
                    builder.Append(" ");
                    builder.Append(tmpData.posY.ToString("F5"));
                    builder.Append(" ");
                    builder.Append(tmpData.posZ.ToString("F5"));
                    builder.AppendLine(string.Empty);
                }
            }

            if (uvXColumn != 0)
            {
                for (int i = 0; i < indexDataList.Count; i++)
                {
                    VertexData tmpData = indexDataList[i];
                    builder.Append("vt ");
                    builder.Append(tmpData.uvX.ToString("F5"));
                    builder.Append(" ");
                    builder.Append(tmpData.uvY.ToString("F5"));
                    builder.AppendLine(string.Empty);
                }
            }

            if (normalXColumn != 0)
            {
                for (int i = 0; i < indexDataList.Count; i++)
                {
                    VertexData tmpData = indexDataList[i];
                    builder.Append("vn ");
                    builder.Append(tmpData.normalX.ToString("F5"));
                    builder.Append(" ");
                    builder.Append(tmpData.normalY.ToString("F5"));
                    builder.Append(" ");
                    builder.Append(tmpData.normalZ.ToString("F5"));
                    builder.AppendLine(string.Empty);
                }
            }


            _formObj.AddListViewItems("write obj face...(3/4)");
            string message = "";
            int f_count = 0;
            f_count = (uvXColumn != 0) ? f_count + 1 : f_count;
            f_count = (normalXColumn != 0) ? f_count + 1 : f_count;
            for (int i = 1; i < _datas.Count; i += 3)
            {
                // unity to maya index order form 0-1-2 to 0-2-1
                var tempDatas1 = _datas[i];
                var tempDatas2 = _datas[i + 2];
                var tempDatas3 = _datas[i + 1];
                int index1 = int.Parse(tempDatas1[1]);
                int index2 = int.Parse(tempDatas2[1]);
                int index3 = int.Parse(tempDatas3[1]);

                if (index1 == index2 || index2 == index3 || index1 == index3)
                {
                    message += ("WARNING:: Duplicated vertex data found: " + index1 + " " + index2 + " " + index3 + "\r\n");
                    continue;
                }

                builder.Append("f ");
                //normaly RenderDoc ouput csv row[1] as the index buffer
                int index = _indexPair[int.Parse(tempDatas1[1])];
                builder.Append(index);
                for (int c = 0; c < f_count; c++)
                {
                    builder.Append("/");
                    builder.Append(index);
                }

                builder.Append(" ");

                index = _indexPair[int.Parse(tempDatas2[1])];
                builder.Append(index);
                for (int c = 0; c < f_count; c++)
                {
                    builder.Append("/");
                    builder.Append(index);
                }

                builder.Append(" ");

                index = _indexPair[int.Parse(tempDatas3[1])];
                builder.Append(index);
                for (int c = 0; c < f_count; c++)
                {
                    builder.Append("/");
                    builder.Append(index);
                }

                builder.AppendLine(string.Empty);
            }

            Console.Write(builder);

            Console.WriteLine(message);

            StreamWriter sw = new StreamWriter(outputPath, false);
            sw.Write(builder);
            sw.Close();

            _formObj.AddListViewItems("success!!!(4/4)");
            _formObj.AddListViewItems(message);

            if (posXColumn == 0)
            {
                Console.WriteLine("No position column found in .CSV");
                _formObj.AddListViewItems("No position column found in .CSV");
            }
            if (normalXColumn == 0)
            {
                Console.WriteLine("No normal column found in .CSV");
                _formObj.AddListViewItems("No normal column found in .CSV");
            }
            if (uvXColumn == 0)
            {
                Console.WriteLine("No uv coordination column found in .CSV");
                _formObj.AddListViewItems("No uv coordination column found in .CSV");
            }

            string consoleMassage = "vertex count: " + indexDataList.Count + ",face count : " + indexDataList.Count / 3;
            _formObj.AddListViewItems(consoleMassage);

            _formObj.AddListViewItems("Output Path is : " + outputPath);
        }




        public void BuildFbxData(string outputPath)
        {
            FbxManager fbxManer = new FbxManager();
            fbxManer.SetFormOBJ(_formObj);
            fbxManer.SetColName(COL_POS_NAME, COL_NM_NAME, COL_UV0_NAME, 
                COL_UV1_NAME, COL_UV2_NAME, COL_UV3_NAME,COL_VETCOLOR_NAME);
            fbxManer.BuildFBXData(_datas, outputPath);
        }



    }





}
