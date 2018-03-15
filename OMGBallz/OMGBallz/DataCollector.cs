using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Windows.Forms;

public static class DataCollector
{
    static string path = "../../../../Data/";

    public static void CollectData(string name, Func<Scene> getScene, double time, double interval, int threads, int tests)
    {
        Parallel.For(0, threads, (thread) =>
        {
            for (int test = 0; test < tests; test++)
            {
                using (StreamWriter file = new StreamWriter($"{path}{name}{thread * tests + test}.txt"))
                {
                    Scene scene = getScene();

                    World world = scene.World();

                    double initial = scene.Data();

                    for (double t = 0; t < time; t += interval)
                    {
                        if (t != 0)
                            file.Write(" ");

                        file.Write($"{ scene.Data() / initial }");

                        world.Advance(interval);
                    }
                }
            }
        });

        using (StreamWriter file = new StreamWriter($"{path}{name}Average.txt"))
        {
            float[] values = new float[(int)(time / interval)];

            int total = tests * threads;

            for (int test = 0; test < total; test++)
            {
                using (StreamReader streamReader = new StreamReader($"{path}{name}{test}.txt"))
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
}