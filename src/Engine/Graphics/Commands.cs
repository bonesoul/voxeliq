using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxeliqEngine.Core;
using VoxeliqEngine.Debugging.Console;

namespace VoxeliqEngine.Graphics
{
    [Command("vsync", "Sets the vsync mode.\nusage: vsync [on|off]")]
    public class VSyncCommand:Command
    {
        private IGraphicsManager _graphicsManager;

        public VSyncCommand()
        {
            this._graphicsManager = (IGraphicsManager)Engine.Instance.Game.Services.GetService(typeof(IGraphicsManager));
        }

        [DefaultCommand]
        public string Default(string[] @params)
        {
            return string.Format("Vsync is currently {0}.\nusage: vsync [on|off].",
                                 this._graphicsManager.VerticalSyncEnabled
                                     ? "on"
                                     : "off");
        }

        [Subcommand("on", "Sets vsync on.")]
        public string On(string[] @params)
        {
            this._graphicsManager.VerticalSyncEnabled = true;
            return "VSync on.";
        }

        [Subcommand("off", "Sets vsync off.")]
        public string Off(string[] @params)
        {
            this._graphicsManager.VerticalSyncEnabled = false;
            return "VSync off.";
        }
    }

    [Command("fullscreen", "Sets the fullscreen mode.\nusage: fullscreen [on|off]")]
    public class FullScreenCommand : Command
    {
        private IGraphicsManager _graphicsManager;

        public FullScreenCommand()
        {
            this._graphicsManager = (IGraphicsManager)Engine.Instance.Game.Services.GetService(typeof(IGraphicsManager));
        }

        [DefaultCommand]
        public string Default(string[] @params)
        {
            return string.Format("Fullscreen is currently {0}.\nusage: fullscreen [on|off].",
                                 this._graphicsManager.FullScreenEnabled
                                     ? "on"
                                     : "off");
        }

        [Subcommand("on", "Sets fullscreen on.")]
        public string On(string[] @params)
        {
            this._graphicsManager.FullScreenEnabled = true;
            return "Fullscreen on.";
        }

        [Subcommand("off", "Sets fullscreen off.")]
        public string Off(string[] @params)
        {
            this._graphicsManager.FullScreenEnabled = false;
            return "Fullscreen off.";
        }
    }
}
