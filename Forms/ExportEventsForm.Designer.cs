namespace Neurotec.Samples.Forms
{
	partial class ExportEventsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportEventsForm));
            this.exportBtn = new System.Windows.Forms.Button();
            this.fromDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.orderCmbBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.typeCmbBox = new System.Windows.Forms.ComboBox();
            this.exportNextBtn = new System.Windows.Forms.Button();
            this.classLbl = new System.Windows.Forms.Label();
            this.classCmbBox = new System.Windows.Forms.ComboBox();
            this.filePathLbl = new System.Windows.Forms.LinkLabel();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // exportBtn
            // 
            this.exportBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exportBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportBtn.Location = new System.Drawing.Point(212, 132);
            this.exportBtn.Name = "exportBtn";
            this.exportBtn.Size = new System.Drawing.Size(102, 23);
            this.exportBtn.TabIndex = 0;
            this.exportBtn.Text = "Export";
            this.exportBtn.UseVisualStyleBackColor = true;
            this.exportBtn.Click += new System.EventHandler(this.OnExportClick);
            // 
            // fromDateTimePicker
            // 
            this.fromDateTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fromDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.fromDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.fromDateTimePicker.Location = new System.Drawing.Point(212, 3);
            this.fromDateTimePicker.Name = "fromDateTimePicker";
            this.fromDateTimePicker.Size = new System.Drawing.Size(210, 20);
            this.fromDateTimePicker.TabIndex = 5;
            this.fromDateTimePicker.ValueChanged += new System.EventHandler(this.OnFilterChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "From:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(12, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Order:";
            // 
            // orderCmbBox
            // 
            this.orderCmbBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.orderCmbBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.orderCmbBox.FormattingEnabled = true;
            this.orderCmbBox.Location = new System.Drawing.Point(212, 31);
            this.orderCmbBox.Name = "orderCmbBox";
            this.orderCmbBox.Size = new System.Drawing.Size(210, 21);
            this.orderCmbBox.TabIndex = 9;
            this.orderCmbBox.SelectedValueChanged += new System.EventHandler(this.OnFilterChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(12, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Type:";
            // 
            // typeCmbBox
            // 
            this.typeCmbBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.typeCmbBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeCmbBox.FormattingEnabled = true;
            this.typeCmbBox.Location = new System.Drawing.Point(212, 58);
            this.typeCmbBox.Name = "typeCmbBox";
            this.typeCmbBox.Size = new System.Drawing.Size(210, 21);
            this.typeCmbBox.TabIndex = 11;
            this.typeCmbBox.SelectedValueChanged += new System.EventHandler(this.OnTypeChanged);
            // 
            // exportNextBtn
            // 
            this.exportNextBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exportNextBtn.Enabled = false;
            this.exportNextBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportNextBtn.Location = new System.Drawing.Point(320, 132);
            this.exportNextBtn.Name = "exportNextBtn";
            this.exportNextBtn.Size = new System.Drawing.Size(102, 23);
            this.exportNextBtn.TabIndex = 12;
            this.exportNextBtn.Text = "Export Next";
            this.exportNextBtn.UseVisualStyleBackColor = true;
            this.exportNextBtn.Click += new System.EventHandler(this.OnExportNextClick);
            // 
            // classLbl
            // 
            this.classLbl.AutoSize = true;
            this.classLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.classLbl.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.classLbl.Location = new System.Drawing.Point(12, 88);
            this.classLbl.Name = "classLbl";
            this.classLbl.Size = new System.Drawing.Size(41, 13);
            this.classLbl.TabIndex = 14;
            this.classLbl.Text = "Class:";
            this.classLbl.Visible = false;
            // 
            // classCmbBox
            // 
            this.classCmbBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.classCmbBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.classCmbBox.FormattingEnabled = true;
            this.classCmbBox.Location = new System.Drawing.Point(212, 85);
            this.classCmbBox.Name = "classCmbBox";
            this.classCmbBox.Size = new System.Drawing.Size(210, 21);
            this.classCmbBox.TabIndex = 15;
            this.classCmbBox.Visible = false;
            this.classCmbBox.SelectedValueChanged += new System.EventHandler(this.OnFilterChanged);
            // 
            // filePathLbl
            // 
            this.filePathLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.filePathLbl.AutoSize = true;
            this.filePathLbl.Location = new System.Drawing.Point(12, 105);
            this.filePathLbl.Name = "filePathLbl";
            this.filePathLbl.Size = new System.Drawing.Size(0, 13);
            this.filePathLbl.TabIndex = 13;
            this.filePathLbl.TabStop = true;
            this.filePathLbl.Click += new System.EventHandler(this.OnFilePathClick);
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportCsv.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportCsv.Location = new System.Drawing.Point(104, 132);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(102, 23);
            this.btnExportCsv.TabIndex = 16;
            this.btnExportCsv.Text = "Export to CSV";
            this.btnExportCsv.UseVisualStyleBackColor = true;
            this.btnExportCsv.Click += new System.EventHandler(this.ButtonExportCsvClick);
            // 
            // ExportEventsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(434, 161);
            this.Controls.Add(this.btnExportCsv);
            this.Controls.Add(this.classCmbBox);
            this.Controls.Add(this.classLbl);
            this.Controls.Add(this.filePathLbl);
            this.Controls.Add(this.exportNextBtn);
            this.Controls.Add(this.typeCmbBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.orderCmbBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fromDateTimePicker);
            this.Controls.Add(this.exportBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "ExportEventsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button exportBtn;
		private System.Windows.Forms.DateTimePicker fromDateTimePicker;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox orderCmbBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox typeCmbBox;
		private System.Windows.Forms.Button exportNextBtn;
		private System.Windows.Forms.Label classLbl;
		private System.Windows.Forms.ComboBox classCmbBox;
		private System.Windows.Forms.LinkLabel filePathLbl;
		private System.Windows.Forms.Button btnExportCsv;
	}
}
