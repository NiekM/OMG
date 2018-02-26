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
        Mass = Math.PI * radius * radius * density;
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