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

    public override bool Collides(PhysicsObject other)
    {
        if (other is Ball ball)
        {
            float radii = Radius + ball.Radius;

            return Util.DistanceSquared(this, other) <= radii * radii;
        }
        return false;
    }

    public override void Update(float dt)
    {
        foreach(PhysicsObject other in Collisions)
        {
            Vector distance = Position - other.Position;
            Vector dv = Velocity - other.Velocity;

            Velocity -= distance * (distance * dv / distance.LengthSquared)
                * 2 * other.Mass / (Mass + other.Mass);
        }

        base.Update(dt);
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