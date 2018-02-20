using System;
using System.Collections.Generic;
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

    public void Update(double dt)
    {
        Collision collision = collisions.Min();

        double timeStep = dt;

        if (collision.Time < dt)
        {
            foreach (PhysicsObject obj in Objects)
                obj.Update(collision.Time);

            PhysicsObject.HandleCollision(collision.First, collision.Second);

            timeStep = collision.Time;

            var newCollisions = new List<Collision>();

            foreach (Collision col in collisions)
            {
                newCollisions.Add(col.Update
                    ( timeStep
                    , col.Contains(collision.First, collision.Second)
                    ));
            }

            collisions = newCollisions;

            Update(dt - timeStep);
        }
        else
        {
            foreach (PhysicsObject obj in Objects)
                obj.Update(timeStep);

            var newCollisions = new List<Collision>();

            foreach (Collision col in collisions)
            {
                newCollisions.Add(col.Update(timeStep, false));
            }

            collisions = newCollisions;
        }
    }
}