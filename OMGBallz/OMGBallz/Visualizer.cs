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
            { new Ball(10, new Vector(100, 100), new Vector(-0.1f, 0))
            , new Ball(10, new Vector(10, 100), new Vector(0.3f, 0.02f), mass : 5)
            };

        world = new World(ballz);

        Timer timer = new Timer
            { Interval = 1
            };

        timer.Tick += (sender, args) =>
        {
            world.Update(2f);
            Render(world.Objects);
            pictureBox.Update();
        };

        timer.Start();
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