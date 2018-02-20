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

    public Visualizer()
    {
        Init();
    }

    public void Init()
    {        
        pictureBox = new PictureBox
            { BackColor = Color.Brown
            , Size = Size
            };

        Controls.Add(pictureBox);

        PhysicsObject[] ballz = new PhysicsObject[]
            { new Ball(10, new Vector(100, 100))
            //, new Ball(10, new Vector(100, 60), new Vector(0, 1))
            //, new Ball(10, new Vector(100, 160), new Vector(0, -2))
            , new Ball(10, new Vector(220, 120), new Vector(-1, 0))
            //, new Ball(10, new Vector(100, 130))
            //, new Ball(10, new Vector(10, 130), new Vector(3, 0))
            //, new Ball(10, new Vector(100, 160))
            //, new Ball(10, new Vector(10, 160), new Vector(3, 0))
            };

        world = new World(ballz);

        KeyPreview = true;

        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Space)
            {
                world.Update();
                Render(world.Objects);
            }
        };

        Render(world.Objects);

        //Timer timer = new Timer
        //    { Interval = 1
        //    };

        //timer.Tick += (sender, args) =>
        //{
        //    world.Update(2f);
        //    Render(world.Objects);
        //    pictureBox.Update();
        //};

        //timer.Start();
    }

    public void Render(IEnumerable<PhysicsObject> objects)
    {
        picture = new Picture(Size.Width, Size.Height);
        pictureBox.Image = picture.Bitmap;

        foreach (PhysicsObject obj in objects)
            obj.Draw(ref picture);

        pictureBox.Image = picture.Bitmap;
    }
}