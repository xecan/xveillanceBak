namespace Neurotec.Samples.Forms
{
	partial class SelectRegionOfInterestForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectRegionOfInterestForm));
			this.lblHint = new System.Windows.Forms.Label();
			this.btnReset = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.rbToolRegions = new System.Windows.Forms.RadioButton();
			this.rbGridTool = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.nudRows = new System.Windows.Forms.NumericUpDown();
			this.nudColumns = new System.Windows.Forms.NumericUpDown();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.tsbIncludeRect = new System.Windows.Forms.ToolStripButton();
			this.tsbExcludeRect = new System.Windows.Forms.ToolStripButton();
			this.tsbIncludePolygon = new System.Windows.Forms.ToolStripButton();
			this.tsbExcludePolygon = new System.Windows.Forms.ToolStripButton();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.btnUndo = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.chbAreaByCenter = new System.Windows.Forms.CheckBox();
			this.panelImage = new Neurotec.Samples.Forms.DoubleBufferedPanel();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudRows)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudColumns)).BeginInit();
			this.toolStrip1.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblHint
			// 
			this.lblHint.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblHint.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.lblHint, 3);
			this.lblHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
			this.lblHint.Location = new System.Drawing.Point(447, 17);
			this.lblHint.Margin = new System.Windows.Forms.Padding(5);
			this.lblHint.Name = "lblHint";
			this.lblHint.Size = new System.Drawing.Size(44, 16);
			this.lblHint.TabIndex = 0;
			this.lblHint.Text = "HINT";
			// 
			// btnReset
			// 
			this.btnReset.Location = new System.Drawing.Point(75, 3);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(53, 23);
			this.btnReset.TabIndex = 1;
			this.btnReset.Text = "&Reset";
			this.btnReset.UseVisualStyleBackColor = true;
			this.btnReset.Click += new System.EventHandler(this.BtnResetClick);
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(624, 561);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "&OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.BtnOkClick);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(705, 561);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 4;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 156F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.lblHint, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.btnCancel, 3, 2);
			this.tableLayoutPanel1.Controls.Add(this.btnOk, 2, 2);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.panelImage, 2, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(783, 587);
			this.tableLayoutPanel1.TabIndex = 5;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 3;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.rbToolRegions, 0, 3);
			this.tableLayoutPanel2.Controls.Add(this.rbGridTool, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.label1, 1, 1);
			this.tableLayoutPanel2.Controls.Add(this.label2, 1, 2);
			this.tableLayoutPanel2.Controls.Add(this.nudRows, 2, 1);
			this.tableLayoutPanel2.Controls.Add(this.nudColumns, 2, 2);
			this.tableLayoutPanel2.Controls.Add(this.toolStrip1, 1, 4);
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 5);
			this.tableLayoutPanel2.Controls.Add(this.label3, 0, 6);
			this.tableLayoutPanel2.Controls.Add(this.chbAreaByCenter, 1, 7);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 53);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 9;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(150, 502);
			this.tableLayoutPanel2.TabIndex = 6;
			// 
			// rbToolRegions
			// 
			this.rbToolRegions.AutoSize = true;
			this.tableLayoutPanel2.SetColumnSpan(this.rbToolRegions, 3);
			this.rbToolRegions.Location = new System.Drawing.Point(3, 78);
			this.rbToolRegions.Name = "rbToolRegions";
			this.rbToolRegions.Size = new System.Drawing.Size(64, 17);
			this.rbToolRegions.TabIndex = 1;
			this.rbToolRegions.Text = "Regions";
			this.rbToolRegions.UseVisualStyleBackColor = true;
			this.rbToolRegions.CheckedChanged += new System.EventHandler(this.RadioBoxSelectionToolCheckedChanged);
			// 
			// rbGridTool
			// 
			this.rbGridTool.AutoSize = true;
			this.rbGridTool.Checked = true;
			this.tableLayoutPanel2.SetColumnSpan(this.rbGridTool, 3);
			this.rbGridTool.Location = new System.Drawing.Point(3, 3);
			this.rbGridTool.Name = "rbGridTool";
			this.rbGridTool.Size = new System.Drawing.Size(44, 17);
			this.rbGridTool.TabIndex = 0;
			this.rbGridTool.TabStop = true;
			this.rbGridTool.Text = "Grid";
			this.rbGridTool.UseVisualStyleBackColor = true;
			this.rbGridTool.CheckedChanged += new System.EventHandler(this.RadioBoxSelectionToolCheckedChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(13, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(50, 26);
			this.label1.TabIndex = 1;
			this.label1.Text = "Rows:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label2.Location = new System.Drawing.Point(13, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(50, 26);
			this.label2.TabIndex = 2;
			this.label2.Text = "Columns:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// nudRows
			// 
			this.nudRows.Location = new System.Drawing.Point(69, 26);
			this.nudRows.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
			this.nudRows.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.nudRows.Name = "nudRows";
			this.nudRows.Size = new System.Drawing.Size(45, 20);
			this.nudRows.TabIndex = 3;
			this.nudRows.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
			this.nudRows.ValueChanged += new System.EventHandler(this.NudGridValueChanged);
			// 
			// nudColumns
			// 
			this.nudColumns.Location = new System.Drawing.Point(69, 52);
			this.nudColumns.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
			this.nudColumns.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.nudColumns.Name = "nudColumns";
			this.nudColumns.Size = new System.Drawing.Size(45, 20);
			this.nudColumns.TabIndex = 4;
			this.nudColumns.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this.nudColumns.ValueChanged += new System.EventHandler(this.NudGridValueChanged);
			// 
			// toolStrip1
			// 
			this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
			this.tableLayoutPanel2.SetColumnSpan(this.toolStrip1, 2);
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbIncludeRect,
            this.tsbExcludeRect,
            this.tsbIncludePolygon,
            this.tsbExcludePolygon});
			this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
			this.toolStrip1.Location = new System.Drawing.Point(10, 98);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(140, 92);
			this.toolStrip1.TabIndex = 5;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// tsbIncludeRect
			// 
			this.tsbIncludeRect.Checked = true;
			this.tsbIncludeRect.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tsbIncludeRect.Image = global::Neurotec.Samples.Properties.Resources.Rectangle;
			this.tsbIncludeRect.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbIncludeRect.Name = "tsbIncludeRect";
			this.tsbIncludeRect.Size = new System.Drawing.Size(121, 20);
			this.tsbIncludeRect.Text = "Include Rectangle";
			this.tsbIncludeRect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tsbIncludeRect.ToolTipText = "Draw recangle where search should be performed.\r\nPress \"1\" to select.";
			this.tsbIncludeRect.Click += new System.EventHandler(this.ToolStripButtonClick);
			// 
			// tsbExcludeRect
			// 
			this.tsbExcludeRect.Image = global::Neurotec.Samples.Properties.Resources.RectangleRed;
			this.tsbExcludeRect.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbExcludeRect.Name = "tsbExcludeRect";
			this.tsbExcludeRect.Size = new System.Drawing.Size(123, 20);
			this.tsbExcludeRect.Text = "Exclude Rectangle";
			this.tsbExcludeRect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tsbExcludeRect.ToolTipText = "Draw recangle where search should NOT be performed.\r\nPress \"2\" to select.\r\n";
			this.tsbExcludeRect.Click += new System.EventHandler(this.ToolStripButtonClick);
			// 
			// tsbIncludePolygon
			// 
			this.tsbIncludePolygon.Image = global::Neurotec.Samples.Properties.Resources.Polygon;
			this.tsbIncludePolygon.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbIncludePolygon.Name = "tsbIncludePolygon";
			this.tsbIncludePolygon.Size = new System.Drawing.Size(113, 20);
			this.tsbIncludePolygon.Text = "Include Polygon";
			this.tsbIncludePolygon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tsbIncludePolygon.ToolTipText = "Draw polygon where search should be performed.\r\nPress \"3\" to select.\r\n";
			this.tsbIncludePolygon.Click += new System.EventHandler(this.ToolStripButtonClick);
			// 
			// tsbExcludePolygon
			// 
			this.tsbExcludePolygon.Image = global::Neurotec.Samples.Properties.Resources.PolygonRed;
			this.tsbExcludePolygon.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbExcludePolygon.Name = "tsbExcludePolygon";
			this.tsbExcludePolygon.Size = new System.Drawing.Size(115, 20);
			this.tsbExcludePolygon.Text = "Exclude Polygon";
			this.tsbExcludePolygon.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tsbExcludePolygon.ToolTipText = "Draw polygon where search should NOT be performed.\r\nPress \"4\" to select.";
			this.tsbExcludePolygon.Click += new System.EventHandler(this.ToolStripButtonClick);
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel2.SetColumnSpan(this.tableLayoutPanel3, 3);
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.Controls.Add(this.btnReset, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this.btnUndo, 0, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 193);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 1;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(144, 29);
			this.tableLayoutPanel3.TabIndex = 7;
			// 
			// btnUndo
			// 
			this.btnUndo.Enabled = false;
			this.btnUndo.Location = new System.Drawing.Point(3, 3);
			this.btnUndo.Name = "btnUndo";
			this.btnUndo.Size = new System.Drawing.Size(57, 23);
			this.btnUndo.TabIndex = 6;
			this.btnUndo.Text = "&Undo";
			this.btnUndo.UseVisualStyleBackColor = true;
			this.btnUndo.Click += new System.EventHandler(this.BtnUndoClick);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.tableLayoutPanel2.SetColumnSpan(this.label3, 3);
			this.label3.Location = new System.Drawing.Point(3, 225);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(46, 13);
			this.label3.TabIndex = 8;
			this.label3.Text = "Options:";
			// 
			// chbAreaByCenter
			// 
			this.chbAreaByCenter.AutoSize = true;
			this.chbAreaByCenter.Checked = true;
			this.chbAreaByCenter.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tableLayoutPanel2.SetColumnSpan(this.chbAreaByCenter, 2);
			this.chbAreaByCenter.Location = new System.Drawing.Point(13, 241);
			this.chbAreaByCenter.Name = "chbAreaByCenter";
			this.chbAreaByCenter.Size = new System.Drawing.Size(133, 30);
			this.chbAreaByCenter.TabIndex = 9;
			this.chbAreaByCenter.Text = "Check search area by \r\nobject center";
			this.toolTip1.SetToolTip(this.chbAreaByCenter, "Checks if object is in area by its center.\r\nOtherwise entire object must be insid" +
        "e area.");
			this.chbAreaByCenter.UseVisualStyleBackColor = true;
			// 
			// panelImage
			// 
			this.panelImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel1.SetColumnSpan(this.panelImage, 2);
			this.panelImage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelImage.Location = new System.Drawing.Point(159, 53);
			this.panelImage.Name = "panelImage";
			this.panelImage.Size = new System.Drawing.Size(621, 502);
			this.panelImage.TabIndex = 7;
			this.panelImage.SizeChanged += new System.EventHandler(this.PanelImageSizeChanged);
			this.panelImage.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelImagePaint);
			this.panelImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelImageMouseDown);
			this.panelImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PanelImageMouseMove);
			this.panelImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PanelImageMouseUp);
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 5000;
			this.toolTip1.InitialDelay = 500;
			this.toolTip1.ReshowDelay = 100;
			// 
			// SelectRegionOfInterestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(783, 587);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimizeBox = false;
			this.Name = "SelectRegionOfInterestForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Search Areas";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectRegionOfInterestFormClosing);
			this.Shown += new System.EventHandler(this.SelectRegionOfInterestFormShown);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudRows)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudColumns)).EndInit();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblHint;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button btnReset;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.RadioButton rbToolRegions;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.RadioButton rbGridTool;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown nudRows;
		private System.Windows.Forms.NumericUpDown nudColumns;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton tsbIncludeRect;
		private System.Windows.Forms.ToolStripButton tsbExcludeRect;
		private System.Windows.Forms.ToolStripButton tsbIncludePolygon;
		private System.Windows.Forms.ToolStripButton tsbExcludePolygon;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.Button btnUndo;
		private Neurotec.Samples.Forms.DoubleBufferedPanel panelImage;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox chbAreaByCenter;
	}
}
