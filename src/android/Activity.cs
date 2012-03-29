using System.Reflection;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Xna.Framework;
using VolumetricStudios.LibVolumetric.Logging;

namespace VolumetricStudios.VoxeliqGame
{
    [Activity(
        Label = "Voxeliq", 
        MainLauncher = true, 
        Icon = "@drawable/icon", 
        Theme = "@style/Theme.Splash", 
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden)]
	public class Activity : AndroidGameActivity
	{
        private static readonly Logger Logger = LogManager.CreateLogger(); // the logger.

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Game.Activity = this;

            InitLoggers(); // init logging facility.
            Logger.Info("Voxeliq v{0} warming-up..", Assembly.GetExecutingAssembly().GetName().Version);

            var game = new VoxeliqGame();
            this.SetContentView(game.Window);
            Logger.Trace("Starting game loop..");
            game.Run();
        }

        private static void InitLoggers()
        {
            LogManager.Enabled = true; // enable logging facility.
            LogManager.AttachLogTarget(new ConsoleTarget(Logger.Level.Trace, Logger.Level.Fatal, false)); // attach a console target.
        }
	}
}


