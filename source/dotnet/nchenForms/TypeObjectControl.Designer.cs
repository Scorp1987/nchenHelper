
namespace System.Windows.Forms
{
    partial class TypeObjectControl<TObject, TType>
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ItemPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.TypeLabel = new System.Windows.Forms.Label();
            this.TypeComboBox = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.ItemPropertyGrid, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.TypeLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.TypeComboBox, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(250, 250);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // ItemPropertyGrid
            // 
            this.ItemPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.ItemPropertyGrid, 2);
            this.ItemPropertyGrid.Location = new System.Drawing.Point(0, 27);
            this.ItemPropertyGrid.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.ItemPropertyGrid.Name = "ItemPropertyGrid";
            this.ItemPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.ItemPropertyGrid.Size = new System.Drawing.Size(250, 223);
            this.ItemPropertyGrid.TabIndex = 3;
            this.ItemPropertyGrid.ToolbarVisible = false;
            // 
            // TypeLabel
            // 
            this.TypeLabel.AutoSize = true;
            this.TypeLabel.Location = new System.Drawing.Point(0, 3);
            this.TypeLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.TypeLabel.Name = "TypeLabel";
            this.TypeLabel.Size = new System.Drawing.Size(31, 13);
            this.TypeLabel.TabIndex = 1;
            this.TypeLabel.Text = "Type";
            // 
            // TypeComboBox
            // 
            this.TypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TypeComboBox.FormattingEnabled = true;
            this.TypeComboBox.Location = new System.Drawing.Point(37, 0);
            this.TypeComboBox.Margin = new System.Windows.Forms.Padding(3, 0, 0, 3);
            this.TypeComboBox.Name = "TypeComboBox";
            this.TypeComboBox.Size = new System.Drawing.Size(213, 21);
            this.TypeComboBox.TabIndex = 2;
            this.TypeComboBox.SelectedValueChanged += new System.EventHandler(this.TypeComboBox_SelectedValueChanged);
            // 
            // TypeObjectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TypeObjectControl";
            this.Size = new System.Drawing.Size(250, 250);
            this.Load += new System.EventHandler(this.AAbstractDataControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label TypeLabel;
        protected System.Windows.Forms.ComboBox TypeComboBox;
        protected System.Windows.Forms.PropertyGrid ItemPropertyGrid;
    }
}
