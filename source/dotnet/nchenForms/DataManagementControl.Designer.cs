namespace System.Windows.Forms
{
    partial class DataManagementControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.StatusPanel = new System.Windows.Forms.Panel();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.TitlePanel = new System.Windows.Forms.Panel();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.TSBAddNew = new System.Windows.Forms.ToolStripButton();
            this.TSBEdit = new System.Windows.Forms.ToolStripButton();
            this.TSBDelete = new System.Windows.Forms.ToolStripButton();
            this.AddEditDeleteTSS = new System.Windows.Forms.ToolStripSeparator();
            this.TSBAccept = new System.Windows.Forms.ToolStripButton();
            this.TSBCancel = new System.Windows.Forms.ToolStripButton();
            this.AcceptCancelTSS = new System.Windows.Forms.ToolStripSeparator();
            this.TSBRefresh = new System.Windows.Forms.ToolStripButton();
            this.RefreshTSS = new System.Windows.Forms.ToolStripSeparator();
            this.TSBPrint = new System.Windows.Forms.ToolStripButton();
            this.TSBPrintPreview = new System.Windows.Forms.ToolStripButton();
            this.TSBExport = new System.Windows.Forms.ToolStripButton();
            this.ToolbarTs = new System.Windows.Forms.ToolStrip();
            this.StatusPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.ToolbarTs.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusPanel
            // 
            this.StatusPanel.AutoSize = true;
            this.StatusPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.StatusPanel.Controls.Add(this.StatusLabel);
            this.StatusPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.StatusPanel.Location = new System.Drawing.Point(0, 379);
            this.StatusPanel.Margin = new System.Windows.Forms.Padding(2);
            this.StatusPanel.Name = "StatusPanel";
            this.StatusPanel.Size = new System.Drawing.Size(600, 21);
            this.StatusPanel.TabIndex = 9;
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(2, 2);
            this.StatusLabel.Margin = new System.Windows.Forms.Padding(2);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(0, 13);
            this.StatusLabel.TabIndex = 2;
            // 
            // TitlePanel
            // 
            this.TitlePanel.AutoSize = true;
            this.TitlePanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.TitlePanel.Controls.Add(this.TitleLabel);
            this.TitlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TitlePanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitlePanel.Location = new System.Drawing.Point(0, 0);
            this.TitlePanel.Name = "TitlePanel";
            this.TitlePanel.Size = new System.Drawing.Size(600, 23);
            this.TitlePanel.TabIndex = 8;
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.TitleLabel.Location = new System.Drawing.Point(5, 5);
            this.TitleLabel.Margin = new System.Windows.Forms.Padding(5);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(32, 13);
            this.TitleLabel.TabIndex = 0;
            this.TitleLabel.Text = "Title";
            this.TitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TSBAddNew
            // 
            this.TSBAddNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBAddNew.Image = global::System.Windows.Forms.Properties.Resources.AddNew;
            this.TSBAddNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBAddNew.Name = "TSBAddNew";
            this.TSBAddNew.Size = new System.Drawing.Size(24, 22);
            this.TSBAddNew.Text = "Add New";
            this.TSBAddNew.Click += new System.EventHandler(this.TSBAddNew_Click);
            // 
            // TSBEdit
            // 
            this.TSBEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBEdit.Image = global::System.Windows.Forms.Properties.Resources.Edit;
            this.TSBEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBEdit.Name = "TSBEdit";
            this.TSBEdit.Size = new System.Drawing.Size(24, 22);
            this.TSBEdit.Text = "Edit";
            this.TSBEdit.Click += new System.EventHandler(this.TSBEdit_Click);
            // 
            // TSBDelete
            // 
            this.TSBDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBDelete.Image = global::System.Windows.Forms.Properties.Resources.Cancel;
            this.TSBDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBDelete.Name = "TSBDelete";
            this.TSBDelete.Size = new System.Drawing.Size(24, 22);
            this.TSBDelete.Text = "Delete";
            this.TSBDelete.Click += new System.EventHandler(this.TSBDelete_Click);
            // 
            // AddEditDeleteTSS
            // 
            this.AddEditDeleteTSS.Name = "AddEditDeleteTSS";
            this.AddEditDeleteTSS.Size = new System.Drawing.Size(6, 25);
            // 
            // TSBAccept
            // 
            this.TSBAccept.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBAccept.Enabled = false;
            this.TSBAccept.Image = global::System.Windows.Forms.Properties.Resources.Accept;
            this.TSBAccept.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBAccept.Name = "TSBAccept";
            this.TSBAccept.Size = new System.Drawing.Size(24, 22);
            this.TSBAccept.Text = "Accept";
            this.TSBAccept.Click += new System.EventHandler(this.TSBAccept_Click);
            // 
            // TSBCancel
            // 
            this.TSBCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBCancel.Enabled = false;
            this.TSBCancel.Image = global::System.Windows.Forms.Properties.Resources.Cancel;
            this.TSBCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBCancel.Name = "TSBCancel";
            this.TSBCancel.Size = new System.Drawing.Size(24, 22);
            this.TSBCancel.Text = "Cancel";
            this.TSBCancel.Click += new System.EventHandler(this.TSBCancel_Click);
            // 
            // AcceptCancelTSS
            // 
            this.AcceptCancelTSS.Name = "AcceptCancelTSS";
            this.AcceptCancelTSS.Size = new System.Drawing.Size(6, 25);
            // 
            // TSBRefresh
            // 
            this.TSBRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBRefresh.Image = global::System.Windows.Forms.Properties.Resources.Refresh;
            this.TSBRefresh.Name = "TSBRefresh";
            this.TSBRefresh.Size = new System.Drawing.Size(24, 22);
            this.TSBRefresh.Text = "Refresh";
            this.TSBRefresh.Click += new System.EventHandler(this.TSBRefresh_Click);
            // 
            // RefreshTSS
            // 
            this.RefreshTSS.Name = "RefreshTSS";
            this.RefreshTSS.Size = new System.Drawing.Size(6, 25);
            // 
            // TSBPrint
            // 
            this.TSBPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBPrint.Image = global::System.Windows.Forms.Properties.Resources.Print;
            this.TSBPrint.Name = "TSBPrint";
            this.TSBPrint.Size = new System.Drawing.Size(24, 22);
            this.TSBPrint.Text = "Print";
            this.TSBPrint.Click += new System.EventHandler(this.TSBPrint_Click);
            // 
            // TSBPrintPreview
            // 
            this.TSBPrintPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBPrintPreview.Image = global::System.Windows.Forms.Properties.Resources.PrintPreview;
            this.TSBPrintPreview.Name = "TSBPrintPreview";
            this.TSBPrintPreview.Size = new System.Drawing.Size(24, 22);
            this.TSBPrintPreview.Text = "PrintPreview";
            this.TSBPrintPreview.Click += new System.EventHandler(this.TSBPrintPreview_Click);
            // 
            // TSBExport
            // 
            this.TSBExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBExport.Image = global::System.Windows.Forms.Properties.Resources.ExportCSV;
            this.TSBExport.Name = "TSBExport";
            this.TSBExport.Size = new System.Drawing.Size(24, 22);
            this.TSBExport.Text = "Export";
            this.TSBExport.Click += new System.EventHandler(this.TSBExport_Click);
            // 
            // ToolbarTs
            // 
            this.ToolbarTs.AutoSize = false;
            this.ToolbarTs.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolbarTs.ImageScalingSize = new System.Drawing.Size(20, 25);
            this.ToolbarTs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSBAddNew,
            this.TSBEdit,
            this.TSBDelete,
            this.AddEditDeleteTSS,
            this.TSBAccept,
            this.TSBCancel,
            this.AcceptCancelTSS,
            this.TSBRefresh,
            this.RefreshTSS,
            this.TSBPrint,
            this.TSBPrintPreview,
            this.TSBExport});
            this.ToolbarTs.Location = new System.Drawing.Point(0, 23);
            this.ToolbarTs.Name = "ToolbarTs";
            this.ToolbarTs.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.ToolbarTs.Size = new System.Drawing.Size(600, 25);
            this.ToolbarTs.TabIndex = 10;
            this.ToolbarTs.Text = "ToolStrip1";
            // 
            // DataManagementControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ToolbarTs);
            this.Controls.Add(this.StatusPanel);
            this.Controls.Add(this.TitlePanel);
            this.Name = "DataManagementControl";
            this.Size = new System.Drawing.Size(600, 400);
            this.Load += new System.EventHandler(this.DataManagementControl_Load);
            this.StatusPanel.ResumeLayout(false);
            this.StatusPanel.PerformLayout();
            this.TitlePanel.ResumeLayout(false);
            this.TitlePanel.PerformLayout();
            this.ToolbarTs.ResumeLayout(false);
            this.ToolbarTs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal System.Windows.Forms.Panel StatusPanel;
        internal System.Windows.Forms.Label StatusLabel;
        internal System.Windows.Forms.Panel TitlePanel;
        internal System.Windows.Forms.Label TitleLabel;
        protected System.Windows.Forms.ToolStripButton TSBAddNew;
        protected System.Windows.Forms.ToolStripButton TSBEdit;
        protected System.Windows.Forms.ToolStripButton TSBDelete;
        internal System.Windows.Forms.ToolStripSeparator AddEditDeleteTSS;
        protected System.Windows.Forms.ToolStripButton TSBAccept;
        protected System.Windows.Forms.ToolStripButton TSBCancel;
        internal System.Windows.Forms.ToolStripSeparator AcceptCancelTSS;
        protected System.Windows.Forms.ToolStripButton TSBRefresh;
        internal System.Windows.Forms.ToolStripSeparator RefreshTSS;
        protected System.Windows.Forms.ToolStripButton TSBPrint;
        protected System.Windows.Forms.ToolStripButton TSBPrintPreview;
        protected System.Windows.Forms.ToolStripButton TSBExport;
        internal System.Windows.Forms.ToolStrip ToolbarTs;
    }
}
