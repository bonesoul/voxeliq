using System;
using System.Collections.Generic;
using SlimDX.Windows;
using VolumetricStudios.VoxeliqClient.Input;
using VolumetricStudios.VoxeliqEngine.Logging;

namespace VolumetricStudios.VoxeliqClient.Core
{
    public class Game : IDisposable
    {
        private readonly GameWindow _gameWindow; // The game render-form.
        private readonly List<IComponent> _components = new List<IComponent>(); // game components. 
        private readonly List<IDrawableComponent> _drawableComponents = new List<IDrawableComponent>(); // drawable game components. 

        private static readonly Logger Logger = LogManager.CreateLogger();
        
        public Game()
        {
            this._gameWindow = new GameWindow();    
            this._components.Add(new InputManager());
        }

        public void Run()
        {
            MessagePump.Run(this._gameWindow, this.MainLoop); // uses interop to directly call into Win32 methods to bypass any allocations on the managed side.
        }

        public void MainLoop()
        {
            this.UpdateComponents();
            this.DrawComponents();
            this._gameWindow.RenderFrame();
        }

        private void UpdateComponents()
        {
            for (int i = 0; i < this._components.Count; i++)
            {
                this._components[i].Update();
            }
        }

        private void DrawComponents()
        {
            for (int i = 0; i < this._drawableComponents.Count; i++)
            {
                this._drawableComponents[i].Draw();
            }
        }

        #region de-ctor

        // IDisposable pattern: http://msdn.microsoft.com/en-us/library/fs2xkftw%28VS.80%29.aspx

        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Take object out the finalization queue to prevent finalization code for it from executing a second time.
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed) return; // if already disposed, just return

            if (disposing) // only dispose managed resources if we're called from directly or in-directly from user code.
            { }           

            _disposed = true;
        }

        ~Game() { Dispose(false); } // finalizer called by the runtime. we should only dispose unmanaged objects and should NOT reference managed ones.
        
        #endregion
    }
}
