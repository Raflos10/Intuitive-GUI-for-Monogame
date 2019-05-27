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
    public class Image : UIItem
    {
        public Texture2D Texture { get; set; }
        public Color Color { get; set; }

        public Image(Texture2D texture, Margin margin = null)
        {
            this.Texture = texture;
            Width = Texture.Width;
            Height = Texture.Height;
            this.Color = Color.White;
            this.Margin = margin ?? Margin.Zero;
        }

        public Image(Texture2D texture, Color color, Margin margin = null)
        {
            this.Texture = texture;
            Width = Texture.Width;
            Height = Texture.Height;
            this.Color = Color;
            this.Margin = margin ?? Margin.Zero;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, Vector2.Zero, Scale, SpriteEffects.None, 0.0f);
        }
    }
}
