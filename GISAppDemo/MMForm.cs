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
using TrjTools;
using TrjTools.RoadNetwork;
using TrjTools.MapMatching;
using System.IO;
using System.Diagnostics;

namespace GISAppDemo
{
    public partial class MMForm : Form
    {
        private IGraphProvider provider = null;
        private List<Trajectory> trjs = new List<Trajectory>();
        private List<String> trjFiles = new List<String>();
        private string outputDir = Directory.GetCurrentDirectory();

        //public MMForm()
        //{
        //    InitializeComponent();
        //    init();
        //}
        public MMForm(IGraphProvider provider)
        {
            InitializeComponent();
            init();
            this.provider = provider;
        }
        private void init()
        {
            tbOutputDir.Text = outputDir;
        }
        private void showTrjList()
        {
            lbTrjs.DataSource = null;
            lbTrjs.DataSource = trjFiles;
            lbTrjs.Refresh();
        }
        private void btnAddTrj_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "Trajectory Files (*.trj;*.txt)|*.trj;*.txt|All Files (*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in dialog.FileNames)
                {
                    trjFiles.Add(file);
                }
            }
            showTrjList();
        }

        private void btnClearTrj_Click(object sender, EventArgs e)
        {
            trjFiles.Clear();
            showTrjList();
        }

        private void btnChooseOutputDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                outputDir = dialog.SelectedPath;
                tbOutputDir.Text = outputDir;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            MM mm = new MM(provider.graph);
            foreach (String trjFile in trjFiles)
            {
                Trajectory trj = new Trajectory(trjFile);
                Trajectory newTrj=mm.match(trj);
                String fileName = Path.Combine(outputDir, Path.GetFileName(trjFile));
                newTrj.Save(fileName);
            }
            String notice = String.Format("Open directory to find the output file(s) in {0}?", outputDir);
            if (MessageBox.Show(notice, "Mission Complete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                Process.Start(outputDir);
            }
        }
    }
}
