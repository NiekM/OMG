﻿using System;
using System.Drawing;
using System.Windows.Forms;

public class Visualizer : Form
{
    Picture picture;
    PictureBox pictureBox;
    World world;
    Scene scene;

    double speed = 2f;
    bool pause = true;

    (int X, int Y)? drag = null;
    
    public Visualizer(Scene scene)
    { 
        this.scene = scene;

        world = scene.World();

        Size = new Size(600, 600);

        pictureBox = new PictureBox
            { BackColor = Util.Colors.DarkPurple
            , Size = Size
            };

        picture = new Picture((pictureBox.Size.Width, pictureBox.Size.Height), scene.TopLeft, scene.BottomRight);

        Controls.Add(pictureBox);

        Render();

        KeyPreview = true;

        Timer timer = new Timer
        {
            Interval = 1
        };
        timer.Tick += (sender, args) =>
        {
            if (!pause)
                world.Advance(speed);

            Render();
        };
        timer.Start();
        
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
                case Keys.ControlKey:
                    Console.WriteLine(world.Data());
                    break;
            }
        };
    
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



        Size = new Size(620, 640);
    }

    public void Render()
    {
        picture.Clear();

        foreach (PhysicsObject obj in world.Objects)
            obj.Draw(ref picture);

        pictureBox.Image = picture.Bitmap;
    }
}