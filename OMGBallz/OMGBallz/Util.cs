using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Util
{
    public static double DistanceSquared(PhysicsObject first, PhysicsObject second)
    {
        return DistanceSquared(first.Position, second.Position);
    }

    public static double DistanceSquared(Vector first, Vector second)
    {
        return (first - second).LengthSquared;
    }
}