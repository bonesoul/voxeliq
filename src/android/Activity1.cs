using Android.App;
using Android.Content.PM;
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
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Game.Activity = this;

            var game = new VoxeliqGame();
            this.SetContentView(game.Window);
            game.Run();
        }
	}
}


