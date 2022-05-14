using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms.Attributes;

namespace System.Windows.Forms
{
    public static class Extensions
    {
        public static void SelectLastRow(this DataGridView dgv)
        {
            var lastRowIndex = dgv.Rows.Count - 1;
            dgv.ClearSelection();
            dgv.Rows[lastRowIndex].Selected = true;
            dgv.FirstDisplayedScrollingRowIndex = lastRowIndex;
        }
        public static void SelectRow<T>(this DataGridView dgv, string columnName, T value)
        {
            dgv.ClearSelection();
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (((T)row.Cells[columnName].Value).Equals(value))
                {
                    row.Selected = true;
                    break;
                }
            }
        }
        public static DataRow GetSelectedRow(this DataGridView dgv)
        {
            if (dgv.SelectedRows.Count != 1) return default;
            return ((DataRowView)dgv.SelectedRows[0].DataBoundItem).Row;
        }
        public static IEnumerable<DataRow> GetSelectedRows(this DataGridView dgv)
        {
            var query = from DataGridViewRow dgvRow in dgv.SelectedRows
                        select ((DataRowView)dgvRow.DataBoundItem).Row;
            if (dgv.SelectedRows.Count < 1) return default;
            return query;
        }
        public static T GetSelectedItem<T>(this DataGridView dgv, string columnName)
        {
            if (dgv.SelectedRows.Count != 1) return default;
            return (T)dgv.SelectedRows[0].Cells[columnName].Value;
        }


        public static void ResetValues<T>(this T dataControl) where T : IDataControl
        {
            var type = dataControl.GetType();
            var controls = from property in type.GetProperties()
                           where property.CanWrite
                           from attrib in property.GetCustomAttributes<ControlValueAttribute>()
                           from defaultValue in property.GetCustomAttributes<DefaultValueAttribute>()
                           select new { property, defaultValue };

            foreach (var control in controls)
                control.property.SetValue(dataControl, control.defaultValue.Value);
        }
        public static void FillValues<T>(this T dataControl, DataRow destination) where T : IDataControl
        {
            var type = dataControl.GetType();
            var query = from property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        where property.CanRead
                        from attrib in property.GetCustomAttributes<ControlValueAttribute>()
                        join DataColumn column in destination.Table.Columns on attrib.ColumnName equals column.ColumnName
                        select new { Property = property, Column = column };

            foreach (var value in query)
                destination[value.Column] = value.Property.GetValue(dataControl);
        }
        public static void SetValues<T>(this T dataControl, DataRow source) where T : IDataControl
        {
            var type = dataControl.GetType();
            var query = from property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        where property.CanWrite
                        from attrib in property.GetCustomAttributes<ControlValueAttribute>()
                        join DataColumn column in source.Table.Columns on attrib.ColumnName equals column.ColumnName
                        select new { Property = property, Column = column };

            foreach (var value in query)
                value.Property.SetValue(dataControl, GetValue(source[value.Column]));
        }
        public static IEnumerable<DataColumn> GetChangedColumns<T>(this T dataControl, DataRow orgValues) where T : IDataControl
        {
            var type = dataControl.GetType();
            var query = from property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        where property.CanRead
                        from attrib in property.GetCustomAttributes<ControlValueAttribute>()
                        join DataColumn column in orgValues.Table.Columns on attrib.ColumnName equals column.ColumnName
                        where !orgValues[column].Equals(property.GetValue(dataControl))
                        select column;
            return query;
        }
        private static object GetValue(object value) => (value is DBNull | value == null) ? default : value;


        /// <summary>
        /// Gets the (private) PropertyGridView instance.
        /// </summary>
        /// <param name="propertyGrid">The property grid.</param>
        /// <returns>The PropertyGridView instance.</returns>
        private static object GetPropertyGridView(PropertyGrid propertyGrid)
        {
            //private PropertyGridView GetPropertyGridView();
            //PropertyGridView is an internal class...
            MethodInfo methodInfo = typeof(PropertyGrid).GetMethod("GetPropertyGridView", BindingFlags.NonPublic | BindingFlags.Instance);
            return methodInfo.Invoke(propertyGrid, new object[] { });
        }

        /// <summary>
        /// Gets the width of the left column.
        /// </summary>
        /// <param name="propertyGrid">The property grid.</param>
        /// <returns>
        /// The width of the left column.
        /// </returns>
        public static int GetInternalLabelWidth(this PropertyGrid propertyGrid)
        {
            //System.Windows.Forms.PropertyGridInternal.PropertyGridView
            object gridView = GetPropertyGridView(propertyGrid);

            //protected int InternalLabelWidth
            PropertyInfo propInfo = gridView.GetType().GetProperty("InternalLabelWidth", BindingFlags.NonPublic | BindingFlags.Instance);
            return (int)propInfo.GetValue(gridView);
        }

        /// <summary>
        /// Moves the splitter to the supplied horizontal position.
        /// </summary>
        /// <param name="propertyGrid">The property grid.</param>
        /// <param name="xpos">The horizontal position.</param>
        public static void MoveSplitterTo(this PropertyGrid propertyGrid, int xpos)
        {
            //System.Windows.Forms.PropertyGridInternal.PropertyGridView
            object gridView = GetPropertyGridView(propertyGrid);

            //private void MoveSplitterTo(int xpos);
            MethodInfo methodInfo = gridView.GetType().GetMethod("MoveSplitterTo", BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(gridView, new object[] { xpos });
        }


        public static void FillCombo(this ComboBox combo, Type enumType)
        {
            foreach (var item in Enum.GetValues(enumType))
                combo.Items.Add(item);
        }

        public static DialogResult ShowDialog(this UserControl control, FormAttribute attribute = null)
        {
            var orgDock = control.Dock;
            control.Dock = DockStyle.Fill;
            Form frm;
            if (attribute == null)
                frm = new Form
                {
                    Size = new Size(control.Size.Width + 15, control.Size.Height + 40),
                    Text = control.Name,
                    MinimizeBox = false,
                    MaximizeBox = false
                };
            else
                frm = new Form
                {
                    Size = (attribute.Width == 0 | attribute.Height == 0) ? new Size(control.Size.Width + 15, control.Size.Height + 40) : new Size(attribute.Width, attribute.Height),
                    Text = string.IsNullOrEmpty(attribute.Title) ? control.Name : attribute.Title,
                    MinimizeBox = attribute.MinimizeBox,
                    MaximizeBox = attribute.MaximizeBox
                };
            frm.Icon = Form.ActiveForm?.Icon;
            frm.Controls.Add(control);
            var result = frm.ShowDialog();
            control.Dock = orgDock;
            frm.Dispose();
            return result;
        }
    }
}
