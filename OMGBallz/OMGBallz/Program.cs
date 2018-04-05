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

        //Application.Run(new Visualizer(Scene.MixScene(new ParticleData(400, 1, 5, 3, energy: 10))));

        DataCollector.CollectData(iterations: 10, timeStep: 10f, threads: 10, tests: 10, simulations: new Simulation[]
            { new Simulation("equal", new ParticleData(10, 1, 3, 3))
            , new Simulation("nequal", new ParticleData(4, 1, 8, 4), new ParticleData(12, 1, 4, 3))
            }
        );
    }
}