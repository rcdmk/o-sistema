using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Jogo.Componentes
{
    class ProfilerComponent : GameComponent
    {
        double totalTime;

        public ProfilerComponent(Game game)
            : base(game)
        { }

        public override void Update(GameTime gameTime)
        {
            totalTime += gameTime.ElapsedGameTime.TotalSeconds;

            if (totalTime >= 5)
            {
                foreach (Profiler profiler in Profiler.AllProfilers)
                    profiler.Print(totalTime);

                Trace.WriteLine(string.Empty);
                totalTime = 0;
            }
        }
    }
}
