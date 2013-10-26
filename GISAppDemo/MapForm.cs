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
using BruTile;
using BruTile.Web;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;
using SharpMap.Layers;
using SharpMap.Forms;
using SharpMap.Data.Providers;
using SharpMap.Styles;
using NetTopologySuite.Geometries;
using Point = NetTopologySuite.Geometries.Point;
using TrjTools.RoadNetwork;
using TrjTools.Tools;
using GeoAPI.Geometries;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeometryTransform = GeoAPI.CoordinateSystems.Transformations.GeometryTransform;
using TrjTools.MapMatching;

namespace GISAppDemo
{
    public partial class MapForm : Form, IMapViewer,IGraphProvider
    {
        /// <summary>
        /// Map layers
        /// </summary>
        private Dictionary<string, TileLayer> m_layers;
        private GeometryProvider outputProvider = new GeometryProvider(new List<IGeometry>());
        private GeometryProvider stdOutputProvider = new GeometryProvider(new List<IGeometry>());
        private GeometryProvider markerProvider = new GeometryProvider(new List<IGeometry>());
        private Timer m_timer = new Timer();
        private TileLayer m_currentLayer = null;
        GeometryFactory geofactory = new GeometryFactory();

        //Layers
        VectorLayer markerLayer = null;
        VectorLayer outputLayer = null;
        VectorLayer stdOutputLayer = null;
        VectorLayer shapeLayer = null;
        LabelLayer edgeIdLayer = null;
        //Graph
        //private static string DATASET_DIR = Path.Combine(Constants.MAP_DIR, "WA");
        //private string MAP_DIR = Path.Combine(Constants.MAP_DIR, "Beijing_2011");
        private string MAP_DIR = Path.Combine(Constants.MAP_DIR, "Beijing_2011");
       
        private Graph m_graph = null;

        public Graph graph
        {
            get 
            {
                if (m_graph == null)
                {
                    initMap(MAP_DIR);
                }
                return m_graph; 
            }
        }
        private bool ruler = false;
        private bool auto_zoom = true;

        //Files
        String shpFileName = Path.Combine(Constants.DATA_DIR, "map\\Beijing_2011\\beijing.shp");

        //private Timer m_timer;
        public MapForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            _InitialLayers();
        }

