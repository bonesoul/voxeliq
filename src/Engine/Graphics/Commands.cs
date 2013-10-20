/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Debugging.Console;

namespace Engine.Graphics
{
    [Command("vsync", "Sets the vsync mode.\nusage: vsync [on|off]")]
    public class VSyncCommand:Command
    {
        private IGraphicsManager _graphicsManager;

        public VSyncCommand()
        {
            this._graphicsManager = (IGraphicsManager)Core.Engine.Instance.Game.Services.GetService(typeof(IGraphicsManager));
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
            this._graphicsManager.EnableVerticalSync(true);
            return "VSync on.";
        }

        [Subcommand("off", "Sets vsync off.")]
        public string Off(string[] @params)
        {
            this._graphicsManager.EnableVerticalSync(false);
            return "VSync off.";
        }
    }

    [Command("fullscreen", "Sets the fullscreen mode.\nusage: fullscreen [on|off]")]
    public class FullScreenCommand : Command
    {
        private IGraphicsManager _graphicsManager;

        public FullScreenCommand()
        {
            this._graphicsManager = (IGraphicsManager)Core.Engine.Instance.Game.Services.GetService(typeof(IGraphicsManager));
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
            this._graphicsManager.EnableFullScreen(true);
            return "Fullscreen on.";
        }

        [Subcommand("off", "Sets fullscreen off.")]
        public string Off(string[] @params)
        {
            this._graphicsManager.EnableFullScreen(false);
            return "Fullscreen off.";
        }
    }
}
