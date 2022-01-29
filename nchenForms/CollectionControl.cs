using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms.Args;
using System.Windows.Forms.Providers;

namespace System.Windows.Forms
{
    public partial class CollectionControl<TObject> : UserControl, IObjectControl<ICollection<TObject>>
    {
        public CollectionControl(DefaultProvider<TObject> defaultProvider)
        {
            InitializeComponent();
            this.DefaultProvider = defaultProvider;
        }
        public CollectionControl() : this(new DefaultProvider<TObject>()) { }


        [Category("Appearance")]
        [Description("Title of the Control")]
        [DefaultValue("Title")]
        public string Title { get => TitleToolStripLabel.Text; set => TitleToolStripLabel.Text = value; }


        [Category("Behavior")]
        [Description("Controls whether the control can be changed or not")]
        [DefaultValue(false)]
        public virtual bool ReadOnly
        {
            get => !ToolbarStrip.Enabled;
            set
            {
                ToolbarStrip.Enabled = !value;
                CollectionListBox.Enabled = !value;
            }
        }


        private ICollection<TObject> _value;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual ICollection<TObject> Value
        {
            get => _value;
            set
            {
                _value = value;

                this.BindingSource?.Dispose();
                this.BindingSource = new BindingSource { DataSource = value };
                this.CollectionListBox.DataSource = this.BindingSource;
            }
        }


        [Category("Behavior")]
        [Description("Controls whether the item can change the sequence")]
        [DefaultValue(true)]
        public bool AllowMove
        {
            get => MoveFirstToolStripButton.Visible;
            set
            {
                ToolStripSeparator1.Visible = value;
                MoveFirstToolStripButton.Visible = value;
                MoveLastToolStripButton.Visible = value;
                MoveUpToolStripButton.Visible = value;
                MoveDownToolStripButton.Visible = value;
            }
        }


        protected BindingSource BindingSource { get; set; }

        private DefaultProvider<TObject> DefaultProvider { get; }


        public event EventHandler<TObject> SelectedValueChanged;
        public event EventHandler<CancelEventArgs> NewItemBeforeAdded;
        public event EventHandler<TObject> NewItemAdded;
        public event EventHandler<CancelEventArgs> ItemBeforeDeleted;
        public event EventHandler<TObject> ItemDeleted;
        public event EventHandler<CancelEventArgs> ItemBeforeMoved;
        public event EventHandler<ItemMovedEventArgs<TObject>> ItemMoved;


        public void AddToolStripItem(ToolStripItem item) => ToolbarStrip.Items.Add(item);
        public void RemoveToolStripItem(ToolStripItem item) => ToolbarStrip.Items.Remove(item);

        private void AObjectCollectionControl_Load(object sender, EventArgs e)
        {
            if (base.DesignMode) return;
            if (this.Value == null) this.Value = new List<TObject>();
        }

        private void CollectionListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!(this.CollectionListBox.SelectedItem is TObject obj)) return;
            this.SelectedValueChanged?.Invoke(this, obj);
        }

        private void AddToolStripButton_Click(object sender, EventArgs e)
        {
            var obj = this.DefaultProvider.GetDefault();
            if (!(obj is TObject)) return;

            var arg = new CancelEventArgs();
            NewItemBeforeAdded?.Invoke(this, arg);
            if (arg.Cancel) return;
            this.CollectionListBox.SelectedValueChanged -= CollectionListBox_SelectedValueChanged;
            this.BindingSource.Add(obj);
            NewItemAdded?.Invoke(this, obj);
            this.CollectionListBox.SelectedItem = obj;
            this.CollectionListBox.SelectedValueChanged += CollectionListBox_SelectedValueChanged;
        }

        private void DeleteToolStripButton_Click(object sender, EventArgs e)
        {
            if (!(this.CollectionListBox.SelectedItem is TObject obj)) return;

            var arg = new CancelEventArgs();
            ItemBeforeDeleted?.Invoke(this, arg);
            if (arg.Cancel) return;
            this.BindingSource.Remove(obj);
            ItemDeleted?.Invoke(this, obj);
        }

        private void MoveItem(int offset)
        {
            if (!(this.CollectionListBox.SelectedItem is TObject obj)) return;

            var orgIndex = this.BindingSource.IndexOf(obj);
            var newIndex = orgIndex + offset;

            if (newIndex < 0) return;
            if (newIndex > this.BindingSource.Count - 1) return;

            var arg = new CancelEventArgs();
            ItemBeforeMoved?.Invoke(this, arg);
            if (arg.Cancel) return;
            this.CollectionListBox.SelectedValueChanged -= CollectionListBox_SelectedValueChanged;
            this.BindingSource.Remove(obj);
            this.BindingSource.Insert(orgIndex + offset, obj);
            this.CollectionListBox.SelectedItem = obj;
            this.CollectionListBox.SelectedValueChanged += CollectionListBox_SelectedValueChanged;

            ItemMoved?.Invoke(this, new ItemMovedEventArgs<TObject>(obj, orgIndex, newIndex));
        }

        private void MoveFirstToolStripButton_Click(object sender, EventArgs e) => MoveItem(-this.CollectionListBox.SelectedIndex);

        private void MoveLastToolStripButton_Click(object sender, EventArgs e) => MoveItem(this.CollectionListBox.Items.Count - this.CollectionListBox.SelectedIndex - 1);

        private void MoveUpToolStripButton_Click(object sender, EventArgs e) => MoveItem(-1);

        private void MoveDownToolStripButton_Click(object sender, EventArgs e) => MoveItem(1);
    }
}
