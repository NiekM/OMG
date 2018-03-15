using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;

class Plotter : Form
{
    Picture picture;
    PictureBox pictureBox;

    public Plotter()
    {
        Size = new Size(600, 600);

        pictureBox = new PictureBox
            { BackColor = Util.Colors.DarkPurple
            , Size = Size
            };

        picture = new Picture((Size.Width, Size.Height), new Vector(-0.2), new Vector(1.2));
        pictureBox.Image = picture.Bitmap;

        Controls.Add(pictureBox);

        picture.DrawHorizontalDivider(1, Color.White);
        picture.DrawVerticalDivider(0, Color.White);
    }

    public void Plot(string path, Color color)
    {
        using (StreamReader streamReader = new StreamReader($"{path}"))
        {
            float[] testValues = streamReader.ReadLine().Split().Select(s => float.Parse(s)).ToArray();

            Plot(testValues, color);
        }
    }

    public void Plot(float[] values, Color color)
    {
        for (int i = 0; i < values.Length; i++)
        {
            picture.DrawCircle(new Vector((double)i / values.Length, 1 - values[i]), 0.01f, color);
        }
    }
}