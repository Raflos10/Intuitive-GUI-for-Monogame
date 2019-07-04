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

        public bool Highlighted { get; private set; }

        /// <summary>
        /// The item will remain highlighted even if the mouse goes outside this element.
        /// </summary>
        public bool PersistantHighlight { get; set; } = false;

        public EventHandler OnHighlight, OnUnhighlight, OnPress, OnRelease;

        public EventArgs Args;

        public void Highlight()
        {
            OnHighlight?.Invoke(this, EventArgs.Empty);
            Highlighted = true;
        }

        public void Unhighlight()
        {
            OnUnhighlight?.Invoke(this, EventArgs.Empty);
            Highlighted = false;
        }

        public void Select()
        {
            OnPress?.Invoke(this, Args);
        }

        public void Unselect()
        {
            OnRelease?.Invoke(this, Args);
        }

        public virtual void InputTrigger(Menu.MenuInputs input)
        {
            if (input == Menu.MenuInputs.OK)
                OnPress?.Invoke(this, Args);
        }

        public virtual void MouseUpdate(Vector2 internalMousePosition) { }

        public virtual void ResetSelection() { }
    }
}
