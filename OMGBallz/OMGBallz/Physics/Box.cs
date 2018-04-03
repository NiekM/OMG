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