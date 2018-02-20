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

    public List<PhysicsObject> Collisions;

    public abstract double Collides(PhysicsObject other);

    public abstract void HandleCollision(PhysicsObject other);

    public abstract void ApplyCollision();

    public virtual void Update(double dt)
    {
        Velocity += Acceleration * dt;
        Position += Velocity * dt;

        Collisions.Clear();
    }

    public abstract void Draw(ref Picture picture);
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