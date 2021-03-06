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

namespace Intuitive_GUI_for_Monogame.Items
{
    public abstract class Selectable : UIElement
    {
        public bool Highlighted { get; private set; }

        /// <summary>
        /// The element will remain highlighted even if the mouse goes outside this element.
        /// </summary>
        public bool PersistantHighlight { get; set; } = false;

        /// <summary>
        /// If true, the element will only highlight when the mouse is directly over it. 
        /// <para>If false, the element will highlight as long as the mouse is inside its territory. </para>
        /// </summary>
        public bool StrictBoundingBox { get; set; } = true;

        /// <summary>
        /// Determines whether the action should be invoked on mouse press or mouse release.
        /// <para>Note: For now, button/key presses will always invoke actions on press, not release. </para>
        /// <para>This also means that OnRelease will not be invoked by button/key presses. </para>
        /// </summary>
        public bool ActionOnRelease { get; set; } = false;

        public EventHandler OnHighlight, OnUnhighlight, OnMouseClick, OnMouseRelease, OnButtonTrigger, Action, OnMouseClickOutside,
            OnMouseReleaseOutside, OnSwitchInputMethod;

        public EventArgs Args { get; set; }

        public virtual void Highlight()
        {
            if (!Highlighted)
                OnHighlight?.Invoke(this, EventArgs.Empty);
            Highlighted = true;
        }

        public virtual void Unhighlight()
        {
            if (Highlighted)
                OnUnhighlight?.Invoke(this, EventArgs.Empty);
            Highlighted = false;
        }

        public virtual void MouseUpdate(Vector2 mouseGlobalPosition)
        {
            if (ContainsMouse(mouseGlobalPosition))
            {
                if (!Highlighted)
                    Highlight();
            }
            else if (Highlighted && !PersistantHighlight)
                Unhighlight();
        }

        public virtual void MouseClick(Vector2 mouseGlobalPosition)
        {
            if (ContainsMouse(mouseGlobalPosition))
            {
                OnMouseClick?.Invoke(this, Args);
                if (!ActionOnRelease)
                    Action?.Invoke(this, Args);
            }
            else
                OnMouseClickOutside?.Invoke(this, Args);
        }

        public virtual void MouseRelease(Vector2 mouseGlobalPosition)
        {
            if (ContainsMouse(mouseGlobalPosition))
            {
                OnMouseRelease?.Invoke(this, Args);
                if (ActionOnRelease)
                    Action?.Invoke(this, Args);
            }
            else
                OnMouseReleaseOutside?.Invoke(this, Args);
        }

        public virtual void InputTrigger(Menu.MenuInputs input)
        {
            if (input == Menu.MenuInputs.OK)
            {
                OnButtonTrigger?.Invoke(this, Args);
                Action?.Invoke(this, Args);
            }
        }

        public virtual bool ContainsMouse(Vector2 mouseGlobalPosition)
        {
            return ContainsMouse(mouseGlobalPosition, StrictBoundingBox);
        }

        public bool ContainsMouse(Vector2 mouseGlobalPosition, bool strictBoundingBox)
        {
            Vector2 mouseLocalPosition = GetMouseLocalPosition(mouseGlobalPosition);

            if (strictBoundingBox)
            {
                if (mouseLocalPosition.X < Margin.Left)
                    return false;
                if (mouseLocalPosition.Y < Margin.Top)
                    return false;
                if (mouseLocalPosition.X > Width + Margin.Left)
                    return false;
                if (mouseLocalPosition.Y > Height + Margin.Top)
                    return false;
            }
            else
            {
                if (mouseLocalPosition.X < 0)
                    return false;
                if (mouseLocalPosition.Y < 0)
                    return false;
                if (mouseLocalPosition.X > BoundingWidth)
                    return false;
                if (mouseLocalPosition.Y > BoundingHeight)
                    return false;
            }

            return true;
        }

        protected Vector2 GetMouseLocalPosition(Vector2 mouseGlobalPosition)
        {
            return Vector2.Transform(mouseGlobalPosition, Matrix.Invert(ParentMatrix));
        }

        public virtual void ResetSelection() { }
    }
}
