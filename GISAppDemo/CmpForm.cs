//******************************
// Written by Yaguang Li (liyaguang0123@gmail.com)
// Copyright (c) 2013, ISCAS
//
// Use and restribution of this code is subject to the GPL v3.
//******************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using TrjTools.Tools;

namespace GISAppDemo
{
    /// <summary>
    /// Provide methods for comparing the output of Map-matching algorithm to the 
    /// standard output.
    /// </summary>
    public partial class CmpForm : Form
    {
        private IMapViewer _parent = null;

        private static String DATASET_DIR = getDataSetDir();
        private String inputFileDir = Path.Combine(DATASET_DIR, "input");
        private String outputFileDir = Path.Combine(DATASET_DIR + "output");
        private String stdOutputFileDir = Path.Combine(DATASET_DIR + "std_output");
        public struct Item
        {
            //public String AttrA
            //{
            //    get
            //    {
            //        return attrA;
            //    }
            //    set
            //    {
            //        this.attrA = value;
            //    }
            //}
            ////private String attrA = "";

            //public String AttrB
            //{
            //    get
            //    {
            //        return attrB;
            //    }
            //    set
            //    {
            //        this.attrB = value;
            //    }
            //}
            ////private String attrB = "";
        }

        private void showList()
        {
            List<Item> list = new List<Item>();
            //....填充
            dgvCmp.AutoGenerateColumns = true;
            dgvCmp.DataSource = list;
            dgvCmp.Refresh();
        }
        public struct Record
        {
            public string Time { get; set; }

            public double Lng { get; set; }

            public double Lat { get; set; }

            public int EdgeId { get; set; }

            public int StdEdgeId { get; set; }

            public double Distance { get; set; }

            public Boolean Correct { get; set; }

        }
        public CmpForm()
        {
            InitializeComponent();
        }
        public CmpForm(IMapViewer parent)
        {
            InitializeComponent();
            this._parent = parent;
        }
        /// <summary>
        /// 获取dataset路径
        /// </summary>
        /// <returns></returns>
        private static string getDataSetDir()
        {
            return Constants.DATA_DIR;
        }

        /// <summary>
        /// 更改数据集路径位置
        /// </summary>
        private void setDataSetDir(string dir)
        {
            DATASET_DIR = dir;
            initDir();
        }

        private void initDir()
        {
            inputFileDir = DATASET_DIR + "input\\";
            outputFileDir = DATASET_DIR + "output\\";
            stdOutputFileDir = DATASET_DIR + "std_output\\";
        }

        private void CmpForm_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputfile"></param>
        /// <param name="file"></param>
        /// <param name="stdFile"></param>
        //private void cmpFile(string inputfile, string file, string stdFile)
        //{
        //    StreamReader inSr = new StreamReader(inputfile);
        //    StreamReader sr = new StreamReader(file);
        //    StreamReader stdSr = new StreamReader(stdFile);
        //    int lineNumber = 0;
        //    int correctNumber = 0;
        //    String line, stdLine, inLine;
        //    List<Record> list = new List<Record>();
        //    while (!sr.EndOfStream && !stdSr.EndOfStream)
        //    {

        //        inLine = inSr.ReadLine();
        //        line = sr.ReadLine();
        //        stdLine = stdSr.ReadLine();
        //        lineNumber++;
        //        Record rec = new Record();
        //        String[] fields = inLine.Split(',');
        //        rec.Time = int.Parse(fields[0]);
        //        rec.Lat = double.Parse(fields[1]);
        //        rec.Lng = double.Parse(fields[2]);
        //        //output
        //        fields = line.Split(',');
        //        int time = int.Parse(fields[0]);
        //        if (time != rec.Time)
        //        {
        //            MessageBox.Show(String.Format("time fields at Line {0} in the input file and the output file should be the same.", lineNumber));
        //            return;
        //        }
        //        rec.EdgeId = int.Parse(fields[1]);
        //        rec.Confidence = double.Parse(fields[2]);
        //        //stdoutput
        //        fields = stdLine.Split(',');
        //        time = int.Parse(fields[0]);
        //        if (time != rec.Time)
        //        {
        //            MessageBox.Show(String.Format("time fields at Line {0} in the input file and the stdoutput file should be the same.", lineNumber));
        //            return;
        //        }
        //        rec.StdEdgeId = int.Parse(fields[1]);
        //        if (rec.EdgeId == rec.StdEdgeId)
        //        {
        //            rec.Correct = true;
        //            correctNumber++;
        //        }
        //        else
        //        {
        //            rec.Correct = false;
        //        }
        //        if (lineNumber > 1)
        //        {
        //            Record prev = list[lineNumber - 2];
        //            rec.Distance = LayerTools.GetDistance(rec.Lng, rec.Lat, prev.Lng, prev.Lat);
        //        }
        //        else
        //        {
        //            rec.Distance = 0;
        //        }
        //        list.Add(rec);
        //    }
        //    inSr.Close();
        //    sr.Close();
        //    stdSr.Close();

