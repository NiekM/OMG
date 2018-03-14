using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Ball : PhysicsObject
{
    public double Radius;

    public Ball(double radius, Vector position, Vector velocity = default(Vector), Vector acceleration = default(Vector), double density = 1)
    {
        Radius = radius;
        Position = position;
        Velocity = velocity;
        Acceleration = acceleration;
        Mass = Size * density;
    }

    public override double Size => Math.PI * Radius * Radius;

    public override void Draw(ref Picture picture)
    {
        picture.DrawCircle(Position, Radius, Color);
    }
}