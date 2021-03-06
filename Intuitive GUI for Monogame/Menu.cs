﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Intuitive_GUI_for_Monogame.Items;

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
                item.UpdateBounding(new Segment(0, Width), new Segment(0, Height));
                if (item is Items.Grid grid)
                    grid.BuildGrid(Width, Height);
                UpdateTransform();
            }
        }

        #endregion

        protected Matrix Transform { get; private set; }

        public bool Active { get; set; } = true;

        public EventHandler Open, Close;

        protected Menu childMenu;

        public enum MenuInputs { Up, Down, Left, Right, OK, Cancel, Pause }

        /// <summary>
        /// Determines whether this menu's (selectable) item should be highlighted by default.
        /// If not, it will take one InputTrigger to highlight it.
        /// </summary>
        public bool HighlightedByDefault { get; set; } = true;

        private bool usingMouse = false;

        public Menu()
        {
            Open += HighlightIfDefault;
            Open += (sender, args) => usingMouse = !HighlightedByDefault;
            Close += ResetSelection;
        }

        public Menu(bool rememberSelection)
        {
            if (!rememberSelection)
                Close += ResetSelection;
            Open += HighlightIfDefault;
            Open += (sender, args) => usingMouse = !HighlightedByDefault;
        }

        void HighlightIfDefault(object sender, EventArgs e)
        {
            if (HighlightedByDefault && item is Items.Selectable selectable)
            {
                if (selectable is UIContainer container && container.IsSelectable)
                    container.HighlightInternal();
                selectable.Highlight();
            }
        }

        void ResetSelection(object sender, EventArgs e)
        {
            if (Item != null && Item is Items.Selectable selectable)
            {
                if (HighlightedByDefault)
                    selectable.ResetSelection();
                else
                    selectable.Unhighlight();
            }
        }

        public virtual void InputTrigger(MenuInputs input)
        {
            if (Active)
            {
                if (Item is Items.Selectable selectable)
                {
                    if (usingMouse)
                    {
                        selectable.OnSwitchInputMethod?.Invoke(this, EventArgs.Empty);
                        usingMouse = false;

                        if (!selectable.Highlighted)
                        {
                            if (selectable is UIContainer container)
                                container.HighlightInternal();
                            selectable.Highlight();
                            return;
                        }
                    }

                    selectable.InputTrigger(input);
                }
            }
            else
                childMenu.InputTrigger(input);
        }

        public virtual void MouseUpdate(MouseState mouseState, MouseState mouseStateLast, Vector2 mouseGlobalPosition)
        {
            if (Active)
            {
                if (Item is Selectable selectable)
                {
                    if (!usingMouse)
                    {
                        selectable.OnSwitchInputMethod?.Invoke(this, EventArgs.Empty);
                        if (selectable is UIContainer container)
                            container.UnhighlightInternal();
                        selectable.Unhighlight();
                        usingMouse = true;
                    }

                    selectable.MouseUpdate(mouseGlobalPosition);

                    if (mouseState.LeftButton == ButtonState.Pressed && mouseStateLast.LeftButton == ButtonState.Released)
                        selectable.MouseClick(mouseGlobalPosition);
                    else if (mouseState.LeftButton == ButtonState.Released && mouseStateLast.LeftButton == ButtonState.Pressed)
                        selectable.MouseRelease(mouseGlobalPosition);
                }
            }
            else
                childMenu.MouseUpdate(mouseState, mouseStateLast, mouseGlobalPosition);
        }

        public void OpenChildMenu(Menu menu)
        {
            Active = false;
            childMenu = menu;
            menu.Open?.Invoke(this, EventArgs.Empty);
        }

        public void CloseChildMenu()
        {
            Active = true;
            childMenu.Close?.Invoke(this, EventArgs.Empty);
            childMenu = null;
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

            childMenu?.Draw(spriteBatch, gameTime);
        }
    }
}
