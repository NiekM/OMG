using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

struct Box
{
    public Vector TopLeft, BottomRight;
    
    public Box(Vector topLeft, Vector bottomRight)
    {
        TopLeft = topLeft;
        BottomRight = bottomRight;
    }

    public Box Expand(double amount)
    {
        return new Box(TopLeft - new Vector(amount), BottomRight + new Vector(amount));
    }

    public double Homog(List<PhysicsObject> first, List<PhysicsObject> second)//, int precision)
    {
        // Way cooler idea:
        // Beforehand: calculate the total surface of all the balls and of the square.
        // Repeat any amount of times:
        // - Choose a random rectangular surface
        // - Pick a random point on that surface
        // - Calculate whether you hit a ball and what kind
        // - Determine the chance of hitting that ball in that surface

        // Another idea:
        // Choose random points, measure difference between distance to closest white/black ball

        // Or:
        // Choose random points, compare total distance to expected total distance

        double ratio = (double)first.Count / second.Count;

        var firstQuads = new List<PhysicsObject>[2, 2];
        var secondQuads = new List<PhysicsObject>[2, 2];

        for (int i = 0; i < 2; i++)
            for (int j = 0; j < 2; j++)
            {
                firstQuads[i, j] = new List<PhysicsObject>();
                secondQuads[i, j] = new List<PhysicsObject>();
            }

        foreach (var obj in first)
        {
            int x = (int)((obj.Position.X - TopLeft.X) / Width * 2);
            int y = (int)((obj.Position.Y - TopLeft.Y) / Height * 2);

            firstQuads[x, y].Add(obj);
        }
        foreach (var obj in second)
        {
            int x = (int)((obj.Position.X - TopLeft.X) / Width * 2);
            int y = (int)((obj.Position.Y - TopLeft.Y) / Height * 2);

            secondQuads[x, y].Add(obj);
        }

        double homogeneity = 0;

        for (int i = 0; i < 2; i++)
            for (int j = 0; j < 2; j++)
            {
                int a = firstQuads[i, j].Count, b = secondQuads[i, j].Count;

                if (a < b)
                    homogeneity += (double)a / b;
                else if (b < a)
                    homogeneity += (double)b / a;
            }

        homogeneity *= ratio;
        homogeneity /= 4;

        return homogeneity;
    }

    public double Homogen(List<PhysicsObject> objects, Func<PhysicsObject, IComparable> separator)
    {
        Dictionary<IComparable, double> sizes = new Dictionary<IComparable, double>();

        foreach(var obj in objects)
        {
            sizes[separator(obj)] += obj.Size;
        }

        throw new NotImplementedException();
    }

    public double Homogeneity(List<PhysicsObject> balls, double radius, int iterations)
    {
        Random random = new Random();
        Box cropped = Expand(-radius);

        double total = 0;

        for (int i = 0; i < iterations; i++)
        {
            Vector reference = random.NextVector(cropped.TopLeft, cropped.BottomRight);

            double expectation = ExpectedLengthSquared(cropped.TopLeft - reference, cropped.BottomRight - reference);

            double actual = balls.Select(ball => (ball.Position - reference).LengthSquared).Average();

            total += Math.Abs(actual - expectation) / expectation;
        }

        return total / iterations;
    }

    public double Width => (BottomRight - TopLeft).X;
    public double Height => (BottomRight - TopLeft).Y;

    public List<PhysicsObject> Walls =>
        new List<PhysicsObject>
            { new VerticalWall(TopLeft.X) { Mass = 1e100 }
            , new HorizontalWall(TopLeft.Y) { Mass = 1e100 }
            , new VerticalWall(BottomRight.X) { Mass = 1e100 }
            , new HorizontalWall(BottomRight.Y) { Mass = 1e100 }
            };

    public static double AverageLengthSquared(IEnumerable<Vector> points) =>
        points.Select(point => point.LengthSquared).Average();

    public static double ExpectedLengthSquared(Vector topLeft, Vector bottomRight)
    {
        double left = topLeft.X, right = bottomRight.X, top = topLeft.Y, bottom = bottomRight.Y;
        double width = right - left, height = top - bottom;

        return 
            ( (right * right * right - left * left * left) * height
            + (top * top * top - bottom * bottom * bottom) * width
            ) / (3 * width * height);
    }

    public static (double monteCarlo, double calculation) ExpectedDistanceProof(Vector topLeft, Vector bottomRight, int iterations)
    {
        Random random = new Random();

        return
            ( AverageLengthSquared(random.Generate(r => r.NextVector(topLeft, bottomRight)).Take(iterations))
            , ExpectedLengthSquared(topLeft, bottomRight)
            );
    }
}