        //    //显示正确率
        //    lbCorrectRate.Text = (correctNumber * 1.0 / lineNumber).ToString();

        //    dgvCmp.AutoGenerateColumns = true;
        //    dgvCmp.DataSource = list;
        //    //更改背景颜色
        //    DataGridViewRowCollection rows = dgvCmp.Rows;
        //    //int correct_idx = dgvCmp.Columns.Count - 2;
        //    int correct_idx = dgvCmp.Columns["Correct"].Index;
        //    for (int i = 0; i < rows.Count; i++)
        //    {
        //        bool correct = (bool)rows[i].Cells[correct_idx].Value;
        //        if (!correct)
        //        {
        //            rows[i].DefaultCellStyle.BackColor = Color.Red;
        //        }
        //    }
        //    dgvCmp.Columns["Time"].Width = 50;
        //    dgvCmp.Columns["Lat"].Width = 100;
        //    dgvCmp.Columns["Lng"].Width = 100;
        //    dgvCmp.Columns["EdgeId"].Width = 60;
        //    dgvCmp.Columns["StdEdgeId"].Width = 60;
        //    dgvCmp.Columns["Distance"].Width = 60;
        //    dgvCmp.Columns["Confidence"].Width = 30;
        //    dgvCmp.Columns["Correct"].Width = 40;
        //    dgvCmp.Refresh();
        //}

        /// <summary>
        /// Read the matched trajectory file
        /// </summary>
        /// <param name="inputfile"></param>
        private void cmpFile(string inputfile)
        {
            StreamReader inSr = new StreamReader(inputfile);
            int lineNumber = 0;
            String inLine;
            List<Record> list = new List<Record>();
            while (!inSr.EndOfStream)
            {
                inLine = inSr.ReadLine();
                lineNumber++;
                Record rec = new Record();
                String[] fields = inLine.Split(',');
                rec.Time = fields[0];
                //rec.Time = int.Parse(fields[0]);
                rec.Lat = double.Parse(fields[1]);
                rec.Lng = double.Parse(fields[2]);
                rec.EdgeId = int.Parse(fields[3]);
                if (lineNumber > 1)
                {
                    Record prev = list[lineNumber - 2];
                    rec.Distance = LayerTools.GetDistance(rec.Lng, rec.Lat, prev.Lng, prev.Lat);
                }
                else
                {
                    rec.Distance = 0;
                }
                list.Add(rec);
            }
            inSr.Close();

            dgvCmp.AutoGenerateColumns = true;
            dgvCmp.DataSource = list;
            dgvCmp.Columns["Time"].Width = 100;
            dgvCmp.Columns["Lat"].Width = 100;
            dgvCmp.Columns["Lng"].Width = 100;
            dgvCmp.Columns["EdgeId"].Width = 60;
            dgvCmp.Columns["StdEdgeId"].Width = 60;
            dgvCmp.Columns["Distance"].Width = 60;
            //dgvCmp.Columns["Confidence"].Width = 30;
            dgvCmp.Columns["Correct"].Width = 40;
            dgvCmp.Refresh();
        }

        private void dgvCmp_SelectionChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(lbCmp.SelectedIndex.ToString());
            //if (dgvCmp.SelectedRows.Count > 0)
            //{
            //    show(dgvCmp.SelectedRows[0].Index);
            //}

        }

        private void dgvCmp_AutoSizeColumnsModeChanged(object sender, DataGridViewAutoSizeColumnsModeEventArgs e)
        {

        }

