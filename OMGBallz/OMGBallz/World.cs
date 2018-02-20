using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class World
{
    public List<PhysicsObject> Objects;
    public List<(double, PhysicsObject, PhysicsObject)> collisions;
    
    public World(params PhysicsObject[] objects) 
        : this (objects.ToList())
    { }
    public World(List<PhysicsObject> objects)
    {
        Objects = objects;
        collisions = new List<(double, PhysicsObject, PhysicsObject)>();
    }

    public void Update()
    {
        for (int i = 0; i < Objects.Count(); i++)
            for (int j = i + 1; j < Objects.Count(); j++)
            {
                PhysicsObject first = Objects[i], second = Objects[j];

                collisions.Add((first.Collides(second), first, second));
            }

        collisions.Sort((alpha, beta) => alpha.Item1.CompareTo(beta.Item1));

        (double dt, PhysicsObject a, PhysicsObject b) = collisions.First();

        foreach (PhysicsObject obj in Objects)
            obj.Update(dt);
        
        a.HandleCollision(b);
        b.HandleCollision(a);
        a.ApplyCollision();
        b.ApplyCollision();

        collisions.Clear();
    }
}