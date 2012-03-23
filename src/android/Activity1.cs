using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.Xna.Framework;

namespace VolumetricStudios.VoxeliqGame
{
	[Activity (
        Label = "Voxeliq.MonoGame.Android", 
        MainLauncher = true,
        Icon = "@drawable/icon"
        )]
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