        private void _InitialLayers()
        {
            m_layers = new Dictionary<string, TileLayer>();
            TileAsyncLayer bingLayer = new TileAsyncLayer(new BingTileSource(BingRequest.UrlBing, "", BingMapType.Aerial), "TileLayer - Bing");
            //TileAsyncLayer googleLayer = new TileAsyncLayer(new BingTileSource(BingRequest.UrlBing, "", BingMapType.Roads), "TileLayer - Bing");
            TileAsyncLayer googleLayer = new TileAsyncLayer(new GoogleTileSource(new GoogleRequest(GoogleMapType.GoogleMap), new BruTile.Cache.MemoryCache<byte[]>(100, 1000)), "TileLayer-Google");
            //TileAsyncLayer bingLayer = new TileAsyncLayer(new GoogleTileSource(new GoogleRequest(GoogleMapType.GoogleSatellite), new BruTile.Cache.MemoryCache<byte[]>(100, 1000)), "Satellite-Google");
            TileAsyncLayer hybrid = new TileAsyncLayer(new BingTileSource(BingRequest.UrlBing, "", BingMapType.Hybrid), "TileLayer - Bing Hybrid");
            m_layers.Add("Bing", bingLayer);
            m_layers.Add("Google", googleLayer);
            m_layers.Add("Hybrid", hybrid);

            //Marker layer
            markerLayer = new VectorLayer("Fixed Marker");
            markerLayer.Style.Symbol = GISAppDemo.Properties.Resources.OutfallSmall;
            markerLayer.DataSource = markerProvider;
            Color tRed = Color.FromArgb(50, Color.Red);
            markerLayer.Style.Fill = new SolidBrush(tRed);

            //output layer
            outputLayer = new VectorLayer("Output");
            outputLayer.Style.Symbol = GISAppDemo.Properties.Resources.g_arrow;
            outputLayer.Style.Line.Color = Color.FromArgb(120, Color.Cyan);
            outputLayer.Style.Line.Width = 3.0F;
            //Color tRed = Color.FromArgb(128, Color.Red);
            //outputLayer.Style.Fill = new SolidBrush(tRed);
            outputLayer.DataSource = outputProvider;


            //standard output layer
            stdOutputLayer = new VectorLayer("StdOutput");
            stdOutputLayer.Style.Symbol = GISAppDemo.Properties.Resources.r_arrow;
            stdOutputLayer.Style.Line.Color = Color.FromArgb(85, Color.Red);
            stdOutputLayer.Style.Line.Width = 3.0F;
            stdOutputLayer.DataSource = stdOutputProvider;

            //Shape Layer
            shapeLayer = new SharpMap.Layers.VectorLayer("Shape");
            shapeLayer.DataSource = new SharpMap.Data.Providers.ShapeFile(shpFileName, true);
            //Set fill-style to green
            shapeLayer.Style.Fill = new SolidBrush(Color.Green);
            //Set the polygons to have a black outline
            shapeLayer.Style.Outline = Pens.Black;
            shapeLayer.Style.Line.Color = Color.Navy;
            shapeLayer.Style.Line.Width = 1;
            shapeLayer.Style.PointSize = 2;
            shapeLayer.Style.PointColor = new SolidBrush(Color.Blue);
            shapeLayer.SRID = 4326;
            shapeLayer.CoordinateTransformation = LayerTools.Wgs84toGoogleMercator;

            //edgeid layer
            edgeIdLayer = new LabelLayer("EdgeId");
            edgeIdLayer.DataSource = shapeLayer.DataSource;
            edgeIdLayer.LabelColumn = "ID";
            edgeIdLayer.Style.ForeColor = Color.Navy;
            edgeIdLayer.Style.Font = new Font(FontFamily.GenericSerif, 16);
            edgeIdLayer.CoordinateTransformation = LayerTools.Wgs84toGoogleMercator;
        }

        private void MapForm_Load(object sender, EventArgs e)
        {
            //Add BackgroundLayer
            mbMap.Map.BackgroundLayer.Add(m_layers["Google"]);
            m_currentLayer = m_layers["Google"];
            changeBgLayer("None");

            //Add Beijing Map
            this.mbMap.Map.Layers.Add(shapeLayer);
            shapeLayer.Enabled = miShowShapeLayer.Checked;
            
            // Edgeid layer
            this.mbMap.Map.Layers.Add(edgeIdLayer);
            edgeIdLayer.Enabled = miShowEdgeId.Checked && miShowShapeLayer.Checked;

            //Add the static layer for symbol
            this.mbMap.Map.Layers.Add(markerLayer);

            //Add a pushpin layer
            this.mbMap.Map.Layers.Add(outputLayer);

            //Add std output layer
            this.mbMap.Map.Layers.Add(stdOutputLayer);

            //Initialize the timer
            initTimer();

            //Config the active tool
            mbMap.ActiveTool = MapBox.Tools.Pan;
            mbMap.ActiveToolChanged += new MapBox.ActiveToolChangedHandler(mbMap_ActiveToolChanged);

            //Move and zoom the map

            IMathTransform mathTransform = LayerTools.Wgs84toGoogleMercator.MathTransform;
            Envelope geom = GeoAPI.CoordinateSystems.Transformations.GeometryTransform.TransformBox(
                new Envelope(116.298, 116.399, 39.9695, 39.9896),
                mathTransform);
            this.mbMap.Map.ZoomToBox(geom);
            RefreshMap();
        }

        private void initTimer()
        {
            m_timer.Interval = 20;
            m_timer.Tick += m_timer_Tick;
            m_timer.Stop();
        }

