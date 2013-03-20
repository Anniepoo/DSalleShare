using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    class TDVGUI : Object2D
    {
        Texture2D presents;

        public TDVGUI(TDVBasicGame game) : 
            base(game)
        {
            Position = new Vector2(100, 600);
            Size = new Vector2(96);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            presents = Game.Content.Load<Texture2D>("presents");
        }

        public override void Draw(Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);

            Rectangle dest = new Rectangle(
                100,600,
                96,96);
            Rectangle source = new Rectangle(
                currentPlayer * 96, 0,
                96, 96);
            SharedSpriteBatch.Begin();
            SharedSpriteBatch.Draw(presents, dest, source, Color.White);
            SharedSpriteBatch.End();

        }

        private int currentPlayer = 0;

        public int Player
        {
            get { return Player; }
            set { currentPlayer = value % 6; }
        }


        internal void causeLoadContent()
        {
            LoadContent();
        }
    }
}
