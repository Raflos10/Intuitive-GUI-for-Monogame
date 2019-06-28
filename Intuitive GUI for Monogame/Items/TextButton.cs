using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Intuitive_GUI_for_Monogame.Items
{
    public class TextButton : Button
    {
        private SpriteFont font;
        private string text;
        public float LayerDepth { get; set; } = 0;

        public Color fontColor;

        private Dictionary<States, Color> colors;

        public TextButton(SpriteFont font, string text, Margin margin = null)
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

            Width = (int)font.MeasureString(text).X;
            Height = (int)font.MeasureString(text).Y;
        }

        public override void ChangeState(States state)
        {
            fontColor = colors[state];
            base.ChangeState(state);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(font, text, Position, colors[State], Rotation, Vector2.Zero, Scale, SpriteEffects.None, LayerDepth);
        }
    }
}
