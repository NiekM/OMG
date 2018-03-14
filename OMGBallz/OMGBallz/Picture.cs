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

    public Vector TopLeft, BottomRight;
        
    public Picture((int x, int y) size, Vector topLeft, Vector bottomRight)
    {
        Size = size;

        TopLeft = topLeft;
        BottomRight = bottomRight;

        Clear();
    }

    public void Clear()
    {
        Bitmap = new Bitmap(Size.X, Size.Y);
    }

    public void Translate(Vector translation)
    {
        TopLeft += translation;
        BottomRight += translation;
    }
    public void Translate((int x, int y) translation)
    {
        double x = (double)translation.x / Size.X;
        double y = (double)translation.y / Size.Y;

        Vector size = BottomRight - TopLeft;

        Vector trans = new Vector(x * size.X, y * size.Y);

        Translate(trans);
    }
    public void Zoom(double scale, Vector position)
    {
        TopLeft = position - (position - TopLeft) * scale;
        BottomRight = position - (position - BottomRight) * scale;
    }

    public (int x, int y) CanvasPosition(Vector realPosition)
    {
        Vector relative = (realPosition - TopLeft) / (BottomRight - TopLeft);

        return ((int)(Size.X * relative.X), (int)(Size.Y * relative.Y));
    }

    public Vector RealPosition((int x, int y) canvasPosition)
    {
        Vector relative = new Vector((double)(canvasPosition.x) / Size.X, (double)(canvasPosition.y) / Size.Y);

        return TopLeft + new Vector((BottomRight.X - TopLeft.X) * relative.X, (BottomRight.Y - TopLeft.Y) * relative.Y);
    }

    public void DrawPixel(int x, int y, Color color)
    {
        if (x < 0 || x >= Bitmap.Width || y < 0 || y >= Bitmap.Height)
            return;
        Bitmap.SetPixel(x, y, color);
    }

    public void DrawCircle(Vector position, double radius, Color color)
    {
        Vector topLeft = position - new Vector(radius);
        Vector bottomRight = position + new Vector(radius);

        if (bottomRight.X < TopLeft.X || bottomRight.Y < TopLeft.Y
            || topLeft.X > BottomRight.X || topLeft.Y > BottomRight.Y)
            return;

        (int left, int top) = CanvasPosition(topLeft);
        (int right, int bottom) = CanvasPosition(bottomRight);

        for (int i = left; i <= right; i++)
            for (int j = top; j <= bottom; j++)
            {
                if ((RealPosition((i, j)) - position).LengthSquared <= radius * radius)
                    DrawPixel(i, j, color);
            }
    }

    public void DrawHorizontalDivider(double y, Color color)
    {
        int position = CanvasPosition(new Vector(y)).y;

        for (int i = 0; i <= Size.X; i++)
            DrawPixel(i, position, color);
    }
    public void DrawVerticalDivider(double x, Color color)
    {
        int position = CanvasPosition(new Vector(x)).x;

        for (int i = 0; i <= Size.Y; i++)
            DrawPixel(position, i, color);
    }
}