        private void dgvCmp_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //show(e.RowIndex);
        }
        /// <summary>
        /// 在地图上显示某列的点
        /// </summary>
        /// <param name="rowIndex"></param>
        private void show(int rowIndex)
        {
            DataGridViewRow row = dgvCmp.Rows[rowIndex];
            double lat, lng;
            //int time = (int)row.Cells["Time"].Value;
            lng = (double)row.Cells["Lng"].Value;
            lat = (double)row.Cells["Lat"].Value;
            int edgeId = (int)row.Cells["EdgeId"].Value;
            int stdEdgeId = (int)row.Cells["StdEdgeId"].Value;
            if (_parent != null)
            {
                _parent.ClearMap();
                _parent.drawPoint(lat, lng);
                if (edgeId > 0)
                {
                    _parent.drawLine(edgeId);
                }
                if (stdEdgeId > 0)
                {
                    _parent.drawStdLine(stdEdgeId);
                }
            }
        }

        private void dgvCmp_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvCmp.SelectedCells.Count > 0)
            {
                int rowIndex = dgvCmp.SelectedCells[0].RowIndex;
                show(rowIndex);
            }
        }

        private void dgvCmp_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex > dgvCmp.RowCount || e.ColumnIndex < 0)
            {
                return;
            }
            String colName = dgvCmp.Columns[e.ColumnIndex].Name;
            if (colName == "EdgeId" || colName == "StdEdgeId")
            {
                //显示详细信息
                int eid = (int)(dgvCmp.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                _parent.showEdgeInfo(eid);
            }
        }

        private void dgvCmp_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
                e.RowBounds.Location.Y,
                dgvCmp.RowHeadersWidth - 4,
                e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex).ToString(),
                dgvCmp.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dgvCmp.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        /// <summary>
        /// 读取输出与标准输出，并绘制图形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miChooseDataset_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog f = new FolderBrowserDialog();
            f.SelectedPath = DATASET_DIR;
            if (f.ShowDialog() == DialogResult.OK)
            {
                // outputFileDir = f.SelectedPath + "\\";
                setDataSetDir(f.SelectedPath + "\\");
                //MessageBox.Show(f.SelectedPath);
            }
        }

        /// <summary>
        /// Draw the whole path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miShowPath_Click(object sender, EventArgs e)
        {
            //int idx = 1;
            //string inputFile = string.Format("{0}input_{1}.txt", inputFileDir, idx);
            //string file = string.Format("{0}output_{1}.txt", outputFileDir, idx);
            //string stdFile = string.Format("{0}output_{1}.txt", stdOutputFileDir, idx);
            //StreamReader inSr = new StreamReader(inputFile);
            //StreamReader sr = new StreamReader(file);
            //StreamReader stdSr = new StreamReader(stdFile);
            //int lineNumber = 0;
            //String line, stdLine, inLine;
            //List<Record> list = new List<Record>();
            //while (!sr.EndOfStream && !stdSr.EndOfStream)
            //{
            //    inLine = inSr.ReadLine();
            //    line = sr.ReadLine();
            //    stdLine = stdSr.ReadLine();
            //    lineNumber++;
            //    Record rec = new Record();
            //    String[] fields = inLine.Split(',');
            //    rec.Time = int.Parse(fields[0]);
            //    rec.Lat = double.Parse(fields[1]);
            //    rec.Lng = double.Parse(fields[2]);
            //    //output
            //    fields = line.Split(',');
            //    int time = int.Parse(fields[0]);
            //    Debug.Assert(time == rec.Time);  //时间须相同
            //    rec.EdgeId = int.Parse(fields[1]);
            //    rec.Confidence = double.Parse(fields[2]);
            //    //stdoutput
            //    fields = stdLine.Split(',');
            //    time = int.Parse(fields[0]);
            //    Debug.Assert(time == rec.Time);  //时间须相同
            //    rec.StdEdgeId = int.Parse(fields[1]);
            //    list.Add(rec);
            //}
            //inSr.Close();
            //sr.Close();
            //stdSr.Close();
            //_parent.drawRoute(list);
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void miOpenTrj_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Trajectory Files (*.trj;*.txt)|*.trj;*.txt|All Files (*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                cmpFile(dlg.FileName);
            }
        }
    }
}
