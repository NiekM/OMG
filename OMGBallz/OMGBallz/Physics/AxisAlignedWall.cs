using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class AxisAlignedWall : PhysicsObject
{
    public override double Size => 0;
}

public class HorizontalWall : AxisAlignedWall
{
    public HorizontalWall(double y)
    {
        Position.Y = y;
    }

    public override void Draw(ref Picture picture)
    {
        picture.DrawHorizontalDivider(Position.Y, Color);
    }
}

public class VerticalWall : AxisAlignedWall
{
    public VerticalWall(double x)
    {
        Position.X = x;
    }

    public override void Draw(ref Picture picture)
    {
        picture.DrawVerticalDivider(Position.X, Color);
    }

    public class Speaker : VerticalWall
    {
        public Membrane Membrane;

        public Speaker(double x, double c = 1e-3) : base(x)
        {
            Membrane = new Membrane(x, c)
                { Color = Color.Aquamarine
                , Mass = Mass
                };
        }

        public override void Update(double dt)
        {
            base.Update(dt);

            Membrane.Update(dt);
        }

        public override void Draw(ref Picture picture)
        {
            base.Draw(ref picture);

            Membrane.Draw(ref picture);
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
    }
}