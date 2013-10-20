using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using VoxeliqEngine.Common.Noise;
using Color = System.Drawing.Color;

namespace NoiseTool
{
    public partial class MainForm : Form
    {
        private bool _generating = false;
        private const int _size=512;
        private Bitmap _bitmap;

        public MainForm()
        {
            InitializeComponent();

            this._bitmap = new Bitmap(_size, _size);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.GenerateNoise();
        }

        private void GenerateNoise()
        {
            if (this._generating)
                return;

            this._generating = true;
            this.DisableControls();

            var frequency = (float)trackBarFrequency.Value / 1000;
            var amplitude = (float)trackBarAmplitude.Value / 100;
            labelFrequency.Text = string.Format("Frequeny: {0}", frequency);
            labelAmplitude.Text = string.Format("Amplitude: {0}", amplitude);

            for (int x = 0; x < 512; x++)
            {
                for (int z = 0; z < 512; z++)
                {
                    var @val = SimplexNoise.noise(x * frequency, 1 * frequency, z * frequency) * amplitude ;
                    @val = Math.Abs(@val);
                    var density = (int)MathHelper.Lerp(0, 255, val);
                    this._bitmap.SetPixel(x, z, Color.FromArgb(255, density, density, density));
                }
            }

            pictureNoise.Image = this._bitmap;

            this.EnableControls();
            this._generating = false;
        }

        private void DisableControls()
        {
            this.trackBarAmplitude.Enabled = false;
            this.trackBarFrequency.Enabled = false;
        }

        private void EnableControls()
        {
            this.trackBarAmplitude.Enabled = true;
            this.trackBarFrequency.Enabled = true;
        }


        private void trackBarAmplitude_ValueChanged(object sender, EventArgs e)
        {
            this.GenerateNoise();
        }

        private void trackBarFrequency_ValueChanged(object sender, EventArgs e)
        {
            this.GenerateNoise();
        }
    }
}
