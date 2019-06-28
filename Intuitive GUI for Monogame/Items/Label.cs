using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Intuitive_GUI_for_Monogame.Items
{
    public class Label : UIElement
    {
        public SpriteFont Font { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; } = Color.Black;
        public SpriteEffects SpriteEffect { get; set; } = SpriteEffects.None;
        public float LayerDepth { get; set; }

        public Label(SpriteFont font, string text, Margin margin = null)
        {
            NewLabel(font, text, Color.Black, margin);
        }

        public Label(SpriteFont font, string text, Color color, Margin margin = null)
        {
            NewLabel(font, text, color, margin);
        }

        private void NewLabel(SpriteFont font, string text, Color color, Margin margin)
        {
            this.Font = font;
            this.Text = text;
            this.Color = color;
            this.Margin = margin ?? Margin.Zero;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(Font, Text, Position, Color, Rotation, Origin, Scale, SpriteEffect, LayerDepth);
        }
    }
}
