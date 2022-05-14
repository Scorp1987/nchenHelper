using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Attributes;

namespace System.Windows.Forms.UITypeEditors
{
    public class FilePathEditor : UITypeEditor
    {
        public virtual FileDialog GetNewFileDialog(string value)
        {
            return new OpenFileDialog()
            {
                InitialDirectory = Helper.GetInitDir(value),
                FileName = Helper.GetFileName(value),
                Title = "File Selection",
                Multiselect = false
            };
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.Modal;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.PropertyDescriptor != null && context.PropertyDescriptor.Attributes[typeof(FilePathEditorAttribute)] is FilePathEditorAttribute attribute) { }
            else
                attribute = new FilePathEditorAttribute();

            using (var dialog = new OpenFileDialog())
            {
                if (!string.IsNullOrEmpty(attribute.Title)) dialog.Title = attribute.Title;
                if (!string.IsNullOrEmpty(attribute.DefaultExt)) dialog.DefaultExt = attribute.DefaultExt;
                if (!string.IsNullOrEmpty(attribute.Filter)) dialog.Filter = attribute.Filter;
                dialog.InitialDirectory = string.IsNullOrEmpty(attribute.InitialDirectory) ? Helper.GetInitDir((string)value) : attribute.InitialDirectory.ToPath();
                dialog.FileName = string.IsNullOrEmpty(attribute.FileName) ? Helper.GetFileName((string)value) : attribute.FileName;
                dialog.Multiselect = attribute.MultiSelect;
                if (dialog.ShowDialog() == DialogResult.OK)
                    value = dialog.FileName.ToShortFormPath();
            }

            return base.EditValue(context, provider, value);
        }
    }
}
