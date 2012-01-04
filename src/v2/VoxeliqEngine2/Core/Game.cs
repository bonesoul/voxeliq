using System;
using System.Collections.Generic;
using System.Threading;
using SlimDX.Windows;
using VolumetricStudios.VoxeliqEngine.Input;
using VolumetricStudios.VoxeliqEngine.Screen;
using VolumetricStudios.VoxeliqEngine.Utility.Logging;

namespace VolumetricStudios.VoxeliqEngine.Core
{
    public class Game : IDisposable
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>(); // client-provided services.
        private readonly List<GameComponent> _components = new List<GameComponent>(); // game components. 

        private readonly RenderWindow _renderWindow; // The game render-form.

        // gametime, frame-limiter code by XNA 4.0 framework and http://code.google.com/p/slimdx/source/browse/trunk/samples/SampleFramework/Core/Game.cs?r=784
        private GameTime gameTime = new GameTime();
        private GameClock clock = new GameClock();
        private TimeSpan totalGameTime;
        private TimeSpan inactiveSleepTime = TimeSpan.FromMilliseconds(20.0);
        private readonly TimeSpan maximumElapsedTime = TimeSpan.FromMilliseconds(500.0);
        private TimeSpan lastFrameElapsedGameTime;
        private bool forceElapsedTimeToZero;
        private bool drawRunningSlowly;
        private int updatesSinceRunningSlowly1 = int.MaxValue;
        private int updatesSinceRunningSlowly2 = int.MaxValue;
        private TimeSpan targetElapsedTime = TimeSpan.FromTicks(166667);
        private TimeSpan accumulatedElapsedGameTime = TimeSpan.Zero;

        private long lastUpdateFrame;
        private float lastUpdateTime;

        public bool IsRunning { get; protected set; }
        public bool IsExiting { get; protected set; }
        public bool IsActive { get; protected set; }
        public bool IsFixedTimeStep { get; protected set; }
  
        private static readonly Logger Logger = LogManager.CreateLogger();

        public Game()
        {
            this.IsRunning = false;
            this.IsExiting = false;
            this.IsFixedTimeStep = false;

            this._renderWindow = new RenderWindow(this);
            this._renderWindow.AppActivated += (sender, e) =>
            {
                if(!this.IsActive)
                    this.IsActive = true;
            };
            this._renderWindow.AppDeactivated += (sender, e) =>
            {
                if (this.IsActive)
                    this.IsActive = false;
            };
        }


        private void Initialize()
        {
            Keyboard.Initialize();
            Mouse.Initialize();

            foreach (var component in this._components)
            {
                component.Initialize();
            }
        }

        /// <summary>
        /// Runs the game.
        /// </summary>
        public void Run()
        {
            this.Initialize();

            this.IsRunning = true;
            this.IsExiting = false;

            this.Update(this.gameTime);
            MessagePump.Run(this._renderWindow, this.Tick); // uses interop to directly call into Win32 methods to bypass any allocations on the managed side.
        }

        /// <summary>
        /// Allows the game to perform logic processing.
        /// </summary>
        protected virtual void Update(GameTime xgameTime)
        {
            foreach (var component in this._components) // update & draw components
            {
                component.Update(xgameTime); // update the component.
            }
        }

