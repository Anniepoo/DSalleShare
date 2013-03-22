﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    class DirtAndScratchesSubrenderer : SubRenderer
    {
        private static Texture2D colorBlow;
        private static Texture2D vignette;
        private static Texture2D dirt;
        private static Texture2D frame;
        private static Color blowColor = new Color(1.0f, 1.0f, 1.0f, 0.37f);

        private static int frameNum = 0;
        private const int FRAME_DIVISOR = 4;

        private static Random rnd = new Random();

        private static int shakeOffset = 0;
        private static Point dirtOffset = new Point();

        public static int Shake
        {
            get { return shakeOffset; }
        }

        public DirtAndScratchesSubrenderer()
        {

        }

        internal override void Draw(Xna.Framework.Graphics.SpriteBatch ssb)
        {
            Rectangle screen = new Rectangle(0, 0, TDVBasicGame.Width, TDVBasicGame.Height);

            Rectangle dirtSource = new Rectangle(0, 0, TDVBasicGame.Width, TDVBasicGame.Height);
            dirtSource.Offset(dirtOffset);
            ssb.Draw(dirt, screen, dirtSource, Color.White);
            ssb.Draw(vignette, screen, vignette.Bounds, Color.White);
            ssb.Draw(frame, screen, frame.Bounds, Color.White);

            frameNum++;
            if (frameNum > FRAME_DIVISOR)
            {
                shakeOffset = rnd.Next(6) - 6;
                dirtOffset.X = rnd.Next(dirt.Width - TDVBasicGame.Width);
                dirtOffset.Y = rnd.Next(dirt.Height - TDVBasicGame.Height);
                frameNum = 0;
            }
        }

        public static void causeLoadContent(Game game)
        {
            frame = game.Content.Load<Texture2D>("filmframe");
            vignette = game.Content.Load<Texture2D>("vignette");
            dirt = game.Content.Load<Texture2D>("dirt");
        }

        public override float Z
        {
            get
            {
                return -0.1f;
            }
        }

    }
}
