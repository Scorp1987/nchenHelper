using System.ComponentModel;
using System.Windows.Forms.Converters;
using System.Windows.Forms.Providers;

namespace System.Windows.Forms
{
    public partial class TypeObjectControl<TObject, TType> : UserControl, IObjectControl<TObject>
    {
        public TypeObjectControl(
            DefaultProvider<TType> defaultTypeProvider,
            GenericConverter<TObject, TType> converter)
        {
            InitializeComponent();
            this.DefaultTypeProvider = defaultTypeProvider;
            this.Converter = converter;
        }
        public TypeObjectControl() : this(new DefaultProvider<TType>(), new GenericConverter<TObject, TType>()) { }


        [Category("Appearance")]
        [DefaultValue("Type")]
        [Description("Type label text to show in Control")]
        public string TypeName { get => this.TypeLabel.Text; set => this.TypeLabel.Text = value; }


        [Category("Behavior")]
        [Description("Controls whether the control can be changed or not")]
        [DefaultValue(false)]
        public virtual bool ReadOnly
        {
            get => !this.TypeComboBox.Enabled;
            set
            {
                this.TypeComboBox.Enabled = !value;
                this.ItemPropertyGrid.Enabled = !value;
            }
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual TObject Value
        {
            get => (TObject)this.ItemPropertyGrid.SelectedObject;
            set
            {
                this.ItemPropertyGrid.SelectedObject = value;
                var type = this.Converter.ConvertFrom(value);
                this.TypeComboBox.SelectedValueChanged -= TypeComboBox_SelectedValueChanged;
                if (value == null)
                    this.TypeComboBox.SelectedIndex = -1;
                else
                    this.TypeComboBox.SelectedItem = type;
                this.TypeComboBox.SelectedValueChanged += TypeComboBox_SelectedValueChanged;
            }
        }


        private DefaultProvider<TType> _defaultTypeProvider;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public DefaultProvider<TType> DefaultTypeProvider
        {
            get => _defaultTypeProvider;
            set => _defaultTypeProvider = value ?? throw new ArgumentNullException(nameof(DefaultTypeProvider));
        }


        public GenericConverter<TObject, TType> _converter;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GenericConverter<TObject, TType> Converter
        {
            get => _converter;
            set => _converter = value ?? throw new ArgumentNullException(nameof(Converter));
        }


        public event EventHandler<TObject> TypeChanged;


        private void AAbstractDataControl_Load(object sender, EventArgs e)
        {
            if (base.DesignMode) return;
            this.TypeComboBox.DataSource = this.DefaultTypeProvider.GetDefaultList();
            this.TypeComboBox.SelectedIndex = -1;
        }
        private void TypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!(TypeComboBox.SelectedItem is TType type)) return;
            var item = this.Converter.ConvertTo(type);
            this.Value = item;
            this.TypeChanged?.Invoke(this, item);
        }
    }
}
