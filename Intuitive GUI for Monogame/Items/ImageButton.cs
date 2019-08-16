using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Intuitive_GUI_for_Monogame.Items
{
    public class ImageButton : Button
    {
        public Texture2D Texture { get; set; }
        public Rectangle? SourceRectangle { get; set; } = null;
        public Color Color { get; set; } = Color.White;

        public ImageButton(Texture2D texture, Margin margin = null, Rectangle? sourceRectangle = null)
        {
            this.Texture = texture;
            this.Margin = margin ?? Margin.Zero;
            this.SourceRectangle = sourceRectangle ?? null;

            if(SourceRectangle != null)
            {
                Rectangle rectangle = (Rectangle)SourceRectangle;
                Width = rectangle.Width;
                Height = rectangle.Height;
            }
            else
            {
                Width = texture.Width;
                Height = texture.Height;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Texture, Position, SourceRectangle, Color, Rotation, Origin, Scale, SpriteEffect, LayerDepth);
        }
    }
}
