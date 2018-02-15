using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class World
{
    public List<PhysicsObject> Objects;

    public World(params PhysicsObject[] objects)
    {
        Objects = objects.ToList();
    }
    public World(List<PhysicsObject> objects)
    {
        Objects = objects;
    }

    public void Update(float dt)
    {
        for (int i = 0; i < Objects.Count(); i++)
            for (int j = i + 1; j < Objects.Count(); j++)
            {
                PhysicsObject first = Objects[i], second = Objects[j];

                if (first.Collides(second))
                {
                    first.Collisions.Add(second);
                    second.Collisions.Add(first);
                }
            }

        foreach (PhysicsObject obj in Objects)
            obj.Update(dt);
    }
}