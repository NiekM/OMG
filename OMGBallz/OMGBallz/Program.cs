using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);


        //Application.Run(new Visualizer(Scene.MixScene
        //    (new ParticleData(300, 1, 5, 3, energy: 10))
        //));
        //Application.Run(new Visualizer(Scene.MixScene
        //    (new ParticleData(900, 1000, 5, 2, energy: 10)
        //    , new ParticleData(200, 1, 8, 4)
        //    )
        //));
        //Application.Run(new Visualizer(Scene.MixScene
        //    (new ParticleData(50, 1, 12, 5))
        //));
        //Application.Run(new Visualizer(Scene.MixScene
        //    ( new ParticleData(900, 1000, 5, 2, energy: 10)
        //    , new ParticleData(200, 1, 8, 4, energy: 5)
        //    )
        //));
        //Application.Run(new Visualizer(Scene.MixScene
        //    (new ParticleData(900, 1000, 5, 2, energy: 10)
        //    , new ParticleData(200, 1, 8, 4, energy: 5)
        //    )
        //));

        //List<Simulation> simulations = new List<Simulation>();

        //for (int i = 1; i <= 5; i++)
        //{
        //    int size = 200 * i - 100;
        //    simulations.Add(new Simulation($"Equalmass_{size}_{size}", new ParticleData(size, 1, 5, 2)));
        //}

        //DataCollector.CollectData(iterations: 100, timeStep: 10, threads: 10, tests: 50, simulations: simulations.ToArray());

        DataCollector.CollectData(iterations: 100, timeStep: 10f, threads: 10, tests: 50, simulations: new []
            { new Simulation("MassEXTREME", new ParticleData(300, 1, 6, 3), new ParticleData(300, 1E4, 6, 3))
            }
        );
    }
}