using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class World
{
    public List<PhysicsObject> Objects;
    public Dictionary<(PhysicsObject, PhysicsObject), float> collisions;

    float dt = 0;

    public World(params PhysicsObject[] objects) 
        : this (objects.ToList())
    { }
    public World(List<PhysicsObject> objects)
    {
        Objects = objects;
        collisions = new Dictionary<(PhysicsObject, PhysicsObject), float>();
    }

    public void Update()
    {
        //for (int i = 0; i < Objects.Count(); i++)
        //    for (int j = i + 1; j < Objects.Count(); j++)
        //    {
        //        PhysicsObject first = Objects[i], second = Objects[j];

        //        if (collisions[(first, second)] == dt)
        //        {
        //            collisions[(first, second)] = first.Collides(second);
        //        }
        //        else
        //        {
        //            collisions[(first, second)] -= dt;
        //        }
        //    }

        for (int i = 0; i < Objects.Count(); i++)
            for (int j = i + 1; j < Objects.Count(); j++)
            {
                PhysicsObject first = Objects[i], second = Objects[j];

                collisions[(first, second)] = first.Collides(second);
            }

        dt = collisions.Min(kv => kv.Value);

        foreach (PhysicsObject obj in Objects)
            obj.Update(dt);

        foreach (var collision in collisions)
        {
            if (collision.Value == dt)
            {
                (PhysicsObject first, PhysicsObject second) = collision.Key;

                first.HandleCollision(second);
                second.HandleCollision(first);
            }
        }
    }
}