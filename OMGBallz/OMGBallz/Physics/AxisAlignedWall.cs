using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class AxisAlignedWall : PhysicsObject
{
}

public class HorizontalWall : AxisAlignedWall
{
    public double Y;

    public HorizontalWall(double y)
    {
        Y = y;
    }

    public override void ApplyCollision()
    { }

    public override void Draw(ref Picture picture)
    {
        int y = (int)Y;

        for (int i = 0; i < picture.Bitmap.Width; i++)
        {
            picture.Draw(i, y, Color.White);
        }
    }

    public override void HandleCollision(PhysicsObject other)
    {
        throw new NotImplementedException();
    }
}