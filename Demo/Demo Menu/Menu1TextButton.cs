using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended.BitmapFonts;

using Intuitive_GUI_for_Monogame.Items;

namespace Demo.Demo_Menu
{
    public class Menu1TextButton : TextButton
    {
        public Menu1TextButton(BitmapFont font, string text, EventHandler e, EventArgs args, Margin margin = null) : base(font, text, margin)
        {
            OnMouseEnter += M_Enter;
            OnMouseExit += M_Exit;

            OnMousePress += e;
            this.Args = args;

            this.Margin = margin ?? Margin.Zero;
        }

        void M_Enter(object sender, EventArgs e)
        {
            State = States.Hover;
        }

        void M_Exit(object sender, EventArgs e)
        {
            State = States.None;
        }
    }
}
