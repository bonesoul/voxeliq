namespace NoiseTool
{
    partial class MainForm
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
            this.pictureNoise = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelAmplitude = new System.Windows.Forms.Label();
            this.trackBarAmplitude = new System.Windows.Forms.TrackBar();
            this.labelFrequency = new System.Windows.Forms.Label();
            this.trackBarFrequency = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureNoise)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAmplitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFrequency)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureNoise
            // 
            this.pictureNoise.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureNoise.Location = new System.Drawing.Point(271, 9);
            this.pictureNoise.Name = "pictureNoise";
            this.pictureNoise.Size = new System.Drawing.Size(512, 512);
            this.pictureNoise.TabIndex = 0;
            this.pictureNoise.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelAmplitude);
            this.groupBox1.Controls.Add(this.trackBarAmplitude);
            this.groupBox1.Controls.Add(this.labelFrequency);
            this.groupBox1.Controls.Add(this.trackBarFrequency);
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(253, 512);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // labelAmplitude
            // 
            this.labelAmplitude.AutoSize = true;
            this.labelAmplitude.Location = new System.Drawing.Point(6, 74);
            this.labelAmplitude.Name = "labelAmplitude";
            this.labelAmplitude.Size = new System.Drawing.Size(53, 13);
            this.labelAmplitude.TabIndex = 4;
            this.labelAmplitude.Text = "Amplitude";
            // 
            // trackBarAmplitude
            // 
            this.trackBarAmplitude.LargeChange = 10;
            this.trackBarAmplitude.Location = new System.Drawing.Point(6, 90);
            this.trackBarAmplitude.Maximum = 100;
            this.trackBarAmplitude.Name = "trackBarAmplitude";
            this.trackBarAmplitude.Size = new System.Drawing.Size(241, 45);
            this.trackBarAmplitude.TabIndex = 3;
            this.trackBarAmplitude.Value = 100;
            this.trackBarAmplitude.ValueChanged += new System.EventHandler(this.trackBarAmplitude_ValueChanged);
            // 
            // labelFrequency
            // 
            this.labelFrequency.AutoSize = true;
            this.labelFrequency.Location = new System.Drawing.Point(6, 23);
            this.labelFrequency.Name = "labelFrequency";
            this.labelFrequency.Size = new System.Drawing.Size(57, 13);
            this.labelFrequency.TabIndex = 2;
            this.labelFrequency.Text = "Frequency";
            // 
            // trackBarFrequency
            // 
            this.trackBarFrequency.LargeChange = 10;
            this.trackBarFrequency.Location = new System.Drawing.Point(6, 39);
            this.trackBarFrequency.Maximum = 100;
            this.trackBarFrequency.Name = "trackBarFrequency";
            this.trackBarFrequency.Size = new System.Drawing.Size(241, 45);
            this.trackBarFrequency.TabIndex = 0;
            this.trackBarFrequency.Value = 10;
            this.trackBarFrequency.ValueChanged += new System.EventHandler(this.trackBarFrequency_ValueChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 533);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureNoise);
            this.Name = "MainForm";
            this.Text = "Noise Tool";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureNoise)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAmplitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFrequency)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureNoise;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TrackBar trackBarFrequency;
        private System.Windows.Forms.Label labelAmplitude;
        private System.Windows.Forms.TrackBar trackBarAmplitude;
        private System.Windows.Forms.Label labelFrequency;
    }
}

