using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Util
{
    public static double CubicRoot(double x)
    {
        return (x < 0) ? -Math.Pow(-x, 1.0 / 3) : Math.Pow(x, 1.0 / 3);
    }

    public static double DistanceSquared(PhysicsObject first, PhysicsObject second)
    {
        return DistanceSquared(first.Position, second.Position);
    }

    public static double DistanceSquared(Vector first, Vector second)
    {
        return (first - second).LengthSquared;
    }
}

public static class VectorExtension
{
    public static double DotProduct(this Vector vector, Vector other)
    {
        return vector * other;
    }
}