using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class PhysicsObject
{
    public Vector Position, Velocity, Acceleration;
    public double Mass;

    public static double Collides(PhysicsObject first, PhysicsObject second)
    {
        double t = double.PositiveInfinity;

        if (first is Ball ball)
        {
            if (second is Ball other)
            {
                double radii = ball.Radius + other.Radius;
                Vector dp = ball.Position - other.Position;
                Vector dv = ball.Velocity - other.Velocity;

                double a = dv.LengthSquared;
                double b = 2 * (dp * dv);
                double c = dp.LengthSquared - radii * radii;

                // ABC - formula
                t = -(b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);

                if (t == 0 && b == 0) // If the balls graze eachother, they will not change velocity and keep finding the same collision.
                    return double.PositiveInfinity;
            }
            else if (second is HorizontalWall horizontalWall)
            {
                double distance = horizontalWall.Y - ball.Position.Y;

                if (distance > 0 && ball.Velocity.Y > 0)
                    t = (distance - ball.Radius) / ball.Velocity.Y;
                else if (distance < 0 && ball.Velocity.Y < 0)
                    t = (distance + ball.Radius) / ball.Velocity.Y;
            }
            else if (second is VerticalWall verticalWall)
            {
                double distance = verticalWall.X - ball.Position.X;

                if (distance > 0 && ball.Velocity.X > 0)
                    t = (distance - ball.Radius) / ball.Velocity.X;
                else if (distance < 0 && ball.Velocity.X < 0)
                    t = (distance + ball.Radius) / ball.Velocity.X;
            }
        }

        if (t >= 0)
            return t;
        else
            return double.PositiveInfinity;
    }

    public static void HandleCollision(PhysicsObject first, PhysicsObject second)
    {
        if (first is Ball ball)
        {
            if (second is Ball other)
            {
                Vector dx = ball.Position - other.Position;
                Vector dv = ball.Velocity - other.Velocity;
                
                Vector diff = dx * 2 * (dx * dv) / (dx.LengthSquared * (ball.Mass + other.Mass));

                ball.Velocity -= diff * other.Mass;
                other.Velocity += diff * ball.Mass;
            }
            else if (second is HorizontalWall)
            {
                var (x, y) = ball.Velocity;
                ball.Velocity = new Vector(x, -y);
            }
            else if (second is VerticalWall)
            {
                var (x, y) = ball.Velocity;
                ball.Velocity = new Vector(-x, y);
            }
        }
    }

    public virtual void Update(double dt)
    {
        Velocity += Acceleration * dt;
        Position += Velocity * dt;
    }

    public abstract void Draw(ref Picture picture);
}

public struct Collision : IComparable<Collision>
{
    public double Time;
    public PhysicsObject First, Second;

    public Collision(double time, PhysicsObject first, PhysicsObject second)
    {
        Time = time;
        First = first;
        Second = second;
    }

    public int CompareTo(Collision other)
    {
        return Time.CompareTo(other.Time);
    }

    public bool Contains(params PhysicsObject[] objects)
    {
        foreach (var obj in objects)
        {
            if (obj == First || obj == Second)
            {
                return true;
            }
        }
        return false;
    }

    public Collision Update(double dt, bool disturbed)
    {
        if (disturbed)
        {
            Time = PhysicsObject.Collides(First, Second);
        }
        else
        {
            Time -= dt;
        }

        return this;
    }
}

public struct Vector
{
    public double X, Y;

    public Vector(double x, double y)
    {
        (X, Y) = (x, y);
    }

    public double LengthSquared => this * this;

    public void Deconstruct(out double x, out double y)
    {
        x = X;
        y = Y;
    }

    public static Vector operator -(Vector vector)
    {
        return new Vector(-vector.X, -vector.Y);
    }
    public static Vector operator +(Vector first, Vector second)
    {
        return new Vector(first.X + second.X, first.Y + second.Y);
    }
    public static Vector operator -(Vector first, Vector second)
    {
        return new Vector(first.X - second.X, first.Y - second.Y);
    }
    public static double operator *(Vector first, Vector second)
    {
        return first.X * second.X + first.Y * second.Y;
    }
    public static Vector operator *(Vector vector, double multiplier)
    {
        return new Vector(vector.X * multiplier, vector.Y * multiplier);
    }
    public static Vector operator /(Vector vector, double divider)
    {
        return new Vector(vector.X / divider, vector.Y / divider);
    }
}