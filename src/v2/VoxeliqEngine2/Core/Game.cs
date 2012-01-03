using System;
using System.Collections.Generic;
using SlimDX.Windows;
using VolumetricStudios.VoxeliqEngine.Graphics.Rendering;
using VolumetricStudios.VoxeliqEngine.Input;
using VolumetricStudios.VoxeliqEngine.Utils.Logging;

namespace VolumetricStudios.VoxeliqEngine.Core
{
    public class Game : IDisposable
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>(); // client-provided services.
        private readonly List<GameComponent> _components = new List<GameComponent>(); // game components. 

        private readonly RenderWindow _gameWindow; // The game render-form.        

        private static readonly Logger Logger = LogManager.CreateLogger();

        public Game()
        {
            this._gameWindow = new RenderWindow();            
        }

        public void Run()
        {
            this.Initialize();
            MessagePump.Run(this._gameWindow, this.MainLoop); // uses interop to directly call into Win32 methods to bypass any allocations on the managed side.
        }

        private void MainLoop()
        {
            foreach (var component in this._components) // update & draw components
            {
                component.Update(); // update the component.

                if (!component.Drawable) // if it's not drawable
                    continue; // just skip.

                component.Draw(); // draw the component.
            }

            this._gameWindow.RenderFrame();
        }

        private void Initialize()
        {
            Keyboard.Initialize();
            Mouse.Initialize();

            foreach(var component in this._components)
            {
                component.Initialize();
            }
        }

        public void AddComponent(GameComponent component)
        {
            this._components.Add(component);
        }

        public void AddService(Type serviceType, object provider)
        {
            this._services.Add(serviceType, provider);
        }

        public IGameService GetService(Type serviceType)
        {
            foreach (var pair in this._services)
            {
                if ((Type)pair.Key == serviceType)
                    return (IGameService)pair.Value;
            }

            return null;
        }

        #region de-ctor

        // IDisposable pattern: http://msdn.microsoft.com/en-us/library/fs2xkftw%28VS.80%29.aspx

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
                // Take object out the finalization queue to prevent finalization code for it from executing a second time.
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed) return; // if already disposed, just return

            if (disposing)
                // only dispose managed resources if we're called from directly or in-directly from user code.
            {
            }

            _disposed = true;
        }

        ~Game()
        {
            Dispose(false);
        }

        // finalizer called by the runtime. we should only dispose unmanaged objects and should NOT reference managed ones.

        #endregion
    }
}