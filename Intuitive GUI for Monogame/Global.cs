using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Intuitive_GUI_for_Monogame
{
    public static class Global
    {
        public static bool UsingMouse { get; set; } = true;

        public static MouseState MouseState { get; private set; }
        public static MouseState MouseStateLast { get; private set; }
        public static List<Menu> ActiveMenus { get; private set; } = new List<Menu>();

        public static Matrix ResolutionMatrix { get; set; } = Matrix.Identity;

        public static Vector2 MousePosition
        {
            get
            {
                return Vector2.Transform(new Vector2(MouseState.Position.X, MouseState.Position.Y), Matrix.Invert(ResolutionMatrix));
            }
        }

        public static void Update(GameTime gameTime)
        {
            foreach (Menu menu in ActiveMenus)
                menu.Update(gameTime);

            MouseStateLast = MouseState;
            MouseState = Mouse.GetState();
        }

        public static void InputTrigger(Menu.MenuInputs input)
        {
            foreach(Menu menu in ActiveMenus)
            {
                if (menu.State == Menu.States.Active)
                {
                    if (UsingMouse) UsingMouse = false;
                    if (menu.Item is Items.Selectable selectable)
                    {
                        selectable.InputTrigger(input);
                        return;
                    }
                }
            }
        }
    }
}
