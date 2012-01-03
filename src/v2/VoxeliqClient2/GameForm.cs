using System;
using SlimDX.Windows;

namespace VolumetricStudios.VoxeliqClient
{
    public class GameForm : RenderForm
    {
        public GameForm():base("Voxeliq")
        { }

        protected override void OnLoad(EventArgs e)
        {
            this.Width = ScreenConfig.Instance.ScreenWidth;
            this.Height = ScreenConfig.Instance.ScreenHeight;
        }
    }
}
