using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.Xna.Framework;

namespace VolumetricStudios.VoxeliqGame
{
    [Activity(
        Label = "Voxeliq", 
        MainLauncher = true, 
        Icon = "@drawable/icon", 
        Theme = "@style/Theme.Splash", 
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden)]
	public class Activity1 : AndroidGameActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
            base.OnCreate (bundle);
            VoxeliqGame.Activity = this;
			var g = new VoxeliqGame();
            SetContentView(g.Window);
            g.Run();
		}
	}
}


