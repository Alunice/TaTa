using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace WinformTest
{
    public partial class CSVToObj : Form
    {
        CSVUtils CSVManager;
        enum OUTPUT_FORMAT{OBJ,FBX };
        public CSVToObj()
        {
            InitializeComponent();
            updateComboBox(new List<string>());
            CSVManager = new CSVUtils();  
        }

        
        private void ResetCustomListBox()
        {
            outputListView.Text = "";
            outputListView.Items.Clear();
            listBox1.Text = "";
            listBox1.Items.Clear();
        }


        private void OnChoseCSVClick(object sender, EventArgs e)
        {
            //open file
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*csv*)|*.csv"; //设置要选择的文件的类型

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                //返回文件的完整路径     
                
                CSVManager.LoadCsvFile(fileDialog.FileName);
                if (CSVManager.LoadDataSuccess())
                {
                    string[] titleLine = CSVManager.GetDataFirstLine();
                    List<string> noIdxTitleLine = new List<string>();
                    for (int i = 0;i < titleLine.Length - 2; i++)
                    {
                        noIdxTitleLine.Add(titleLine[i + 2]);
                    }
                    csvPathLabel.Text = fileDialog.FileName;
                    f_csvPathLabel.Text = fileDialog.FileName;
                    updateComboBox(noIdxTitleLine);
                    ResetCustomListBox();
                }
            }
        }

        private void updateComboBox(List<string> titleList)
        {
            //obj comboBox update
            PosComboBox.Items.Clear();
            UVComboBox.Items.Clear();
            NormalComboBox.Items.Clear();

            
            
            titleList.Add(" NULL");
            var titles = titleList.ToArray();
            int selectIndex = titles.Length - 1;

            PosComboBox.Items.AddRange(titles);
            PosComboBox.SelectedIndex = selectIndex;

            UVComboBox.Items.AddRange(titles);
            UVComboBox.SelectedIndex = selectIndex;

            NormalComboBox.Items.AddRange(titles);
            NormalComboBox.SelectedIndex = selectIndex;

            //fbx comboBox update
            f_PosComboBox.Items.Clear();
            f_UV0ComboBox.Items.Clear();
            f_NormalComboBox.Items.Clear();
            f_UV1ComboBox.Items.Clear();
            f_UV2ComboBox.Items.Clear();
            f_UV3ComboBox.Items.Clear();
            f_VColorComboBox.Items.Clear();

            f_PosComboBox.Items.AddRange(titles);
            f_PosComboBox.SelectedIndex = selectIndex;

            f_UV0ComboBox.Items.AddRange(titles);
            f_UV0ComboBox.SelectedIndex = selectIndex;

            f_NormalComboBox.Items.AddRange(titles);
            f_NormalComboBox.SelectedIndex = selectIndex;

            f_UV1ComboBox.Items.AddRange(titles);
            f_UV1ComboBox.SelectedIndex = selectIndex;

            f_UV2ComboBox.Items.AddRange(titles);
            f_UV2ComboBox.SelectedIndex = selectIndex;

            f_UV3ComboBox.Items.AddRange(titles);
            f_UV3ComboBox.SelectedIndex = selectIndex;

            f_VColorComboBox.Items.AddRange(titles);
            f_VColorComboBox.SelectedIndex = selectIndex;
        }

        private void OnExportClick(object sender, EventArgs e)
        {
            if (CheckIsSettingValid())
            {
                outputListView.Text = "";
                outputListView.Items.Clear();
                listBox1.Text = "";
                listBox1.Items.Clear();
                var outputFormat = GetOutPutFormat();
                if (outputFormat == OUTPUT_FORMAT.FBX)
                {
                    string outputFile;
                    outputFile = csvPathLabel.Text.Replace(".csv", ".fbx");
                    if (File.Exists(outputFile))
                    {
                        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        string tempTime = Convert.ToInt64(ts.TotalMilliseconds).ToString();
                        outputFile = outputFile.Replace(".obj", tempTime + ".fbx");
                    }
                    CSVManager.SetColName(f_PosComboBox.Text, f_NormalComboBox.Text, f_UV0ComboBox.Text,
                        f_UV1ComboBox.Text,f_UV2ComboBox.Text,f_UV3ComboBox.Text,f_VColorComboBox.Text);
                    CSVManager.SetFormOBJ(this);
                    CSVManager.BuildFbxData(outputFile);
                }
                else
                {
                    string outputFile;
                    outputFile = csvPathLabel.Text.Replace(".csv", ".obj");
                    if (File.Exists(outputFile))
                    {
                        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        string tempTime = Convert.ToInt64(ts.TotalMilliseconds).ToString();
                        outputFile = outputFile.Replace(".obj", tempTime + ".obj");
                    }

                    CSVManager.SetColName(PosComboBox.Text, UVComboBox.Text, NormalComboBox.Text);
                    CSVManager.SetFormOBJ(this);
                    CSVManager.BuildObjData(outputFile);
                }
                
            }
        }

        private bool CheckIsSettingValid()
        {
            var format = GetOutPutFormat();
            if(format == OUTPUT_FORMAT.OBJ)
            {
                if (csvPathLabel.Text == "")
                {
                    MessageBox.Show("请选择正确的文件路径!");
                    return false;
                }

                if (PosComboBox.Text == "" || PosComboBox.Text == "NULL")
                {
                    MessageBox.Show("请选择正确的PostionX Label!");
                    return false;
                }

                if (UVComboBox.Text == "")
                {
                    UVComboBox.Text = "NULL";
                }

                if (NormalComboBox.Text == "")
                {
                    NormalComboBox.Text = "NULL";
                }
                return true;
            }
            else{
                if (f_csvPathLabel.Text == "")
                {
                    MessageBox.Show("请选择正确的文件路径!");
                    return false;
                }

                if (f_PosComboBox.Text == "" || f_PosComboBox.Text == "NULL")
                {
                    MessageBox.Show("请选择正确的PostionX Label!");
                    return false;
                }

                if (f_UV0ComboBox.Text == "")
                {
                    f_UV0ComboBox.Text = "NULL";
                }

                if (f_NormalComboBox.Text == "")
                {
                    f_NormalComboBox.Text = "NULL";
                }
                if (f_UV1ComboBox.Text == "")
                {
                    f_UV1ComboBox.Text = "NULL";
                }
                if (f_UV2ComboBox.Text == "")
                {
                    f_UV2ComboBox.Text = "NULL";
                }
                if (f_UV3ComboBox.Text == "")
                {
                    f_UV3ComboBox.Text = "NULL";
                }
                if (f_VColorComboBox.Text == "")
                {
                    f_VColorComboBox.Text = "NULL";
                }
                return true;
            } 
        }

        public void AddListViewItems(string item)
        {
            var format = GetOutPutFormat();

            ListBox _listBox = (format == OUTPUT_FORMAT.FBX) ? listBox1 : outputListView;
            bool scroll = false;
            if (_listBox.TopIndex == _listBox.Items.Count - (int)_listBox.Height / _listBox.ItemHeight)
                scroll = true;
            _listBox.Items.Add(item);
            if(scroll)
                _listBox.TopIndex = _listBox.Items.Count - (int)_listBox.Height / _listBox.ItemHeight;
        }

        private void OnCloseClick(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private OUTPUT_FORMAT GetOutPutFormat()
        {
            return (OUTPUT_FORMAT)tabControl1.SelectedIndex;
        }
    }
}
