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
        public BitmapFont Font { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; } = Color.Black;

        public BMTextButton(BitmapFont font, string text, Margin margin = null)
        {
            this.Font = font;
            this.Text = text;

            this.Margin = margin ?? Margin.Zero;

            Width = (int)font.MeasureString(text).Width;
            Height = (int)font.MeasureString(text).Height;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(Font, Text, Position, Color, Rotation, Origin, Scale, SpriteEffect, 0f);
        }
    }
}
