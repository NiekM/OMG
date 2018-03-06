using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                collisions.Add(Collision.Find(first, second));
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
    public World World;
    public Vector TopLeft, BottomRight;

    public Scene(World world, Vector topLeft, Vector bottomRight)
    {
        World = world;
        TopLeft = topLeft;
        BottomRight = bottomRight;
    }

    public static Scene Bullet
    {
        get
        {
            List<PhysicsObject> obj = new List<PhysicsObject>
                { new HorizontalWall(-40) { Mass = 1e100 }
                , new HorizontalWall(40) { Mass = 1e100 }
                , new VerticalWall(-100) { Mass = 1e100 }
                , new VerticalWall(-200) { Mass = 1e100, Velocity = new Vector(10, 0) }
                };

            for (int i = -90; i <= -20; i += 10)
                for (int j = -30; j <= 30; j += 10)
                {
                    obj.Add(new Ball(4, new Vector(i, j)));
                }

            obj.Add(new Ball(1, new Vector(0, -50), density: 1e300));

            obj.Add(new Ball(39, new Vector(40, 0), density: 1));

            return new Scene(new World(obj), new Vector(-200), new Vector(200));
        }
    }

    public static Scene Mix
    {
        get
        {
            List<PhysicsObject> obj = new List<PhysicsObject>
                    { new HorizontalWall(-120) { Mass = 1e100 }
                    , new HorizontalWall(120) { Mass = 1e100 }
                    , new VerticalWall(-120) { Mass = 1e10 }
                    , new VerticalWall(120) { Mass = 1e100 }
                    };

            Random r = new Random();

            for (int i = -90; i <= 90; i += 30)
                for (int j = -90; j <= -10; j += 30)
                    obj.Add(new Ball(12, new Vector(i, j), density: 1e-1)
                    {
                        Color = Color.WhiteSmoke
                        ,
                        Velocity = new Vector(r.NextDouble() * 2 - 1, r.NextDouble() * 2 - 1)
                    });

            for (int i = -90; i <= 90; i += 30)
                for (int j = 10; j <= 90; j += 30)
                    obj.Add(new Ball(7, new Vector(i, j))
                    {
                        Color = Color.Black
                        ,
                        Velocity = new Vector(r.NextDouble() * 2 - 1, r.NextDouble() * 2 - 1)
                    });

            return new Scene(new World(obj), new Vector(-140), new Vector(140));
        }
    }

    public static Scene Compress
    {
        get
        {
            List<PhysicsObject> obj = new List<PhysicsObject>
                    { new HorizontalWall(-120) { Mass = 1e6 }
                    , new HorizontalWall(120) { Mass = 1e6 }
                    , new VerticalWall(-120) { Mass = 1e6 }
                    , new VerticalWall(120) { Mass = 1e6 }
                    };

            foreach (var wall in obj)
                wall.Velocity = wall.Position / -240;

            obj.AddRange(new List<PhysicsObject>
                { new Ball(5, new Vector(200, 0), density: 1e100)
                , new Ball(5, new Vector(-200, 0), density: 1e100)
                , new Ball(5, new Vector(0, 200), density: 1e100)
                , new Ball(5, new Vector(0, -200), density: 1e100)
                });

            Random r = new Random();

            for (int i = -90; i <= 90; i += 60)
                for (int j = -90; j <= 90; j += 60)
                    obj.Add(new Ball(12, new Vector(i, j), density: 1e-1)
                    {
                        Color = Color.WhiteSmoke
                        ,
                        Velocity = new Vector(r.NextDouble() * 2 - 1, r.NextDouble() * 2 - 1)
                    });

            return new Scene(new World(obj), new Vector(-140), new Vector(140));
        }
    }
}