using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Picture
{
    public Bitmap Bitmap;
    public (int X, int Y) Size;
    public (int X, int Y) Offset;
        
    public Picture(int x, int y)
    {
        Size = (x, y);
        Clear();
    }

    public void Clear()
    {
        Bitmap = new Bitmap(Size.X, Size.Y);
    }

    public void Draw(int x, int y, Color color)
    {
        x += Offset.X;
        y += Offset.Y;

        if (x < 0 || x >= Bitmap.Width || y < 0 || y >= Bitmap.Height)
            return;
        Bitmap.SetPixel(x, y, color);
    }
}