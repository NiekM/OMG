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
            { new HorizontalWall(-90) { Mass = 1e100 }
            , new HorizontalWall(90) { Mass = 1e100 }
            , new VerticalWall(-90) { Mass = 1e100 }
            , new VerticalWall(90) { Mass = 1e100 }
            //, new Membrane(90) { Mass = 1e4, Velocity = new Vector(-1, 0), C = 0.001, Color = Color.Black }
            //, new Membrane(-90, 0.001) { Mass = 1e4, Velocity = new Vector(-1, 0), Color = Color.Black }
            };

        //ballz.AddRange(new List<PhysicsObject>
        //    { //new Ball(3, new Vector(160, 0), density: 1e10)
        //    //, new Ball(3, new Vector(-160, 0), density: 1e10)
        //     new Ball(10, new Vector(0,0), new Vector(1, 0))
        //    });

        Random r = new Random();

        for (int i = -4; i <= 4; i++)
            for (int j = -4; j <= 4; j++)
                if (r.NextDouble() > 0.8)
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

        bool dynamic = true;
        
        Timer timer = new Timer
        {
            Interval = 1
        };
        timer.Tick += (sender, args) =>
        {
            if (dynamic)
                world.Update(speed);

            Render(world.Objects);
        };
        timer.Start();

        if (dynamic)
        {
            KeyDown += (_, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                        speed *= 1.1f;
                        break;
                    case Keys.Down:
                        speed /= 1.1f;
                        break;
                }
            };
        }
        else
        {
            KeyDown += (_, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.Space:
                        NextCollision();
                        break;
                }
            };

            void NextCollision()
            {

                var collisions = new List<Collision>();

                for (int i = 0; i < world.Objects.Count(); i++)
                    for (int j = i + 1; j < world.Objects.Count(); j++)
                    {
                        PhysicsObject first = world.Objects[i], second = world.Objects[j];

                        collisions.Add(Collision.Find(first, second));
                    }

                Collision collision = collisions.Min();

                foreach (var obj in world.Objects)
                    obj.Update(collision.Time);

                Collision.Execute(collision.First, collision.Second);

                Render(world.Objects);
            }
        }

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