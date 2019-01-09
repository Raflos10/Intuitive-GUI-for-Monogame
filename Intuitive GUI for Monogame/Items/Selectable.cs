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

        public override void Update(GameTime gameTime)
        {
            Vector2 mousePosition = Vector2.Transform(Global.MousePosition, Matrix.Invert(ParentMatrix));
            if (mousePosition.X > 0 && mousePosition.Y > 0 && mousePosition.X < Width && mousePosition.Y < Height)
            {
                if (!ContainsMouse)
                    ContainsMouse = true;

                if (Global.MouseState.LeftButton == ButtonState.Pressed && Global.MouseStateLast.LeftButton == ButtonState.Released)
                    OnMousePress?.Invoke(this, Args);

                if (Global.MouseStateLast.LeftButton == ButtonState.Pressed && Global.MouseState.LeftButton == ButtonState.Released)
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
