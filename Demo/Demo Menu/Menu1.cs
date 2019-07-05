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

using MonoGame.Extended.BitmapFonts;

using Intuitive_GUI_for_Monogame;
using Intuitive_GUI_for_Monogame.Items;

namespace Demo.Demo_Menu
{
    public class Menu1 : Menu
    {
        public Texture2D Background { get; set; }

        public Menu1(Texture2D texture, Texture2D buttonTexture1, Texture2D buttonTexture2, BitmapFont bitmapFont)
        {
            this.Background = texture;
            this.Width = texture.Width;
            this.Height = texture.Height;
            Origin = new Vector2(Width * .5f, Height * .5f);

            Grid mainGrid = new Grid(new Margin(100));
            mainGrid.PersistantHighlight = true;

            Grid subGrid = new Grid();

            subGrid.AddColumnDefinition(ColumnDefinition.Auto);
            subGrid.AddColumnDefinition(ColumnDefinition.Auto);
            subGrid.AddRowDefinition(RowDefinition.Auto);
            subGrid.AddRowDefinition(RowDefinition.Auto);
            subGrid.AddChild(new Menu1Button(buttonTexture2, OnButtonPress, new ButtonArgs("Subgrid 0")), 0, 0);
            subGrid.AddChild(new Menu1Button(buttonTexture2, OnButtonPress, new ButtonArgs("Subgrid 1")), 1, 0);
            subGrid.AddChild(new Menu1Button(buttonTexture2, OnButtonPress, new ButtonArgs("Subgrid 2")), 0, 1);
            subGrid.AddChild(new Menu1Button(buttonTexture2, OnButtonPress, new ButtonArgs("Subgrid 3")), 1, 1);

            mainGrid.AddColumnDefinition(ColumnDefinition.Auto);
            mainGrid.AddColumnDefinition(ColumnDefinition.Auto);
            mainGrid.AddColumnDefinition(ColumnDefinition.Auto);
            mainGrid.AddColumnDefinition(ColumnDefinition.Auto);
            //mainGrid.AddColumnDefinition(ColumnDefinition.Fill);
            mainGrid.AddRowDefinition(RowDefinition.Auto);
            mainGrid.AddRowDefinition(RowDefinition.Auto);
            //mainGrid.AddRowDefinition(RowDefinition.Fill);
            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("Grid 0")), 0, 0);
            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("Grid 1")), 1, 0);
            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("Grid 2")), 2, 0);
            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("Grid 3")), 3, 0);
            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("Grid 4")), 0, 1);
            mainGrid.AddChild(subGrid, 1, 1);
            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("Grid 5")), 2, 1);
            mainGrid.AddChild(new Menu1TextButton(bitmapFont, "Button", OnButtonPress, new ButtonArgs("Grid 6")), 3, 1);

            Item = mainGrid;
        }

        void OnButtonPress(object sender, EventArgs args)
        {
            if (args is ButtonArgs buttonArgs)
                Debug.WriteLine(buttonArgs.Text);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Background, Position, null, Color.White, Rotation, Origin, 1f, SpriteEffects.None, 0f);

            base.Draw(spriteBatch, gameTime);
        }

    }

    public class ButtonArgs : EventArgs
    {
        public string Text { get; set; }
        public ButtonArgs(string text)
        {
            this.Text = text;
        }
    }
}
