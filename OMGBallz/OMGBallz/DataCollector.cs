using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Text;

public static class DataCollector // TODO: only write average to a file
{
    static string path = "../../../../Data/";

    public static void CollectData(string name, Func<Scene> getScene, int iterations, double timeStep, int threads, int tests)
    {
        var bag = new ConcurrentBag<double[]>();

        Parallel.For(0, threads, (thread) =>
        {
            for (int test = 0; test < tests; test++)
            {
                Scene scene = getScene();

                World world = scene.World();

                double[] results = new double[iterations];

                double initial = world.Data();
                for (int iteration = 0; iteration < iterations; iteration++)
                {
                    double data = world.Data() / initial;
                    results[iteration] = data;
                    world.Advance(timeStep);
                }

                bag.Add(results);
            }
        });

        var average = new double[iterations];
        for (int i = 0; i < iterations; i++)
        {
            foreach(var item in bag)
            {
                average[i] += item[i];
            }
            average[i] /= bag.Count;
        }

        using (StreamWriter file = new StreamWriter($"{path}{name}.txt"))
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (double value in average)
            {
                stringBuilder.Append($"{value} ");
            }
            file.Write(stringBuilder.ToString());
        }
    }

    public static void CollectData(int iterations, double timeStep, int threads, int tests, params Simulation[] simulations)
    {
        foreach(var simulation in simulations)
        {
            CollectData(simulation.Name, () => Scene.MixScene(simulation.First, simulation.Second), iterations, timeStep, threads, tests);
        }
    }
}

public struct Simulation
{
    public string Name;
    public ParticleData First, Second;

    public Simulation(string name, ParticleData first, ParticleData second)
    {
        Name = name;
        First = first;
        Second = second;
    }
    public Simulation(string name, ParticleData data)
    {
        Name = name;
        First = data;
        Second = data;
    }
}