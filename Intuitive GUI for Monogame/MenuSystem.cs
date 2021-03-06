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

namespace Intuitive_GUI_for_Monogame
{
    public class MenuSystem : DrawableGameComponent
    {
        public bool MouseInputEnabled { get; set; } = true;
        public bool KeyboardInputEnabled { get; set; } = true;

        public Matrix ResolutionMatrix { get; set; } = Matrix.Identity;
        private SpriteBatch spriteBatch;

        // graphics properties
        public SpriteSortMode SpriteSortMode { get; set; } = SpriteSortMode.Deferred;
        public BlendState BlendState { get; set; } = BlendState.AlphaBlend;
        public SamplerState SamplerState { get; set; } = SamplerState.LinearClamp;
        public DepthStencilState DepthStencilState { get; set; } = DepthStencilState.None;
        public RasterizerState RasterizerState { get; set; } = RasterizerState.CullCounterClockwise;
        public Effect Effect { get; set; }


        private List<Menu> menus = new List<Menu>();

        private MouseState mouseState, mouseStateLast;

        public MenuSystem(Game game, SpriteBatch spriteBatch) : base(game)
        {
            this.spriteBatch = spriteBatch;

            // prevent MouseUpdate from being called on entry
            mouseStateLast = mouseState;
            mouseState = Mouse.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            if (MouseInputEnabled)
            {
                mouseStateLast = mouseState;
                mouseState = Mouse.GetState();

                if (menus.Count > 0 && mouseStateLast != mouseState)
                    for (int i = 0; i < menus.Count; i++)
                        menus[i].MouseUpdate(mouseState, mouseStateLast, GetVirtualPosition(mouseState.Position.ToVector2()));
            }
        }

        public void InputTrigger(Menu.MenuInputs input)
        {
            if (KeyboardInputEnabled)
                for (int i = 0; i < menus.Count; i++)
                    menus[i].InputTrigger(input);
        }

        Vector2 GetVirtualPosition(Vector2 position)
        {
            return Vector2.Transform(position, Matrix.Invert(ResolutionMatrix));
        }

        public void OpenMenu(Menu menu)
        {
            menus.Add(menu);
            menu.Open?.Invoke(this, EventArgs.Empty);
        }

        public void CloseMenu(Menu menu)
        {
            menus.Remove(menu);
            menu.Close?.Invoke(this, EventArgs.Empty);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (menus.Count > 0)
            {
                spriteBatch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, ResolutionMatrix);
                foreach (Menu menu in menus)
                    menu.Draw(spriteBatch, gameTime);
                spriteBatch.End();
            }
        }
    }
}
