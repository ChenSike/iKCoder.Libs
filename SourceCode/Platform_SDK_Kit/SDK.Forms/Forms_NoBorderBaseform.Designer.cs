namespace iKCoder_Platform_SDK_Kit
{
    partial class Forms_NoBorderBaseform
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Forms_NoBorderBaseform
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(284, 242);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Forms_NoBorderBaseform";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Forms_NoBorderBaseform_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Forms_NoBorderBaseform_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Forms_NoBorderBaseform_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.Forms_NoBorderBaseform_DragOver);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Forms_NoBorderBaseform_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Forms_NoBorderBaseform_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Forms_NoBorderBaseform_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}