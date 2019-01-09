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
    public abstract class UIItem
    {
        protected Vector2 Position { get; private set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        protected Vector2 Scale { get; private set; }
        protected Vector2 Origin { get; private set; }
        protected float Rotation { get; private set; }
        protected Matrix ParentMatrix { get; private set; }
        public Margin Margin { get; set; } = Margin.Zero;

        public abstract void Update(GameTime gameTime);

        public virtual void UpdateTransformProperties(Matrix parentMatrix)
        {
            this.ParentMatrix = parentMatrix;
            Vector3 position3, scale3;
            Quaternion rotationQ;
            parentMatrix.Decompose(out scale3, out rotationQ, out position3);
            Vector2 direction = Vector2.Transform(Vector2.UnitX, rotationQ);
            Rotation = (float)Math.Atan2(direction.Y, direction.X);
            Position = new Vector2(position3.X, position3.Y);
            Scale = new Vector2(scale3.X, scale3.Y);
        }

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
