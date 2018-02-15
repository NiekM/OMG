using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Picture
{
    public Bitmap Bitmap;

    public Picture(int x, int y)
    {
        Bitmap = new Bitmap(x, y);
    }

    public void Draw(int x, int y, Color color)
    {
        if (x < 0 || x >= Bitmap.Width || y < 0 || y >= Bitmap.Height)
            return;
        Bitmap.SetPixel(x, y, color);
    }
}