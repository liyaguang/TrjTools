namespace GISAppDemo
{
    partial class MMForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbOutputDir = new System.Windows.Forms.TextBox();
            this.btnAddTrj = new System.Windows.Forms.Button();
            this.btnChooseOutputDir = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnClearTrj = new System.Windows.Forms.Button();
            this.lbTrjs = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Input File(s):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Output Directory:";
            // 
            // tbOutputDir
            // 
            this.tbOutputDir.Location = new System.Drawing.Point(137, 148);
            this.tbOutputDir.Name = "tbOutputDir";
            this.tbOutputDir.Size = new System.Drawing.Size(375, 21);
            this.tbOutputDir.TabIndex = 4;
            // 
            // btnAddTrj
            // 
            this.btnAddTrj.Location = new System.Drawing.Point(532, 28);
            this.btnAddTrj.Name = "btnAddTrj";
            this.btnAddTrj.Size = new System.Drawing.Size(62, 23);
            this.btnAddTrj.TabIndex = 2;
            this.btnAddTrj.Text = "Add";
            this.btnAddTrj.UseVisualStyleBackColor = true;
            this.btnAddTrj.Click += new System.EventHandler(this.btnAddTrj_Click);
            // 
            // btnChooseOutputDir
            // 
            this.btnChooseOutputDir.Location = new System.Drawing.Point(532, 146);
            this.btnChooseOutputDir.Name = "btnChooseOutputDir";
            this.btnChooseOutputDir.Size = new System.Drawing.Size(62, 23);
            this.btnChooseOutputDir.TabIndex = 5;
            this.btnChooseOutputDir.Text = "Choose";
            this.btnChooseOutputDir.UseVisualStyleBackColor = true;
            this.btnChooseOutputDir.Click += new System.EventHandler(this.btnChooseOutputDir_Click);
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStart.Location = new System.Drawing.Point(137, 198);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(108, 41);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnClearTrj
            // 
            this.btnClearTrj.Location = new System.Drawing.Point(532, 73);
            this.btnClearTrj.Name = "btnClearTrj";
            this.btnClearTrj.Size = new System.Drawing.Size(62, 23);
            this.btnClearTrj.TabIndex = 3;
            this.btnClearTrj.Text = "Clear";
            this.btnClearTrj.UseVisualStyleBackColor = true;
            this.btnClearTrj.Click += new System.EventHandler(this.btnClearTrj_Click);
            // 
            // lbTrjs
            // 
            this.lbTrjs.FormattingEnabled = true;
            this.lbTrjs.ItemHeight = 12;
            this.lbTrjs.Location = new System.Drawing.Point(137, 28);
            this.lbTrjs.Name = "lbTrjs";
            this.lbTrjs.Size = new System.Drawing.Size(375, 100);
            this.lbTrjs.TabIndex = 1;
            // 
            // MMForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 257);
            this.Controls.Add(this.lbTrjs);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnChooseOutputDir);
            this.Controls.Add(this.btnClearTrj);
            this.Controls.Add(this.btnAddTrj);
            this.Controls.Add(this.tbOutputDir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MMForm";
            this.Text = "Mapmatching";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbOutputDir;
        private System.Windows.Forms.Button btnAddTrj;
        private System.Windows.Forms.Button btnChooseOutputDir;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnClearTrj;
        private System.Windows.Forms.ListBox lbTrjs;
    }
}