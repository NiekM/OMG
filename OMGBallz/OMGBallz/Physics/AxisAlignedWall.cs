using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class AxisAlignedWall : PhysicsObject
{
}

public class HorizontalWall : AxisAlignedWall
{
    public double Y;

    public HorizontalWall(double y)
    {
        Y = y;
    }

    public override void Draw(ref Picture picture)
    {
        int y = (int)Y;

        for (int i = 0; i < picture.Bitmap.Width; i++)
        {
            picture.Draw(i - picture.Offset.X, y, Color.White);
        }
    }
}

public class VerticalWall : AxisAlignedWall
{
    public double X;

    public VerticalWall(double x)
    {
        X = x;
    }

    public override void Draw(ref Picture picture)
    {
        int x = (int)X;

        for (int i = 0; i < picture.Bitmap.Height; i++)
        {
            picture.Draw(x, i - picture.Offset.Y, Color.White);
        }
    }
}