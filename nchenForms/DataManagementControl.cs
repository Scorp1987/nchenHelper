using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms.Enums;

namespace System.Windows.Forms
{
    public delegate void CustomEventHandler(DataManagementControl sender, EventArgs e);
    public delegate void CustomEventHandler<T>(DataManagementControl sender, T e) where T : EventArgs;

    public partial class DataManagementControl : UserControl
    {
        protected DataManagementControl()
        {
            InitializeComponent();
        }

        #region Data Events
        [Category("Data")]
        public event CustomEventHandler<ChangingEventArgs<FormStatusType>> FormStatusChanging;

        [Category("Data")]
        public event CustomEventHandler<ChangedEventArgs<FormStatusType>> FormStatusChanged;

        [Category("Data")]
        public event CustomEventHandler<CancelEventArgs> RefreshDataEvent;

        [Category("Data")]
        public event CustomEventHandler<CancelEventArgs> ResetDataEvent;

        [Category("Data")]
        public event CustomEventHandler<CancelEventArgs> LoadDataEvent;

        [Category("Data")]
        public event CustomEventHandler<CancelEventArgs> CheckDataEvent;

        [Category("Data")]
        public event CustomEventHandler<CancelEventArgs> AddDataEvent;

        [Category("Data")]
        public event CustomEventHandler<CancelEventArgs> EditDataEvent;

        [Category("Data")]
        public event CustomEventHandler<CancelEventArgs> DeleteConfirmationEvent;

        [Category("Data")]
        public event CustomEventHandler DeleteDataEvent;

        [Category("Data")]
        public event CustomEventHandler PrintEvent;

        [Category("Data")]
        public event CustomEventHandler PrintPreviewEvent;

        [Category("Data")]
        public event CustomEventHandler ExportEvent;
        #endregion


        private Font GetFont(float ratio, FontStyle style)
            => new Font(this.Font.FontFamily, this.Font.Size * ratio, style);
        private void UpdateAcceptCancelButtonVisible()
        {
            if (!ShowAddNew && !ShowEdit)
            {
                TSBAccept.Visible = false;
                TSBCancel.Visible = false;
                AcceptCancelTSS.Visible = false;
            }
        }
        private void UpdateAcceptCancelTSSVisible() => AcceptCancelTSS.Visible = TSBAccept.Visible | TSBCancel.Visible;
        private void UpdateAddEditDeleteTSSVisible()
            => AddEditDeleteTSS.Visible = ShowAddNew || ShowEdit || ShowDelete;
        private bool GetViewRelatedButtonEnabled(bool allow) => allow && AllowView && (FormStatus == FormStatusType.View);


        #region TitleProperties
        [Category("Title"), Description("Title of the Control."), DefaultValue("Title")]
        public string Title
        {
            get => TitleLabel.Text;
            set => TitleLabel.Text = value;
        }

        [Category("Title"), Description("Determine whether title is shown at the top of the control."), DefaultValue(true)]
        public bool ShowTitle
        {
            get => TitlePanel.Visible;
            set => TitlePanel.Visible = value;
        }

        private float _titleRatio = 1;
        [Category("Title"), Description("Determine font size ratio of the title."), DefaultValue(1f)]
        public float TitleRatio
        {
            get => _titleRatio;
            set
            {
                _titleRatio = value;
                TitleLabel.Font = GetFont(value, TitleStyle);
            }
        }

        [Category("Title"), Description("Determine font style of the title."), DefaultValue(FontStyle.Regular)]
        public FontStyle TitleStyle
        {
            get => TitleLabel.Font.Style;
            set => TitleLabel.Font = GetFont(TitleRatio, value);
        }

        [Category("Title"), Description("Determine backcolor of the title."), DefaultValue(typeof(SystemColors), "ActiveCaption")]
        public Color TitleBackColor
        {
            get => TitleLabel.BackColor;
            set
            {
                TitleLabel.BackColor = value;
                TitlePanel.BackColor = value;
            }
        }

        [Category("Title"), Description("Determine forecolor of the title."), DefaultValue(typeof(SystemColors), "ControlText")]
        public Color TitleForeColor
        {
            get => TitleLabel.ForeColor;
            set => TitleLabel.ForeColor = value;
        }
        #endregion


        #region StatusProperties
        [Category("Status"), Description("Get or Set the status of the control."), DefaultValue("")]
        public string Status
        {
            get => StatusLabel.Text;
            set => StatusLabel.Text = value;
        }