        private void initMap(String mapDir)
        {
            string EDGE_FILE = Path.Combine(MAP_DIR, "edges.txt");
            string VERTEX_FILE = Path.Combine(MAP_DIR, "vertices.txt");
            string EDGE_GEOMETRY_FILE = Path.Combine(MAP_DIR, "geos.txt");
            if (File.Exists(EDGE_FILE) && File.Exists(VERTEX_FILE) && File.Exists(EDGE_GEOMETRY_FILE))
            {
                m_graph = new Graph(VERTEX_FILE, EDGE_FILE, EDGE_GEOMETRY_FILE);
            }
            else
            {
                String notice = "The map directory should contain the following three files: vertices.txt, edges.txt and geos.txt";
                MessageBox.Show(notice, "Invalid directory");
            }
        }

        /// <summary>
        /// Move the object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_timer_Tick(object sender, EventArgs e)
        {
            VariableLayerCollection.TouchTimer();
            //RefreshMap();
        }

        void mbMap_ActiveToolChanged(MapBox.Tools tool)
        {
        }

        private void MapForm_SizeChanged(object sender, EventArgs e)
        {
            //mbMap.Map.
            RefreshMap();
        }

        #region Buttons_Click
        private void btnPan_Click(object sender, EventArgs e)
        {
            mbMap.ActiveTool = SharpMap.Forms.MapBox.Tools.Pan;
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            //mbMap.ActiveTool = SharpMap.Forms.MapBox.Tools.Query;
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            mbMap.ActiveTool = MapBox.Tools.ZoomOut;
        }

        private void btnZoomWin_Click(object sender, EventArgs e)
        {
            mbMap.ActiveTool = MapBox.Tools.ZoomWindow;
        }

        private void btnDrawPoint_Click(object sender, EventArgs e)
        {
            mbMap.ActiveTool = MapBox.Tools.DrawPoint;
        }

        private void btnDrawLine_Click(object sender, EventArgs e)
        {
            mbMap.ActiveTool = MapBox.Tools.DrawLine;
        }

        private void btnRuler_Click(object sender, EventArgs e)
        {
            mbMap.ActiveTool = MapBox.Tools.DrawLine;
            ruler = true;
        }

        private void mbMap_GeometryDefined(IGeometry geometry)
        {
            double length = 0;
            if (geometry.GetType() == typeof(LineString) && ruler)
            {
                geometry = GeometryTransform.TransformGeometry(geometry, LayerTools.GoogleMercatorToWgs84.MathTransform, geofactory);
                length = LayerTools.GetLength(geometry as LineString);
                MessageBox.Show("Length:" + length + "m.");
                ruler = false;
            }
            else
            {
                outputProvider.Geometries.Add(geometry);
            }
            mbMap.ActiveTool = MapBox.Tools.Pan;
            RefreshMap();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearMap();
            RefreshMap();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Test.Start();
        }

        private void btnCmp_Click(object sender, EventArgs e)
        {
            CmpForm frm = new CmpForm(this);
            frm.Show();
        }
        #endregion Buttons_Click

        #region MenuItems_Click
        private void miExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Load a new shape file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miOpenShp_Click(object sender, EventArgs e)
        {
            //OpenFileDialog f = new OpenFileDialog();
            OpenFileDialog f = new OpenFileDialog();
            //f.InitialDirectory = Directory.GetCurrentDirectory();
            f.FileName = shpFileName;
            f.Filter = "Shape Files (.shp)|*.shp|All Files (*.*)|*.*"; 
            if (f.ShowDialog() == DialogResult.OK)
            {
                // outputFileDir = f.SelectedPath + "\\";
                //MessageBox.Show(f.SelectedPath);
                shpFileName = f.FileName;
                shapeLayer.DataSource = new SharpMap.Data.Providers.ShapeFile(shpFileName, true);
                shapeLayer.Enabled = true;
                edgeIdLayer.DataSource = shapeLayer.DataSource;
                edgeIdLayer.Enabled = miShowEdgeId.Checked;
                //Disable bglayer
                //m_currentLayer.Enabled = false;
                mbMap.Map.ZoomToBox(shapeLayer.Envelope);
            }
        }

