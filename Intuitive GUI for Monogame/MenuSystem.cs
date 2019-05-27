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

namespace Intuitive_GUI_for_Monogame
{
    public class MenuSystem : DrawableGameComponent
    {
        public List<Menu> ActiveMenus { get; private set; } = new List<Menu>();
        public Matrix ResolutionMatrix { get; set; } = Matrix.Identity;
        private SpriteBatch spriteBatch;

        private MouseState mouseState, mouseStateLast;

        public MenuSystem(Game game, SpriteBatch spriteBatch) : base(game)
        {
            this.spriteBatch = spriteBatch;
        }

        public override void Update(GameTime gameTime)
        {
            mouseStateLast = mouseState;
            mouseState = Mouse.GetState();

            if (mouseStateLast != mouseState)
                foreach (Menu menu in ActiveMenus)
                    menu.MouseUpdate(mouseState, mouseStateLast, GetVirtualPosition(mouseState.Position.ToVector2()));
        }

        public void InputTrigger(Menu.MenuInputs input)
        {
            foreach (Menu menu in ActiveMenus)
            {
                if (menu.State == Menu.States.Active)
                {
                    if (menu.Item is Items.Selectable selectable)
                    {
                        selectable.InputTrigger(input);
                        return;
                    }
                }
            }
        }

        Vector2 GetVirtualPosition(Vector2 position)
        {
            return Vector2.Transform(position, Matrix.Invert(ResolutionMatrix));
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin(transformMatrix: ResolutionMatrix);
            foreach (Menu menu in ActiveMenus)
                menu.Draw(spriteBatch, gameTime);
            spriteBatch.End();
        }
    }
}
