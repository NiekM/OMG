using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class PhysicsObject
{
    public Vector Position, Velocity, Acceleration;
    public float Mass;

    public List<PhysicsObject> Collisions;

    public abstract float Collides(PhysicsObject other);

    public abstract void HandleCollision(PhysicsObject other);

    public virtual void Update(float dt)
    {
        Velocity += Acceleration * dt;
        Position += Velocity * dt;

        Collisions.Clear();
    }

    public abstract void Draw(ref Picture picture);
}

public struct Vector
{
    public float X, Y;

    public Vector(float x, float y)
    {
        (X, Y) = (x, y);
    }

    public float LengthSquared => this * this;

    public void Deconstruct(out float x, out float y)
    {
        x = X;
        y = Y;
    }

    public static Vector operator +(Vector first, Vector second)
    {
        return new Vector(first.X + second.X, first.Y + second.Y);
    }
    public static Vector operator -(Vector first, Vector second)
    {
        return new Vector(first.X - second.X, first.Y - second.Y);
    }
    public static float operator *(Vector first, Vector second)
    {
        return first.X * second.X + first.Y * second.Y;
    }
    public static Vector operator *(Vector vector, float multiplier)
    {
        return new Vector(vector.X * multiplier, vector.Y * multiplier);
    }
    public static Vector operator /(Vector vector, float divider)
    {
        return new Vector(vector.X / divider, vector.Y / divider);
    }
}