        /// <summary>
        /// Performs one complete frame for the game.
        /// </summary>
        private void Tick()
        {
            if(this.IsExiting)
                return;
            
            if(!this.IsActive)
                Thread.Sleep((int) this.inactiveSleepTime.TotalMilliseconds);

            this.clock.Step();

            var elapsedAdjustedTime = this.clock.ElapsedAdjustedTime;
            if (elapsedAdjustedTime < TimeSpan.Zero)
                elapsedAdjustedTime = TimeSpan.Zero;

            if (this.forceElapsedTimeToZero)
            {
                elapsedAdjustedTime = TimeSpan.Zero;
                this.forceElapsedTimeToZero = false;
            }

            // cap the adjusted time
            if (elapsedAdjustedTime > this.maximumElapsedTime)
                elapsedAdjustedTime = this.maximumElapsedTime;


            if (!this.IsFixedTimeStep)
            {
                this.drawRunningSlowly = false;
                this.updatesSinceRunningSlowly1 = int.MaxValue;
                this.updatesSinceRunningSlowly2 = int.MaxValue;

                if (this.IsExiting)
                    return;

                this.gameTime.ElapsedGameTime = this.lastFrameElapsedGameTime = elapsedAdjustedTime;
                this.gameTime.TotalGameTime = this.totalGameTime;
                this.gameTime.IsRunningSlowly = false;
                this.Update(this.gameTime);
                this.totalGameTime += elapsedAdjustedTime;
            }
            else
            {
                if (Math.Abs(elapsedAdjustedTime.Ticks - this.targetElapsedTime.Ticks) < (this.targetElapsedTime.Ticks >> 6)) elapsedAdjustedTime = this.targetElapsedTime;

                this.accumulatedElapsedGameTime += elapsedAdjustedTime;
                long num = this.accumulatedElapsedGameTime.Ticks/this.targetElapsedTime.Ticks;
                this.accumulatedElapsedGameTime =
                    TimeSpan.FromTicks(this.accumulatedElapsedGameTime.Ticks%this.targetElapsedTime.Ticks);
                this.lastFrameElapsedGameTime = TimeSpan.Zero;

                if (num == 0)
                    return;

                if (num > 1)
                {
                    this.updatesSinceRunningSlowly2 = this.updatesSinceRunningSlowly1;
                    this.updatesSinceRunningSlowly1 = 0;
                }

                else
                {
                    if (this.updatesSinceRunningSlowly1 < int.MaxValue)
                        this.updatesSinceRunningSlowly1++;
                    if (this.updatesSinceRunningSlowly2 < int.MaxValue)
                        this.updatesSinceRunningSlowly2++;
                }

                this.drawRunningSlowly = this.updatesSinceRunningSlowly2 < 20;

                while ((num > 0) && !this.IsExiting)
                {
                    num -= 1;
                    try
                    {
                        this.gameTime.ElapsedGameTime = targetElapsedTime;
                        this.gameTime.TotalGameTime = this.totalGameTime;
                        this.gameTime.IsRunningSlowly = this.drawRunningSlowly;
                        this.Update(this.gameTime);
                    }
                    finally
                    {
                        this.lastFrameElapsedGameTime += targetElapsedTime;
                        this.totalGameTime += targetElapsedTime;
                    }
                }
            }

            this.DrawFrame();            
            this.UpdateFPS();
        }

        private void UpdateFPS()
        {
            // refresh the FPS counter once per second
            lastUpdateFrame++;

            if ((float) clock.CurrentTime.TotalSeconds - lastUpdateTime <= 1.0f) return;

            gameTime.FramesPerSecond = (float)lastUpdateFrame / (float)(clock.CurrentTime.TotalSeconds - lastUpdateTime);
            lastUpdateTime = (float)clock.CurrentTime.TotalSeconds;
            lastUpdateFrame = 0;
        }
     
        private void DrawFrame()
        {
            this.gameTime.TotalGameTime = this.totalGameTime;
            this.gameTime.ElapsedGameTime = this.lastFrameElapsedGameTime;
            this.gameTime.IsRunningSlowly = this.drawRunningSlowly;
            this.DrawComponents(this.gameTime);
            this.lastFrameElapsedGameTime = TimeSpan.Zero;

            this._renderWindow.RenderFrame();
        }

        private void DrawComponents(GameTime xgameTime)
        {
            foreach (var component in this._components) // draw components
            {
                if(!component.Drawable)
                    continue;

                component.Draw(xgameTime); // draw the component.
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

        public object GetService(Type serviceType)
        {
            foreach (var pair in this._services)
            {
                if ((Type)pair.Key == serviceType)
                    return pair.Value;
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