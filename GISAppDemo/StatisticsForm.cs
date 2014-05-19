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
using TrjTools.MapMatching;
using GeoAPI.Geometries;
using BruTile;
using BruTile.Web;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;
using SharpMap.Layers;
using SharpMap.Forms;
using SharpMap.Data.Providers;
using SharpMap.Styles;
using NetTopologySuite.Geometries;
using TrjTools.RoadNetwork;
using TrjTools.Tools;
using Point = NetTopologySuite.Geometries.Point;

namespace GISAppDemo
{
    public partial class StatisticsForm : Form
    {
        MapForm parent = null;
        public StatisticsForm(MapForm parent)
        {
            this.parent = parent;
            InitializeComponent();

        }
        private List<Point>[] startPoints = new List<Point>[] { new List<Point>(), new List<Point>() };
        private List<Point>[] endPoints = new List<Point>[] { new List<Point>(), new List<Point>() };
        public StatisticsForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            double threshold = double.Parse(tbThreshold.Text);
            String fileName = Path.Combine(Constants.DATA_DIR, "query_result",
                String.Format("d{0}_q{1}.txt", lbSrc.SelectedItem, lbQuery.SelectedItem));
            for (int i = 0; i < 2; ++i)
            {
                startPoints[i].Clear();
                endPoints[i].Clear();
            }
            loadFile(fileName, threshold);
            showStatus();
            if (cbStartSuccess.Checked)
            {
                parent.drawPoint(startPoints[0], MapForm.PointType.GreenPoint);
            }
            if (cbStartFail.Checked)
            {
                parent.drawPoint(startPoints[1], MapForm.PointType.RedPoint);
            }
            if (cbEndSuccess.Checked)
            {
                parent.drawPoint(endPoints[1], MapForm.PointType.GreenPoint);
            }
            if (cbEndFail.Checked)
            {
                parent.drawPoint(endPoints[1], MapForm.PointType.RedPoint);
            }
        }

        private void showStatus()
        {
            int totalCount = 0;
            int[] startCounts = new int[2] { startPoints[0].Count, startPoints[1].Count };
            int[] endCounts = new int[2] { endPoints[0].Count, endPoints[1].Count };
            totalCount = startCounts[0] + startCounts[1];
            lbStart.Text = String.Format("S:{0:0.0}%, F:{1:0.0}",
                startCounts[0] * 100.0 / totalCount,
                startCounts[1] * 100.0 / totalCount);
            lbEnd.Text = String.Format("S:{0:0.0}%, F:{1:0.0}",
                endCounts[0] * 100.0 / totalCount,
                endCounts[1] * 100.0 / totalCount);
            lbCount.Text = totalCount.ToString();

        }
        private void loadFile(String fileName, double threshold)
        {
            String[] lines = File.ReadAllLines(fileName);
            int lineCount = lines.Length;
            for (int i = 0; i < lineCount; ++i)
            {
                String[] fields = lines[i].Split(',');
                GeoPoint startPoint = new GeoPoint(
                    double.Parse(fields[3]), double.Parse(fields[4]));
                GeoPoint endPoint = new GeoPoint(
                    double.Parse(fields[5]), double.Parse(fields[6]));
                double startDist = double.Parse(fields[7]);
                double endDist = double.Parse(fields[8]);
                if (startDist <= threshold)
                {
                    startPoints[0].Add(startPoint.ToPoint());
                }
                else
                {
                    startPoints[1].Add(startPoint.ToPoint());
                }
                if (endDist <= threshold)
                {
                    endPoints[0].Add(endPoint.ToPoint());
                }
                else
                {
                    endPoints[1].Add(endPoint.ToPoint());
                }
            }
        }

        private void StatisticsForm_Load(object sender, EventArgs e)
        {
            lbSrc.SelectedIndex = 0;
            lbQuery.SelectedIndex = 0;
            tbThreshold.Text = "30";
        }

        private void cbStartFail_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbStartSuccess_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbEndSuccess_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbEndFail_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            parent.ClearMap();
            parent.RefreshMap();
        }

        private void btnLoadQuery_Click(object sender, EventArgs e)
        {
            String targetDir = Path.Combine(Constants.DATA_DIR, "query_result/bus_query");
            String busQueryFileName = Path.Combine(targetDir, "d20121201_bus_dev800_sample0.1.csv");
            //String busQueryFileName = Path.Combine(Constants.DATA_DIR, "query/bus_query.csv");
            double threshold = double.Parse(tbThreshold.Text);
            for (int i = 0; i < 2; ++i)
            {
                startPoints[i].Clear();
                endPoints[i].Clear();
            }
            loadFile(busQueryFileName, threshold);
            showStatus();
            if (cbStartSuccess.Checked)
            {
                parent.drawPoint(startPoints[0], MapForm.PointType.GreenPoint);
            }
            if (cbStartFail.Checked)
            {
                parent.drawPoint(startPoints[1], MapForm.PointType.RedPoint);
            }
            if (cbEndSuccess.Checked)
            {
                parent.drawPoint(endPoints[1], MapForm.PointType.GreenPoint);
            }
            if (cbEndFail.Checked)
            {
                parent.drawPoint(endPoints[1], MapForm.PointType.RedPoint);
            }
        }
    }
} 
