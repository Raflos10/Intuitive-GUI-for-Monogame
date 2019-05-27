using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Intuitive_GUI_for_Monogame
{
    public abstract class Menu
    {
        #region Properties that update the matrix

        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                UpdateTransform();
            }
        }

        private int width;
        public int Width
        {
            get { return width; }
            set
            {
                width = value;
                UpdateTransform();
            }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                UpdateTransform();
            }
        }

        private Vector2 scale = Vector2.One;
        public Vector2 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                UpdateTransform();
            }
        }

        private Vector2 origin = Vector2.Zero;
        public Vector2 Origin
        {
            get { return origin; }
            set
            {
                origin = value;
                UpdateTransform();
            }
        }

        private float rotation = 0;
        public float Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                UpdateTransform();
            }
        }

        private Items.UIItem item;
        public Items.UIItem Item
        {
            get { return item; }
            set
            {
                item = value;
                if (item is Items.Grid grid)
                    grid.BuildGrid(new Items.ColumnDefinition(Width), new Items.RowDefinition(Height));
                UpdateTransform();
            }
        }

        #endregion

        public Color Color { get; set; } = Color.White;
        public Matrix Transform { get; private set; }

        public EventHandler Open, Close;

        public enum MenuInputs { Up, Down, Left, Right, OK, Cancel, Pause }
        public enum States { Invisible, Active, VisibleInactive }
        public States State { get; set; } = States.Invisible;

        public Menu()
        {
            Close += ResetSelection;
        }

        public Menu(bool rememberSelection)
        {
            if (!rememberSelection)
                Close += ResetSelection;
        }

        void ResetSelection(object sender, EventArgs e)
        {
            if (Item != null && Item is Items.Selectable selectable)
            {
                //TODO make sure internal grids are deselected too
                selectable.Selected = false;
            }
        }

        public virtual void InputTrigger(MenuInputs input)
        {
            if (State == States.Active)
            {
                if (Item is Items.Selectable selectable)
                {
                    selectable.InputTrigger(input);
                    return;
                }
            }
        }

        public void ChangeStateInvisible(object sender, EventArgs e)
        {
            State = States.Invisible;
        }

        public void ChangeStateActive(object sender, EventArgs e)
        {
            State = States.Active;
        }

        public void ChangeStateVisibleInactive(object sender, EventArgs e)
        {
            State = States.VisibleInactive;
        }

        public void MouseUpdate(MouseState mouseState, MouseState mouseStateLast, Vector2 mousePosition)
        {
            if (State == States.Active)
                if (Item is Items.Selectable selectable)
                    selectable.MouseUpdate(mouseState, mouseStateLast, mousePosition);
        }

        void UpdateTransform()
        {
            //Transform = -Origin * Scale * Rotation * Translation
            Transform = Matrix.CreateTranslation(-Origin.X, -Origin.Y, 0f) *
                Matrix.CreateScale(Scale.X, Scale.Y, 1f) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(Position.X, Position.Y, 0f);
            Item?.UpdateTransformProperties(Transform);
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (State != States.Invisible)
                Item?.Draw(spriteBatch, gameTime);
        }
    }
}
