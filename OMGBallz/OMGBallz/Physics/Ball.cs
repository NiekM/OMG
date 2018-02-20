using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Ball : PhysicsObject
{
    public double Radius;

    public Vector velocityDifference = default(Vector);

    public Ball(double radius, Vector position, Vector velocity = default(Vector), Vector acceleration = default(Vector), double mass = 1)
    {
        Radius = radius;
        Position = position;
        Velocity = velocity;
        Acceleration = acceleration;
        Mass = mass;

        //Collisions = new List<PhysicsObject>();
    }

    //public override double Collides(PhysicsObject other)
    //{
    //    if (other is Ball ball)
    //    {
    //        double radii = Radius + ball.Radius;
    //        Vector dp = Position - other.Position;
    //        Vector dv = Velocity - other.Velocity;

    //        double a = dv.LengthSquared;
    //        double b = 2 * (dp * dv);
    //        double c = dp.LengthSquared - radii * radii;

    //        // ABC - formula
    //        double t = -(b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);

    //        if (t == 0 && b == 0) // If the balls graze eachother, they will not change velocity and keep finding the same collision.
    //            return double.PositiveInfinity;
    //        if (t >= 0)
    //            return t;
    //    }
    //    return double.PositiveInfinity;
    //}

    public override void HandleCollision(PhysicsObject other)
    {
        Vector dx = Position - other.Position;
        Vector dv = Velocity - other.Velocity;

        double mass = 2 * other.Mass / (Mass + other.Mass);

        Vector difference = -dx * (dx * dv) / dx.LengthSquared * mass;

        velocityDifference += difference;
    }

    public override void ApplyCollision()
    {
        Velocity += velocityDifference;
        velocityDifference = default(Vector);
    }

    public override void Draw(ref Picture picture)
    {
        int r = (int)Radius, x = (int)Position.X, y = (int)Position.Y;

        for (int i = -r; i <= r; i++)
            for (int j = -r; j <= r; j++)
            {
                if (i * i + j * j <= r * r)
                {
                    picture.Draw(x + i, y + j, Color.White);
                }
            }
    }
}