using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    class TDVGUISubRenderer : SubRenderer
    {
        static Texture2D presents;

        public TDVGUISubRenderer(int playerID)
        {
            this.Player = playerID;
        }

        internal override void Draw(SpriteBatch SharedSpriteBatch)
        {
            Rectangle dest = new Rectangle(
                100,600,   // where present appears on screen
                96,96);
            Rectangle source = new Rectangle(
                currentPlayer * 96, 0,
                96, 96);
            SharedSpriteBatch.Draw(presents, dest, source, Color.White);
        }

        private int currentPlayer = 0;

        public int Player
        {
            get { return Player; }
            set { currentPlayer = value % 6; }
        }


        internal static void causeLoadContent(Game game)
        {
            presents = game.Content.Load<Texture2D>("presents");
        }
    }
}
