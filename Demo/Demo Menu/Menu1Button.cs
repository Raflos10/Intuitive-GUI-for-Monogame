using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Intuitive_GUI_for_Monogame.Items;

namespace Demo.Demo_Menu
{
    public class Menu1Button : ImageButton
    {
        public Menu1Button(Texture2D texture, EventHandler e, EventArgs args, Margin margin = null) : base(texture, margin)
        {
            OnMouseEnter += M_Enter;
            OnMouseExit += M_Exit;

            OnMousePress += e;
            this.Args = args;

            this.Margin = margin ?? Margin.Zero;
        }

        void M_Enter(object sender, EventArgs e)
        {
            Color = new Color(1f, 1f, 1f, .5f);
        }

        void M_Exit(object sender, EventArgs e)
        {
            Color = Color.White;
        }
    }
}
