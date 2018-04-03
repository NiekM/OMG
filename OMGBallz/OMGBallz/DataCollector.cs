using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Windows.Forms;

public static class DataCollector
{
    static string path = "../../../../Data/";

    public static void CollectData(string name, Func<Scene> getScene, int iterations, double timeStep, int threads, int tests)
    {
        string directory = $"{path}{name}/";
        Directory.CreateDirectory(directory);

        Parallel.For(0, threads, (thread) =>
        {
            for (int test = 0; test < tests; test++)
            {
                using (StreamWriter file = new StreamWriter($"{directory}{name}{thread * tests + test}.txt"))
                {
                    Scene scene = getScene();

                    World world = scene.World();

                    double initial = world.Data();
                    for (int iteration = 0; iteration < iterations; iteration++)
                    {
                        if (iteration != 0)
                            file.Write(" ");

                        file.Write($"{ world.Data() / initial }");

                        world.Advance(timeStep);
                    }
                }
            }
        });

        using (StreamWriter file = new StreamWriter($"{directory}{name}Average.txt"))
        {
            float[] values = new float[iterations];

            int total = tests * threads;

            for (int test = 0; test < total; test++)
            {
                using (StreamReader streamReader = new StreamReader($"{directory}{name}{test}.txt"))
                {
                    float[] testValues = streamReader.ReadLine().Split().Select(s => float.Parse(s)).ToArray();

                    for(int i = 0; i < testValues.Length; i++)
                    {
                        values[i] += testValues[i];
                    }
                }
            }
            for (int i = 0; i < values.Length; i++)
            {
                values[i] /= total;
            }
            for (int i = 0; i < values.Length; i++)
            {
                if (i != 0)
                    file.Write(" ");

                file.Write($"{values[i]}");
            }
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