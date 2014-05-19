namespace GISAppDemo
{
    partial class StatisticsForm
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
            this.lbSrc = new System.Windows.Forms.ListBox();
            this.lbQuery = new System.Windows.Forms.ListBox();
            this.gbParams = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbThreshold = new System.Windows.Forms.TextBox();
            this.gbResult = new System.Windows.Forms.GroupBox();
            this.lbEnd = new System.Windows.Forms.Label();
            this.lbStart = new System.Windows.Forms.Label();
            this.lbCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gbOps = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.cbEndFail = new System.Windows.Forms.CheckBox();
            this.cbStartFail = new System.Windows.Forms.CheckBox();
            this.cbEndSuccess = new System.Windows.Forms.CheckBox();
            this.cbStartSuccess = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnLoadQuery = new System.Windows.Forms.Button();
            this.gbParams.SuspendLayout();
            this.gbResult.SuspendLayout();
            this.gbOps.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbSrc
            // 
            this.lbSrc.FormattingEnabled = true;
            this.lbSrc.ItemHeight = 12;
            this.lbSrc.Items.AddRange(new object[] {
            "20121201",
            "20121202",
            "20121203",
            "20121204",
            "20121205",
            "20121206",
            "20121207"});
            this.lbSrc.Location = new System.Drawing.Point(21, 20);
            this.lbSrc.Name = "lbSrc";
            this.lbSrc.Size = new System.Drawing.Size(107, 124);
            this.lbSrc.TabIndex = 0;
            // 
            // lbQuery
            // 
            this.lbQuery.FormattingEnabled = true;
            this.lbQuery.ItemHeight = 12;
            this.lbQuery.Items.AddRange(new object[] {
            "20121208",
            "20121209",
            "20121210",
            "20121211",
            "20121212",
            "20121213",
            "20121214",
            "20131215",
            "20121216",
            "20131217",
            "20121218",
            "20121219",
            "20121220",
            "20121221",
            "20121222"});
            this.lbQuery.Location = new System.Drawing.Point(160, 20);
            this.lbQuery.Name = "lbQuery";
            this.lbQuery.Size = new System.Drawing.Size(104, 124);
            this.lbQuery.TabIndex = 0;
            // 
            // gbParams
            // 
            this.gbParams.Controls.Add(this.label1);
            this.gbParams.Controls.Add(this.tbThreshold);
            this.gbParams.Controls.Add(this.lbSrc);
            this.gbParams.Controls.Add(this.lbQuery);
            this.gbParams.Location = new System.Drawing.Point(23, 35);
            this.gbParams.Name = "gbParams";
            this.gbParams.Size = new System.Drawing.Size(479, 165);
            this.gbParams.TabIndex = 1;
            this.gbParams.TabStop = false;
            this.gbParams.Text = "参数";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(290, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "阈值：";
            // 
            // tbThreshold
            // 
            this.tbThreshold.Location = new System.Drawing.Point(333, 20);
            this.tbThreshold.Name = "tbThreshold";
            this.tbThreshold.Size = new System.Drawing.Size(76, 21);
            this.tbThreshold.TabIndex = 1;
            // 
            // gbResult
            // 
            this.gbResult.Controls.Add(this.lbEnd);
            this.gbResult.Controls.Add(this.lbStart);
            this.gbResult.Controls.Add(this.lbCount);
            this.gbResult.Controls.Add(this.label4);
            this.gbResult.Controls.Add(this.label3);
            this.gbResult.Controls.Add(this.label2);
            this.gbResult.Location = new System.Drawing.Point(23, 224);
            this.gbResult.Name = "gbResult";
            this.gbResult.Size = new System.Drawing.Size(215, 156);
            this.gbResult.TabIndex = 2;
            this.gbResult.TabStop = false;
            this.gbResult.Text = "统计结果";
            // 
            // lbEnd
            // 
            this.lbEnd.AutoSize = true;
            this.lbEnd.Location = new System.Drawing.Point(67, 64);
            this.lbEnd.Name = "lbEnd";
            this.lbEnd.Size = new System.Drawing.Size(41, 12);
            this.lbEnd.TabIndex = 0;
            this.lbEnd.Text = "label2";
            // 
            // lbStart
            // 
            this.lbStart.AutoSize = true;
            this.lbStart.Location = new System.Drawing.Point(68, 39);
            this.lbStart.Name = "lbStart";
            this.lbStart.Size = new System.Drawing.Size(41, 12);
            this.lbStart.TabIndex = 0;
            this.lbStart.Text = "label2";
            // 
            // lbCount
            // 
            this.lbCount.AutoSize = true;
            this.lbCount.Location = new System.Drawing.Point(68, 89);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(11, 12);
            this.lbCount.TabIndex = 0;
            this.lbCount.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "总数：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "终点：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "起点：";
            // 
            // gbOps
            // 
            this.gbOps.Controls.Add(this.btnLoadQuery);
            this.gbOps.Controls.Add(this.btnClear);
            this.gbOps.Controls.Add(this.cbEndFail);
            this.gbOps.Controls.Add(this.cbStartFail);
            this.gbOps.Controls.Add(this.cbEndSuccess);
            this.gbOps.Controls.Add(this.cbStartSuccess);
            this.gbOps.Controls.Add(this.btnOK);
            this.gbOps.Location = new System.Drawing.Point(264, 224);
            this.gbOps.Name = "gbOps";
            this.gbOps.Size = new System.Drawing.Size(238, 156);
            this.gbOps.TabIndex = 3;
            this.gbOps.TabStop = false;
            this.gbOps.Text = "操作";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(116, 119);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // cbEndFail
            // 
            this.cbEndFail.AutoSize = true;
            this.cbEndFail.Checked = true;
            this.cbEndFail.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbEndFail.Location = new System.Drawing.Point(116, 62);
            this.cbEndFail.Name = "cbEndFail";
            this.cbEndFail.Size = new System.Drawing.Size(72, 16);
            this.cbEndFail.TabIndex = 1;
            this.cbEndFail.Text = "终点失败";
            this.cbEndFail.UseVisualStyleBackColor = true;
            this.cbEndFail.CheckedChanged += new System.EventHandler(this.cbEndFail_CheckedChanged);
            // 
            // cbStartFail
            // 
            this.cbStartFail.AutoSize = true;
            this.cbStartFail.Checked = true;
            this.cbStartFail.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbStartFail.Location = new System.Drawing.Point(116, 34);
            this.cbStartFail.Name = "cbStartFail";
            this.cbStartFail.Size = new System.Drawing.Size(72, 16);
            this.cbStartFail.TabIndex = 1;
            this.cbStartFail.Text = "起点失败";
            this.cbStartFail.UseVisualStyleBackColor = true;
            this.cbStartFail.CheckedChanged += new System.EventHandler(this.cbStartFail_CheckedChanged);
            // 
            // cbEndSuccess
            // 
            this.cbEndSuccess.AutoSize = true;
            this.cbEndSuccess.Location = new System.Drawing.Point(20, 63);
            this.cbEndSuccess.Name = "cbEndSuccess";
            this.cbEndSuccess.Size = new System.Drawing.Size(72, 16);
            this.cbEndSuccess.TabIndex = 1;
            this.cbEndSuccess.Text = "终点成功";
            this.cbEndSuccess.UseVisualStyleBackColor = true;
            this.cbEndSuccess.CheckedChanged += new System.EventHandler(this.cbEndSuccess_CheckedChanged);
            // 
            // cbStartSuccess
            // 
            this.cbStartSuccess.AutoSize = true;
            this.cbStartSuccess.Location = new System.Drawing.Point(20, 34);
            this.cbStartSuccess.Name = "cbStartSuccess";
            this.cbStartSuccess.Size = new System.Drawing.Size(72, 16);
            this.cbStartSuccess.TabIndex = 1;
            this.cbStartSuccess.Text = "起点成功";
            this.cbStartSuccess.UseVisualStyleBackColor = true;
            this.cbStartSuccess.CheckedChanged += new System.EventHandler(this.cbStartSuccess_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(20, 120);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnLoadQuery
            // 
            this.btnLoadQuery.Location = new System.Drawing.Point(20, 89);
            this.btnLoadQuery.Name = "btnLoadQuery";
            this.btnLoadQuery.Size = new System.Drawing.Size(75, 23);
            this.btnLoadQuery.TabIndex = 3;
            this.btnLoadQuery.Text = "LoadQuery";
            this.btnLoadQuery.UseVisualStyleBackColor = true;
            this.btnLoadQuery.Click += new System.EventHandler(this.btnLoadQuery_Click);
            // 
            // StatisticsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 407);
            this.Controls.Add(this.gbOps);
            this.Controls.Add(this.gbResult);
            this.Controls.Add(this.gbParams);
            this.Name = "StatisticsForm";
            this.Text = "StatisticsForm";
            this.Load += new System.EventHandler(this.StatisticsForm_Load);
            this.gbParams.ResumeLayout(false);
            this.gbParams.PerformLayout();
            this.gbResult.ResumeLayout(false);
            this.gbResult.PerformLayout();
            this.gbOps.ResumeLayout(false);
            this.gbOps.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbSrc;
        private System.Windows.Forms.ListBox lbQuery;
        private System.Windows.Forms.GroupBox gbParams;
        private System.Windows.Forms.GroupBox gbResult;
        private System.Windows.Forms.GroupBox gbOps;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox tbThreshold;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbEnd;
        private System.Windows.Forms.Label lbStart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbEndFail;
        private System.Windows.Forms.CheckBox cbStartFail;
        private System.Windows.Forms.CheckBox cbEndSuccess;
        private System.Windows.Forms.CheckBox cbStartSuccess;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnLoadQuery;
    }
}