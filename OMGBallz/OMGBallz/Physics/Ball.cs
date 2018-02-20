using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Ball : PhysicsObject
{
    public float Radius;

    public Ball(float radius, Vector position, Vector velocity = default(Vector), Vector acceleration = default(Vector), float mass = 1)
    {
        Radius = radius;
        Position = position;
        Velocity = velocity;
        Acceleration = acceleration;
        Mass = mass;

        Collisions = new List<PhysicsObject>();
    }

    public override float Collides(PhysicsObject other)
    {
        if (other is Ball ball)
        {
            float radii = Radius + ball.Radius;
            Vector dp = Position - other.Position;
            Vector dv = Velocity - other.Velocity;

            float a = dv.LengthSquared;
            float b = 2 * (dp * dv);
            float c = dp.LengthSquared - radii * radii;

            // ABC - formula
            float t = -(b + (float)Math.Sqrt(b * b - 4 * a * c)) / (2 * a);

            if (t >= 0)
                return t;
        }
        return float.PositiveInfinity;
    }

    public override void HandleCollision(PhysicsObject other)
    {
        Vector dx = Position - other.Position;
        Vector dv = Velocity - other.Velocity;

        float mass = 2 * other.Mass / (Mass + other.Mass);

        Velocity -= dx * (dx * dv) / dx.LengthSquared * mass;
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