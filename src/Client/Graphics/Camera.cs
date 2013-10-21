/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Serilog;

namespace Client.Graphics
{
    /// <summary>
    /// Camera service.
    /// </summary>
    public interface ICameraService
    {
        /// <summary>
        /// Camera's projection matrix (lens).
        /// </summary>
        Matrix Projection { get; }

        /// <summary>
        /// Cameras view matrix (position).
        /// </summary>
        Matrix View { get; }

        /// <summary>
        /// The world matrix.
        /// </summary>
        Matrix World { get; }

        /// <summary>
        /// Camera's position.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// Current camera elevation.
        /// </summary>
        float CurrentElevation { get; }

        /// <summary>
        /// Current camera rotation.
        /// </summary>
        float CurrentRotation { get; }
    }

    public interface ICameraManService
    {
        /// <summary>
        /// Rotates camera in x-axis.
        /// </summary>
        /// <param name="step">The amount to rotate.</param>        
        void RotateCamera(float step);

        /// <summary>
        /// Elevates camera in y-axis.
        /// </summary>
        /// <param name="step">The amount to elavate</param>        
        void ElevateCamera(float step);

        /// <summary>
        /// Turns the camera to a given target.
        /// </summary>
        /// <param name="target">Target vector.</param>
        void LookAt(Vector3 target);   
    }

    /// <summary>
    /// The game camera.
    /// </summary>
    public class Camera : GameComponent, ICameraService, ICameraManService
    {
        // TODO add camera tests

        public Camera(Game game) 
            : base(game)
        {
            // export the services.
            game.Services.AddService(typeof(ICameraService), this);
            game.Services.AddService(typeof(ICameraManService), this);
        }

        public override void Initialize()
        {
            Log.Verbose("{0}.{1}", this.GetType().Name,  System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        #region camera service

        public Matrix Projection { get; private set; }
        public Matrix View { get; private set; }
        public Matrix World { get; private set; }
        public Vector3 Position { get; private set; }
        public float CurrentElevation { get; private set; }
        public float CurrentRotation { get; private set; }

        #endregion

        #region camera-man service

        public void RotateCamera(float step)
        {
            throw new NotImplementedException();
        }

        public void ElevateCamera(float step)
        {
            throw new NotImplementedException();
        }

        public void LookAt(Vector3 target)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
