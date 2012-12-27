using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VoxeliqEngine.Debugging.Graphs
{
    public interface IStatsGraphs
    {
        void AddGraph(string name, ref int valueToObserve);
    }

    public class StatsGraphs : DrawableGameComponent, IStatsGraphs
    {
        // required services
        private IStatistics _statistics;

        public StatsGraphs(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IStatsGraphs), this); // export the service.
        }

        public override void Initialize()
        {
            // import required services.
            this._statistics = (IStatistics)this.Game.Services.GetService(typeof(IStatistics));

            this.Game.Components.Add(new Graph(this.Game));
        }

        public void AddGraph(string name, ref int valueToObserve)
        {
            throw new NotImplementedException();
        }
    }
}
