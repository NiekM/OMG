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

    public virtual void Update(double dt)
    {
        // This is not correct. Updating position with velocity only works when the accelaration is 0.
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
            return Find(First, Second);
        }
        else
        {
            Time -= dt;
        }

        return this;
    }

    public static void Execute(PhysicsObject first, PhysicsObject second)
    {
        switch (first)
        {
            case Ball ball:
                switch (second)
                {
                    case Ball other:
                        {
                            Vector dx = ball.Position - other.Position;
                            Vector dv = ball.Velocity - other.Velocity;

                            Vector diff = dx * 2 * (dx * dv) / (dx.LengthSquared * (ball.Mass + other.Mass));

                            ball.Velocity -= diff * other.Mass;
                            other.Velocity += diff * ball.Mass;
                        }
                        break;
                    case HorizontalWall hWall:
                        {
                            double newBallY = (ball.Velocity.Y * (ball.Mass - hWall.Mass) + 2 * hWall.Mass * hWall.Velocity.Y) / (ball.Mass + hWall.Mass);
                            double newWallY = (hWall.Velocity.Y * (hWall.Mass - ball.Mass) + 2 * ball.Mass * ball.Velocity.Y) / (ball.Mass + hWall.Mass);

                            ball.Velocity.Y = newBallY;
                            hWall.Velocity.Y = newWallY;
                        }
                        break;
                    case VerticalWall vWall:
                        {
                            double newBallX = (ball.Velocity.X * (ball.Mass - vWall.Mass) + 2 * vWall.Mass * vWall.Velocity.X) / (ball.Mass + vWall.Mass);
                            double newWallX = (vWall.Velocity.X * (vWall.Mass - ball.Mass) + 2 * ball.Mass * ball.Velocity.X) / (ball.Mass + vWall.Mass);

                            ball.Velocity.X = newBallX;
                            vWall.Velocity.X = newWallX;

                            //var (x, y) = ball.Velocity;
                            //ball.Velocity = new Vector(-x + vWall.Velocity.X, y);
                        }
                        break;
                }
                break;
            case HorizontalWall hWall:
                switch (second)
                {
                    case Ball ball:
                        Execute(second, first);
                        break;
                }
                break;
            case VerticalWall vWall:
                switch (second)
                {
                    case Ball ball:
                        Execute(second, first);
                        break;
                }
                break;
        }
    }

    // NOOOOOOOOO! Stupid precision errors... Now objects move through eachother
    // depending on the speed when they are too close...
    public static Collision Find(PhysicsObject first, PhysicsObject second)
    {
        double time = Await(first, second);

        if (time < 0 || double.IsNaN(time))
        {
            time = double.PositiveInfinity;
        }

        return new Collision(time, first, second);
    }

    public static double Await(PhysicsObject first, PhysicsObject second)
    {
        switch(first)
        {
            case Ball ball:
                switch (second)
                {
                    case Ball other:

                        double time;
                        { 
                            double radii = ball.Radius + other.Radius;
                            Vector dp = ball.Position - other.Position;
                            Vector dv = ball.Velocity - other.Velocity;

                            double a = dv.LengthSquared;
                            double b = 2 * (dp * dv);
                            double c = dp.LengthSquared - radii * radii;

                            // ABC - formula
                            time = -(b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);

                            /* If the balls graze eachother, 
                             * they will not change velocity and 
                             * keep finding the same collision
                             */
                            if (time == 0 && b == 0)
                                return double.PositiveInfinity;
                            else
                                return time;
                        }
                    case HorizontalWall hWall:
                        {
                            double distance = hWall.Y - ball.Position.Y;

                            if (distance > 0 && ball.Velocity.Y - hWall.Velocity.Y > 0)
                                return (distance - ball.Radius) / (ball.Velocity.Y - hWall.Velocity.Y);
                            else if (distance < 0 && ball.Velocity.Y - hWall.Velocity.Y < 0)
                                return (distance + ball.Radius) / (ball.Velocity.Y - hWall.Velocity.Y);
                            else
                                return double.PositiveInfinity;
                        }
                    case VerticalWall vWall:
                        {
                            double distance = vWall.X - ball.Position.X;

                            if (distance > 0 && ball.Velocity.X - vWall.Velocity.X > 0)
                                return (distance - ball.Radius) / (ball.Velocity.X - vWall.Velocity.X);
                            else if (distance < 0 && ball.Velocity.X - vWall.Velocity.X < 0)
                                return (distance + ball.Radius) / (ball.Velocity.X - vWall.Velocity.X);
                            else
                                return double.PositiveInfinity;
                        }
                } break;
            case HorizontalWall hWall:
                switch(second)
                {
                    case Ball ball:
                        return Await(second, first);
                } break;
            case VerticalWall vWall:
                switch (second)
                {
                    case Ball ball:
                        return Await(second, first);
                }
                break;
        }

        return double.PositiveInfinity;
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