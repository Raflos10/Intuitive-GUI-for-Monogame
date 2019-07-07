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

        private Items.UIElement item;
        public Items.UIElement Item
        {
            get { return item; }
            set
            {
                item = value;
                item.UpdateBounding(new Items.Column(0, Width), new Items.Row(0, height));
                if (item is Items.Grid grid)
                    grid.BuildGrid(new Items.ColumnDefinition(Width), new Items.RowDefinition(Height));
                UpdateTransform();
            }
        }

        #endregion

        protected Matrix Transform { get; private set; }

        public EventHandler Open, Close;

        public enum MenuInputs { Up, Down, Left, Right, OK, Cancel, Pause }

        /// <summary>
        /// If false, the menu will still be visible but not interactable.
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        /// Determines whether this menu's (selectable) item should be highlighted by default.
        /// If not, it will take one InputTrigger to highlight it.
        /// </summary>
        public bool HighlightedByDefault { get; set; } = true;

        private bool usingMouse = false;

        public Menu()
        {
            Open += HighlightIfDefault;
            Close += ResetSelection;
        }

        public Menu(bool rememberSelection)
        {
            if (!rememberSelection)
                Close += ResetSelection;
            Open += HighlightIfDefault;
        }

        void HighlightIfDefault(object sender, EventArgs e)
        {
            if (HighlightedByDefault && item is Items.Selectable selectable)
                selectable.Highlight();
        }

        void ResetSelection(object sender, EventArgs e)
        {
            if (Item != null && Item is Items.Selectable selectable)
                if (HighlightedByDefault)
                    selectable.ResetSelection();
                else
                    selectable.Unhighlight();
        }

        public virtual void InputTrigger(MenuInputs input)
        {
            if (Item is Items.Selectable selectable)
            {
                if (usingMouse)
                    usingMouse = false;

                if (!HighlightedByDefault && !selectable.Highlighted)
                {
                    selectable.Highlight();
                    return;
                }

                selectable.InputTrigger(input);
            }
        }

        public void MouseUpdate(MouseState mouseState, MouseState mouseStateLast, Vector2 mouseGlobalPosition)
        {
            if (Active)
                if (Item is Items.Selectable selectable)
                {
                    if (!usingMouse)
                        usingMouse = true;

                    selectable.MouseUpdate(mouseGlobalPosition);

                    if (mouseState.LeftButton == ButtonState.Pressed && mouseStateLast.LeftButton == ButtonState.Released)
                        selectable.MouseClick(mouseGlobalPosition);
                    else if (mouseState.LeftButton == ButtonState.Released && mouseStateLast.LeftButton == ButtonState.Pressed)
                        selectable.MouseRelease(mouseGlobalPosition);
                }

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
            Item?.Draw(spriteBatch, gameTime);
        }
    }
}
