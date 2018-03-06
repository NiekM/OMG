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

    public Color Color = Color.White;

    public virtual void Update(double dt)
    {
        // This is not correct. Updating position with velocity only works when the accelaration is 0.
        // Velocity += Acceleration * dt;
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

                            double diff = ball.Mass - hWall.Mass;
                            double div = 1 / (ball.Mass + hWall.Mass);

                            (ball.Velocity.Y, hWall.Velocity.Y) =
                                ( (ball.Velocity.Y * diff + 2 * hWall.Mass * hWall.Velocity.Y) * div
                                , (hWall.Velocity.Y * -diff + 2 * ball.Mass * ball.Velocity.Y) * div
                                );
                        }
                        break;
                    case VerticalWall vWall:
                        {
                            double diff = ball.Mass - vWall.Mass;
                            double div = 1 / (ball.Mass + vWall.Mass);

                            (ball.Velocity.X, vWall.Velocity.X) =
                                ( (ball.Velocity.X * diff + 2 * vWall.Mass * vWall.Velocity.X) * div
                                , (vWall.Velocity.X * -diff + 2 * ball.Mass * ball.Velocity.X) * div
                                );

                            if (vWall is VerticalWall.Speaker speaker)
                            {
                                speaker.Membrane.Velocity.X -= ball.Velocity.X;
                            }
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

        // With friction:
        //
        //first.Velocity *= 0.95;
        //second.Velocity *= 0.95;
        //
    }
    
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
                    case Membrane membrane: // Something something does not always find correct answer
                        {
                            int compare = ball.Position.X.CompareTo(membrane.Position.X);

                            double ballX = ball.Position.X - compare * ball.Radius;

                            double a = ball.Velocity.X * membrane.C;
                            double b = (ballX - membrane.Origin) * membrane.C;
                            double c = ball.Velocity.X - membrane.Velocity.X;
                            double d = ballX - membrane.Position.X;

                            double delta_0 = b * b - 3 * a * c;
                            double delta_1 = 2 * b * b * b - 9 * a * b * c + 27 * a * a * d;

                            double e = delta_1 * delta_1 - 4 * delta_0 * delta_0 * delta_0;

                            double delta = e / (-27 * a * a);

                            // (delta_1 +- Math.Sqrt(delta_1 * delta_1 - 4 * delta_0 * delta_0 * delta_0)) / 2;

                            double root = (delta_1 + Math.Sqrt(delta_1 * delta_1 - 4 * delta_0 * delta_0 * delta_0)) / 2;

                            double coefficient = Util.CubicRoot(root);

                            // +- (b + coefficient + delta_0 / coefficient) / (3 * a)

                            double x = -(b + coefficient + delta_0 / coefficient) / (3 * a);
                            
                            return x;
                        }
                    case HorizontalWall hWall:
                        {
                            double distance = hWall.Position.Y - ball.Position.Y;

                            if (distance > 0 && ball.Velocity.Y - hWall.Velocity.Y > 0)
                                return (distance - ball.Radius) / (ball.Velocity.Y - hWall.Velocity.Y);
                            else if (distance < 0 && ball.Velocity.Y - hWall.Velocity.Y < 0)
                                return (distance + ball.Radius) / (ball.Velocity.Y - hWall.Velocity.Y);
                            else
                                return double.PositiveInfinity;
                        }
                    case VerticalWall vWall:
                        {
                            double distance = vWall.Position.X - ball.Position.X;

                            if (distance > 0 && ball.Velocity.X - vWall.Velocity.X > 0)
                                return (distance - ball.Radius) / (ball.Velocity.X - vWall.Velocity.X);
                            else if (distance < 0 && ball.Velocity.X - vWall.Velocity.X < 0)
                                return (distance + ball.Radius) / (ball.Velocity.X - vWall.Velocity.X);
                            else
                                return double.PositiveInfinity;
                        }
                } break;
            case Membrane membrane:
                switch(second)
                {
                    case Ball ball:
                        return Await(second, first);
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
    public Vector(double xy) : this(xy, xy) { }

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
    public static Vector operator /(Vector first, Vector second)
    {
        return new Vector(first.X / second.X, first.Y / second.Y);
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