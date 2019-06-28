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
    public abstract class Selectable : UIElement
    {
        //public bool IsSelectable { get; protected set; }

        public bool ContainsMouse { get; private set; }

        public bool Selected { get; private set; }

        public EventHandler OnHighlight, OnUnhighlight, OnPress, OnRelease;

        public EventArgs Args;

        public void Select()
        {
            OnHighlight?.Invoke(this, EventArgs.Empty);
            Selected = true;
        }

        public void Unselect()
        {
            OnUnhighlight?.Invoke(this, EventArgs.Empty);
            Selected = false;
        }

        // move to menu and grid?
        public virtual void MouseUpdate(MouseState mouseState, MouseState mouseStateLast, Vector2 virtualPosition)
        {
            Vector2 mousePosition = Vector2.Transform(virtualPosition, Matrix.Invert(ParentMatrix));
            if (mousePosition.X > 0 && mousePosition.Y > 0 && mousePosition.X < Width && mousePosition.Y < Height)
            {
                if (!ContainsMouse)
                {
                    OnHighlight?.Invoke(this, EventArgs.Empty);
                    ContainsMouse = true;
                }

                if (mouseState.LeftButton == ButtonState.Pressed && mouseStateLast.LeftButton == ButtonState.Released)
                    OnPress?.Invoke(this, Args);

                if (mouseStateLast.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                    OnRelease?.Invoke(this, Args);
            }
            else
            {
                if (ContainsMouse)
                {
                    OnUnhighlight?.Invoke(this, EventArgs.Empty);
                    ContainsMouse = false;
                }
            }
        }

        public virtual void InputTrigger(Menu.MenuInputs input)
        {
            if (input == Menu.MenuInputs.OK)
                OnPress?.Invoke(this, Args);
        }
    }
}
