namespace Neurotec.Samples.Forms
{
	partial class ConnectToDeviceForm
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
            this.pluginLabel = new System.Windows.Forms.Label();
            this.pluginComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // pluginLabel
            // 
            this.pluginLabel.AutoSize = true;
            this.pluginLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pluginLabel.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.pluginLabel.Location = new System.Drawing.Point(12, 15);
            this.pluginLabel.Name = "pluginLabel";
            this.pluginLabel.Size = new System.Drawing.Size(46, 13);
            this.pluginLabel.TabIndex = 1;
            this.pluginLabel.Text = "&Plugin:";
            // 
            // pluginComboBox
            // 
            this.pluginComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pluginComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pluginComboBox.FormattingEnabled = true;
            this.pluginComboBox.Location = new System.Drawing.Point(57, 12);
            this.pluginComboBox.Name = "pluginComboBox";
            this.pluginComboBox.Size = new System.Drawing.Size(311, 21);
            this.pluginComboBox.TabIndex = 2;
            this.pluginComboBox.SelectedIndexChanged += new System.EventHandler(this.PluginComboBoxSelectedIndexChanged);
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.Controls.Add(this.propertyGrid);
            this.groupBox.Location = new System.Drawing.Point(12, 51);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(359, 211);
            this.groupBox.TabIndex = 0;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "P&arameters";
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.Location = new System.Drawing.Point(3, 16);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(353, 192);
            this.propertyGrid.TabIndex = 0;

            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(215, 268);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(296, 268);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ConnectToDeviceForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(383, 303);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.pluginComboBox);
            this.Controls.Add(this.pluginLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "ConnectToDeviceForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connect to Device";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConnectToDeviceFormFormClosing);
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label pluginLabel;
		private System.Windows.Forms.ComboBox pluginComboBox;
		private System.Windows.Forms.GroupBox groupBox;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
	}
}
