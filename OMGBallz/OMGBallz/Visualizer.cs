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
    Scene scene;

    double speed = 2f;
    bool pause = true;

    (int X, int Y)? drag = null;
    
    public Visualizer()
    {
        Init();
    }

    public void Init()
    {
        scene = Scene.Mix;

        Size = new Size(600, 600);

        pictureBox = new PictureBox
            { BackColor = Color.Brown
            , Size = Size
            };

        picture = new Picture((Size.Width, Size.Height), scene.TopLeft, scene.BottomRight);

        Controls.Add(pictureBox);

        Render(scene.World.Objects);

        KeyPreview = true;

        bool dynamic = true;
        
        Timer timer = new Timer
        {
            Interval = 1
        };
        timer.Tick += (sender, args) =>
        {
            if (dynamic && !pause)
                scene.World.Advance(speed);

            Render(scene.World.Objects);
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
                    case Keys.Space:
                        pause = false;
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

                for (int i = 0; i < scene.World.Objects.Count(); i++)
                    for (int j = i + 1; j < scene.World.Objects.Count(); j++)
                    {
                        PhysicsObject first = scene.World.Objects[i], second = scene.World.Objects[j];

                        collisions.Add(Collision.Find(first, second));
                    }

                Collision collision = collisions.Min();

                scene.World.Update(collision.Time);

                Collision.Execute(collision.First, collision.Second);

                Render(scene.World.Objects);
            }
        }

        pictureBox.MouseWheel += (_, e) =>
        {
            picture.Zoom(1 - e.Delta / 1000d, picture.RealPosition((e.X, e.Y)));
        };
        pictureBox.MouseDown += (_, e) =>
        {
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

            picture.Translate((drag.Value.X - e.X, drag.Value.Y - e.Y));
            drag = (e.X, e.Y);
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