namespace DrawTools2
{
    partial class DrawArea
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
            this.SuspendLayout();
            // 
            // DrawArea
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Name = "DrawArea";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawArea_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DrawArea_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DrawArea_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DrawArea_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DrawArea_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DrawArea_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.DrawArea_MouseWheel);
            this.Move += new System.EventHandler(this.DrawArea_Move);
            this.Resize += new System.EventHandler(this.DrawArea_Resize);
            this.ResumeLayout(false);

        }


        #endregion
    }
}
