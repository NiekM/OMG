using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class Visualizer : Form
{
    Picture picture;
    PictureBox pictureBox;
    World world;

    double speed = 2f;

    (int X, int Y) prev;
    (int X, int Y)? drag = null;
    
    public Visualizer()
    {
        Init();
    }

    public void Init()
    {
        Size = new Size(600, 600);

        pictureBox = new PictureBox
            { BackColor = Color.Brown
            , Size = Size
            };

        picture = new Picture(Size.Width, Size.Height)
            { Offset = (Size.Width / 2, Size.Height / 2)
            };

        Controls.Add(pictureBox);

        List<PhysicsObject> ballz = new List<PhysicsObject>
            { new HorizontalWall(-90) { Velocity = new Vector(0, 0.1), Mass = 1e7 }
            , new HorizontalWall(90) { Velocity = new Vector(0, -0.1), Mass = 1e7 }
            , new VerticalWall(-90) { Velocity = new Vector(0.1, 0), Mass = 1e7 }
            , new VerticalWall(90) { Velocity = new Vector(-0.1, 0), Mass = 1e7 }
            };

        Random r = new Random();

        for (int i = -4; i <= 4; i++)
            for (int j = -4; j <= 4; j++)
                if (r.NextDouble() > 0.9)
                {
                    Vector velocity = new Vector(r.NextDouble() * 2 - 1, r.NextDouble() * 2 - 1);

                    
                    //velocity = new Vector(1, 0.01);

                    ballz.Add(new Ball(10, new Vector(i * 20, j * 20), velocity));
                }

        // A ball jammed between two walls moving into either wall causes a stack overflow,
        // because the gamestate can never advance; there will always be a new collision
        // at time 0.
        // Also works with any other very massive objects.

        //ballz = new List<PhysicsObject>()
        //    { new HorizontalWall(-10)
        //    , new HorizontalWall(10)
        //    , new VerticalWall(-100)
        //    , new VerticalWall(100)
        //    , new Ball(10, new Vector(-40, 0), density : 1)
        //    , new Ball(10, new Vector(-20, 0), density : 1)
        //    , new Ball(10, new Vector(0, 0), new Vector(1, 0))
        //    , new Ball(10, new Vector(20, 0), density : 1)
        //    , new Ball(10, new Vector(40, 0), density : 1)
        //    };

        world = new World(ballz);
        Render(world.Objects);

        KeyPreview = true;
        KeyDown += (_, e) =>
        {
            switch(e.KeyCode)
            {
                case Keys.Up:
                    speed *= 1.1f;
                    break;
                case Keys.Down:
                    speed /= 1.1f;
                    break;
            }
        };

        Timer timer = new Timer
        {
            Interval = 1
        };
        timer.Tick += (sender, args) =>
        {
            world.Update(speed);
            Render(world.Objects);
        };
        timer.Start();

        //world.Update(1e7);
        //Render(world.Objects);

        pictureBox.MouseDown += (_, e) =>
        {
            prev = picture.Offset;
            drag = (e.X, e.Y);
        };
        pictureBox.MouseUp += (_, e) =>
        {
            drag = null;
        };
        pictureBox.MouseMove += (_, e) =>
        {
            if (drag == null)
                return;

            picture.Offset.X = prev.X + e.X - drag.Value.X;
            picture.Offset.Y = prev.Y + e.Y - drag.Value.Y;
        };
    }

    public void Render(IEnumerable<PhysicsObject> objects)
    {
        picture.Clear();

        foreach (PhysicsObject obj in objects)
            obj.Draw(ref picture);

        pictureBox.Image = picture.Bitmap;
    }
}