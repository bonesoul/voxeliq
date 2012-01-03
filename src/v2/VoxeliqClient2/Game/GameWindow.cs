using System;
using System.Reflection;
using VolumetricStudios.VoxeliqClient.Screen;
using VolumetricStudios.VoxeliqEngine.Graphics.Rendering;

namespace VolumetricStudios.VoxeliqClient.Game
{
    public class GameWindow:RenderWindow
    {
        protected override void OnLoad(EventArgs e)
        {
            this.Text = "Voxeliq Client " + Assembly.GetExecutingAssembly().GetName().Version;
            this.Width = ScreenConfig.Instance.ScreenWidth;
            this.Height = ScreenConfig.Instance.ScreenHeight;   
        }
    }
}
