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
        public enum States
        {
            None,
            Pressed,
            Hover,
            Released,
            Selected
        }

        public States State { get; set; } = States.None;
        public bool WaitUntilRelease { get; protected set; } = false;

        public Button()
        {
            OnSelect += Button_OnSelect;
            OnDeselect += Button_OnDeselect;
        }

        private void Button_OnSelect(object sender, EventArgs e)
        {
            State = States.Selected;
        }

        private void Button_OnDeselect(object sender, EventArgs e)
        {
            State = States.None;
        }

        public virtual void ChangeState(States state)
        {
            this.State = state;
        }
    }
}
