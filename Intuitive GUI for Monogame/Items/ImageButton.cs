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
        public Color Color { get; set; } = Color.White;

        private Dictionary<States, Texture2D> textures;

        public ImageButton(Texture2D texture, Margin margin = null)
        {
            textures = new Dictionary<States, Texture2D>
                {
                    { States.None, texture }
                };

            Width = texture.Width;
            Height = texture.Height;
            this.Margin = margin ?? Margin.Zero;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //TODO texture states
            spriteBatch.Draw(textures[States.None], Position, null, Color, Rotation, Vector2.Zero, Scale, SpriteEffects.None, 0.0f);
        }
    }
}
