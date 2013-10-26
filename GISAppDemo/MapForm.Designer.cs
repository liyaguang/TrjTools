namespace GISAppDemo
{
    partial class MapForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mbMap = new SharpMap.Forms.MapBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnDrawLine = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnZoomWin = new System.Windows.Forms.Button();
            this.btnPan = new System.Windows.Forms.Button();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.miOpenShp = new System.Windows.Forms.MenuItem();
            this.miOpenTrj = new System.Windows.Forms.MenuItem();
            this.miLoadMap = new System.Windows.Forms.MenuItem();
            this.miExit = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.miNoneView = new System.Windows.Forms.MenuItem();
            this.miStreetView = new System.Windows.Forms.MenuItem();
            this.miSatelliteView = new System.Windows.Forms.MenuItem();
            this.miHybridView = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.miShowMarker = new System.Windows.Forms.MenuItem();
            this.miShowTrj = new System.Windows.Forms.MenuItem();
            this.miShowStandardTrj = new System.Windows.Forms.MenuItem();
            this.miShowEdgeId = new System.Windows.Forms.MenuItem();
            this.miShowShapeLayer = new System.Windows.Forms.MenuItem();
            this.miClearView = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.miDrawPoint = new System.Windows.Forms.MenuItem();
            this.miDrawLine = new System.Windows.Forms.MenuItem();
            this.miDrawPolygon = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.miFindEdge = new System.Windows.Forms.MenuItem();
            this.miFindVertex = new System.Windows.Forms.MenuItem();
            this.miMapmatching = new System.Windows.Forms.MenuItem();
            this.miCompare = new System.Windows.Forms.MenuItem();
            this.btnTest = new System.Windows.Forms.MenuItem();
            this.miOption = new System.Windows.Forms.MenuItem();
            this.miAutoScale = new System.Windows.Forms.MenuItem();
            this.miAbout = new System.Windows.Forms.MenuItem();
            this.lbPosition = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mbMap
            // 
            this.mbMap.ActiveTool = SharpMap.Forms.MapBox.Tools.Pan;
            this.mbMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mbMap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mbMap.FineZoomFactor = 10D;
            this.mbMap.Location = new System.Drawing.Point(3, 2);
            this.mbMap.MapQueryMode = SharpMap.Forms.MapBox.MapQueryType.LayerByIndex;
            this.mbMap.Name = "mbMap";
            this.mbMap.QueryGrowFactor = 5F;
            this.mbMap.QueryLayerIndex = 0;
            this.mbMap.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.mbMap.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.mbMap.ShowProgressUpdate = true;
            this.mbMap.Size = new System.Drawing.Size(581, 521);
            this.mbMap.TabIndex = 0;
            this.mbMap.Text = "mapBox1";
            this.mbMap.WheelZoomMagnitude = -2D;
            this.mbMap.MouseMove += new SharpMap.Forms.MapBox.MouseEventHandler(this.mbMap_MouseMove);
            this.mbMap.GeometryDefined += new SharpMap.Forms.MapBox.GeometryDefinedHandler(this.mbMap_GeometryDefined);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnClear);
            this.groupBox2.Controls.Add(this.btnDrawLine);
            this.groupBox2.Controls.Add(this.btnZoomOut);
            this.groupBox2.Controls.Add(this.btnZoomWin);
            this.groupBox2.Controls.Add(this.btnPan);
            this.groupBox2.Location = new System.Drawing.Point(592, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(84, 317);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tools";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnClear.Location = new System.Drawing.Point(12, 259);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(62, 30);
            this.btnClear.TabIndex = 20;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnDrawLine
            // 
            this.btnDrawLine.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnDrawLine.Location = new System.Drawing.Point(12, 202);
            this.btnDrawLine.Name = "btnDrawLine";
            this.btnDrawLine.Size = new System.Drawing.Size(62, 30);
            this.btnDrawLine.TabIndex = 18;
            this.btnDrawLine.Text = "Ruler";
            this.btnDrawLine.UseVisualStyleBackColor = true;
            this.btnDrawLine.Click += new System.EventHandler(this.btnRuler_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnZoomOut.Location = new System.Drawing.Point(12, 145);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(62, 30);
            this.btnZoomOut.TabIndex = 16;
            this.btnZoomOut.Text = "Zoom";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnZoomWin
            // 
            this.btnZoomWin.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnZoomWin.Location = new System.Drawing.Point(12, 88);
            this.btnZoomWin.Name = "btnZoomWin";
            this.btnZoomWin.Size = new System.Drawing.Size(62, 30);
            this.btnZoomWin.TabIndex = 15;
            this.btnZoomWin.Text = "ZoomWin";
            this.btnZoomWin.UseVisualStyleBackColor = true;
            this.btnZoomWin.Click += new System.EventHandler(this.btnZoomWin_Click);
            // 
            // btnPan
            // 
            this.btnPan.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnPan.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnPan.Location = new System.Drawing.Point(12, 31);
            this.btnPan.Name = "btnPan";
            this.btnPan.Size = new System.Drawing.Size(62, 30);
            this.btnPan.TabIndex = 13;
            this.btnPan.Text = "Move";
            this.btnPan.UseVisualStyleBackColor = true;
            this.btnPan.Click += new System.EventHandler(this.btnPan_Click);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2,
            this.menuItem6,
            this.miOption,
            this.miAbout});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miOpenShp,
            this.miOpenTrj,
            this.miLoadMap,
            this.miExit});
            this.menuItem1.Text = "File(&F)";
            // 
            // miOpenShp
            // 
            this.miOpenShp.Index = 0;
            this.miOpenShp.Text = "Open Shape(&S)";
            this.miOpenShp.Click += new System.EventHandler(this.miOpenShp_Click);
            // 
            // miOpenTrj
            // 
            this.miOpenTrj.Index = 1;
            this.miOpenTrj.Text = "Open Trajectory(&T)";
            this.miOpenTrj.Click += new System.EventHandler(this.miOpenTrj_Click);
            // 
            // miLoadMap
            // 
            this.miLoadMap.Index = 2;
            this.miLoadMap.Text = "Load Map(&L)";
            this.miLoadMap.Click += new System.EventHandler(this.miLoadMap_Click);
            // 
            // miExit
            // 
            this.miExit.Index = 3;
            this.miExit.Text = "Exit(&E)";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem3,
            this.menuItem4,
            this.miShowShapeLayer,
            this.miClearView});
            this.menuItem2.Text = "View(&V)";
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 0;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miNoneView,
            this.miStreetView,
            this.miSatelliteView,
            this.miHybridView});
            this.menuItem3.Text = "Bg Layer(&B)";
            // 
            // miNoneView
            // 
            this.miNoneView.Checked = true;
            this.miNoneView.Index = 0;
            this.miNoneView.Text = "None";
            this.miNoneView.Click += new System.EventHandler(this.miNoneView_Click);
            // 
            // miStreetView
            // 
            this.miStreetView.Index = 1;
            this.miStreetView.RadioCheck = true;
            this.miStreetView.Text = "Street";
            this.miStreetView.Click += new System.EventHandler(this.miStreetView_Click);
            // 
            // miSatelliteView
            // 
            this.miSatelliteView.Index = 2;
            this.miSatelliteView.RadioCheck = true;
            this.miSatelliteView.Text = "Satellite";
            this.miSatelliteView.Click += new System.EventHandler(this.miSatelliteView_Click);
            // 
            // miHybridView
            // 
            this.miHybridView.Index = 3;
            this.miHybridView.RadioCheck = true;
            this.miHybridView.Text = "Hybrid";
            this.miHybridView.Click += new System.EventHandler(this.miHybridView_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 1;
            this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miShowMarker,
            this.miShowTrj,
            this.miShowStandardTrj,
            this.miShowEdgeId});
            this.menuItem4.Text = "Markers(&M)";
            // 
            // miShowMarker
            // 
            this.miShowMarker.Checked = true;
            this.miShowMarker.Index = 0;
            this.miShowMarker.Text = "Marker(&M)";
            this.miShowMarker.Click += new System.EventHandler(this.miShowMarker_Click);
            // 
            // miShowTrj
            // 
            this.miShowTrj.Checked = true;
            this.miShowTrj.Index = 1;
            this.miShowTrj.Text = "Trajectory(&T)";
            this.miShowTrj.Click += new System.EventHandler(this.miShowTrj_Click);
            // 
            // miShowStandardTrj
            // 
            this.miShowStandardTrj.Checked = true;
            this.miShowStandardTrj.Index = 2;
            this.miShowStandardTrj.Text = "Standard Trj(&S)";
            this.miShowStandardTrj.Click += new System.EventHandler(this.miShowStandardTrj_Click);
            // 
            // miShowEdgeId
            // 
            this.miShowEdgeId.Checked = true;
            this.miShowEdgeId.Index = 3;
            this.miShowEdgeId.Text = "EdgeId";
            this.miShowEdgeId.Click += new System.EventHandler(this.miShowEdgeId_Click);
            // 
            // miShowShapeLayer
            // 
            this.miShowShapeLayer.Checked = true;
            this.miShowShapeLayer.Index = 2;
            this.miShowShapeLayer.Text = "Shape Layer(&S)";
            this.miShowShapeLayer.Click += new System.EventHandler(this.miShowShapeLayer_Click);
            // 
            // miClearView
            // 
            this.miClearView.Index = 3;
            this.miClearView.Text = "Clear(&C)";
            this.miClearView.Click += new System.EventHandler(this.miClearView_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 2;
            this.menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem7,
            this.menuItem5,
            this.miMapmatching,
            this.miCompare,
            this.btnTest});
            this.menuItem6.Text = "Tools(&T)";
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 0;
            this.menuItem7.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miDrawPoint,
            this.miDrawLine,
            this.miDrawPolygon});
            this.menuItem7.Text = "Draw(&D)";
            // 
            // miDrawPoint
            // 
            this.miDrawPoint.Index = 0;
            this.miDrawPoint.Text = "Point(&P)";
            this.miDrawPoint.Click += new System.EventHandler(this.miDrawPoint_Click);
            // 
            // miDrawLine
            // 
            this.miDrawLine.Index = 1;
            this.miDrawLine.Text = "Line(&L)";
            this.miDrawLine.Click += new System.EventHandler(this.miDrawLine_Click);
            // 
            // miDrawPolygon
            // 
            this.miDrawPolygon.Index = 2;
            this.miDrawPolygon.Text = "Polygon(&O)";
            this.miDrawPolygon.Click += new System.EventHandler(this.miDrawPolygon_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 1;
            this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miFindEdge,
            this.miFindVertex});
            this.menuItem5.Text = "Find(&F)";
            // 
            // miFindEdge
            // 
            this.miFindEdge.Index = 0;
            this.miFindEdge.Text = "Edge(&E)";
            this.miFindEdge.Click += new System.EventHandler(this.miFindEdge_Click);
            // 
            // miFindVertex
            // 
            this.miFindVertex.Index = 1;
            this.miFindVertex.Text = "Vertex(&V)";
            this.miFindVertex.Click += new System.EventHandler(this.miFindVertex_Click);
            // 
            // miMapmatching
            // 
            this.miMapmatching.Index = 2;
            this.miMapmatching.Text = "Mapmatching(&M)";
            this.miMapmatching.Click += new System.EventHandler(this.miMapmatching_Click);
            // 
            // miCompare
            // 
            this.miCompare.Index = 3;
            this.miCompare.Text = "Compare(&C)";
            this.miCompare.Click += new System.EventHandler(this.miCompare_Click);
            // 
            // btnTest
            // 
            this.btnTest.Index = 4;
            this.btnTest.Text = "Test(&T)";
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // miOption
            // 
            this.miOption.Index = 3;
            this.miOption.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miAutoScale});
            this.miOption.Text = "Options(&O)";
            // 
            // miAutoScale
            // 
            this.miAutoScale.Checked = true;
            this.miAutoScale.Index = 0;
            this.miAutoScale.Text = "Auto Scale(&A)";
            this.miAutoScale.Click += new System.EventHandler(this.miAutoScale_Click);
            // 
            // miAbout
            // 
            this.miAbout.Index = 4;
            this.miAbout.Text = "About(&A)";
            this.miAbout.Click += new System.EventHandler(this.miAbout_Click);
            // 
            // lbPosition
            // 
            this.lbPosition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbPosition.AutoSize = true;
            this.lbPosition.Location = new System.Drawing.Point(6, 32);
            this.lbPosition.Name = "lbPosition";
            this.lbPosition.Size = new System.Drawing.Size(53, 12);
            this.lbPosition.TabIndex = 16;
            this.lbPosition.Text = "Position";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lbPosition);
            this.groupBox1.Location = new System.Drawing.Point(592, 377);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(84, 128);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
            // 
            // MapForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(679, 526);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.mbMap);
            this.DoubleBuffered = true;
            this.Menu = this.mainMenu1;
            this.MinimumSize = new System.Drawing.Size(657, 548);
            this.Name = "MapForm";
            this.Text = "Viewer";
            this.Load += new System.EventHandler(this.MapForm_Load);
            this.SizeChanged += new System.EventHandler(this.MapForm_SizeChanged);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SharpMap.Forms.MapBox mbMap;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnDrawLine;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnZoomWin;
        private System.Windows.Forms.Button btnPan;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem miOpenShp;
        private System.Windows.Forms.MenuItem miOpenTrj;
        private System.Windows.Forms.MenuItem miExit;
        private System.Windows.Forms.MenuItem miAbout;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem miStreetView;
        private System.Windows.Forms.MenuItem miSatelliteView;
        private System.Windows.Forms.MenuItem miHybridView;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem miShowMarker;
        private System.Windows.Forms.MenuItem miShowTrj;
        private System.Windows.Forms.MenuItem miShowStandardTrj;
        private System.Windows.Forms.MenuItem miShowEdgeId;
        private System.Windows.Forms.MenuItem miClearView;
        private System.Windows.Forms.MenuItem miOption;
        private System.Windows.Forms.Label lbPosition;
        private System.Windows.Forms.MenuItem miAutoScale;
        private System.Windows.Forms.MenuItem miShowShapeLayer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.MenuItem miNoneView;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem miMapmatching;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem miDrawPoint;
        private System.Windows.Forms.MenuItem miDrawLine;
        private System.Windows.Forms.MenuItem miDrawPolygon;
        private System.Windows.Forms.MenuItem miLoadMap;
        private System.Windows.Forms.MenuItem miCompare;
        private System.Windows.Forms.MenuItem btnTest;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem miFindEdge;
        private System.Windows.Forms.MenuItem miFindVertex;
    }
}