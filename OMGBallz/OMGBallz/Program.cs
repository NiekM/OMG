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

#if true
        Application.Run(new Visualizer());

        if (true)
        {
            Plotter averagePlotter = new Plotter();

            string file(string name) => $"../../../../Data/equalWeight{ name }.txt";

            for (int i = 0; i < 10; i++)
                averagePlotter.Plot(file($"{i}"), Util.Colors.Pink);

            averagePlotter.Plot(file("Average"), Color.White);

            Application.Run(averagePlotter);
        }
#else
        DataCollector.CollectData("equalWeight", () => Scene.Mix, 1000, 10, 10, 1);
#endif

    }
}