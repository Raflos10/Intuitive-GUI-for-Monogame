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
    public class Label : UIItem
    {
        private SpriteFont font;
        string text;

        public Vector2 Size { get; set; }
        public Color Color { get; set; }

        public Label(SpriteFont font, string text, Margin margin = null)
        {
            NewLabel(font, text, Color.Black, Margin.Zero);
        }

        public Label(SpriteFont font, string text, Color color, Margin margin = null)
        {
            NewLabel(font, text, color, Margin.Zero);
        }

        private void NewLabel(SpriteFont font, string text, Color color, Margin margin)
        {
            this.font = font;
            this.text = text;
            this.Color = color;
            this.Margin = margin;
        }

        public void NewText(string text)
        {
            //TODO
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(font, text, Position, Color, Rotation, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }
    }
}