        [Category("Status"), Description("Determine whether status is shown at the bottom of the control."), DefaultValue(true)]
        public bool ShowStatus
        {
            get => StatusPanel.Visible;
            set => StatusPanel.Visible = value;
        }

        private float _statusRatio = 1;
        [Category("Status"), Description("Determine font size ratio of the status."), DefaultValue(1f)]
        public float StatusRatio
        {
            get => _statusRatio;
            set
            {
                _statusRatio = value;
                StatusLabel.Font = GetFont(value, StatusStyle);
            }
        }

        [Category("Status"), Description("Determine font style of the status."), DefaultValue(FontStyle.Regular)]
        public FontStyle StatusStyle
        {
            get => StatusLabel.Font.Style;
            set => StatusLabel.Font = GetFont(StatusRatio, StatusStyle);
        }

        [Category("Status"), Description("Determine backcolor of the status."), DefaultValue(typeof(SystemColors), "Control")]
        public Color StatusBackColor
        {
            get => StatusLabel.BackColor;
            set
            {
                StatusLabel.BackColor = value;
                StatusPanel.BackColor = value;
            }
        }

        [Category("Status"), Description("Determine forecolor of the status."), DefaultValue(typeof(SystemColors), "ControlText")]
        public Color StatusForeColor
        {
            get => StatusLabel.ForeColor;
            set => StatusLabel.ForeColor = value;
        }
        #endregion


        #region ToolbarProperties
        [Category("Toolbar"), Description("Determine whether Toolbar is shown"), DefaultValue(true)]
        public bool ShowToolbar
        {
            get => ToolbarTs.Visible;
            set => ToolbarTs.Visible = value;
        }

        [Category("Toolbar"), Description("Determine whether AddNew tool strip button is shown."), DefaultValue(true)]
        public bool ShowAddNew
        {
            get => TSBAddNew.Visible;
            set
            {
                TSBAddNew.Visible = value;
                UpdateAddEditDeleteTSSVisible();
                UpdateAcceptCancelButtonVisible();
            }
        }

        [Category("Toolbar"), Description("Determine whether Edit tool strip button is shown."), DefaultValue(true)]
        public bool ShowEdit
        {
            get => TSBEdit.Visible;
            set
            {
                TSBEdit.Visible = value;
                UpdateAddEditDeleteTSSVisible();
                UpdateAcceptCancelButtonVisible();
            }
        }

        [Category("Toolbar"), Description("Determine whether Delete tool strip button is shown."), DefaultValue(true)]
        public bool ShowDelete
        {
            get => TSBDelete.Visible;
            set
            {
                TSBDelete.Visible = value;
                UpdateAddEditDeleteTSSVisible();
            }
        }

        [Category("Toolbar"), Description("Determine whether Accept tool strip button is shown."), DefaultValue(true)]
        public bool ShowAccept
        {
            get => TSBAccept.Visible;
            set
            {
                TSBAccept.Visible = value;
                UpdateAcceptCancelTSSVisible();
            }
        }

        [Category("Toolbar"), Description("Determine whether Cancel tool strip button is shown."), DefaultValue(true)]
        public bool ShowCancel
        {
            get => TSBCancel.Visible;
            set
            {
                TSBCancel.Visible = value;
                UpdateAcceptCancelTSSVisible();
            }
        }

        [Category("Toolbar"), Description("Determine whether Refresh tool strip button is shown."), DefaultValue(true)]
        public bool ShowRefresh
        {
            get => TSBRefresh.Visible;
            set
            {
                TSBRefresh.Visible = value;
                RefreshTSS.Visible = value;
            }
        }

        [Category("Toolbar"), Description("Determine whether Export tool strip button is shown."), DefaultValue(true)]
        public bool ShowExport
        {
            get => TSBExport.Visible;
            set => TSBExport.Visible = value;
        }

        [Category("Toolbar"), Description("Determine whether Print tool strip button is shown."), DefaultValue(true)]
        public bool ShowPrint
        {
            get => TSBPrint.Visible;
            set => TSBPrint.Visible = value;
        }

        [Category("Toolbar"), Description("Determine whether PrintPreview tool strip button is shown."), DefaultValue(true)]
        public bool ShowPrintPreview
        {
            get => TSBPrintPreview.Visible;
            set => TSBPrintPreview.Visible = value;
        }


        [Category("Toolbar"), Description("AddNew tool strip button tool tip."), DefaultValue("AddNew")]
        public string AddNewToolTipText { get => TSBAddNew.ToolTipText; set => TSBAddNew.ToolTipText = value; }

