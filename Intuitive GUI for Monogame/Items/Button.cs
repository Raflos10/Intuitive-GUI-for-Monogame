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
    public abstract class Button : Selectable
    {
        public float LayerDepth { get; set; } = 0;
        public SpriteEffects SpriteEffect { get; set; } = SpriteEffects.None;

        public enum ButtonState
        {
            None,
            Highlighted,
            Pressed,
            Released
        }

        private ButtonState state = ButtonState.None;
        public ButtonState State
        {
            get { return state; }
            set
            {
                OnStateChange?.Invoke(this, new ButtonStateChangedEventArgs(state,value));
                state = value;
            }
        }

        public EventHandler OnStateChange;

        public Button()
        {
            OnHighlight += Button_OnHighlight;
            OnUnhighlight += Button_OnUnhighlight;
            OnPress += Button_OnPress;
            OnRelease += Button_OnRelease;
        }

        private void Button_OnHighlight(object sender, EventArgs e)
        {
            State = ButtonState.Highlighted;
        }

        private void Button_OnUnhighlight(object sender, EventArgs e)
        {
            State = ButtonState.None;
        }

        private void Button_OnPress(object sender, EventArgs e)
        {
            State = ButtonState.Pressed;
        }

        private void Button_OnRelease(object sender, EventArgs e)
        {
            State = ButtonState.Released;
        }
    }

    public class ButtonStateChangedEventArgs : EventArgs
    {
        public Button.ButtonState OldState { get; private set; }
        public Button.ButtonState NewState { get; private set; }

        public ButtonStateChangedEventArgs(Button.ButtonState oldState, Button.ButtonState newState)
        {
            this.OldState = oldState;
            this.NewState = newState;
        }
    }
}