        private void miStreetView_Click(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                changeBgLayer(mi.Text);
                foreach (MenuItem i in mi.Parent.MenuItems)
                {
                    i.Checked = false;
                }
                mi.Checked = true;
            }
        }

        private void miSatelliteView_Click(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                changeBgLayer(mi.Text);
                foreach (MenuItem i in mi.Parent.MenuItems)
                {
                    i.Checked = false;
                }
                mi.Checked = true;
            }
        }

        private void miHybridView_Click(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                changeBgLayer(mi.Text);
                foreach (MenuItem i in mi.Parent.MenuItems)
                {
                    i.Checked = false;
                }
                mi.Checked = true;
            }
        }

        private void miNoneView_Click(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                changeBgLayer(mi.Text);
                foreach (MenuItem i in mi.Parent.MenuItems)
                {
                    i.Checked = false;
                }
                mi.Checked = true;
            }
        }

        private void miShowMarker_Click(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null && markerLayer != null)
            {
                mi.Checked = !mi.Checked;
                markerLayer.Enabled = mi.Checked;
                RefreshMap();
            }
        }

        private void miShowTrj_Click(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null && outputLayer != null)
            {
                mi.Checked = !mi.Checked;
                outputLayer.Enabled = mi.Checked;
                RefreshMap();
            }
        }

        private void miShowStandardTrj_Click(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null && stdOutputLayer != null)
            {
                mi.Checked = !mi.Checked;
                stdOutputLayer.Enabled = mi.Checked;
                RefreshMap();
            }
        }

        private void miShowEdgeId_Click(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null && edgeIdLayer != null)
            {
                mi.Checked = !mi.Checked;
                edgeIdLayer.Enabled = mi.Checked;
                RefreshMap();
            }
        }

        private void miAutoScale_Click(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                mi.Checked = !mi.Checked;
                auto_zoom = mi.Checked;
            }
        }

        private void miClearView_Click(object sender, EventArgs e)
        {
            ClearMap();
            RefreshMap();
        }

        private void miAbout_Click(object sender, EventArgs e)
        {
            About frm = new About();
            frm.Show();
        }

        private void miOpenTrj_Click(object sender, EventArgs e)
        {
            //OpenFileDialog f = new OpenFileDialog();
            OpenFileDialog f = new OpenFileDialog();
            //f.InitialDirectory = Directory.GetCurrentDirectory();
            f.FileName = shpFileName;
            f.Filter = "Trajectory Files (*.trj;*.txt)|*.trj;*.txt|All Files (*.*)|*.*";
            if (f.ShowDialog() == DialogResult.OK)
            {
                // outputFileDir = f.SelectedPath + "\\";
                //MessageBox.Show(f.SelectedPath);
                Trajectory trj = new Trajectory(f.FileName, 2);
                drawTrj(trj);
                //MM mm = new MM(graph);
                //var newTrj = mm.match(trj);
                //drawPath(newTrj.Path.Edges);
            }
            
        }

        private void mbMap_MouseMove(Coordinate worldPos, MouseEventArgs imagePos)
        {
            Point p = new Point(worldPos);
            p = GeometryTransform.TransformGeometry(p, LayerTools.GoogleMercatorToWgs84.MathTransform, geofactory) as Point;
            lbPosition.Text = String.Format("X:{0:#.000000}\nY: {1:#.000000}", p.X, p.Y);
            //lbPosition.Text = String.Format("X:{0}\nY:{1}", worldPos.X, worldPos.Y);
        }

        private void miShowShapeLayer_Click(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                mi.Checked = !mi.Checked;
                shapeLayer.Enabled = mi.Checked;
                RefreshMap();
            }
        }

        private void miDrawPoint_Click(object sender, EventArgs e)
        {
            mbMap.ActiveTool = MapBox.Tools.DrawPoint;
        }

        private void miDrawLine_Click(object sender, EventArgs e)
        {
            mbMap.ActiveTool = MapBox.Tools.DrawLine;
        }

        private void miDrawPolygon_Click(object sender, EventArgs e)
        {
            mbMap.ActiveTool = MapBox.Tools.DrawPolygon;
        }

        private void miMapmatching_Click(object sender, EventArgs e)
        {
            MMForm frm = new MMForm(this);
            frm.Show();
        }

        private void miLoadMap_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Choose the directory of the map.\nNotice that this directory should contain the following three files: vertices.txt, edges.txt and geos.txt";
            dialog.SelectedPath = this.MAP_DIR;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.MAP_DIR = dialog.SelectedPath;
                initMap(this.MAP_DIR);
                MessageBox.Show("Load complete!");
            }
        }

        private void miCompare_Click(object sender, EventArgs e)
        {
            CmpForm frm = new CmpForm(this);
            frm.Show();
        }
        #endregion MenuItems_Click

        #region Draw_Methods
        public void drawPoint(double lat, double lng)
        {
            drawPoint(new Point(lng, lat));
        }

        public void drawLine(long edgeId)
        {
            Edge e = graph.Edges[edgeId];
            LineString line = (LineString)GeometryTransform.TransformGeometry(e.ToLineString(), LayerTools.Wgs84toGoogleMercator.MathTransform, geofactory);
            outputProvider.Geometries.Add(line);
            outputProvider.Geometries.Add(line.EndPoint);
            include(e);
            RefreshMap();
        }

        private void drawLine(Edge e)
        {
            LineString line = e.ToLineString();
            drawLine(line);
        }

        private void drawLine(LineString line)
        {
            line = (LineString)GeometryTransform.TransformGeometry(line, LayerTools.Wgs84toGoogleMercator.MathTransform, geofactory);
            outputProvider.Geometries.Add(line);
            outputProvider.Geometries.Add(line.EndPoint);
            include(line);
            RefreshMap();
        }

        private void drawTrj(Trajectory trj)
        {
            drawLine(trj.ToLineString());
            List<Point> points = new List<Point>();
            for (int i = 0; i < trj.Count; ++i)
            {
                points.Add(trj[i].point.ToPoint());
            }
            drawPoint(points);
        }

        private void drawStdLine(LineString line)
        {
            line = (LineString)GeometryTransform.TransformGeometry(line, LayerTools.Wgs84toGoogleMercator.MathTransform, geofactory);
            stdOutputProvider.Geometries.Add(line);
            stdOutputProvider.Geometries.Add(line.EndPoint);
            include(line);
            RefreshMap();
        }

        public void drawStdLine(long edgeId)
        {
            Edge e = graph.Edges[edgeId];
            LineString line = (LineString)GeometryTransform.TransformGeometry(e.ToLineString(), LayerTools.Wgs84toGoogleMercator.MathTransform, geofactory);
            stdOutputProvider.Geometries.Add(line);
            stdOutputProvider.Geometries.Add(line.EndPoint);
            include(e);
            RefreshMap();
        }
        /// <summary>
        /// 绘制出
        /// </summary>
        /// <param name="records"></param>
        public void drawRoute(List<CmpForm.Record> records)
        {
            ClearMap();
            List<LineString> routes = new List<LineString>();
            List<LineString> stdRoutes = new List<LineString>();
            LineString route = null;
            LineString stdRoute = null;
            CoordinateList routeCoord = new CoordinateList();
            CoordinateList stdRouteCoord = new CoordinateList();
            List<Point> points = new List<Point>();
            Edge edge = null, prev_edge = null;
            Edge stdEdge = null, prev_stdEdge = null;
            for (int i = 0; i < records.Count; i++)
            {
                CmpForm.Record r = records[i];
                if (r.EdgeId >= 0)
                {
                    edge = graph.Edges[r.EdgeId];
                    if (edge != prev_edge)
                    {
                        if (prev_edge == null)
                        {
                            routeCoord.Add(edge.Start.Corrdinate);
                            routeCoord.Add(edge.End.Corrdinate);
                        }
                        else if (edge.Start.ID != prev_edge.End.ID)
                        {
                            routes.Add(new LineString(routeCoord.ToArray()));
                            routeCoord.Clear();
                            routeCoord.Add(edge.Start.Corrdinate);
                            routeCoord.Add(edge.End.Corrdinate);
                        }
                        else
                        {
                            routeCoord.Add(edge.End.Corrdinate);
                        }

                    }
                    prev_edge = edge;
                }
                if (r.StdEdgeId >= 0)
                {
                    stdEdge = graph.Edges[r.StdEdgeId];
                    if (stdEdge != prev_stdEdge)
                    {
                        if (prev_stdEdge == null)
                        {
                            stdRouteCoord.Add(stdEdge.Start.Corrdinate);
                            stdRouteCoord.Add(stdEdge.End.Corrdinate);
                        }
                        else if (stdEdge.Start.ID != prev_stdEdge.End.ID)
                        {
                            stdRoutes.Add(new LineString(stdRouteCoord.ToArray()));
                            stdRouteCoord.Add(stdEdge.Start.Corrdinate);
                            stdRouteCoord.Add(stdEdge.End.Corrdinate);
                        }
                        else
                        {
                            stdRouteCoord.Add(stdEdge.End.Corrdinate);
                        }
                        //stdRoutes.Add(stdRoute);
                    }
                    prev_stdEdge = stdEdge;
                }
                Point p = new Point(r.Lng, r.Lat);
                points.Add(p);

            }
            if (routeCoord.Count > 0)
            {
                routes.Add(new LineString(routeCoord.ToArray()));
            }
            if (stdRouteCoord.Count > 0)
            {
                stdRoutes.Add(new LineString(stdRouteCoord.ToArray()));
            }

            foreach (LineString r in routes)
            {
                drawLine(r);
            }
            foreach (LineString r in stdRoutes)
            {
                drawStdLine(r);
            }
            //drawStdLine(stdRoute);
            drawPoint(points);
            RefreshMap();
        }

        private void drawPath(List<Edge> path)
        {
            if (path.Count < 1) return;
            // 1. Get full path
            List<Edge> fullPath = new List<Edge>();
            Edge lastEdge = path[0];
            fullPath.Add(lastEdge);
            for (int i = 1; i < path.Count; ++i)
            {
                Edge edge = path[i];
                if (edge == null) continue;
                if (lastEdge != null && edge.Start != lastEdge.End)
                {
                    double maxDist = 2000;
                    if (GeoPoint.GetDistance(lastEdge.End.Point, edge.Start.Point) < maxDist) //fill with shortest path if less than five 
                    {
                        var edges = graph.FindPath(lastEdge.End, edge.Start, maxDist * 1.5);
                        if (edges != null)
                        {
                            fullPath.AddRange(edges);
                        }
                    }
                }
                fullPath.Add(edge);
                lastEdge = edge;
            }
            List<Coordinate> route = new List<Coordinate>();
            foreach (Edge edge in fullPath)
            {
                if (edge == null) continue;
                route.AddRange(edge.Geo.ToCoordinateArray());
            }
            drawStdLine(new LineString(route.ToArray()));
            RefreshMap();
        }
        /// <summary>
        /// 缩放视图以使e占全图的50%
        /// </summary>
        /// <param name="e"></param>
        private void include(Edge e)
        {
            Envelope box = mbMap.Map.Envelope;
            Envelope edgeBox = GeometryTransform.TransformBox(e.GetBox(), LayerTools.Wgs84toGoogleMercator.MathTransform);
            include(edgeBox);
        }
        /// <summary>
        /// 缩放视图以使e占全图的50%
        /// </summary>
        /// <param name="e"></param>
        private void include(LineString line)
        {
            Envelope box = mbMap.Map.Envelope;
            foreach (var coord in line.Coordinates)
            {
                box.Merge(new Point(coord));
            }
            mbMap.Map.ZoomToBox(box);
        }
        /// <summary>
        /// 缩放视图以使地图包容p
        /// </summary>
        /// <param name="e"></param>
        private void include(Point p)
        {
            Envelope box = mbMap.Map.Envelope;
            if (!box.Contains(p.Coordinate))
            {
                box = box.Merge(p);
                mbMap.Map.ZoomToBox(box);
                RefreshMap();
            }
        }

        private void include(Envelope box)
        {
            Envelope mainBox = mbMap.Map.Envelope;
            if (mainBox.Contains(box))
            {
                if (mainBox.Width > 30 * box.Width)
                {
                    //窗体过大
                    mainBox = box.Magnify(2);
                    if (auto_zoom)
                    {
                        mbMap.Map.ZoomToBox(mainBox);
                    }
                    RefreshMap();
                }
            }
            else
            {
                mainBox = mainBox.Merge(box);
                if (auto_zoom)
                {
                    mbMap.Map.ZoomToBox(mainBox);
                }
                RefreshMap();
            }
        }

        private void drawPoint(Vertex v)
        {
            drawPoint(new Point(v.Corrdinate));
        }

        private void drawPoint(Point p)
        {
            p = (Point)GeometryTransform.TransformGeometry(p, LayerTools.Wgs84toGoogleMercator.MathTransform, geofactory);
            markerProvider.Geometries.Add(p);
            // include(p);
            RefreshMap();
        }

        private void drawPoint(IEnumerable<Point> points)
        {
            foreach (Point p in points)
            {
                Point tmp_p = (Point)GeometryTransform.TransformGeometry(p, LayerTools.Wgs84toGoogleMercator.MathTransform, geofactory);
                markerProvider.Geometries.Add(tmp_p);
                // include(tmp_p);
            }
            RefreshMap();
        }

        #endregion Draw_Methods

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.S:
                    {
                        btnCmp_Click(null, null);
                        break;
                    }
                case Keys.M:
                    {
                        btnPan_Click(null, null);
                        break;
                    }
                case Keys.P:
                    {
                        btnDrawPoint_Click(null, null);
                        break;
                    }
                case Keys.L:
                    {
                        btnDrawLine_Click(null, null);
                        break;
                    }
                case Keys.E:
                    {
                        break;
                    }
                case Keys.Control | Keys.T:
                    {
                        btnTest_Click(null, null);
                        break;
                    }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void changeBgLayer(String mode)
        {
            TileLayer tempLayer = null;
            switch (mode.ToLower())
            {
                case "street":
                    {
                        tempLayer = m_layers["Google"];
                        break;
                    }
                case "satellite":
                    {
                        tempLayer = m_layers["Bing"];
                        break;
                    }
                case "hybrid":
                    {
                        tempLayer = m_layers["Hybrid"];
                        break;
                    }
                case "none":
                    {
                        tempLayer = null;
                        break;
                    }
            }
            if (tempLayer != null)
            {
                m_currentLayer = tempLayer;
                m_currentLayer.Enabled = true;
                mbMap.Map.BackgroundLayer.Clear();
                mbMap.Map.BackgroundLayer.Add(tempLayer);
            }
            else
            {
                m_currentLayer.Enabled = false;
            }
            RefreshMap();

        }

        public void ClearMap()
        {
            outputProvider.Geometries.Clear();
            stdOutputProvider.Geometries.Clear();
            markerProvider.Geometries.Clear();
        }

        public void RefreshMap()
        {
            if (mbMap.InvokeRequired)
            {
                Action a = () => mbMap.Refresh();
                mbMap.Invoke(a);
            }
            else
            {
                mbMap.Refresh();
            }
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
        ///  显示edge的详细信息
        /// </summary>
        /// <param name="eid">edge id</param>
        public void showEdgeInfo(int eid)
        {

            Edge e = graph.Edges[eid];
            if (e != null)
            {
                double length = LayerTools.GetDistance(e.Start.Point, e.End.Point);
                //double cost = e.Cost;
                double cost = 0;
                MessageBox.Show(string.Format("{0} --> {1} : {2}m\nCost:{3},Length/Cost:{4}", e.Start.ID, e.End.ID, length, cost, length / cost));
            }
        }

        private void miFindEdge_Click(object sender, EventArgs e)
        {
            InputDialog dialog = new InputDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                long eid;
                if (long.TryParse(dialog.Result, out eid))
                {
                    drawStdLine(eid);
                }
            }
        }

        private void miFindVertex_Click(object sender, EventArgs e)
        {

        }
    }
}
