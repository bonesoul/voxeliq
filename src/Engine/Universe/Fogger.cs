/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using Engine.Chunks;
using Engine.Common.Logging;
using Engine.Debugging.Console;
using Microsoft.Xna.Framework;

namespace Engine.Universe
{
    public interface IFogger
    {
        /// <summary>
        /// Fog state.
        /// </summary>
        FogState State { get; set; }

        /// <summary>
        /// Fog vector value for current fog-state.
        /// </summary>
        Vector2 FogVector { get; }
    }

    public class Fogger : GameComponent, IFogger
    {
        public FogState State { get; set; }

        public Vector2 FogVector
        {
            get { return this._fogVectors[(byte) this.State]; }
        }

        // fog vectors.
        private readonly Vector2[] _fogVectors = new[]
        {
            new Vector2(0, 0), // none
            new Vector2(Chunk.WidthInBlocks*(ChunkCache.ViewRange - 7), Chunk.WidthInBlocks*(ChunkCache.ViewRange)), // near
            new Vector2(Chunk.WidthInBlocks*(ChunkCache.ViewRange - 2),Chunk.WidthInBlocks*(ChunkCache.ViewRange)) // far
        };

        private static readonly Logger Logger = LogManager.CreateLogger(); // logging-facility

        public Fogger(Game game)
            : base(game)
        {
            Logger.Trace("init()");
            this.State = FogState.None;
            this.Game.Services.AddService(typeof (IFogger), this);
        }
    }

    /// <summary>
    /// Fog state enum.
    /// </summary>
    public enum FogState : byte
    {
        None,
        Near,
        Far
    }

    [Command("fog", "Sets fog mode.\nusage: fog [off|near|far]")]
    public class FoggerCommand : Command
    {
        private IFogger _fogger;

        public FoggerCommand()
        {
            this._fogger = (IFogger)Core.Engine.Instance.Game.Services.GetService(typeof(IFogger));
        }

        [DefaultCommand]
        public string Default(string[] @params)
        {
            return string.Format("Fog is currently set to {0} mode.\nusage: fog [off|near|far]",
                                 this._fogger.State.ToString().ToLower());
        }

        [Subcommand("off", "Sets fog to off.")]
        public string Off(string[] @params)
        {
            this._fogger.State = FogState.None;
            return "Fog is off.";
        }

        [Subcommand("near", "Sets fog to near.")]
        public string Near(string[] @params)
        {
            this._fogger.State = FogState.Near;
            return "Fog is near.";
        }

        [Subcommand("far", "Sets fog to far.")]
        public string Far(string[] @params)
        {
            this._fogger.State = FogState.Far;
            return "Fog is far.";
        }
    }
}