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
        public SpriteFont Font { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; } = Color.Black;

        public TextButton(SpriteFont font, string text, Margin margin = null)
        {
            this.Font = font;
            this.Text = text;

            this.StrictBoundingBox = true;

            this.Margin = margin ?? Margin.Zero;

            Width = (int)font.MeasureString(text).X;
            Height = (int)font.MeasureString(text).Y;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(Font, Text, Position, Color, Rotation, Origin, Scale, SpriteEffect, LayerDepth);
        }
    }
}
