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
        Dictionary<ButtonState, Color> statesColors;

        public Menu1Button(Texture2D texture, EventHandler e, EventArgs args, Margin margin = null) : base(texture, margin)
        {
            OnPress += e;
            this.Args = args;

            OnStateChange += State_Change;

            statesColors = new Dictionary<ButtonState, Color>
            {
                { ButtonState.None,Color.White },
                { ButtonState.Highlighted,new Color( 1f,1f,1f,.5f) },
                { ButtonState.Pressed,new Color(0.8f,0.8f,0.8f,1f )},
                { ButtonState.Released,new Color(1f,1f,1f,.5f )}
            };

            this.Margin = margin ?? Margin.Zero;
        }

        void State_Change(object sender, EventArgs e)
        {
            if (e is ButtonStateChangedEventArgs args)
                Color = statesColors[args.NewState];
        }
    }
}
