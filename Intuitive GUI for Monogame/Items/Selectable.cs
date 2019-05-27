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

namespace Intuitive_GUI_for_Monogame.Items
{
    public abstract class Selectable : UIItem
    {
        public bool IsSelectable { get; protected set; }

        private bool containsMouse;
        public bool ContainsMouse
        {
            get { return containsMouse; }
            set
            {
                if (value)
                    OnMouseEnter?.Invoke(this, EventArgs.Empty);
                else
                    OnMouseExit?.Invoke(this, EventArgs.Empty);
                containsMouse = value;
            }
        }

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (value)
                    OnSelect?.Invoke(this, EventArgs.Empty);
                else
                    OnDeselect?.Invoke(this, EventArgs.Empty);
                selected = value;
            }
        }

        public EventHandler OnSelect, OnDeselect, OnMouseEnter, OnMouseExit, OnMousePress, OnMouseRelease, OnOK;

        public EventArgs Args;

        public virtual void MouseUpdate(MouseState mouseState, MouseState mouseStateLast, Vector2 virtualPosition)
        {
            Vector2 mousePosition = Vector2.Transform(virtualPosition, Matrix.Invert(ParentMatrix));
            if (mousePosition.X > 0 && mousePosition.Y > 0 && mousePosition.X < Width && mousePosition.Y < Height)
            {
                if (!ContainsMouse)
                    ContainsMouse = true;

                if (mouseState.LeftButton == ButtonState.Pressed && mouseStateLast.LeftButton == ButtonState.Released)
                    OnMousePress?.Invoke(this, Args);

                if (mouseStateLast.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                    OnMouseRelease?.Invoke(this, Args);
            }
            else
            {
                if (ContainsMouse)
                    ContainsMouse = false;
            }
        }

        public virtual void InputTrigger(Menu.MenuInputs input)
        {
            if (input == Menu.MenuInputs.OK)
                OnOK?.Invoke(this, Args);
        }
    }
}
