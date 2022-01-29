using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.UITypeEditors
{
    public class FolderPathEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.Modal;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService _)
                using (var popedControl = new FolderBrowserDialog()
                {
                    SelectedPath = Helper.GetInitDir((string)value),
                    ShowNewFolderButton = true
                })
                    if (popedControl.ShowDialog() == DialogResult.OK)
                        value = popedControl.SelectedPath.ToShortFormPath() + @"\";

            return base.EditValue(context, provider, value);
        }
    }
}
