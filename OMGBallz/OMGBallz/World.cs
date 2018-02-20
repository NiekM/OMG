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
        
        for (int i = 0; i < Objects.Count(); i++)
            for (int j = i + 1; j < Objects.Count(); j++)
            {
                PhysicsObject first = Objects[i], second = Objects[j];

                collisions.Add((first.Collides(second), first, second));
            }
    }

    public void Update(double dt)
    {
        //for (int i = 0; i < Objects.Count(); i++)
        //    for (int j = i + 1; j < Objects.Count(); j++)
        //    {
        //        PhysicsObject first = Objects[i], second = Objects[j];

        //        collisions.Add((first.Collides(second), first, second));
        //    }

        collisions.Sort((alpha, beta) => alpha.Item1.CompareTo(beta.Item1));

        (double next, PhysicsObject a, PhysicsObject b) = collisions.First();

        double timeStep = dt;

        if (next < dt)
        {
            foreach (PhysicsObject obj in Objects)
                obj.Update(next);

            a.HandleCollision(b);
            b.HandleCollision(a);
            a.ApplyCollision();
            b.ApplyCollision();

            timeStep = next;

            var newCollisions = new List<(double, PhysicsObject, PhysicsObject)>();

            foreach (var collision in collisions)
            {
                PhysicsObject first = collision.Item2, second = collision.Item3;

                if (first == a || first == b || second == a || second == b)
                    newCollisions.Add((first.Collides(second), first, second));
                else
                    newCollisions.Add((collision.Item1 - timeStep, first, second));
            }

            collisions = newCollisions;

            Update(dt - timeStep);
        }
        else
        {
            foreach (PhysicsObject obj in Objects)
                obj.Update(timeStep);

            var newCollisions = new List<(double, PhysicsObject, PhysicsObject)>();

            foreach (var collision in collisions)
            {
                PhysicsObject first = collision.Item2, second = collision.Item3;
                
                newCollisions.Add((collision.Item1 - timeStep, first, second));
            }

            collisions = newCollisions;
        }

        
        
        //collisions = collisions.Select<(double, PhysicsObject, PhysicsObject),(double, PhysicsObject, PhysicsObject)>((time, first, second) =>
        //{
        //    if (first == a || first == b || second == a || second == b)
        //        return (first.Collides(second), first, second);
        //    else
        //        return (time - timeStep, first, second);
        //}).ToList();

        
    }
}