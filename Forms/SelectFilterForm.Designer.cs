namespace Neurotec.Samples.Forms
{
	partial class SelectFilterForm
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
			this.bntOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnDefault = new System.Windows.Forms.Button();
			this.trackBarColorConf = new System.Windows.Forms.TrackBar();
			this.trackBarTypeConfidence = new System.Windows.Forms.TrackBar();
			this.trackBarDirectionConf = new System.Windows.Forms.TrackBar();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.panelRed = new System.Windows.Forms.Panel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panelGray = new System.Windows.Forms.Panel();
			this.panelBrown = new System.Windows.Forms.Panel();
			this.panelBlack = new System.Windows.Forms.Panel();
			this.panelWhite = new System.Windows.Forms.Panel();
			this.panelSilver = new System.Windows.Forms.Panel();
			this.panelBlue = new System.Windows.Forms.Panel();
			this.panelGreen = new System.Windows.Forms.Panel();
			this.panelYellow = new System.Windows.Forms.Panel();
			this.panelOrange = new System.Windows.Forms.Panel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.panelPerson = new System.Windows.Forms.Panel();
			this.panelBike = new System.Windows.Forms.Panel();
			this.panelBus = new System.Windows.Forms.Panel();
			this.panelTruck = new System.Windows.Forms.Panel();
			this.panelCar = new System.Windows.Forms.Panel();
			this.panelNorthWest = new System.Windows.Forms.Panel();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.panelSouthEast = new System.Windows.Forms.Panel();
			this.panelEast = new System.Windows.Forms.Panel();
			this.panelNorthEast = new System.Windows.Forms.Panel();
			this.panelSouth = new System.Windows.Forms.Panel();
			this.panelSouthWest = new System.Windows.Forms.Panel();
			this.panelWest = new System.Windows.Forms.Panel();
			this.panelNorth = new System.Windows.Forms.Panel();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.lblTypeConf = new System.Windows.Forms.Label();
			this.lblColorConfidence = new System.Windows.Forms.Label();
			this.lblDirectionConf = new System.Windows.Forms.Label();
			this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
			this.label4 = new System.Windows.Forms.Label();
			this.chbWithFace = new System.Windows.Forms.CheckBox();
			this.chbWithLicensePlate = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
			this.rbAll = new System.Windows.Forms.RadioButton();
			this.rbInWatchlist = new System.Windows.Forms.RadioButton();
			this.rbNotInWatchlist = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.trackBarColorConf)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarTypeConfidence)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarDirectionConf)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.tableLayoutPanel4.SuspendLayout();
			this.tableLayoutPanel5.SuspendLayout();
			this.tableLayoutPanel6.SuspendLayout();
			this.SuspendLayout();
			// 
			// bntOk
			// 
			this.bntOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bntOk.Location = new System.Drawing.Point(524, 3);
			this.bntOk.Name = "bntOk";
			this.bntOk.Size = new System.Drawing.Size(75, 23);
			this.bntOk.TabIndex = 0;
			this.bntOk.Text = "OK";
			this.bntOk.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(605, 3);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnDefault
			// 
			this.btnDefault.Location = new System.Drawing.Point(3, 3);
			this.btnDefault.Name = "btnDefault";
			this.btnDefault.Size = new System.Drawing.Size(75, 23);
			this.btnDefault.TabIndex = 2;
			this.btnDefault.Text = "Default";
			this.btnDefault.UseVisualStyleBackColor = true;
			this.btnDefault.Click += new System.EventHandler(this.BtnDefaultClick);
			// 
			// trackBarColorConf
			// 
			this.trackBarColorConf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.trackBarColorConf.Location = new System.Drawing.Point(351, 178);
			this.trackBarColorConf.Maximum = 100;
			this.trackBarColorConf.Minimum = 1;
			this.trackBarColorConf.Name = "trackBarColorConf";
			this.trackBarColorConf.Size = new System.Drawing.Size(335, 45);
			this.trackBarColorConf.TabIndex = 3;
			this.trackBarColorConf.Value = 50;
			this.trackBarColorConf.Scroll += new System.EventHandler(this.TrackBarColorConfScroll);
			// 
			// trackBarTypeConfidence
			// 
			this.trackBarTypeConfidence.Dock = System.Windows.Forms.DockStyle.Fill;
			this.trackBarTypeConfidence.Location = new System.Drawing.Point(351, 113);
			this.trackBarTypeConfidence.Maximum = 100;
			this.trackBarTypeConfidence.Minimum = 1;
			this.trackBarTypeConfidence.Name = "trackBarTypeConfidence";
			this.trackBarTypeConfidence.Size = new System.Drawing.Size(335, 45);
			this.trackBarTypeConfidence.TabIndex = 4;
			this.trackBarTypeConfidence.Value = 50;
			this.trackBarTypeConfidence.Scroll += new System.EventHandler(this.TrackBarTypeConfidenceScroll);
			// 
			// trackBarDirectionConf
			// 
			this.trackBarDirectionConf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.trackBarDirectionConf.Location = new System.Drawing.Point(351, 276);
			this.trackBarDirectionConf.Maximum = 100;
			this.trackBarDirectionConf.Minimum = 1;
			this.trackBarDirectionConf.Name = "trackBarDirectionConf";
			this.trackBarDirectionConf.Size = new System.Drawing.Size(335, 45);
			this.trackBarDirectionConf.TabIndex = 5;
			this.trackBarDirectionConf.Value = 30;
			this.trackBarDirectionConf.Scroll += new System.EventHandler(this.TrackBarDirectionConfScroll);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
			this.label1.Location = new System.Drawing.Point(3, 110);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 51);
			this.label1.TabIndex = 6;
			this.label1.Text = "Type";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
			this.label2.Location = new System.Drawing.Point(3, 161);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 80);
			this.label2.TabIndex = 7;
			this.label2.Text = "Color";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
			this.label3.Location = new System.Drawing.Point(3, 241);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 116);
			this.label3.TabIndex = 8;
			this.label3.Text = "Direction";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panelRed
			// 
			this.panelRed.Location = new System.Drawing.Point(3, 3);
			this.panelRed.Name = "panelRed";
			this.panelRed.Size = new System.Drawing.Size(30, 30);
			this.panelRed.TabIndex = 18;
			this.toolTip.SetToolTip(this.panelRed, "Red");
			this.panelRed.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelRedPaint);
			this.panelRed.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelRedMouseDown);
			this.panelRed.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelRed.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 5;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.panelGray, 4, 1);
			this.tableLayoutPanel1.Controls.Add(this.panelBrown, 3, 1);
			this.tableLayoutPanel1.Controls.Add(this.panelBlack, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.panelWhite, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.panelSilver, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.panelBlue, 4, 0);
			this.tableLayoutPanel1.Controls.Add(this.panelGreen, 3, 0);
			this.tableLayoutPanel1.Controls.Add(this.panelYellow, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.panelOrange, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.panelRed, 0, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(69, 164);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(181, 74);
			this.tableLayoutPanel1.TabIndex = 19;
			// 
			// panelGray
			// 
			this.panelGray.Location = new System.Drawing.Point(147, 39);
			this.panelGray.Name = "panelGray";
			this.panelGray.Size = new System.Drawing.Size(30, 30);
			this.panelGray.TabIndex = 19;
			this.toolTip.SetToolTip(this.panelGray, "Gray");
			this.panelGray.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelGrayPaint);
			this.panelGray.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelGrayMouseDown);
			this.panelGray.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelGray.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelBrown
			// 
			this.panelBrown.Location = new System.Drawing.Point(111, 39);
			this.panelBrown.Name = "panelBrown";
			this.panelBrown.Size = new System.Drawing.Size(30, 30);
			this.panelBrown.TabIndex = 19;
			this.toolTip.SetToolTip(this.panelBrown, "Brown");
			this.panelBrown.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelBrownPaint);
			this.panelBrown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelBrownMouseDown);
			this.panelBrown.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelBrown.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelBlack
			// 
			this.panelBlack.Location = new System.Drawing.Point(75, 39);
			this.panelBlack.Name = "panelBlack";
			this.panelBlack.Size = new System.Drawing.Size(30, 30);
			this.panelBlack.TabIndex = 19;
			this.toolTip.SetToolTip(this.panelBlack, "Black");
			this.panelBlack.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelBlackPaint);
			this.panelBlack.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelBlackMouseDown);
			this.panelBlack.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelBlack.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelWhite
			// 
			this.panelWhite.Location = new System.Drawing.Point(39, 39);
			this.panelWhite.Name = "panelWhite";
			this.panelWhite.Size = new System.Drawing.Size(30, 30);
			this.panelWhite.TabIndex = 19;
			this.toolTip.SetToolTip(this.panelWhite, "White");
			this.panelWhite.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelWhitePaint);
			this.panelWhite.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelWhiteMouseDown);
			this.panelWhite.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelWhite.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelSilver
			// 
			this.panelSilver.Location = new System.Drawing.Point(3, 39);
			this.panelSilver.Name = "panelSilver";
			this.panelSilver.Size = new System.Drawing.Size(30, 30);
			this.panelSilver.TabIndex = 19;
			this.toolTip.SetToolTip(this.panelSilver, "Silver");
			this.panelSilver.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelSilverPaint);
			this.panelSilver.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelSilverMouseDown);
			this.panelSilver.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelSilver.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelBlue
			// 
			this.panelBlue.Location = new System.Drawing.Point(147, 3);
			this.panelBlue.Name = "panelBlue";
			this.panelBlue.Size = new System.Drawing.Size(30, 30);
			this.panelBlue.TabIndex = 19;
			this.toolTip.SetToolTip(this.panelBlue, "Blue");
			this.panelBlue.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelBluePaint);
			this.panelBlue.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelBlueMouseDown);
			this.panelBlue.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelBlue.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelGreen
			// 
			this.panelGreen.Location = new System.Drawing.Point(111, 3);
			this.panelGreen.Name = "panelGreen";
			this.panelGreen.Size = new System.Drawing.Size(30, 30);
			this.panelGreen.TabIndex = 19;
			this.toolTip.SetToolTip(this.panelGreen, "Green");
			this.panelGreen.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelGreenPaint);
			this.panelGreen.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelGreenMouseDown);
			this.panelGreen.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelGreen.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelYellow
			// 
			this.panelYellow.Location = new System.Drawing.Point(75, 3);
			this.panelYellow.Name = "panelYellow";
			this.panelYellow.Size = new System.Drawing.Size(30, 30);
			this.panelYellow.TabIndex = 19;
			this.toolTip.SetToolTip(this.panelYellow, "Yellow");
			this.panelYellow.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelYellowPaint);
			this.panelYellow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelYellowMouseDown);
			this.panelYellow.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelYellow.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelOrange
			// 
			this.panelOrange.Location = new System.Drawing.Point(39, 3);
			this.panelOrange.Name = "panelOrange";
			this.panelOrange.Size = new System.Drawing.Size(30, 30);
			this.panelOrange.TabIndex = 19;
			this.toolTip.SetToolTip(this.panelOrange, "Orange");
			this.panelOrange.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelOrangePaint);
			this.panelOrange.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelOrangeMouseDown);
			this.panelOrange.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelOrange.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 5;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.Controls.Add(this.panelPerson, 4, 0);
			this.tableLayoutPanel2.Controls.Add(this.panelBike, 3, 0);
			this.tableLayoutPanel2.Controls.Add(this.panelBus, 2, 0);
			this.tableLayoutPanel2.Controls.Add(this.panelTruck, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.panelCar, 0, 0);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(69, 113);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 1;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(182, 40);
			this.tableLayoutPanel2.TabIndex = 20;
			// 
			// panelPerson
			// 
			this.panelPerson.Location = new System.Drawing.Point(147, 3);
			this.panelPerson.Name = "panelPerson";
			this.panelPerson.Size = new System.Drawing.Size(30, 30);
			this.panelPerson.TabIndex = 20;
			this.toolTip.SetToolTip(this.panelPerson, "Person");
			this.panelPerson.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelPersonPaint);
			this.panelPerson.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelPersonMouseDown);
			this.panelPerson.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelPerson.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelBike
			// 
			this.panelBike.Location = new System.Drawing.Point(111, 3);
			this.panelBike.Name = "panelBike";
			this.panelBike.Size = new System.Drawing.Size(30, 30);
			this.panelBike.TabIndex = 20;
			this.toolTip.SetToolTip(this.panelBike, "Bike");
			this.panelBike.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelBikePaint);
			this.panelBike.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelBikeMouseDown);
			this.panelBike.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelBike.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelBus
			// 
			this.panelBus.Location = new System.Drawing.Point(75, 3);
			this.panelBus.Name = "panelBus";
			this.panelBus.Size = new System.Drawing.Size(30, 30);
			this.panelBus.TabIndex = 20;
			this.toolTip.SetToolTip(this.panelBus, "Bus");
			this.panelBus.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelBusPaint);
			this.panelBus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelBusMouseDown);
			this.panelBus.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelBus.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelTruck
			// 
			this.panelTruck.Location = new System.Drawing.Point(39, 3);
			this.panelTruck.Name = "panelTruck";
			this.panelTruck.Size = new System.Drawing.Size(30, 30);
			this.panelTruck.TabIndex = 20;
			this.toolTip.SetToolTip(this.panelTruck, "Truck");
			this.panelTruck.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelTruckPaint);
			this.panelTruck.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelTruckMouseDown);
			this.panelTruck.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelTruck.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelCar
			// 
			this.panelCar.Location = new System.Drawing.Point(3, 3);
			this.panelCar.Name = "panelCar";
			this.panelCar.Size = new System.Drawing.Size(30, 30);
			this.panelCar.TabIndex = 20;
			this.toolTip.SetToolTip(this.panelCar, "Car");
			this.panelCar.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelCarPaint);
			this.panelCar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelCarMouseDown);
			this.panelCar.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelCar.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelNorthWest
			// 
			this.panelNorthWest.Location = new System.Drawing.Point(3, 3);
			this.panelNorthWest.Name = "panelNorthWest";
			this.panelNorthWest.Size = new System.Drawing.Size(30, 30);
			this.panelNorthWest.TabIndex = 21;
			this.toolTip.SetToolTip(this.panelNorthWest, "North West");
			this.panelNorthWest.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelNorthWestPaint);
			this.panelNorthWest.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelNorthWestMouseDown);
			this.panelNorthWest.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelNorthWest.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.tableLayoutPanel3.ColumnCount = 3;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.Controls.Add(this.panelSouthEast, 2, 2);
			this.tableLayoutPanel3.Controls.Add(this.panelEast, 2, 1);
			this.tableLayoutPanel3.Controls.Add(this.panelNorthEast, 2, 0);
			this.tableLayoutPanel3.Controls.Add(this.panelSouth, 1, 2);
			this.tableLayoutPanel3.Controls.Add(this.panelSouthWest, 0, 2);
			this.tableLayoutPanel3.Controls.Add(this.panelWest, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this.panelNorth, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this.panelNorthWest, 0, 0);
			this.tableLayoutPanel3.Location = new System.Drawing.Point(105, 244);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 3;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.Size = new System.Drawing.Size(110, 110);
			this.tableLayoutPanel3.TabIndex = 22;
			// 
			// panelSouthEast
			// 
			this.panelSouthEast.Location = new System.Drawing.Point(75, 75);
			this.panelSouthEast.Name = "panelSouthEast";
			this.panelSouthEast.Size = new System.Drawing.Size(30, 30);
			this.panelSouthEast.TabIndex = 22;
			this.toolTip.SetToolTip(this.panelSouthEast, "South East");
			this.panelSouthEast.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelSouthEastPaint);
			this.panelSouthEast.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelSouthEastMouseDown);
			this.panelSouthEast.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelSouthEast.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelEast
			// 
			this.panelEast.Location = new System.Drawing.Point(75, 39);
			this.panelEast.Name = "panelEast";
			this.panelEast.Size = new System.Drawing.Size(30, 30);
			this.panelEast.TabIndex = 22;
			this.toolTip.SetToolTip(this.panelEast, "East");
			this.panelEast.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelEastPaint);
			this.panelEast.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelEastMouseDown);
			this.panelEast.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelEast.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelNorthEast
			// 
			this.panelNorthEast.Location = new System.Drawing.Point(75, 3);
			this.panelNorthEast.Name = "panelNorthEast";
			this.panelNorthEast.Size = new System.Drawing.Size(30, 30);
			this.panelNorthEast.TabIndex = 22;
			this.toolTip.SetToolTip(this.panelNorthEast, "North East");
			this.panelNorthEast.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelNorthEastPaint);
			this.panelNorthEast.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelNorthEastMouseDown);
			this.panelNorthEast.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelNorthEast.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelSouth
			// 
			this.panelSouth.Location = new System.Drawing.Point(39, 75);
			this.panelSouth.Name = "panelSouth";
			this.panelSouth.Size = new System.Drawing.Size(30, 30);
			this.panelSouth.TabIndex = 22;
			this.toolTip.SetToolTip(this.panelSouth, "South");
			this.panelSouth.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelSouthPaint);
			this.panelSouth.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelSouthMouseDown);
			this.panelSouth.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelSouth.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelSouthWest
			// 
			this.panelSouthWest.Location = new System.Drawing.Point(3, 75);
			this.panelSouthWest.Name = "panelSouthWest";
			this.panelSouthWest.Size = new System.Drawing.Size(30, 30);
			this.panelSouthWest.TabIndex = 22;
			this.toolTip.SetToolTip(this.panelSouthWest, "South West");
			this.panelSouthWest.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelSouthWestPaint);
			this.panelSouthWest.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelSouthWestMouseDown);
			this.panelSouthWest.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelSouthWest.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelWest
			// 
			this.panelWest.Location = new System.Drawing.Point(3, 39);
			this.panelWest.Name = "panelWest";
			this.panelWest.Size = new System.Drawing.Size(30, 30);
			this.panelWest.TabIndex = 22;
			this.toolTip.SetToolTip(this.panelWest, "West");
			this.panelWest.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelWestPaint);
			this.panelWest.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelWestMouseDown);
			this.panelWest.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelWest.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// panelNorth
			// 
			this.panelNorth.Location = new System.Drawing.Point(39, 3);
			this.panelNorth.Name = "panelNorth";
			this.panelNorth.Size = new System.Drawing.Size(30, 30);
			this.panelNorth.TabIndex = 22;
			this.toolTip.SetToolTip(this.panelNorth, "North");
			this.panelNorth.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelNorthPaint);
			this.panelNorth.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelNorthMouseDown);
			this.panelNorth.MouseEnter += new System.EventHandler(this.OnPanelMouseEnter);
			this.panelNorth.MouseLeave += new System.EventHandler(this.OnPanelMouseLeave);
			// 
			// lblTypeConf
			// 
			this.lblTypeConf.AutoSize = true;
			this.lblTypeConf.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblTypeConf.Location = new System.Drawing.Point(257, 110);
			this.lblTypeConf.Name = "lblTypeConf";
			this.lblTypeConf.Size = new System.Drawing.Size(88, 51);
			this.lblTypeConf.TabIndex = 23;
			this.lblTypeConf.Text = "Confidence: 1.00";
			this.lblTypeConf.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblColorConfidence
			// 
			this.lblColorConfidence.AutoSize = true;
			this.lblColorConfidence.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblColorConfidence.Location = new System.Drawing.Point(257, 161);
			this.lblColorConfidence.Name = "lblColorConfidence";
			this.lblColorConfidence.Size = new System.Drawing.Size(88, 80);
			this.lblColorConfidence.TabIndex = 24;
			this.lblColorConfidence.Text = "Confidence: 1.00";
			this.lblColorConfidence.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblDirectionConf
			// 
			this.lblDirectionConf.AutoSize = true;
			this.lblDirectionConf.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblDirectionConf.Location = new System.Drawing.Point(257, 241);
			this.lblDirectionConf.Name = "lblDirectionConf";
			this.lblDirectionConf.Size = new System.Drawing.Size(88, 116);
			this.lblDirectionConf.TabIndex = 25;
			this.lblDirectionConf.Text = "Confidence: 1.00";
			this.lblDirectionConf.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tableLayoutPanel4
			// 
			this.tableLayoutPanel4.ColumnCount = 4;
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 0, 11);
			this.tableLayoutPanel4.Controls.Add(this.trackBarColorConf, 3, 8);
			this.tableLayoutPanel4.Controls.Add(this.trackBarDirectionConf, 3, 9);
			this.tableLayoutPanel4.Controls.Add(this.lblDirectionConf, 2, 9);
			this.tableLayoutPanel4.Controls.Add(this.lblColorConfidence, 2, 8);
			this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel3, 1, 9);
			this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel1, 1, 8);
			this.tableLayoutPanel4.Controls.Add(this.label3, 0, 9);
			this.tableLayoutPanel4.Controls.Add(this.label2, 0, 8);
			this.tableLayoutPanel4.Controls.Add(this.label1, 0, 7);
			this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel2, 1, 7);
			this.tableLayoutPanel4.Controls.Add(this.lblTypeConf, 2, 7);
			this.tableLayoutPanel4.Controls.Add(this.trackBarTypeConfidence, 3, 7);
			this.tableLayoutPanel4.Controls.Add(this.label4, 0, 0);
			this.tableLayoutPanel4.Controls.Add(this.chbWithFace, 1, 1);
			this.tableLayoutPanel4.Controls.Add(this.chbWithLicensePlate, 1, 2);
			this.tableLayoutPanel4.Controls.Add(this.label5, 0, 4);
			this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel6, 1, 4);
			this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel4.Name = "tableLayoutPanel4";
			this.tableLayoutPanel4.RowCount = 12;
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.Size = new System.Drawing.Size(689, 404);
			this.tableLayoutPanel4.TabIndex = 26;
			// 
			// tableLayoutPanel5
			// 
			this.tableLayoutPanel5.ColumnCount = 3;
			this.tableLayoutPanel4.SetColumnSpan(this.tableLayoutPanel5, 4);
			this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel5.Controls.Add(this.btnDefault, 0, 0);
			this.tableLayoutPanel5.Controls.Add(this.bntOk, 1, 0);
			this.tableLayoutPanel5.Controls.Add(this.btnCancel, 2, 0);
			this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 370);
			this.tableLayoutPanel5.Name = "tableLayoutPanel5";
			this.tableLayoutPanel5.RowCount = 1;
			this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
			this.tableLayoutPanel5.Size = new System.Drawing.Size(683, 31);
			this.tableLayoutPanel5.TabIndex = 27;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(3, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(41, 13);
			this.label4.TabIndex = 28;
			this.label4.Text = "Filters";
			// 
			// chbWithFace
			// 
			this.chbWithFace.AutoSize = true;
			this.chbWithFace.Location = new System.Drawing.Point(69, 16);
			this.chbWithFace.Name = "chbWithFace";
			this.chbWithFace.Size = new System.Drawing.Size(100, 17);
			this.chbWithFace.TabIndex = 29;
			this.chbWithFace.Text = "Must have face";
			this.chbWithFace.UseVisualStyleBackColor = true;
			this.chbWithFace.CheckedChanged += new System.EventHandler(this.ChbWithFaceCheckedChanged);
			// 
			// chbWithLicensePlate
			// 
			this.chbWithLicensePlate.AutoSize = true;
			this.chbWithLicensePlate.Location = new System.Drawing.Point(69, 39);
			this.chbWithLicensePlate.Name = "chbWithLicensePlate";
			this.chbWithLicensePlate.Size = new System.Drawing.Size(138, 17);
			this.chbWithLicensePlate.TabIndex = 30;
			this.chbWithLicensePlate.Text = "Must have license plate";
			this.chbWithLicensePlate.UseVisualStyleBackColor = true;
			this.chbWithLicensePlate.CheckedChanged += new System.EventHandler(this.ChbWithLicensePlateCheckedChanged);
			// 
			// label5
			// 
			this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(3, 78);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(60, 13);
			this.label5.TabIndex = 31;
			this.label5.Text = "Watchlist";
			// 
			// tableLayoutPanel6
			// 
			this.tableLayoutPanel6.ColumnCount = 3;
			this.tableLayoutPanel4.SetColumnSpan(this.tableLayoutPanel6, 3);
			this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel6.Controls.Add(this.rbAll, 0, 0);
			this.tableLayoutPanel6.Controls.Add(this.rbInWatchlist, 1, 0);
			this.tableLayoutPanel6.Controls.Add(this.rbNotInWatchlist, 2, 0);
			this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel6.Location = new System.Drawing.Point(69, 72);
			this.tableLayoutPanel6.Name = "tableLayoutPanel6";
			this.tableLayoutPanel6.RowCount = 1;
			this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel6.Size = new System.Drawing.Size(617, 25);
			this.tableLayoutPanel6.TabIndex = 32;
			// 
			// rbAll
			// 
			this.rbAll.AutoSize = true;
			this.rbAll.Checked = true;
			this.rbAll.Location = new System.Drawing.Point(3, 3);
			this.rbAll.Name = "rbAll";
			this.rbAll.Size = new System.Drawing.Size(75, 17);
			this.rbAll.TabIndex = 0;
			this.rbAll.TabStop = true;
			this.rbAll.Text = "Everything";
			this.rbAll.UseVisualStyleBackColor = true;
			this.rbAll.CheckedChanged += new System.EventHandler(this.RbAllCheckedChanged);
			// 
			// rbInWatchlist
			// 
			this.rbInWatchlist.AutoSize = true;
			this.rbInWatchlist.Location = new System.Drawing.Point(84, 3);
			this.rbInWatchlist.Name = "rbInWatchlist";
			this.rbInWatchlist.Size = new System.Drawing.Size(81, 17);
			this.rbInWatchlist.TabIndex = 1;
			this.rbInWatchlist.Text = "In Watchlist";
			this.rbInWatchlist.UseVisualStyleBackColor = true;
			this.rbInWatchlist.CheckedChanged += new System.EventHandler(this.RbInWatchlistCheckedChanged);
			// 
			// rbNotInWatchlist
			// 
			this.rbNotInWatchlist.AutoSize = true;
			this.rbNotInWatchlist.Location = new System.Drawing.Point(171, 3);
			this.rbNotInWatchlist.Name = "rbNotInWatchlist";
			this.rbNotInWatchlist.Size = new System.Drawing.Size(101, 17);
			this.rbNotInWatchlist.TabIndex = 2;
			this.rbNotInWatchlist.Text = "Not In Watchlist";
			this.rbNotInWatchlist.UseVisualStyleBackColor = true;
			this.rbNotInWatchlist.CheckedChanged += new System.EventHandler(this.RbNotInWatchlistCheckedChanged);
			// 
			// SelectFilterForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(689, 404);
			this.Controls.Add(this.tableLayoutPanel4);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "SelectFilterForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Filter";
			this.Shown += new System.EventHandler(this.SelectFilterFormShown);
			((System.ComponentModel.ISupportInitialize)(this.trackBarColorConf)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarTypeConfidence)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarDirectionConf)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel4.ResumeLayout(false);
			this.tableLayoutPanel4.PerformLayout();
			this.tableLayoutPanel5.ResumeLayout(false);
			this.tableLayoutPanel6.ResumeLayout(false);
			this.tableLayoutPanel6.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button bntOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnDefault;
		private System.Windows.Forms.TrackBar trackBarColorConf;
		private System.Windows.Forms.TrackBar trackBarTypeConfidence;
		private System.Windows.Forms.TrackBar trackBarDirectionConf;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel panelRed;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panelGray;
		private System.Windows.Forms.Panel panelBrown;
		private System.Windows.Forms.Panel panelBlack;
		private System.Windows.Forms.Panel panelWhite;
		private System.Windows.Forms.Panel panelSilver;
		private System.Windows.Forms.Panel panelBlue;
		private System.Windows.Forms.Panel panelGreen;
		private System.Windows.Forms.Panel panelYellow;
		private System.Windows.Forms.Panel panelOrange;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Panel panelPerson;
		private System.Windows.Forms.Panel panelBike;
		private System.Windows.Forms.Panel panelBus;
		private System.Windows.Forms.Panel panelTruck;
		private System.Windows.Forms.Panel panelCar;
		private System.Windows.Forms.Panel panelNorthWest;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.Panel panelSouthEast;
		private System.Windows.Forms.Panel panelEast;
		private System.Windows.Forms.Panel panelNorthEast;
		private System.Windows.Forms.Panel panelSouth;
		private System.Windows.Forms.Panel panelSouthWest;
		private System.Windows.Forms.Panel panelWest;
		private System.Windows.Forms.Panel panelNorth;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Label lblTypeConf;
		private System.Windows.Forms.Label lblColorConfidence;
		private System.Windows.Forms.Label lblDirectionConf;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox chbWithFace;
		private System.Windows.Forms.CheckBox chbWithLicensePlate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
		private System.Windows.Forms.RadioButton rbAll;
		private System.Windows.Forms.RadioButton rbInWatchlist;
		private System.Windows.Forms.RadioButton rbNotInWatchlist;
	}
}
