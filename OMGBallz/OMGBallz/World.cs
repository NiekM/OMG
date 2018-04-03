using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public class World
{
    public List<PhysicsObject> Objects;
    public Func<double> Data = delegate { return 0; };
    List<Collision> collisions;
    
    public World(params PhysicsObject[] objects) 
        : this (objects.ToList())
    { }
    public World(List<PhysicsObject> objects)
    {
        Objects = objects;
        collisions = new List<Collision>();
        
        for (int i = 0; i < Objects.Count(); i++)
            for (int j = i + 1; j < Objects.Count(); j++)
            {
                PhysicsObject first = Objects[i], second = Objects[j];

                Collision? collision = Collision.Find(first, second);
                
                if (collision.HasValue)
                    collisions.Add(collision.Value);
            }
    }

    public void Update(double time)
    {
        if (time == 0)
            return;

        foreach (PhysicsObject obj in Objects)
        {
            obj.Update(time);
        }
    }

    public void Advance(double time)
    {
        while (time > 0)
        {
            Collision collision = collisions.Min();
            
            var newCollisions = new List<Collision>();

            if (collision.Time < time)
            {
                Update(collision.Time);

                Collision.Execute(collision.First, collision.Second);

                foreach (Collision col in collisions)
                {
                    newCollisions.Add(col.Update
                        (collision.Time
                        , col.Contains(collision.First, collision.Second)
                        ));
                }
            }
            else
            {
                Update(time);

                foreach (Collision col in collisions)
                {
                    newCollisions.Add(col.Update(time, false));
                }
            }

            collisions = newCollisions;

            time -= collision.Time;
        }
    }
}

public struct ParticleData
{
    public double Radius;
    public double Mass;
    public int Rows;
    public int Columns;
    public double Energy;
    
    public ParticleData(double surface, double mass, int rows, int columns, double? energy = null)
    {
        Radius = Math.Sqrt(surface / Math.PI);
        Mass = mass;
        Rows = rows;
        Columns = columns;
        Energy = energy ?? rows * columns;
    }
}

public struct Scene
{
    static Color[] colors = { Util.Colors.Pink, Color.White };
    static int colorIndex = 0;
    static Color NextColor
    {
        get
        {
            colorIndex = (colorIndex + 1) % colors.Length;
            return colors[colorIndex];
        }
    }

    public Vector TopLeft, BottomRight;

    public Func<World> World;

    public Scene(Func<World> world, Vector topLeft, Vector bottomRight)
    {
        World = world;
        TopLeft = topLeft;
        BottomRight = bottomRight;
    }

    public static Scene MixScene(ParticleData data) => MixScene(data, data);
    public static Scene MixScene(ParticleData firstParticles, ParticleData secondParticles)
    {
        double size = 200;
        double space = 20;

        var box = new Box(new Vector(-size / 2), new Vector(size / 2));
        List<PhysicsObject> first, second;

        World World()
        {
            List<PhysicsObject> objects = new List<PhysicsObject>();

            objects.AddRange(box.Walls);

            var random = new Random();

            List<PhysicsObject> AddParticles(ParticleData data, Vector offset)
            {
                Color color = NextColor;

                var result = new List<PhysicsObject>();

                double height = size - 2 * data.Radius - space;
                double yOffset = height / (data.Rows - 1);

                if (yOffset < data.Radius * 2)
                    throw new Exception("Too many rows!");

                double width = size / 2 - 2 * data.Radius - space;
                double xOffset = width / (data.Columns - 1);

                if (xOffset < data.Radius * 2)
                    throw new Exception("Too many columns!");

                double total = 0;

                for (int i = 0; i < data.Columns; i++)
                    for (int j = 0; j < data.Rows; j++)
                    {
                        var position = offset + new Vector(-width / 2 + i * xOffset, -height / 2 + j * yOffset);
                        double energy = random.NextDouble();
                        total += energy;
                        var velocity = Vector.Directional(random.NextDouble() * 2 * Math.PI) * energy;
                        result.Add(new Ball(data.Radius, position, velocity)
                            { Mass = data.Mass
                            , Color = color
                            }
                        );
                    }
                
                foreach (var obj in result)
                {
                    obj.Velocity /= total;
                    obj.Velocity *= data.Energy;
                }

                return result;
            }

            first = AddParticles(firstParticles, new Vector(-size / 4, 0));
            second = AddParticles(secondParticles, new Vector(size / 4, 0));

            objects.AddRange(first);
            objects.AddRange(second);
            
            double Data()
            {
                double f = box.Homogeneity(first, firstParticles.Radius, 1000);
                double s = box.Homogeneity(second, secondParticles.Radius, 1000);

                return f + s;
            }

            return new World(objects) { Data = Data };
        }

        return new Scene(World, box.TopLeft - new Vector(10), box.BottomRight + new Vector(10));
    }
}