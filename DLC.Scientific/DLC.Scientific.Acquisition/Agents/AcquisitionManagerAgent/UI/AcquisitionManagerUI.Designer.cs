using Telerik.WinControls.UI;
namespace DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI
{
	partial class AcquisitionManagerUI
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
			this.pnlPrepareAcqui = new System.Windows.Forms.Panel();
			this.imgDeleteOperator = new System.Windows.Forms.PictureBox();
			this.imgDeleteDriver = new System.Windows.Forms.PictureBox();
			this.ddlSequenceTypes = new Telerik.WinControls.UI.RadDropDownList();
			this.ddlVehicles = new Telerik.WinControls.UI.RadDropDownList();
			this.ddlOperators = new Telerik.WinControls.UI.RadDropDownList();
			this.ddlDrivers = new Telerik.WinControls.UI.RadDropDownList();
			this.imgAddOperator = new System.Windows.Forms.PictureBox();
			this.imgAddDriver = new System.Windows.Forms.PictureBox();
			this.imgPrepareAcquisition = new System.Windows.Forms.PictureBox();
			this.lblSequenceType = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomLabel();
			this.lblDriver = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomLabel();
			this.lblVehicle = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomLabel();
			this.lblOperator = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomLabel();
			this.pnlStopAcqui = new System.Windows.Forms.Panel();
			this.ddlStopMode = new Telerik.WinControls.UI.RadDropDownList();
			this.ddlProximityStop = new Telerik.WinControls.UI.RadDropDownList();
			this.pnlStopDirection = new System.Windows.Forms.Panel();
			this.lblStopDirection = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomLabel();
			this.rdStopDirection2 = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomRadioButton();
			this.rdStopDirection1 = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomRadioButton();
			this.imgStopAcquisition = new System.Windows.Forms.PictureBox();
			this.tcStopAcqui = new System.Windows.Forms.TabControl();
			this.tpStopDistance = new System.Windows.Forms.TabPage();
			this.lblDistanceStop = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomLabel();
			this.txtDistanceStop = new System.Windows.Forms.TextBox();
			this.tpStopRtssDdls = new System.Windows.Forms.TabPage();
			this.ddlSousRouteStop = new Telerik.WinControls.UI.RadDropDownList();
			this.ddlSectionStop = new Telerik.WinControls.UI.RadDropDownList();
			this.ddlRouteStop = new Telerik.WinControls.UI.RadDropDownList();
			this.ddlTronconStop = new Telerik.WinControls.UI.RadDropDownList();
			this.mtxtChainageSelectionStop = new System.Windows.Forms.MaskedTextBox();
			this.imgGetCurrentRTSSStop = new System.Windows.Forms.PictureBox();
			this.tpStopInvisible = new System.Windows.Forms.TabPage();
			this.tpStopWaitingForBGR = new System.Windows.Forms.TabPage();
			this.picStopWaitingForBGR = new System.Windows.Forms.PictureBox();
			this.lblStopWaitingForBGR = new System.Windows.Forms.Label();
			this.lblStopMode = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomLabel();
			this.txtCommentaires = new System.Windows.Forms.TextBox();
			this.pnlValidation = new System.Windows.Forms.Panel();
			this.imgSaveValidation = new System.Windows.Forms.PictureBox();
			this.lblMsgValidation = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomLabel();
			this.rdInvalide = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomRadioButton();
			this.lblCommentsValidation = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomLabel();
			this.rdValide = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomRadioButton();
			this.pnlMessageStartMode = new System.Windows.Forms.Panel();
			this.AcquisitionManagerContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.StartInfoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.CurrentRtssMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.NextRtssMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tcStartAcqui = new System.Windows.Forms.TabControl();
			this.tpStartRtssDdls = new System.Windows.Forms.TabPage();
			this.ddlSousRouteStart = new Telerik.WinControls.UI.RadDropDownList();
			this.ddlSectionStart = new Telerik.WinControls.UI.RadDropDownList();
			this.ddlTronconStart = new Telerik.WinControls.UI.RadDropDownList();
			this.ddlRouteStart = new Telerik.WinControls.UI.RadDropDownList();
			this.mtxtChainageSelectionStart = new System.Windows.Forms.MaskedTextBox();
			this.imgGetCurrentRTSSStart = new System.Windows.Forms.PictureBox();
			this.tpStartInvisible = new System.Windows.Forms.TabPage();
			this.tpStartWaitingForBGR = new System.Windows.Forms.TabPage();
			this.picStartWaitingForBGR = new System.Windows.Forms.PictureBox();
			this.lblStartWaitingForBGR = new System.Windows.Forms.Label();
			this.pnlStartDirection = new System.Windows.Forms.Panel();
			this.lblStartDirection = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomLabel();
			this.rdStartDirection2 = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomRadioButton();
			this.rdStartDirection1 = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomRadioButton();
			this.pnlStartAcqui = new System.Windows.Forms.Panel();
			this.ddlStartMode = new Telerik.WinControls.UI.RadDropDownList();
			this.ddlProximityStart = new Telerik.WinControls.UI.RadDropDownList();
			this.imgStartAcquisition = new System.Windows.Forms.PictureBox();
			this.lblStartMode = new DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.CustomLabel();
			this.compactGuiToolTips = new System.Windows.Forms.ToolTip(this.components);
			this.staticToolTips = new System.Windows.Forms.ToolTip(this.components);
			this.pnlGeneralInfos.SuspendLayout();
			this.pnlPrepareAcqui.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.imgDeleteOperator)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imgDeleteDriver)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlSequenceTypes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlVehicles)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlOperators)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlDrivers)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imgAddOperator)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imgAddDriver)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imgPrepareAcquisition)).BeginInit();
			this.pnlStopAcqui.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ddlStopMode)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlProximityStop)).BeginInit();
			this.pnlStopDirection.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.imgStopAcquisition)).BeginInit();
			this.tcStopAcqui.SuspendLayout();
			this.tpStopDistance.SuspendLayout();
			this.tpStopRtssDdls.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ddlSousRouteStop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlSectionStop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlRouteStop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlTronconStop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imgGetCurrentRTSSStop)).BeginInit();
			this.tpStopWaitingForBGR.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picStopWaitingForBGR)).BeginInit();
			this.pnlValidation.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.imgSaveValidation)).BeginInit();
			this.AcquisitionManagerContextMenu.SuspendLayout();
			this.tcStartAcqui.SuspendLayout();
			this.tpStartRtssDdls.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ddlSousRouteStart)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlSectionStart)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlTronconStart)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlRouteStart)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imgGetCurrentRTSSStart)).BeginInit();
			this.tpStartWaitingForBGR.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picStartWaitingForBGR)).BeginInit();
			this.pnlStartDirection.SuspendLayout();
			this.pnlStartAcqui.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ddlStartMode)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlProximityStart)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imgStartAcquisition)).BeginInit();
			this.SuspendLayout();
			//
			// pnlGeneralInfos
			//
			this.pnlGeneralInfos.Controls.Add(this.txtCommentaires);
			this.pnlGeneralInfos.Controls.Add(this.pnlValidation);
			this.pnlGeneralInfos.Controls.Add(this.pnlPrepareAcqui);
			this.pnlGeneralInfos.Controls.Add(this.pnlStopAcqui);
			this.pnlGeneralInfos.Controls.Add(this.pnlStartAcqui);
			this.pnlGeneralInfos.Size = new System.Drawing.Size(649, 726);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.pnlStartAcqui, 0);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.pnlStopAcqui, 0);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.pnlPrepareAcqui, 0);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.pnlValidation, 0);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.txtCommentaires, 0);
			//
			// pnlPrepareAcqui
			//
			this.pnlPrepareAcqui.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.pnlPrepareAcqui.Controls.Add(this.imgDeleteOperator);
			this.pnlPrepareAcqui.Controls.Add(this.imgDeleteDriver);
			this.pnlPrepareAcqui.Controls.Add(this.ddlSequenceTypes);
			this.pnlPrepareAcqui.Controls.Add(this.ddlVehicles);
			this.pnlPrepareAcqui.Controls.Add(this.ddlOperators);
			this.pnlPrepareAcqui.Controls.Add(this.ddlDrivers);
			this.pnlPrepareAcqui.Controls.Add(this.imgAddOperator);
			this.pnlPrepareAcqui.Controls.Add(this.imgAddDriver);
			this.pnlPrepareAcqui.Controls.Add(this.imgPrepareAcquisition);
			this.pnlPrepareAcqui.Controls.Add(this.lblSequenceType);
			this.pnlPrepareAcqui.Controls.Add(this.lblDriver);
			this.pnlPrepareAcqui.Controls.Add(this.lblVehicle);
			this.pnlPrepareAcqui.Controls.Add(this.lblOperator);
			this.pnlPrepareAcqui.Enabled = false;
			this.pnlPrepareAcqui.Location = new System.Drawing.Point(8, 12);
			this.pnlPrepareAcqui.Name = "pnlPrepareAcqui";
			this.pnlPrepareAcqui.Size = new System.Drawing.Size(637, 111);
			this.pnlPrepareAcqui.TabIndex = 40;
			//
			// imgDeleteOperator
			//
			this.imgDeleteOperator.BackgroundImage = global::DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.ImageResources.Delete;
			this.imgDeleteOperator.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.imgDeleteOperator.Cursor = System.Windows.Forms.Cursors.Hand;
			this.imgDeleteOperator.Location = new System.Drawing.Point(249, 83);
			this.imgDeleteOperator.Name = "imgDeleteOperator";
			this.imgDeleteOperator.Size = new System.Drawing.Size(24, 24);
			this.imgDeleteOperator.TabIndex = 78;
			this.imgDeleteOperator.TabStop = false;
			this.staticToolTips.SetToolTip(this.imgDeleteOperator, "Delete selected operator");
			this.imgDeleteOperator.Click += new System.EventHandler(this.imgDeleteOperator_Click);
			//
			// imgDeleteDriver
			//
			this.imgDeleteDriver.BackgroundImage = global::DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.ImageResources.Delete;
			this.imgDeleteDriver.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.imgDeleteDriver.Cursor = System.Windows.Forms.Cursors.Hand;
			this.imgDeleteDriver.Location = new System.Drawing.Point(249, 29);
			this.imgDeleteDriver.Name = "imgDeleteDriver";
			this.imgDeleteDriver.Size = new System.Drawing.Size(24, 24);
			this.imgDeleteDriver.TabIndex = 77;
			this.imgDeleteDriver.TabStop = false;
			this.staticToolTips.SetToolTip(this.imgDeleteDriver, "Delete selected driver");
			this.imgDeleteDriver.Click += new System.EventHandler(this.imgDeleteDriver_Click);
			//
			// ddlSequenceTypes
			//
			this.ddlSequenceTypes.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ddlSequenceTypes.AutoSize = false;
			this.ddlSequenceTypes.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
			this.ddlSequenceTypes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ddlSequenceTypes.Location = new System.Drawing.Point(297, 83);
			this.ddlSequenceTypes.Name = "ddlSequenceTypes";
			this.ddlSequenceTypes.Size = new System.Drawing.Size(202, 24);
			this.ddlSequenceTypes.SortStyle = Telerik.WinControls.Enumerations.SortStyle.Ascending;
			this.ddlSequenceTypes.TabIndex = 76;
			//
			// ddlVehicles
			//
			this.ddlVehicles.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ddlVehicles.AutoSize = false;
			this.ddlVehicles.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
			this.ddlVehicles.Enabled = false;
			this.ddlVehicles.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ddlVehicles.Location = new System.Drawing.Point(297, 29);
			this.ddlVehicles.Name = "ddlVehicles";
			this.ddlVehicles.Size = new System.Drawing.Size(202, 24);
			this.ddlVehicles.SortStyle = Telerik.WinControls.Enumerations.SortStyle.Ascending;
			this.ddlVehicles.TabIndex = 75;
			//
			// ddlOperators
			//
			this.ddlOperators.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ddlOperators.AutoSize = false;
			this.ddlOperators.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
			this.ddlOperators.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ddlOperators.Location = new System.Drawing.Point(11, 83);
			this.ddlOperators.Name = "ddlOperators";
			this.ddlOperators.Size = new System.Drawing.Size(202, 24);
			this.ddlOperators.SortStyle = Telerik.WinControls.Enumerations.SortStyle.Ascending;
			this.ddlOperators.TabIndex = 74;
			//
			// ddlDrivers
			//
			this.ddlDrivers.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ddlDrivers.AutoSize = false;
			this.ddlDrivers.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
			this.ddlDrivers.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ddlDrivers.Location = new System.Drawing.Point(11, 29);
			this.ddlDrivers.Name = "ddlDrivers";
			this.ddlDrivers.Size = new System.Drawing.Size(202, 24);
			this.ddlDrivers.SortStyle = Telerik.WinControls.Enumerations.SortStyle.Ascending;
			this.ddlDrivers.TabIndex = 73;
			//
			// imgAddOperator
			//
			this.imgAddOperator.BackgroundImage = global::DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.ImageResources.Add;
			this.imgAddOperator.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.imgAddOperator.Cursor = System.Windows.Forms.Cursors.Hand;
			this.imgAddOperator.Location = new System.Drawing.Point(219, 83);
			this.imgAddOperator.Name = "imgAddOperator";
			this.imgAddOperator.Size = new System.Drawing.Size(24, 24);
			this.imgAddOperator.TabIndex = 72;
			this.imgAddOperator.TabStop = false;
			this.staticToolTips.SetToolTip(this.imgAddOperator, "Ajouter un opérateur");
			this.imgAddOperator.Click += new System.EventHandler(this.imgAddOperator_Click);
			//
			// imgAddDriver
			//
			this.imgAddDriver.BackgroundImage = global::DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.ImageResources.Add;
			this.imgAddDriver.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.imgAddDriver.Cursor = System.Windows.Forms.Cursors.Hand;
			this.imgAddDriver.Location = new System.Drawing.Point(219, 29);
			this.imgAddDriver.Name = "imgAddDriver";
			this.imgAddDriver.Size = new System.Drawing.Size(24, 24);
			this.imgAddDriver.TabIndex = 71;
			this.imgAddDriver.TabStop = false;
			this.staticToolTips.SetToolTip(this.imgAddDriver, "Ajouter un conducteur");
			this.imgAddDriver.Click += new System.EventHandler(this.imgAddDriver_Click);
			//
			// imgPrepareAcquisition
			//
			this.imgPrepareAcquisition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.imgPrepareAcquisition.BackgroundImage = global::DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.ImageResources.PrepareButton;
			this.imgPrepareAcquisition.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.imgPrepareAcquisition.Cursor = System.Windows.Forms.Cursors.Hand;
			this.imgPrepareAcquisition.Location = new System.Drawing.Point(552, 14);
			this.imgPrepareAcquisition.Name = "imgPrepareAcquisition";
			this.imgPrepareAcquisition.Size = new System.Drawing.Size(82, 82);
			this.imgPrepareAcquisition.TabIndex = 70;
			this.imgPrepareAcquisition.TabStop = false;
			this.staticToolTips.SetToolTip(this.imgPrepareAcquisition, "Prepare acquisition");
			//
			// lblSequenceType
			//
			this.lblSequenceType.AutoSize = true;
			this.lblSequenceType.DisabledColor = System.Drawing.Color.Gray;
			this.lblSequenceType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSequenceType.ForeColor = System.Drawing.Color.Yellow;
			this.lblSequenceType.Location = new System.Drawing.Point(293, 60);
			this.lblSequenceType.Name = "lblSequenceType";
			this.lblSequenceType.Size = new System.Drawing.Size(104, 20);
			this.lblSequenceType.TabIndex = 40;
			this.lblSequenceType.Text = "Acquisition Type:";
			//
			// lblDriver
			//
			this.lblDriver.AutoSize = true;
			this.lblDriver.DisabledColor = System.Drawing.Color.Gray;
			this.lblDriver.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDriver.ForeColor = System.Drawing.Color.Yellow;
			this.lblDriver.Location = new System.Drawing.Point(10, 7);
			this.lblDriver.Name = "lblDriver";
			this.lblDriver.Size = new System.Drawing.Size(112, 20);
			this.lblDriver.TabIndex = 31;
			this.lblDriver.Text = "Driver:";
			//
			// lblVehicle
			//
			this.lblVehicle.AutoSize = true;
			this.lblVehicle.DisabledColor = System.Drawing.Color.Gray;
			this.lblVehicle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblVehicle.ForeColor = System.Drawing.Color.Yellow;
			this.lblVehicle.Location = new System.Drawing.Point(293, 7);
			this.lblVehicle.Name = "lblVehicle";
			this.lblVehicle.Size = new System.Drawing.Size(88, 20);
			this.lblVehicle.TabIndex = 35;
			this.lblVehicle.Text = "Vehicle:";
			//
			// lblOperator
			//
			this.lblOperator.AutoSize = true;
			this.lblOperator.DisabledColor = System.Drawing.Color.Gray;
			this.lblOperator.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblOperator.ForeColor = System.Drawing.Color.Yellow;
			this.lblOperator.Location = new System.Drawing.Point(6, 60);
			this.lblOperator.Name = "lblOperator";
			this.lblOperator.Size = new System.Drawing.Size(100, 20);
			this.lblOperator.TabIndex = 32;
			this.lblOperator.Text = "Operator:";
			//
			// pnlStopAcqui
			//
			this.pnlStopAcqui.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnlStopAcqui.Controls.Add(this.ddlStopMode);
			this.pnlStopAcqui.Controls.Add(this.ddlProximityStop);
			this.pnlStopAcqui.Controls.Add(this.pnlStopDirection);
			this.pnlStopAcqui.Controls.Add(this.imgStopAcquisition);
			this.pnlStopAcqui.Controls.Add(this.tcStopAcqui);
			this.pnlStopAcqui.Controls.Add(this.lblStopMode);
			this.pnlStopAcqui.Location = new System.Drawing.Point(8, 258);
			this.pnlStopAcqui.Name = "pnlStopAcqui";
			this.pnlStopAcqui.Size = new System.Drawing.Size(634, 127);
			this.pnlStopAcqui.TabIndex = 99;
			//
			// ddlStopMode
			//
			this.ddlStopMode.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ddlStopMode.AutoSize = false;
			this.ddlStopMode.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
			this.ddlStopMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ddlStopMode.Location = new System.Drawing.Point(5, 32);
			this.ddlStopMode.Name = "ddlStopMode";
			this.ddlStopMode.Size = new System.Drawing.Size(248, 28);
			this.ddlStopMode.TabIndex = 122;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlStopMode.GetChildAt(0))).DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlStopMode.GetChildAt(0))).AutoSize = true;
			//
			// ddlProximityStop
			//
			this.ddlProximityStop.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ddlProximityStop.AutoSize = false;
			this.ddlProximityStop.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
			this.ddlProximityStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ddlProximityStop.Location = new System.Drawing.Point(259, 32);
			this.ddlProximityStop.Name = "ddlProximityStop";
			this.ddlProximityStop.Size = new System.Drawing.Size(63, 28);
			this.ddlProximityStop.TabIndex = 121;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlProximityStop.GetChildAt(0))).DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlProximityStop.GetChildAt(0))).AutoSize = true;
			//
			// pnlStopDirection
			//
			this.pnlStopDirection.Controls.Add(this.lblStopDirection);
			this.pnlStopDirection.Controls.Add(this.rdStopDirection2);
			this.pnlStopDirection.Controls.Add(this.rdStopDirection1);
			this.pnlStopDirection.Location = new System.Drawing.Point(326, 33);
			this.pnlStopDirection.Name = "pnlStopDirection";
			this.pnlStopDirection.Size = new System.Drawing.Size(215, 27);
			this.pnlStopDirection.TabIndex = 118;
			//
			// lblStopDirection
			//
			this.lblStopDirection.AutoSize = true;
			this.lblStopDirection.DisabledColor = System.Drawing.Color.Gray;
			this.lblStopDirection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblStopDirection.ForeColor = System.Drawing.Color.Yellow;
			this.lblStopDirection.Location = new System.Drawing.Point(8, 6);
			this.lblStopDirection.Name = "lblStopDirection";
			this.lblStopDirection.Size = new System.Drawing.Size(78, 16);
			this.lblStopDirection.TabIndex = 42;
			this.lblStopDirection.Text = "Direction:";
			//
			// rdStopDirection2
			//
			this.rdStopDirection2.AutoSize = true;
			this.rdStopDirection2.DisabledColor = System.Drawing.Color.Gray;
			this.rdStopDirection2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rdStopDirection2.ForeColor = System.Drawing.Color.Yellow;
			this.rdStopDirection2.Location = new System.Drawing.Point(153, 4);
			this.rdStopDirection2.Name = "rdStopDirection2";
			this.rdStopDirection2.Size = new System.Drawing.Size(54, 20);
			this.rdStopDirection2.TabIndex = 44;
			this.rdStopDirection2.TabStop = true;
			this.rdStopDirection2.Text = "       ";
			this.rdStopDirection2.TextCustom = "2";
			this.rdStopDirection2.UseVisualStyleBackColor = true;
			//
			// rdStopDirection1
			//
			this.rdStopDirection1.AutoSize = true;
			this.rdStopDirection1.Checked = true;
			this.rdStopDirection1.DisabledColor = System.Drawing.Color.Gray;
			this.rdStopDirection1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rdStopDirection1.ForeColor = System.Drawing.Color.Yellow;
			this.rdStopDirection1.Location = new System.Drawing.Point(93, 4);
			this.rdStopDirection1.Name = "rdStopDirection1";
			this.rdStopDirection1.Size = new System.Drawing.Size(54, 20);
			this.rdStopDirection1.TabIndex = 43;
			this.rdStopDirection1.TabStop = true;
			this.rdStopDirection1.Text = "       ";
			this.rdStopDirection1.TextCustom = "1";
			this.rdStopDirection1.UseVisualStyleBackColor = true;
			//
			// imgStopAcquisition
			//
			this.imgStopAcquisition.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.imgStopAcquisition.BackgroundImage = global::DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.ImageResources.SuccessButton;
			this.imgStopAcquisition.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.imgStopAcquisition.Cursor = System.Windows.Forms.Cursors.Hand;
			this.imgStopAcquisition.Location = new System.Drawing.Point(547, 18);
			this.imgStopAcquisition.Name = "imgStopAcquisition";
			this.imgStopAcquisition.Size = new System.Drawing.Size(82, 82);
			this.imgStopAcquisition.TabIndex = 101;
			this.imgStopAcquisition.TabStop = false;
			//
			// tcStopAcqui
			//
			this.tcStopAcqui.Appearance = System.Windows.Forms.TabAppearance.Buttons;
			this.tcStopAcqui.Controls.Add(this.tpStopDistance);
			this.tcStopAcqui.Controls.Add(this.tpStopRtssDdls);
			this.tcStopAcqui.Controls.Add(this.tpStopInvisible);
			this.tcStopAcqui.Controls.Add(this.tpStopWaitingForBGR);
			this.tcStopAcqui.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
			this.tcStopAcqui.ItemSize = new System.Drawing.Size(75, 20);
			this.tcStopAcqui.Location = new System.Drawing.Point(-5, 66);
			this.tcStopAcqui.Name = "tcStopAcqui";
			this.tcStopAcqui.SelectedIndex = 0;
			this.tcStopAcqui.Size = new System.Drawing.Size(546, 54);
			this.tcStopAcqui.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tcStopAcqui.TabIndex = 112;
			this.tcStopAcqui.TabStop = false;
			//
			// tpStopDistance
			//
			this.tpStopDistance.BackColor = System.Drawing.Color.Black;
			this.tpStopDistance.Controls.Add(this.lblDistanceStop);
			this.tpStopDistance.Controls.Add(this.txtDistanceStop);
			this.tpStopDistance.ForeColor = System.Drawing.Color.Yellow;
			this.tpStopDistance.Location = new System.Drawing.Point(4, 24);
			this.tpStopDistance.Name = "tpStopDistance";
			this.tpStopDistance.Padding = new System.Windows.Forms.Padding(3);
			this.tpStopDistance.Size = new System.Drawing.Size(538, 26);
			this.tpStopDistance.TabIndex = 0;
			this.tpStopDistance.Text = "StopDistance";
			//
			// lblDistanceStop
			//
			this.lblDistanceStop.AutoSize = true;
			this.lblDistanceStop.DisabledColor = System.Drawing.Color.Gray;
			this.lblDistanceStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDistanceStop.ForeColor = System.Drawing.Color.Yellow;
			this.lblDistanceStop.Location = new System.Drawing.Point(6, 9);
			this.lblDistanceStop.Name = "lblDistanceStop";
			this.lblDistanceStop.Size = new System.Drawing.Size(138, 16);
			this.lblDistanceStop.TabIndex = 108;
			this.lblDistanceStop.Text = "Distance (meters):";
			//
			// txtDistanceStop
			//
			this.txtDistanceStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtDistanceStop.Location = new System.Drawing.Point(163, 6);
			this.txtDistanceStop.Name = "txtDistanceStop";
			this.txtDistanceStop.Size = new System.Drawing.Size(93, 22);
			this.txtDistanceStop.TabIndex = 45;
			//
			// tpStopRtssDdls
			//
			this.tpStopRtssDdls.BackColor = System.Drawing.Color.Black;
			this.tpStopRtssDdls.Controls.Add(this.ddlSousRouteStop);
			this.tpStopRtssDdls.Controls.Add(this.ddlSectionStop);
			this.tpStopRtssDdls.Controls.Add(this.ddlRouteStop);
			this.tpStopRtssDdls.Controls.Add(this.ddlTronconStop);
			this.tpStopRtssDdls.Controls.Add(this.mtxtChainageSelectionStop);
			this.tpStopRtssDdls.Controls.Add(this.imgGetCurrentRTSSStop);
			this.tpStopRtssDdls.ForeColor = System.Drawing.Color.Yellow;
			this.tpStopRtssDdls.Location = new System.Drawing.Point(4, 24);
			this.tpStopRtssDdls.Name = "tpStopRtssDdls";
			this.tpStopRtssDdls.Padding = new System.Windows.Forms.Padding(3);
			this.tpStopRtssDdls.Size = new System.Drawing.Size(538, 26);
			this.tpStopRtssDdls.TabIndex = 2;
			this.tpStopRtssDdls.Text = "StopRtssDdls";
			//
			// ddlSousRouteStop
			//
			this.ddlSousRouteStop.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ddlSousRouteStop.AutoSize = false;
			this.ddlSousRouteStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ddlSousRouteStop.Location = new System.Drawing.Point(315, 7);
			this.ddlSousRouteStop.Name = "ddlSousRouteStop";
			this.ddlSousRouteStop.Size = new System.Drawing.Size(96, 26);
			this.ddlSousRouteStop.TabIndex = 121;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlSousRouteStop.GetChildAt(0))).AutoSize = true;
			//
			// ddlSectionStop
			//
			this.ddlSectionStop.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ddlSectionStop.AutoSize = false;
			this.ddlSectionStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ddlSectionStop.Location = new System.Drawing.Point(213, 7);
			this.ddlSectionStop.Name = "ddlSectionStop";
			this.ddlSectionStop.Size = new System.Drawing.Size(96, 26);
			this.ddlSectionStop.TabIndex = 120;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlSectionStop.GetChildAt(0))).AutoSize = true;
			//
			// ddlRouteStop
			//
			this.ddlRouteStop.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ddlRouteStop.AutoSize = false;
			this.ddlRouteStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ddlRouteStop.Location = new System.Drawing.Point(7, 6);
			this.ddlRouteStop.Name = "ddlRouteStop";
			this.ddlRouteStop.Size = new System.Drawing.Size(96, 26);
			this.ddlRouteStop.TabIndex = 119;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlRouteStop.GetChildAt(0))).AutoSize = true;
			//
			// ddlTronconStop
			//
			this.ddlTronconStop.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ddlTronconStop.AutoSize = false;
			this.ddlTronconStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ddlTronconStop.Location = new System.Drawing.Point(109, 6);
			this.ddlTronconStop.Name = "ddlTronconStop";
			this.ddlTronconStop.Size = new System.Drawing.Size(96, 26);
			this.ddlTronconStop.TabIndex = 119;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlTronconStop.GetChildAt(0))).AutoSize = true;
			//
			// mtxtChainageSelectionStop
			//
			this.mtxtChainageSelectionStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mtxtChainageSelectionStop.HidePromptOnLeave = true;
			this.mtxtChainageSelectionStop.Location = new System.Drawing.Point(418, 6);
			this.mtxtChainageSelectionStop.Mask = "99990";
			this.mtxtChainageSelectionStop.Name = "mtxtChainageSelectionStop";
			this.mtxtChainageSelectionStop.PromptChar = ' ';
			this.mtxtChainageSelectionStop.Size = new System.Drawing.Size(78, 26);
			this.mtxtChainageSelectionStop.TabIndex = 55;
			this.mtxtChainageSelectionStop.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.staticToolTips.SetToolTip(this.mtxtChainageSelectionStop, "Chaînage");
			this.mtxtChainageSelectionStop.ValidatingType = typeof(int);
			//
			// imgGetCurrentRTSSStop
			//
			this.imgGetCurrentRTSSStop.BackgroundImage = global::DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.ImageResources.Pinpoint;
			this.imgGetCurrentRTSSStop.Cursor = System.Windows.Forms.Cursors.Hand;
			this.imgGetCurrentRTSSStop.Location = new System.Drawing.Point(502, 6);
			this.imgGetCurrentRTSSStop.Name = "imgGetCurrentRTSSStop";
			this.imgGetCurrentRTSSStop.Size = new System.Drawing.Size(29, 28);
			this.imgGetCurrentRTSSStop.TabIndex = 106;
			this.imgGetCurrentRTSSStop.TabStop = false;
			this.staticToolTips.SetToolTip(this.imgGetCurrentRTSSStop, "Current RTSSC");
			//
			// tpStopInvisible
			//
			this.tpStopInvisible.BackColor = System.Drawing.Color.Black;
			this.tpStopInvisible.ForeColor = System.Drawing.Color.Yellow;
			this.tpStopInvisible.Location = new System.Drawing.Point(4, 24);
			this.tpStopInvisible.Name = "tpStopInvisible";
			this.tpStopInvisible.Padding = new System.Windows.Forms.Padding(3);
			this.tpStopInvisible.Size = new System.Drawing.Size(538, 26);
			this.tpStopInvisible.TabIndex = 4;
			this.tpStopInvisible.Text = "StopInvisible";
			//
			// tpStopWaitingForBGR
			//
			this.tpStopWaitingForBGR.BackColor = System.Drawing.Color.Black;
			this.tpStopWaitingForBGR.Controls.Add(this.picStopWaitingForBGR);
			this.tpStopWaitingForBGR.Controls.Add(this.lblStopWaitingForBGR);
			this.tpStopWaitingForBGR.ForeColor = System.Drawing.Color.Yellow;
			this.tpStopWaitingForBGR.Location = new System.Drawing.Point(4, 24);
			this.tpStopWaitingForBGR.Name = "tpStopWaitingForBGR";
			this.tpStopWaitingForBGR.Padding = new System.Windows.Forms.Padding(3);
			this.tpStopWaitingForBGR.Size = new System.Drawing.Size(538, 26);
			this.tpStopWaitingForBGR.TabIndex = 5;
			this.tpStopWaitingForBGR.Text = "StopWaitingForBGR";
			//
			// picStopWaitingForBGR
			//
			this.picStopWaitingForBGR.Image = global::DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.ImageResources.Loading;
			this.picStopWaitingForBGR.Location = new System.Drawing.Point(251, 0);
			this.picStopWaitingForBGR.Name = "picStopWaitingForBGR";
			this.picStopWaitingForBGR.Size = new System.Drawing.Size(36, 36);
			this.picStopWaitingForBGR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picStopWaitingForBGR.TabIndex = 3;
			this.picStopWaitingForBGR.TabStop = false;
			//
			// lblStopWaitingForBGR
			//
			this.lblStopWaitingForBGR.AutoSize = true;
			this.lblStopWaitingForBGR.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
			this.lblStopWaitingForBGR.Location = new System.Drawing.Point(6, 12);
			this.lblStopWaitingForBGR.Name = "lblStopWaitingForBGR";
			this.lblStopWaitingForBGR.Size = new System.Drawing.Size(235, 16);
			this.lblStopWaitingForBGR.TabIndex = 2;
			this.lblStopWaitingForBGR.Text = "Waiting for BGR";
			//
			// lblStopMode
			//
			this.lblStopMode.AutoSize = true;
			this.lblStopMode.DisabledColor = System.Drawing.Color.Gray;
			this.lblStopMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblStopMode.ForeColor = System.Drawing.Color.Yellow;
			this.lblStopMode.Location = new System.Drawing.Point(1, 9);
			this.lblStopMode.Name = "lblStopMode";
			this.lblStopMode.Size = new System.Drawing.Size(120, 20);
			this.lblStopMode.TabIndex = 98;
			this.lblStopMode.Text = "Stop mode:";
			//
			// txtCommentaires
			//
			this.txtCommentaires.Enabled = false;
			this.txtCommentaires.Location = new System.Drawing.Point(13, 447);
			this.txtCommentaires.Multiline = true;
			this.txtCommentaires.Name = "txtCommentaires";
			this.txtCommentaires.Size = new System.Drawing.Size(509, 58);
			this.txtCommentaires.TabIndex = 73;
			//
			// pnlValidation
			//
			this.pnlValidation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.pnlValidation.Controls.Add(this.imgSaveValidation);
			this.pnlValidation.Controls.Add(this.lblMsgValidation);
			this.pnlValidation.Controls.Add(this.rdInvalide);
			this.pnlValidation.Controls.Add(this.lblCommentsValidation);
			this.pnlValidation.Controls.Add(this.rdValide);
			this.pnlValidation.Location = new System.Drawing.Point(8, 391);
			this.pnlValidation.Name = "pnlValidation";
			this.pnlValidation.Size = new System.Drawing.Size(634, 125);
			this.pnlValidation.TabIndex = 111;
			//
			// imgSaveValidation
			//
			this.imgSaveValidation.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.imgSaveValidation.BackgroundImage = global::DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.ImageResources.SaveButton;
			this.imgSaveValidation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.imgSaveValidation.Cursor = System.Windows.Forms.Cursors.Hand;
			this.imgSaveValidation.Location = new System.Drawing.Point(549, 22);
			this.imgSaveValidation.Name = "imgSaveValidation";
			this.imgSaveValidation.Size = new System.Drawing.Size(82, 82);
			this.imgSaveValidation.TabIndex = 102;
			this.imgSaveValidation.TabStop = false;
			this.staticToolTips.SetToolTip(this.imgSaveValidation, "Validate acquisition");
			//
			// lblMsgValidation
			//
			this.lblMsgValidation.AutoSize = true;
			this.lblMsgValidation.DisabledColor = System.Drawing.Color.Gray;
			this.lblMsgValidation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblMsgValidation.ForeColor = System.Drawing.Color.Yellow;
			this.lblMsgValidation.Location = new System.Drawing.Point(4, 10);
			this.lblMsgValidation.Name = "lblMsgValidation";
			this.lblMsgValidation.Size = new System.Drawing.Size(100, 20);
			this.lblMsgValidation.TabIndex = 70;
			this.lblMsgValidation.Text = "Acquisition is ";
			//
			// rdInvalide
			//
			this.rdInvalide.AutoSize = true;
			this.rdInvalide.DisabledColor = System.Drawing.Color.Gray;
			this.rdInvalide.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rdInvalide.ForeColor = System.Drawing.Color.Yellow;
			this.rdInvalide.Location = new System.Drawing.Point(195, 8);
			this.rdInvalide.Name = "rdInvalide";
			this.rdInvalide.Size = new System.Drawing.Size(97, 24);
			this.rdInvalide.TabIndex = 72;
			this.rdInvalide.Text = "              ";
			this.rdInvalide.TextCustom = "Invalid";
			this.rdInvalide.UseVisualStyleBackColor = true;
			//
			// lblCommentsValidation
			//
			this.lblCommentsValidation.AutoSize = true;
			this.lblCommentsValidation.DisabledColor = System.Drawing.Color.Gray;
			this.lblCommentsValidation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCommentsValidation.ForeColor = System.Drawing.Color.Yellow;
			this.lblCommentsValidation.Location = new System.Drawing.Point(3, 33);
			this.lblCommentsValidation.Name = "lblCommentsValidation";
			this.lblCommentsValidation.Size = new System.Drawing.Size(134, 20);
			this.lblCommentsValidation.TabIndex = 53;
			this.lblCommentsValidation.Text = "Comments:";
			//
			// rdValide
			//
			this.rdValide.AutoSize = true;
			this.rdValide.Checked = true;
			this.rdValide.DisabledColor = System.Drawing.Color.Gray;
			this.rdValide.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rdValide.ForeColor = System.Drawing.Color.Yellow;
			this.rdValide.Location = new System.Drawing.Point(112, 8);
			this.rdValide.Name = "rdValide";
			this.rdValide.Size = new System.Drawing.Size(87, 24);
			this.rdValide.TabIndex = 71;
			this.rdValide.TabStop = true;
			this.rdValide.Text = "            ";
			this.rdValide.TextCustom = "Valid";
			this.rdValide.UseVisualStyleBackColor = true;
			//
			// pnlMessageStartMode
			//
			this.pnlMessageStartMode.Location = new System.Drawing.Point(1, 36);
			this.pnlMessageStartMode.Name = "pnlMessageStartMode";
			this.pnlMessageStartMode.Size = new System.Drawing.Size(529, 42);
			this.pnlMessageStartMode.TabIndex = 102;
			//
			// AcquisitionManagerContextMenu
			//
			this.AcquisitionManagerContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem});
			this.AcquisitionManagerContextMenu.Name = "contextMenuStrip1";
			this.AcquisitionManagerContextMenu.Size = new System.Drawing.Size(212, 26);
			//
			// testToolStripMenuItem
			//
			this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AllMenuItem,
            this.toolStripMenuItem1,
            this.StartInfoMenuItem,
            this.CurrentRtssMenuItem,
            this.NextRtssMenuItem});
			this.testToolStripMenuItem.Name = "testToolStripMenuItem";
			this.testToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.testToolStripMenuItem.Text = "Copy to clipboard";
			//
			// AllMenuItem
			//
			this.AllMenuItem.Name = "AllMenuItem";
			this.AllMenuItem.Size = new System.Drawing.Size(219, 22);
			this.AllMenuItem.Text = "Everything";
			//
			// toolStripMenuItem1
			//
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(216, 6);
			//
			// StartInfoMenuItem
			//
			this.StartInfoMenuItem.Name = "StartInfoMenuItem";
			this.StartInfoMenuItem.Size = new System.Drawing.Size(219, 22);
			this.StartInfoMenuItem.Text = "Start/stop information";
			//
			// CurrentRtssMenuItem
			//
			this.CurrentRtssMenuItem.Name = "CurrentRtssMenuItem";
			this.CurrentRtssMenuItem.Size = new System.Drawing.Size(219, 22);
			this.CurrentRtssMenuItem.Text = "Current RTSSC";
			//
			// NextRtssMenuItem
			//
			this.NextRtssMenuItem.Name = "NextRtssMenuItem";
			this.NextRtssMenuItem.Size = new System.Drawing.Size(219, 22);
			this.NextRtssMenuItem.Text = "Next RTSSC";
			//
			// tcStartAcqui
			//
			this.tcStartAcqui.Appearance = System.Windows.Forms.TabAppearance.Buttons;
			this.tcStartAcqui.Controls.Add(this.tpStartRtssDdls);
			this.tcStartAcqui.Controls.Add(this.tpStartInvisible);
			this.tcStartAcqui.Controls.Add(this.tpStartWaitingForBGR);
			this.tcStartAcqui.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
			this.tcStartAcqui.ItemSize = new System.Drawing.Size(75, 20);
			this.tcStartAcqui.Location = new System.Drawing.Point(-5, 66);
			this.tcStartAcqui.Name = "tcStartAcqui";
			this.tcStartAcqui.SelectedIndex = 0;
			this.tcStartAcqui.Size = new System.Drawing.Size(546, 50);
			this.tcStartAcqui.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tcStartAcqui.TabIndex = 113;
			this.tcStartAcqui.TabStop = false;
			//
			// tpStartRtssDdls
			//
			this.tpStartRtssDdls.BackColor = System.Drawing.Color.Black;
			this.tpStartRtssDdls.Controls.Add(this.ddlSousRouteStart);
			this.tpStartRtssDdls.Controls.Add(this.ddlSectionStart);
			this.tpStartRtssDdls.Controls.Add(this.ddlTronconStart);
			this.tpStartRtssDdls.Controls.Add(this.ddlRouteStart);
			this.tpStartRtssDdls.Controls.Add(this.mtxtChainageSelectionStart);
			this.tpStartRtssDdls.Controls.Add(this.imgGetCurrentRTSSStart);
			this.tpStartRtssDdls.ForeColor = System.Drawing.Color.Yellow;
			this.tpStartRtssDdls.Location = new System.Drawing.Point(4, 24);
			this.tpStartRtssDdls.Name = "tpStartRtssDdls";
			this.tpStartRtssDdls.Padding = new System.Windows.Forms.Padding(3);
			this.tpStartRtssDdls.Size = new System.Drawing.Size(538, 22);
			this.tpStartRtssDdls.TabIndex = 1;
			this.tpStartRtssDdls.Text = "StartRtssDdls";
			//
			// ddlSousRouteStart
			//
			this.ddlSousRouteStart.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ddlSousRouteStart.AutoSize = false;
			this.ddlSousRouteStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ddlSousRouteStart.Location = new System.Drawing.Point(315, 6);
			this.ddlSousRouteStart.Name = "ddlSousRouteStart";
			this.ddlSousRouteStart.Size = new System.Drawing.Size(96, 26);
			this.ddlSousRouteStart.TabIndex = 120;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlSousRouteStart.GetChildAt(0))).DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDown;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlSousRouteStart.GetChildAt(0))).AutoSize = true;
			//
			// ddlSectionStart
			//
			this.ddlSectionStart.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ddlSectionStart.AutoSize = false;
			this.ddlSectionStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ddlSectionStart.Location = new System.Drawing.Point(213, 6);
			this.ddlSectionStart.Name = "ddlSectionStart";
			this.ddlSectionStart.Size = new System.Drawing.Size(96, 26);
			this.ddlSectionStart.TabIndex = 119;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlSectionStart.GetChildAt(0))).DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDown;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlSectionStart.GetChildAt(0))).AutoSize = true;
			//
			// ddlTronconStart
			//
			this.ddlTronconStart.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ddlTronconStart.AutoSize = false;
			this.ddlTronconStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ddlTronconStart.Location = new System.Drawing.Point(109, 6);
			this.ddlTronconStart.Name = "ddlTronconStart";
			this.ddlTronconStart.Size = new System.Drawing.Size(96, 26);
			this.ddlTronconStart.TabIndex = 118;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlTronconStart.GetChildAt(0))).DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDown;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlTronconStart.GetChildAt(0))).AutoSize = true;
			//
			// ddlRouteStart
			//
			this.ddlRouteStart.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ddlRouteStart.AutoSize = false;
			this.ddlRouteStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ddlRouteStart.Location = new System.Drawing.Point(7, 6);
			this.ddlRouteStart.Name = "ddlRouteStart";
			this.ddlRouteStart.Size = new System.Drawing.Size(96, 26);
			this.ddlRouteStart.TabIndex = 77;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlRouteStart.GetChildAt(0))).DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDown;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlRouteStart.GetChildAt(0))).AutoSize = true;
			//
			// mtxtChainageSelectionStart
			//
			this.mtxtChainageSelectionStart.HidePromptOnLeave = true;
			this.mtxtChainageSelectionStart.Location = new System.Drawing.Point(418, 6);
			this.mtxtChainageSelectionStart.Mask = "99990";
			this.mtxtChainageSelectionStart.Name = "mtxtChainageSelectionStart";
			this.mtxtChainageSelectionStart.PromptChar = ' ';
			this.mtxtChainageSelectionStart.Size = new System.Drawing.Size(78, 26);
			this.mtxtChainageSelectionStart.TabIndex = 34;
			this.mtxtChainageSelectionStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.staticToolTips.SetToolTip(this.mtxtChainageSelectionStart, "Chaînage");
			//
			// imgGetCurrentRTSSStart
			//
			this.imgGetCurrentRTSSStart.BackgroundImage = global::DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.ImageResources.Pinpoint;
			this.imgGetCurrentRTSSStart.Cursor = System.Windows.Forms.Cursors.Hand;
			this.imgGetCurrentRTSSStart.Location = new System.Drawing.Point(502, 6);
			this.imgGetCurrentRTSSStart.Name = "imgGetCurrentRTSSStart";
			this.imgGetCurrentRTSSStart.Size = new System.Drawing.Size(29, 28);
			this.imgGetCurrentRTSSStart.TabIndex = 93;
			this.imgGetCurrentRTSSStart.TabStop = false;
			this.staticToolTips.SetToolTip(this.imgGetCurrentRTSSStart, "Current RTSSC");
			//
			// tpStartInvisible
			//
			this.tpStartInvisible.BackColor = System.Drawing.Color.Black;
			this.tpStartInvisible.ForeColor = System.Drawing.Color.Yellow;
			this.tpStartInvisible.Location = new System.Drawing.Point(4, 24);
			this.tpStartInvisible.Name = "tpStartInvisible";
			this.tpStartInvisible.Padding = new System.Windows.Forms.Padding(3);
			this.tpStartInvisible.Size = new System.Drawing.Size(538, 22);
			this.tpStartInvisible.TabIndex = 3;
			this.tpStartInvisible.Text = "StartInvisible";
			//
			// tpStartWaitingForBGR
			//
			this.tpStartWaitingForBGR.BackColor = System.Drawing.Color.Black;
			this.tpStartWaitingForBGR.Controls.Add(this.picStartWaitingForBGR);
			this.tpStartWaitingForBGR.Controls.Add(this.lblStartWaitingForBGR);
			this.tpStartWaitingForBGR.ForeColor = System.Drawing.Color.Yellow;
			this.tpStartWaitingForBGR.Location = new System.Drawing.Point(4, 24);
			this.tpStartWaitingForBGR.Name = "tpStartWaitingForBGR";
			this.tpStartWaitingForBGR.Padding = new System.Windows.Forms.Padding(3);
			this.tpStartWaitingForBGR.Size = new System.Drawing.Size(538, 22);
			this.tpStartWaitingForBGR.TabIndex = 4;
			this.tpStartWaitingForBGR.Text = "StartWaitingForBGR";
			//
			// picStartWaitingForBGR
			//
			this.picStartWaitingForBGR.Image = global::DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.ImageResources.Loading;
			this.picStartWaitingForBGR.Location = new System.Drawing.Point(251, -1);
			this.picStartWaitingForBGR.Name = "picStartWaitingForBGR";
			this.picStartWaitingForBGR.Size = new System.Drawing.Size(36, 36);
			this.picStartWaitingForBGR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picStartWaitingForBGR.TabIndex = 1;
			this.picStartWaitingForBGR.TabStop = false;
			//
			// lblStartWaitingForBGR
			//
			this.lblStartWaitingForBGR.AutoSize = true;
			this.lblStartWaitingForBGR.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
			this.lblStartWaitingForBGR.Location = new System.Drawing.Point(6, 11);
			this.lblStartWaitingForBGR.Name = "lblStartWaitingForBGR";
			this.lblStartWaitingForBGR.Size = new System.Drawing.Size(235, 16);
			this.lblStartWaitingForBGR.TabIndex = 0;
			this.lblStartWaitingForBGR.Text = "Waiting for BGR";
			//
			// pnlStartDirection
			//
			this.pnlStartDirection.Controls.Add(this.lblStartDirection);
			this.pnlStartDirection.Controls.Add(this.rdStartDirection2);
			this.pnlStartDirection.Controls.Add(this.rdStartDirection1);
			this.pnlStartDirection.Location = new System.Drawing.Point(326, 32);
			this.pnlStartDirection.Name = "pnlStartDirection";
			this.pnlStartDirection.Size = new System.Drawing.Size(215, 27);
			this.pnlStartDirection.TabIndex = 112;
			//
			// lblStartDirection
			//
			this.lblStartDirection.AutoSize = true;
			this.lblStartDirection.DisabledColor = System.Drawing.Color.Gray;
			this.lblStartDirection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblStartDirection.ForeColor = System.Drawing.Color.Yellow;
			this.lblStartDirection.Location = new System.Drawing.Point(7, 6);
			this.lblStartDirection.Name = "lblStartDirection";
			this.lblStartDirection.Size = new System.Drawing.Size(78, 16);
			this.lblStartDirection.TabIndex = 22;
			this.lblStartDirection.Text = "Direction:";
			//
			// rdStartDirection2
			//
			this.rdStartDirection2.AutoSize = true;
			this.rdStartDirection2.DisabledColor = System.Drawing.Color.Gray;
			this.rdStartDirection2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rdStartDirection2.ForeColor = System.Drawing.Color.Yellow;
			this.rdStartDirection2.Location = new System.Drawing.Point(152, 4);
			this.rdStartDirection2.Name = "rdStartDirection2";
			this.rdStartDirection2.Size = new System.Drawing.Size(54, 20);
			this.rdStartDirection2.TabIndex = 24;
			this.rdStartDirection2.TabStop = true;
			this.rdStartDirection2.Text = "       ";
			this.rdStartDirection2.TextCustom = "2";
			this.rdStartDirection2.UseVisualStyleBackColor = true;
			//
			// rdStartDirection1
			//
			this.rdStartDirection1.AutoSize = true;
			this.rdStartDirection1.Checked = true;
			this.rdStartDirection1.DisabledColor = System.Drawing.Color.Gray;
			this.rdStartDirection1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rdStartDirection1.ForeColor = System.Drawing.Color.Yellow;
			this.rdStartDirection1.Location = new System.Drawing.Point(92, 4);
			this.rdStartDirection1.Name = "rdStartDirection1";
			this.rdStartDirection1.Size = new System.Drawing.Size(54, 20);
			this.rdStartDirection1.TabIndex = 23;
			this.rdStartDirection1.TabStop = true;
			this.rdStartDirection1.Text = "       ";
			this.rdStartDirection1.TextCustom = "1";
			this.rdStartDirection1.UseVisualStyleBackColor = true;
			//
			// pnlStartAcqui
			//
			this.pnlStartAcqui.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnlStartAcqui.Controls.Add(this.ddlStartMode);
			this.pnlStartAcqui.Controls.Add(this.ddlProximityStart);
			this.pnlStartAcqui.Controls.Add(this.pnlStartDirection);
			this.pnlStartAcqui.Controls.Add(this.tcStartAcqui);
			this.pnlStartAcqui.Controls.Add(this.imgStartAcquisition);
			this.pnlStartAcqui.Controls.Add(this.lblStartMode);
			this.pnlStartAcqui.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.pnlStartAcqui.ForeColor = System.Drawing.Color.Red;
			this.pnlStartAcqui.Location = new System.Drawing.Point(8, 129);
			this.pnlStartAcqui.Name = "pnlStartAcqui";
			this.pnlStartAcqui.Size = new System.Drawing.Size(634, 123);
			this.pnlStartAcqui.TabIndex = 89;
			//
			// ddlStartMode
			//
			this.ddlStartMode.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ddlStartMode.AutoSize = false;
			this.ddlStartMode.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
			this.ddlStartMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ddlStartMode.Location = new System.Drawing.Point(5, 32);
			this.ddlStartMode.Name = "ddlStartMode";
			this.ddlStartMode.Size = new System.Drawing.Size(248, 28);
			this.ddlStartMode.TabIndex = 121;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlStartMode.GetChildAt(0))).DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlStartMode.GetChildAt(0))).AutoSize = true;
			//
			// ddlProximityStart
			//
			this.ddlProximityStart.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ddlProximityStart.AutoSize = false;
			this.ddlProximityStart.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
			this.ddlProximityStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ddlProximityStart.Location = new System.Drawing.Point(259, 32);
			this.ddlProximityStart.Name = "ddlProximityStart";
			this.ddlProximityStart.Size = new System.Drawing.Size(63, 28);
			this.ddlProximityStart.TabIndex = 120;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlProximityStart.GetChildAt(0))).DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
			((Telerik.WinControls.UI.RadDropDownListElement)(this.ddlProximityStart.GetChildAt(0))).AutoSize = true;
			//
			// imgStartAcquisition
			//
			this.imgStartAcquisition.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.imgStartAcquisition.BackgroundImage = global::DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI.ImageResources.SuccessButton;
			this.imgStartAcquisition.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.imgStartAcquisition.Cursor = System.Windows.Forms.Cursors.Hand;
			this.imgStartAcquisition.Location = new System.Drawing.Point(547, 18);
			this.imgStartAcquisition.Name = "imgStartAcquisition";
			this.imgStartAcquisition.Size = new System.Drawing.Size(82, 82);
			this.imgStartAcquisition.TabIndex = 86;
			this.imgStartAcquisition.TabStop = false;
			//
			// lblStartMode
			//
			this.lblStartMode.AutoSize = true;
			this.lblStartMode.DisabledColor = System.Drawing.Color.Gray;
			this.lblStartMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblStartMode.ForeColor = System.Drawing.Color.Yellow;
			this.lblStartMode.Location = new System.Drawing.Point(1, 9);
			this.lblStartMode.Name = "lblStartMode";
			this.lblStartMode.Size = new System.Drawing.Size(179, 20);
			this.lblStartMode.TabIndex = 88;
			this.lblStartMode.Text = "Start mode:";
			//
			// compactGuiToolTips
			//
			this.compactGuiToolTips.AutoPopDelay = 5000;
			this.compactGuiToolTips.InitialDelay = 200;
			this.compactGuiToolTips.ReshowDelay = 100;
			//
			// AcquisitionManagerUI
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.ClientSize = new System.Drawing.Size(649, 726);
			this.ContextMenuStrip = this.AcquisitionManagerContextMenu;
			this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.Location = new System.Drawing.Point(0, 0);
			this.MaximizeBox = false;
			this.Name = "AcquisitionManagerUI";
			this.Text = "Acquisition Manager";
			this.Controls.SetChildIndex(this.pnlGeneralInfos, 0);
			this.pnlGeneralInfos.ResumeLayout(false);
			this.pnlGeneralInfos.PerformLayout();
			this.pnlPrepareAcqui.ResumeLayout(false);
			this.pnlPrepareAcqui.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.imgDeleteOperator)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imgDeleteDriver)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlSequenceTypes)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlVehicles)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlOperators)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlDrivers)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imgAddOperator)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imgAddDriver)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imgPrepareAcquisition)).EndInit();
			this.pnlStopAcqui.ResumeLayout(false);
			this.pnlStopAcqui.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.ddlStopMode)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlProximityStop)).EndInit();
			this.pnlStopDirection.ResumeLayout(false);
			this.pnlStopDirection.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.imgStopAcquisition)).EndInit();
			this.tcStopAcqui.ResumeLayout(false);
			this.tpStopDistance.ResumeLayout(false);
			this.tpStopDistance.PerformLayout();
			this.tpStopRtssDdls.ResumeLayout(false);
			this.tpStopRtssDdls.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.ddlSousRouteStop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlSectionStop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlRouteStop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlTronconStop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imgGetCurrentRTSSStop)).EndInit();
			this.tpStopWaitingForBGR.ResumeLayout(false);
			this.tpStopWaitingForBGR.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.picStopWaitingForBGR)).EndInit();
			this.pnlValidation.ResumeLayout(false);
			this.pnlValidation.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.imgSaveValidation)).EndInit();
			this.AcquisitionManagerContextMenu.ResumeLayout(false);
			this.tcStartAcqui.ResumeLayout(false);
			this.tpStartRtssDdls.ResumeLayout(false);
			this.tpStartRtssDdls.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.ddlSousRouteStart)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlSectionStart)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlTronconStart)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlRouteStart)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imgGetCurrentRTSSStart)).EndInit();
			this.tpStartWaitingForBGR.ResumeLayout(false);
			this.tpStartWaitingForBGR.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.picStartWaitingForBGR)).EndInit();
			this.pnlStartDirection.ResumeLayout(false);
			this.pnlStartDirection.PerformLayout();
			this.pnlStartAcqui.ResumeLayout(false);
			this.pnlStartAcqui.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.ddlStartMode)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ddlProximityStart)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imgStartAcquisition)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private CustomLabel lblCommentsValidation;
		private System.Windows.Forms.PictureBox imgPrepareAcquisition;
		private System.Windows.Forms.Panel pnlPrepareAcqui;
		private System.Windows.Forms.Panel pnlStopAcqui;
		private System.Windows.Forms.PictureBox imgStopAcquisition;
		private System.Windows.Forms.PictureBox imgSaveValidation;
		private System.Windows.Forms.PictureBox imgAddOperator;
		private System.Windows.Forms.PictureBox imgAddDriver;
		private CustomLabel lblDriver;
		private CustomLabel lblStopMode;
		private System.Windows.Forms.TextBox txtCommentaires;
		private CustomRadioButton rdInvalide;
		private CustomRadioButton rdValide;
		private CustomLabel lblMsgValidation;
		private System.Windows.Forms.Panel pnlValidation;
		private CustomLabel lblSequenceType;
		private CustomLabel lblVehicle;
		private CustomLabel lblOperator;
		private System.Windows.Forms.PictureBox imgGetCurrentRTSSStop;
		private System.Windows.Forms.TextBox txtDistanceStop;
		private CustomLabel lblDistanceStop;
		private System.Windows.Forms.Panel pnlMessageStartMode;
		private System.Windows.Forms.Panel pnlStopDirection;
		private CustomLabel lblStopDirection;
		private CustomRadioButton rdStopDirection2;
		private CustomRadioButton rdStopDirection1;
		private System.Windows.Forms.ContextMenuStrip AcquisitionManagerContextMenu;
		private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AllMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem StartInfoMenuItem;
		private System.Windows.Forms.ToolStripMenuItem CurrentRtssMenuItem;
		private System.Windows.Forms.ToolStripMenuItem NextRtssMenuItem;
		private System.Windows.Forms.TabControl tcStopAcqui;
		private System.Windows.Forms.TabPage tpStopDistance;
		private System.Windows.Forms.TabPage tpStopRtssDdls;
		private System.Windows.Forms.TabPage tpStopInvisible;
		private System.Windows.Forms.TabPage tpStopWaitingForBGR;
		private System.Windows.Forms.PictureBox picStopWaitingForBGR;
		private System.Windows.Forms.Label lblStopWaitingForBGR;
		private System.Windows.Forms.MaskedTextBox mtxtChainageSelectionStop;
		private CustomLabel lblStartMode;
		private System.Windows.Forms.PictureBox imgStartAcquisition;
		private System.Windows.Forms.TabControl tcStartAcqui;
		private System.Windows.Forms.TabPage tpStartRtssDdls;
		private System.Windows.Forms.MaskedTextBox mtxtChainageSelectionStart;
		private System.Windows.Forms.PictureBox imgGetCurrentRTSSStart;
		private System.Windows.Forms.TabPage tpStartInvisible;
		private System.Windows.Forms.TabPage tpStartWaitingForBGR;
		private System.Windows.Forms.PictureBox picStartWaitingForBGR;
		private System.Windows.Forms.Label lblStartWaitingForBGR;
		private System.Windows.Forms.Panel pnlStartDirection;
		private CustomLabel lblStartDirection;
		private CustomRadioButton rdStartDirection2;
		private CustomRadioButton rdStartDirection1;
		private System.Windows.Forms.Panel pnlStartAcqui;
		private System.Windows.Forms.ToolTip compactGuiToolTips;
		private System.Windows.Forms.ToolTip staticToolTips;
		private Telerik.WinControls.UI.RadDropDownList ddlSequenceTypes;
		private Telerik.WinControls.UI.RadDropDownList ddlVehicles;
		private Telerik.WinControls.UI.RadDropDownList ddlOperators;
		private Telerik.WinControls.UI.RadDropDownList ddlDrivers;
		private RadDropDownList ddlRouteStart;
		private RadDropDownList ddlSousRouteStart;
		private RadDropDownList ddlSectionStart;
		private RadDropDownList ddlTronconStart;
		private RadDropDownList ddlSousRouteStop;
		private RadDropDownList ddlSectionStop;
		private RadDropDownList ddlRouteStop;
		private RadDropDownList ddlTronconStop;
		private RadDropDownList ddlProximityStart;
		private RadDropDownList ddlStopMode;
		private RadDropDownList ddlProximityStop;
		private RadDropDownList ddlStartMode;
		private System.Windows.Forms.PictureBox imgDeleteOperator;
		private System.Windows.Forms.PictureBox imgDeleteDriver;
	}
}