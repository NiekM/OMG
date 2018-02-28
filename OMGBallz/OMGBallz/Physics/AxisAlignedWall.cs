using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class AxisAlignedWall : PhysicsObject
{ }

public class HorizontalWall : AxisAlignedWall
{
    public double Y
    {
        get { return Position.Y; }
        set { Position.Y = value; }
    }

    public HorizontalWall(double y)
    {
        Y = y;
    }

    public override void Draw(ref Picture picture)
    {
        int y = (int)Y;

        for (int i = 0; i < picture.Bitmap.Width; i++)
        {
            picture.Draw(i - picture.Offset.X, y, Color);
        }
    }
}

public class VerticalWall : AxisAlignedWall
{
    public double X
    {
        get { return Position.X; }
        set { Position.X = value; }
    }

    public VerticalWall(double x)
    {
        X = x;
    }

    public override void Draw(ref Picture picture)
    {
        int x = (int)X;

        for (int i = 0; i < picture.Bitmap.Height; i++)
        {
            picture.Draw(x, i - picture.Offset.Y, Color);
        }
    }
}

public class Membrane : VerticalWall
{
    public double Origin;
    public double C;

    public Membrane(double x, double c) : base(x)
    {
        Origin = x;
        C = c;
    }

    public override void Update(double dt)
    {
        double tSquared = dt * dt;

        (Position.X, Velocity.X) =
            ( (Position.X / tSquared + Velocity.X / dt + Origin * C) / (1 / tSquared + C)
            //, Velocity.X + (Origin - Position.X) * C * dt
            , Velocity.X / (1 + C * tSquared) + (Origin - Position.X) * C / (1 / dt + C * dt)
            );

        if (double.IsNaN(Position.X))
            throw new Exception();
    }
}