using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended.BitmapFonts;

using Intuitive_GUI_for_Monogame;
using Intuitive_GUI_for_Monogame.Items;
using Intuitive_GUI_for_Monogame_MGE_Items;

namespace Demo.Demo_Menu
{
    public class Menu1TextButton : BMTextButton
    {
        Dictionary<ButtonState, Color> statesColors;

        public Menu1TextButton(BitmapFont font, string text, EventHandler e, EventArgs args, Margin margin = null) : base(font, text, margin)
        {
            OnPress += e;
            this.Args = args;

            statesColors = new Dictionary<ButtonState, Color>
            {
                { ButtonState.None,Color.Black },
                { ButtonState.Highlighted,Color.White },
                { ButtonState.Pressed,Color.Wheat },
                { ButtonState.Released,Color.White }
            };

            OnStateChange += State_Change;

            this.Margin = margin ?? Margin.Zero;
        }

        void State_Change(object sender, EventArgs e)
        {
            if (e is ButtonStateChangedEventArgs args)
                Color = statesColors[args.NewState];
        }
    }
}
