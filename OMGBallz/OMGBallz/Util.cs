using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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

    public static class Colors
    {
        public static Color DarkPurple => Color.FromArgb(74, 40, 86);
        public static Color Pink => Color.FromArgb(212, 76, 152);
    }
}

public static class RandomExtensions
{
    public static IEnumerable<T> Generate<T>(this Random random, Func<Random, T> generator)
    {
        while (true)
        {
            yield return generator(random);
        }
    }

    public static Vector NextVector(this Random random, Vector topLeft, Vector bottomRight)
    {
        double x = random.NextDouble() * (bottomRight.X - topLeft.X) + topLeft.X
            , y = random.NextDouble() * (bottomRight.Y - topLeft.Y) + topLeft.Y;

        return new Vector(x, y);
    }
}