        [Category("Toolbar"), Description("Edit tool strip button tool tip."), DefaultValue("Edit")]
        public string EditToolTipText { get => TSBEdit.ToolTipText; set => TSBEdit.ToolTipText = value; }

        [Category("Toolbar"), Description("Delete tool strip button tool tip."), DefaultValue("Delete")]
        public string DeleteToolTipText { get => TSBDelete.ToolTipText; set => TSBDelete.ToolTipText = value; }

        [Category("Toolbar"), Description("Refresh tool strip button tool tip."), DefaultValue("Refresh")]
        public string RefreshToolTipText { get => TSBRefresh.ToolTipText; set => TSBRefresh.ToolTipText = value; }

        [Category("Toolbar"), Description("Export tool strip button tool tip."), DefaultValue("Export")]
        public string ExportToolTipText { get => TSBExport.ToolTipText; set => TSBExport.ToolTipText = value; }

        [Category("Toolbar"), Description("Print tool strip button tool tip."), DefaultValue("Print")]
        public string PrintToolTipText { get => TSBPrint.ToolTipText; set => TSBPrint.ToolTipText = value; }

        [Category("Toolbar"), Description("Print Preview tool strip button tool tip."), DefaultValue("PrintPreview")]
        public string PrintPreviewToolTipText { get => TSBPrintPreview.ToolTipText; set => TSBPrintPreview.ToolTipText = value; }
        #endregion


        #region AccessProperties
        private bool _allowAddNew = true;
        [Category("Access"), Description("Allow to Add New Items."), DefaultValue(true)]
        public virtual bool AllowAddNew
        {
            get => _allowAddNew;
            set
            {
                _allowAddNew = value;
                TSBAddNew.Enabled = value && (this.FormStatus == FormStatusType.View);
            }
        }

        private bool _allowEdit = true;
        [Category("Access"), Description("Allow to Edit Items."), DefaultValue(true)]
        public virtual bool AllowEdit
        {
            get => _allowEdit;
            set
            {
                _allowEdit = value;
                TSBEdit.Enabled = GetViewRelatedButtonEnabled(value);
            }
        }

        private bool _allowDelete = true;
        [Category("Access"), Description("Allow to Delete Items."), DefaultValue(true)]
        public virtual bool AllowDelete
        {
            get => _allowDelete;
            set
            {
                _allowDelete = value;
                TSBDelete.Enabled = GetViewRelatedButtonEnabled(value);
            }
        }

        private bool _allowExport = true;
        [Category("Access"), Description("Allow to Export Items."), DefaultValue(true)]
        public virtual bool AllowExport
        {
            get => _allowExport;
            set
            {
                _allowExport = value;
                TSBExport.Enabled = GetViewRelatedButtonEnabled(value);
            }
        }

        private bool _allowPrint = true;
        [Category("Access"), Description("Allow to Print Items."), DefaultValue(true)]
        public virtual bool AllowPrint
        {
            get => _allowPrint;
            set
            {
                _allowPrint = value;
                TSBPrint.Enabled = GetViewRelatedButtonEnabled(value);
                TSBPrintPreview.Enabled = TSBPrint.Enabled;
            }
        }

        private bool _allowView = true;
        [Category("Access"), Description("Allow to View Items"), DefaultValue(true)]
        public virtual bool AllowView
        {
            get => _allowView;
            set
            {
                _allowView = value;
                TSBEdit.Enabled = GetViewRelatedButtonEnabled(AllowEdit);
                TSBDelete.Enabled = GetViewRelatedButtonEnabled(AllowDelete);
                TSBExport.Enabled = GetViewRelatedButtonEnabled(AllowExport);
                TSBPrint.Enabled = GetViewRelatedButtonEnabled(AllowPrint);
                TSBPrintPreview.Enabled = TSBPrint.Enabled;
            }
        }
        #endregion


