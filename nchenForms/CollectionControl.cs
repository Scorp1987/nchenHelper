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
        public event EventHandler<ListItemAddingEventArgs<TObject>> NewItemAdding;
        public event EventHandler<ListItemAddedEventArgs<TObject>> NewItemAdded;
        public event EventHandler<ListItemRemovingEventArgs<TObject>> ItemRemoving;
        public event EventHandler<ListItemRemovedEventArgs<TObject>> ItemRemoved;
        public event EventHandler<ListItemMovingEventArgs<TObject>> ItemMoving;
        public event EventHandler<ListItemMovedEventArgs<TObject>> ItemMoved;


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

        private void OnChangingChanged<TChangingEventArgs, TChangedEventArgs>(
            Func<TChangingEventArgs> getChangingArgs, EventHandler<TChangingEventArgs> changingHandler, Action action,
            Func<TChangedEventArgs> getChangedArgs, EventHandler<TChangedEventArgs> changedHandler)
            where TChangingEventArgs : CancelEventArgs
            where TChangedEventArgs : EventArgs
        {
            if(changingHandler != null)
            {
                var args = getChangingArgs();
                changingHandler?.Invoke(this, args);
                if (args.Cancel) return;
            }
            action();
            changedHandler?.Invoke(this, getChangedArgs());
        }

        private void OnAdd(int index, TObject value, Action action)
            => OnChangingChanged(
                () => new ListItemAddingEventArgs<TObject>(index, value), NewItemAdding, action,
                () => new ListItemAddedEventArgs<TObject>(index, value), NewItemAdded);
        private void OnDelete(int index, TObject value, Action action)
            => OnChangingChanged(
                () => new ListItemRemovingEventArgs<TObject>(index, value), ItemRemoving, action,
                () => new ListItemRemovedEventArgs<TObject>(index, value), ItemRemoved);
        private void OnMove(TObject value, int originalIndex, int newIndex, Action action)
            => OnChangingChanged(
                () => new ListItemMovingEventArgs<TObject>(value, originalIndex, newIndex), ItemMoving, action,
                () => new ListItemMovedEventArgs<TObject>(value, originalIndex, newIndex), ItemMoved);


        private void AddToolStripButton_Click(object sender, EventArgs e)
        {
            var obj = this.DefaultProvider.GetDefault();
            if (!(obj is TObject)) return;

            var index = this.BindingSource.Count;
            OnAdd(index, obj, () =>
            {
                this.CollectionListBox.SelectedValueChanged -= CollectionListBox_SelectedValueChanged;
                this.BindingSource.Add(obj);
                this.CollectionListBox.SelectedItem = obj;
                this.CollectionListBox.SelectedValueChanged += CollectionListBox_SelectedValueChanged;
            });
        }

        private void DeleteToolStripButton_Click(object sender, EventArgs e)
        {
            if (!(this.CollectionListBox.SelectedItem is TObject obj)) return;
            var index = this.CollectionListBox.SelectedIndex;
            OnDelete(index, obj, () => this.BindingSource.Remove(obj));
        }

        private void MoveItem(int offset)
        {
            if (!(this.CollectionListBox.SelectedItem is TObject obj)) return;

            var orgIndex = this.BindingSource.IndexOf(obj);
            var newIndex = orgIndex + offset;

            if (newIndex < 0) return;
            if (newIndex > this.BindingSource.Count - 1) return;

            OnMove(obj, orgIndex, newIndex, () =>
            {
                this.CollectionListBox.SelectedValueChanged -= CollectionListBox_SelectedValueChanged;
                this.BindingSource.Remove(obj);
                this.BindingSource.Insert(orgIndex + offset, obj);
                this.CollectionListBox.SelectedItem = obj;
            });
        }

        private void MoveFirstToolStripButton_Click(object sender, EventArgs e) => MoveItem(-this.CollectionListBox.SelectedIndex);

        private void MoveLastToolStripButton_Click(object sender, EventArgs e) => MoveItem(this.CollectionListBox.Items.Count - this.CollectionListBox.SelectedIndex - 1);

        private void MoveUpToolStripButton_Click(object sender, EventArgs e) => MoveItem(-1);

        private void MoveDownToolStripButton_Click(object sender, EventArgs e) => MoveItem(1);
    }
}
