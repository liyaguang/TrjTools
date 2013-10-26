namespace GISAppDemo
{
    partial class CmpForm
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
            this.dgvCmp = new System.Windows.Forms.DataGridView();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.miOpenTrj = new System.Windows.Forms.MenuItem();
            this.miChooseDataset = new System.Windows.Forms.MenuItem();
            this.miExit = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.miShowPath = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCmp)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvCmp
            // 
            this.dgvCmp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvCmp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCmp.Location = new System.Drawing.Point(12, 12);
            this.dgvCmp.Name = "dgvCmp";
            this.dgvCmp.Size = new System.Drawing.Size(570, 429);
            this.dgvCmp.TabIndex = 2;
            this.dgvCmp.AutoSizeColumnsModeChanged += new System.Windows.Forms.DataGridViewAutoSizeColumnsModeEventHandler(this.dgvCmp_AutoSizeColumnsModeChanged);
            this.dgvCmp.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCmp_CellClick);
            this.dgvCmp.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCmp_CellDoubleClick);
            this.dgvCmp.CurrentCellChanged += new System.EventHandler(this.dgvCmp_CurrentCellChanged);
            this.dgvCmp.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvCmp_RowPostPaint);
            this.dgvCmp.SelectionChanged += new System.EventHandler(this.dgvCmp_SelectionChanged);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem3,
            this.menuItem4,
            this.menuItem2});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miOpenTrj,
            this.miChooseDataset,
            this.miExit});
            this.menuItem1.Text = "File(&F)";
            // 
            // miOpenTrj
            // 
            this.miOpenTrj.Index = 0;
            this.miOpenTrj.Text = "Open Trajectory(&O)";
            this.miOpenTrj.Click += new System.EventHandler(this.miOpenTrj_Click);
            // 
            // miChooseDataset
            // 
            this.miChooseDataset.Index = 1;
            this.miChooseDataset.Text = "Choose DataSet(&D)";
            this.miChooseDataset.Click += new System.EventHandler(this.miChooseDataset_Click);
            // 
            // miExit
            // 
            this.miExit.Index = 2;
            this.miExit.Text = "Exit(&X)";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miShowPath});
            this.menuItem3.Text = "View(&V)";
            // 
            // miShowPath
            // 
            this.miShowPath.Index = 0;
            this.miShowPath.Text = "Show Path(&P)";
            this.miShowPath.Click += new System.EventHandler(this.miShowPath_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 2;
            this.menuItem4.Text = "Tools(&T)";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 3;
            this.menuItem2.Text = "About(&A)";
            // 
            // CmpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 454);
            this.Controls.Add(this.dgvCmp);
            this.Menu = this.mainMenu1;
            this.Name = "CmpForm";
            this.Text = "结果分析";
            this.Load += new System.EventHandler(this.CmpForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCmp)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvCmp;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem miOpenTrj;
        private System.Windows.Forms.MenuItem miChooseDataset;
        private System.Windows.Forms.MenuItem miExit;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem miShowPath;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItem2;
    }
}