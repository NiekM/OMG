using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public class World
{
    public List<PhysicsObject> Objects;
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

public struct Scene
{
    public Vector TopLeft, BottomRight;

    public Func<World> World;
    public Func<double> Data;

    public Scene(Func<World> world, Vector topLeft, Vector bottomRight, Func<double> data = null)
    {
        World = world;
        TopLeft = topLeft;
        BottomRight = bottomRight;
        Data = data;
    }

    public static Scene Bullet
    {
        get
        {
            World World()
            {
                List<PhysicsObject> objects = new List<PhysicsObject>
                { new HorizontalWall(-40) { Mass = 1e100 }
                , new HorizontalWall(40) { Mass = 1e100 }
                , new VerticalWall(-100) { Mass = 1e100 }
                , new VerticalWall(-200) { Mass = 1e100, Velocity = new Vector(10, 0) }
                };

                for (int i = -90; i <= -20; i += 10)
                    for (int j = -30; j <= 30; j += 10)
                    {
                        objects.Add(new Ball(4, new Vector(i, j)));
                    }

                objects.Add(new Ball(1, new Vector(0, -50), density: 1e300));

                objects.Add(new Ball(39, new Vector(40, 0), density: 1));

                return new World(objects);
            }

            return new Scene(World, new Vector(-200), new Vector(200));
        }
    }

    public static Scene Mix
    {
        get // Maybe add divider in the middle, remove divider after starting parameters have no direct influence on the outcome anymore.
        {
            Box box = new Box(new Vector(-100), new Vector(100));
            List<PhysicsObject> first = new List<PhysicsObject>(), second = new List<PhysicsObject>();

            World World()
            {

                List<PhysicsObject> objects = new List<PhysicsObject>();

                objects.AddRange(box.Walls);

                Random r = new Random();

                for (int i = -3; i < 3; i++)
                    for (int j = -3; j < 3; j++)
                    {
                        bool f = i >= 0;

                        Ball ball = new Ball(f ? 9 : 5, new Vector(i * 30 + 15, j * 30 + 15))
                        {
                            Color = f ? Util.Colors.Pink : Color.White
                            ,
                            Velocity = Vector.Directional(r.NextDouble() * 2 * Math.PI)
                            ,
                            Mass = f ? 1:1
                        };

                        (f ? first : second).Add(ball);
                    }


                objects.AddRange(first);
                objects.AddRange(second);

                return new World(objects);
            }

            double Data()
            {
                double f = box.Homogeneity(first, 6, 1000);
                double s = box.Homogeneity(second, 6, 1000);

                return f + s; //$"{f :#.##} + {s :#.##} = {f + s :#.##}";
            }

            return new Scene(World, new Vector(-140), new Vector(140), Data);
        }
    }

    public static Scene Compress
    {
        get
        {
            World World()
            {
                List<PhysicsObject> objects = new List<PhysicsObject>
                    { new HorizontalWall(-120) { Mass = 1e6 }
                    , new HorizontalWall(120) { Mass = 1e6 }
                    , new VerticalWall(-120) { Mass = 1e6 }
                    , new VerticalWall(120) { Mass = 1e6 }
                    };

                foreach (var wall in objects)
                    wall.Velocity = wall.Position / -240;

                objects.AddRange(new List<PhysicsObject>
                { new Ball(5, new Vector(200, 0), density: 1e100)
                , new Ball(5, new Vector(-200, 0), density: 1e100)
                , new Ball(5, new Vector(0, 200), density: 1e100)
                , new Ball(5, new Vector(0, -200), density: 1e100)
                });

                Random r = new Random();

                for (int i = -90; i <= 90; i += 60)
                    for (int j = -90; j <= 90; j += 60)
                        objects.Add(new Ball(12, new Vector(i, j), density: 1e-1)
                        {
                            Color = Color.WhiteSmoke
                            ,
                            Velocity = Vector.Directional(r.NextDouble() * 2 * Math.PI)//new Vector(r.NextDouble() * 2 - 1, r.NextDouble() * 2 - 1)
                        });

                return new World(objects);
            }

            return new Scene(World, new Vector(-140), new Vector(140));
        }
    }
}