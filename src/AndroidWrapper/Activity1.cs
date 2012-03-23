using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using VolumetricStudios.VoxeliqGame;

namespace AndroidWrapper
{
    namespace Microsoft.Xna.Samples.Draw2D
    {
        [Activity(Label = "Draw2D", MainLauncher = true, Icon = "@drawable/icon")]
        public class Activity1 : Activity
        {
            protected override void OnCreate(Bundle bundle)
            {
                base.OnCreate(bundle);
                
                var g = new VoxeliqGame();
                SetContentView(g.Window);
                g.Run();
            }
        }
    }
}

