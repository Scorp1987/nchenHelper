
namespace System.Windows.Forms
{
    partial class CollectionControl<TObject>
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
            this.ToolbarStrip = new System.Windows.Forms.ToolStrip();
            this.TitleToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.AddToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.DeleteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MoveFirstToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.MoveUpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.MoveDownToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.MoveLastToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.SplitContainer = new System.Windows.Forms.SplitContainer();
            this.CollectionListBox = new System.Windows.Forms.ListBox();
            this.ToolbarStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
            this.SplitContainer.Panel1.SuspendLayout();
            this.SplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // ToolbarStrip
            // 
            this.ToolbarStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolbarStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TitleToolStripLabel,
            this.AddToolStripButton,
            this.DeleteToolStripButton,
            this.ToolStripSeparator1,
            this.MoveFirstToolStripButton,
            this.MoveUpToolStripButton,
            this.MoveDownToolStripButton,
            this.MoveLastToolStripButton});
            this.ToolbarStrip.Location = new System.Drawing.Point(0, 0);
            this.ToolbarStrip.Name = "ToolbarStrip";
            this.ToolbarStrip.Size = new System.Drawing.Size(300, 25);
            this.ToolbarStrip.TabIndex = 16;
            this.ToolbarStrip.Text = "Toolbar";
            // 
            // TitleToolStripLabel
            // 
            this.TitleToolStripLabel.Name = "TitleToolStripLabel";
            this.TitleToolStripLabel.Size = new System.Drawing.Size(29, 22);
            this.TitleToolStripLabel.Text = "Title";
            // 
            // AddToolStripButton
            // 
            this.AddToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.AddToolStripButton.Image = global::System.Windows.Forms.Properties.Resources.AddNew;
            this.AddToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.AddToolStripButton.Name = "AddToolStripButton";
            this.AddToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.AddToolStripButton.Text = "Add";
            this.AddToolStripButton.Click += new System.EventHandler(this.AddToolStripButton_Click);
            // 
            // DeleteToolStripButton
            // 
            this.DeleteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.DeleteToolStripButton.Image = global::System.Windows.Forms.Properties.Resources.Delete;
            this.DeleteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DeleteToolStripButton.Name = "DeleteToolStripButton";
            this.DeleteToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.DeleteToolStripButton.Text = "Delete";
            this.DeleteToolStripButton.Click += new System.EventHandler(this.DeleteToolStripButton_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // MoveFirstToolStripButton
            // 
            this.MoveFirstToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MoveFirstToolStripButton.Image = global::System.Windows.Forms.Properties.Resources.top;
            this.MoveFirstToolStripButton.Name = "MoveFirstToolStripButton";
            this.MoveFirstToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.MoveFirstToolStripButton.Text = "Move First";
            this.MoveFirstToolStripButton.Click += new System.EventHandler(this.MoveFirstToolStripButton_Click);
            // 
            // MoveUpToolStripButton
            // 
            this.MoveUpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MoveUpToolStripButton.Image = global::System.Windows.Forms.Properties.Resources.move_up;
            this.MoveUpToolStripButton.Name = "MoveUpToolStripButton";
            this.MoveUpToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.MoveUpToolStripButton.Text = "Move Up";
            this.MoveUpToolStripButton.Click += new System.EventHandler(this.MoveUpToolStripButton_Click);
            // 
            // MoveDownToolStripButton
            // 
            this.MoveDownToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MoveDownToolStripButton.Image = global::System.Windows.Forms.Properties.Resources.move_down;
            this.MoveDownToolStripButton.Name = "MoveDownToolStripButton";
            this.MoveDownToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.MoveDownToolStripButton.Text = "Move Down";
            this.MoveDownToolStripButton.Click += new System.EventHandler(this.MoveDownToolStripButton_Click);
            // 
            // MoveLastToolStripButton
            // 
            this.MoveLastToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MoveLastToolStripButton.Image = global::System.Windows.Forms.Properties.Resources.bottom;
            this.MoveLastToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MoveLastToolStripButton.Name = "MoveLastToolStripButton";
            this.MoveLastToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.MoveLastToolStripButton.Text = "Move Last";
            this.MoveLastToolStripButton.Click += new System.EventHandler(this.MoveLastToolStripButton_Click);
            // 
            // SplitContainer
            // 
            this.SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.SplitContainer.Location = new System.Drawing.Point(0, 25);
            this.SplitContainer.Name = "SplitContainer";
            // 
            // SplitContainer.Panel1
            // 
            this.SplitContainer.Panel1.Controls.Add(this.CollectionListBox);
            this.SplitContainer.Size = new System.Drawing.Size(300, 277);
            this.SplitContainer.SplitterDistance = 100;
            this.SplitContainer.TabIndex = 17;
            // 
            // CollectionListBox
            // 
            this.CollectionListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CollectionListBox.FormattingEnabled = true;
            this.CollectionListBox.Location = new System.Drawing.Point(0, 0);
            this.CollectionListBox.Name = "CollectionListBox";
            this.CollectionListBox.Size = new System.Drawing.Size(100, 277);
            this.CollectionListBox.TabIndex = 14;
            this.CollectionListBox.SelectedValueChanged += new System.EventHandler(this.CollectionListBox_SelectedValueChanged);
            // 
            // CollectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SplitContainer);
            this.Controls.Add(this.ToolbarStrip);
            this.Name = "CollectionControl";
            this.Size = new System.Drawing.Size(300, 302);
            this.Load += new System.EventHandler(this.AObjectCollectionControl_Load);
            this.ToolbarStrip.ResumeLayout(false);
            this.ToolbarStrip.PerformLayout();
            this.SplitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).EndInit();
            this.SplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStripButton AddToolStripButton;
        private System.Windows.Forms.ToolStripButton DeleteToolStripButton;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
        private System.Windows.Forms.ToolStripButton MoveFirstToolStripButton;
        private System.Windows.Forms.ToolStripButton MoveUpToolStripButton;
        private System.Windows.Forms.ToolStripButton MoveDownToolStripButton;
        private System.Windows.Forms.ToolStripButton MoveLastToolStripButton;
        private System.Windows.Forms.ToolStripLabel TitleToolStripLabel;
        protected System.Windows.Forms.SplitContainer SplitContainer;
        protected System.Windows.Forms.ToolStrip ToolbarStrip;
        protected System.Windows.Forms.ListBox CollectionListBox;
    }
}
