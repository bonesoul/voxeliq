namespace Voxeliq.MonsterEditor
{
    partial class Form1
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
            this.xnaHost1 = new WinFormsGraphicsDevice.XNAHost();
            this.SuspendLayout();
            // 
            // xnaHost1
            // 
            this.xnaHost1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.xnaHost1.Location = new System.Drawing.Point(-3, -2);
            this.xnaHost1.Name = "xnaHost1";
            this.xnaHost1.Size = new System.Drawing.Size(644, 321);
            this.xnaHost1.TabIndex = 0;
            this.xnaHost1.Text = "xnaHost1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 455);
            this.Controls.Add(this.xnaHost1);
            this.Name = "Form1";
            this.Text = "Voxeliq Monster Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private WinFormsGraphicsDevice.XNAHost xnaHost1;



    }
}

