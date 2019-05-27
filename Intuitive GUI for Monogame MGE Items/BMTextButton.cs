using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

using Intuitive_GUI_for_Monogame.Items;

namespace Intuitive_GUI_for_Monogame_MGE_Items
{
    public class BMTextButton : Button
    {
        private BitmapFont font;
        private string text;

        public Color fontColor;

        private Dictionary<States, Color> colors;

        public BMTextButton(BitmapFont font, string text, Margin margin = null)
        {
            this.font = font;
            this.text = text;

            this.Margin = margin ?? Margin.Zero;

            colors = new Dictionary<States, Color>
                {
                    { States.None, Color.Black },
                    { States.Hover, Color.White },
                    { States.Pressed, Color.Gray },
                    { States.Released, Color.LightGray },
                    { States.Selected, Color.White }
                };
            ChangeState(States.None);

            Width = (int)font.MeasureString(text).Width;
            Height = (int)font.MeasureString(text).Height;
        }

        public override void ChangeState(States state)
        {
            fontColor = colors[state];
            base.ChangeState(state);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(font, text, Position, colors[State], Rotation, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }
    }
}
