using System.Collections.Generic;
using System.Windows.Forms.Providers;

namespace System.Windows.Forms
{
    public class CollectionControl<TControl, TObject> : CollectionControl<TObject>
        where TControl : Control, IObjectControl<TObject>, new()
    {
        public CollectionControl(DefaultProvider<TObject> defaultProvider) : base(defaultProvider)
        {
            this.ObjectControl = new TControl { Dock = DockStyle.Fill };
            this.SplitContainer.Panel2.Controls.Add(this.ObjectControl);
            base.SelectedValueChanged += ObjectCollectionControl_SelectedValueChanged;
            base.NewItemAdded += ObjectCollectionControl_NewItemAdded;
            base.ItemRemoved += ObjectCollectionControl_ItemRemoved;
        }
        public CollectionControl() : this(new DefaultProvider<TObject>()) { }


        public override ICollection<TObject> Value
        {
            get => base.Value;
            set
            {
                base.Value = value;
                ObjectControl.ReadOnly = value == null || value.Count < 1;
            }
        }

        public TControl ObjectControl { get; }


        private void ObjectCollectionControl_SelectedValueChanged(object sender, TObject e)
        {
            this.ObjectControl.Value = e;
        }

        private void ObjectCollectionControl_NewItemAdded(object sender, ListItemAddedEventArgs<TObject> e)
        {
            this.ObjectControl.Value = e.Value;
            this.ObjectControl.ReadOnly = false;
        }

        private void ObjectCollectionControl_ItemRemoved(object sender, ListItemRemovedEventArgs<TObject> e)
        {
            this.ObjectControl.ReadOnly = base.BindingSource.Count < 1;
        }
    }
}