        public virtual ILog Log { get; }


        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                TitleLabel.Font = GetFont(TitleRatio, TitleStyle);
                StatusLabel.Font = GetFont(StatusRatio, StatusStyle);
                int imageScale = (int)(base.Font.Size * 3);
                ToolbarTs.ImageScalingSize = new Size(imageScale, imageScale);
            }
        }

        private FormStatusType _formStatus = FormStatusType.View;
        [Category("Behavior"), Description("Current Form Status"), DefaultValue(FormStatusType.View)]
        public FormStatusType FormStatus
        {
            get => _formStatus;
            set
            {
                var arg = new ChangingEventArgs<FormStatusType>(this._formStatus, value);
                FormStatusChanging?.Invoke(this, arg);
                if (arg.Cancel) return;
                var oldStatus = _formStatus;
                _formStatus = value;
                this.RefreshControls();
                FormStatusChanged?.Invoke(this, new ChangedEventArgs<FormStatusType>(oldStatus, value));
            }
        }


        public void AddToolStripItem(ToolStripItem item) => ToolbarTs.Items.Add(item);
        public void RemoveToolStripItem(ToolStripItem item) => ToolbarTs.Items.Remove(item);


        public virtual void RefreshControls()
        {
            switch (FormStatus)
            {
                case FormStatusType.View:
                    TSBAddNew.Enabled = AllowAddNew;
                    TSBEdit.Enabled = GetViewRelatedButtonEnabled(AllowEdit);
                    TSBDelete.Enabled = GetViewRelatedButtonEnabled(AllowDelete);
                    TSBExport.Enabled = GetViewRelatedButtonEnabled(AllowExport);
                    TSBPrint.Enabled = GetViewRelatedButtonEnabled(AllowPrint);
                    TSBPrintPreview.Enabled = TSBPrint.Enabled;
                    TSBRefresh.Enabled = AllowView;
                    TSBAccept.Enabled = false;
                    TSBCancel.Enabled = false;
                    break;
                case FormStatusType.AddNew:
                case FormStatusType.Edit:
                    TSBAddNew.Enabled = false;
                    TSBEdit.Enabled = false;
                    TSBDelete.Enabled = false;
                    TSBExport.Enabled = false;
                    TSBPrint.Enabled = false;
                    TSBPrintPreview.Enabled = false;
                    TSBRefresh.Enabled = false;
                    TSBAccept.Enabled = true;
                    TSBCancel.Enabled = true;
                    break;
                default:
                    throw new NotImplementedException("Unknown Form Status in RefreshControls");
            }
        }

        public virtual void Suspend() => this.ToolbarTs.Enabled = false;
        public virtual void Resume() => this.ToolbarTs.Enabled = true;


        private void ChangeStatus(string status, Cursor cursor)
        {
            this.Status = status;
            base.Cursor = cursor;
            this.Refresh();
        }
        private void ChangeBusy(string status) => ChangeStatus(status, Cursors.WaitCursor);
        private void ChangeDefault(string status) => ChangeStatus(status, Cursors.Default);

        public void Busy(string status, Action action)
        {
            string prevStatus = this.Status;
            try
            {
                ChangeBusy(status);
                action();
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                ChangeDefault(prevStatus);
            }
        }
        public T Busy<T>(string status, Func<T> func)
        {
            string prevStatus = this.Status;
            try
            {
                ChangeBusy(status);
                return func();
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                ChangeDefault(prevStatus);
            }
        }
        public async Task Busy(string status, Func<Task> action)
        {
            string prevStatus = this.Status;
            try
            {
                ChangeBusy(status);
                await action();
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                ChangeDefault(prevStatus);
            }
        }
        public async Task<T> Busy<T>(string status, Func<Task<T>> func)
        {
            string prevStatus = this.Status;
            try
            {
                ChangeBusy(status);
                return await func();
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                ChangeDefault(prevStatus);
            }
        }

        public bool Busy(string status, string msgIferr, Action action)
        {
            try
            {
                Busy(status, action);
                return true;
            }
            catch (Exception ex)
            {
                Log?.LogError(msgIferr, ex);
                return false;
            }
        }
        public bool Busy(string status, string msgIferr, Func<bool> func)
        {
            try
            {
                return Busy(status, func);
            }
            catch (Exception ex)
            {
                Log?.LogError(msgIferr, ex);
                return false;
            }
        }
        public async Task<bool> Busy(string status, string msgIfer, Func<Task> action)
        {
            try
            {
                await Busy(status, action);
                return true;
            }
            catch (Exception ex)
            {
                Log?.LogError(msgIfer, ex);
                return false;
            }
        }
        public async Task<bool> Busy(string status, string msgIfer, Func<Task<bool>> func)
        {
            try
            {
                return await Busy(status, func);
            }
            catch (Exception ex)
            {
                Log?.LogError(msgIfer, ex);
                return false;
            }
        }


        private bool CallCancelEvent(CustomEventHandler<CancelEventArgs> handler)
        {
            if (handler == null) return true;
            var arg = new CancelEventArgs();
            handler.Invoke(this, arg);
            return !arg.Cancel;
        }

        private void DataManagementControl_Load(object sender, EventArgs e)
        {
            if (this.DesignMode) return;
            this.RefreshControls();
        }

        private void TSBAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                Log?.LogUI($"{Log?.ObjectName}:Add Button Clicked");
                if (!this.AllowAddNew) return;
                if (this.FormStatus != FormStatusType.View) return;

                if (!CallCancelEvent(this.ResetDataEvent)) return;
                this.FormStatus = FormStatusType.AddNew;
            }
            catch (Exception ex)
            {
                if (Log == null) throw ex;
                Log?.LogError($"Unhandled Error at {Log?.ObjectName}:Add Button Clicked", ex);
            }
        }

        private void TSBEdit_Click(object sender, EventArgs e)
        {
            try
            {
                Log?.LogUI($"{Log?.ObjectName}:Edit Button Clicked");
                if (!this.AllowEdit) return;
                if (this.FormStatus != FormStatusType.View) return;

                if (!CallCancelEvent(this.LoadDataEvent)) return;
                this.FormStatus = FormStatusType.Edit;
            }
            catch (Exception ex)
            {
                if (Log == null) throw ex;
                Log?.LogError($"Unhandled Error at {Log?.ObjectName}:Edit Button Clicked", ex);
            }
        }

        private void TSBDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Log?.LogUI($"{Log?.ObjectName}:Delete Button Clicked");

                if (!this.AllowDelete) return;
                if (this.FormStatus != FormStatusType.View) return;

                if (DeleteConfirmationEvent != null)
                {
                    if (!CallCancelEvent(this.DeleteConfirmationEvent)) return;
                }
                else if (MessageBox.Show("Are you sure to delete selected rows?", "Delete Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                this.DeleteDataEvent?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                if (Log == null) throw ex;
                Log?.LogError($"Unhandled Error at {Log?.ObjectName}:Delete Button Clicked", ex);
            }
        }

        private void TSBAccept_Click(object sender, EventArgs e)
        {
            try
            {
                Log?.LogUI($"{Log?.ObjectName}:Accept Button Clicked");

                if (!CallCancelEvent(this.CheckDataEvent)) return;

                switch (this.FormStatus)
                {
                    case FormStatusType.AddNew:
                        if (!this.AllowAddNew) return;

                        if (!CallCancelEvent(this.AddDataEvent)) return;
                        break;
                    case FormStatusType.Edit:
                        if (!this.AllowEdit) return;

                        if (!CallCancelEvent(this.EditDataEvent)) return;
                        break;
                    default:
                        throw new NotImplementedException("Unknown Form Status in TSBAccept_Click");
                }
                this.FormStatus = FormStatusType.View;
            }
            catch (Exception ex)
            {
                if (Log == null) throw ex;
                Log?.LogError($"Unhandled Error at {Log?.ObjectName}:Accept Button Clicked", ex);
            }
        }

        private void TSBCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Log?.LogUI($"{Log?.ObjectName}:Cancel Button Clicked");
                this.FormStatus = FormStatusType.View;
            }
            catch (Exception ex)
            {
                if (Log == null) throw ex;
                Log?.LogError($"Unhandled Error at {Log?.ObjectName}:Accept Button Clicked", ex);
            }
        }

        private void TSBRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                Log?.LogUI($"{Log?.ObjectName}:Refresh Button Clicked");

                if (!CallCancelEvent(this.RefreshDataEvent)) return;
                this.RefreshControls();
            }
            catch (Exception ex)
            {
                if (Log == null) throw ex;
                Log?.LogError($"Unhandled Error at {Log?.ObjectName}:Refresh Button Clicked", ex);
            }
        }

        private void TSBPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Log?.LogUI($"{Log?.ObjectName}:Print Button Clicked");
                this.PrintEvent?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                if (Log == null) throw ex;
                Log?.LogError($"Unhandled Error at {Log?.ObjectName}:Print Button Clicked", ex);
            }
        }

        private void TSBPrintPreview_Click(object sender, EventArgs e)
        {
            try
            {
                Log?.LogUI($"{Log?.ObjectName}:PrintPreview Button Clicked");
                this.PrintPreviewEvent?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                if (Log == null) throw ex;
                Log?.LogError($"Unhandled Error at {Log?.ObjectName}:PrintPreview Button Clicked", ex);
            }
        }

        private void TSBExport_Click(object sender, EventArgs e)
        {
            try
            {
                Log?.LogUI($"{Log?.ObjectName}:Export Button Clicked");
                this.ExportEvent?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                if (Log == null) throw ex;
                Log?.LogError($"Unhandled Error at {Log?.ObjectName}:Export Button Clicked", ex);
            }
        }
    }
}
