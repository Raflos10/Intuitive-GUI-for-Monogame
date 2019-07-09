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

		private Texture2D buttonTexture1, buttonTexture2, imageTexture1, imageTexture2;
		private BitmapFont bitmapFont;

        public Menu1(Texture2D texture, Texture2D buttonTexture1, Texture2D buttonTexture2, Texture2D imageTexture1, 
			Texture2D imageTexture2, BitmapFont bitmapFont)
        {
            this.Background = texture;
            this.Width = texture.Width;
            this.Height = texture.Height;
            Origin = new Vector2(Width * .5f, Height * .5f);

			this.buttonTexture1 = buttonTexture1;
			this.buttonTexture2 = buttonTexture2;
			this.imageTexture1 = imageTexture1;
			this.imageTexture2 = imageTexture2;
			this.bitmapFont = bitmapFont;

            Grid subGrid = new Grid();

            subGrid.AddColumnDefinition(ColumnDefinition.Auto);
            subGrid.AddColumnDefinition(ColumnDefinition.Auto);
            subGrid.AddRowDefinition(RowDefinition.Auto);
            subGrid.AddRowDefinition(RowDefinition.Auto);
            subGrid.AddChild(new Menu1Button(buttonTexture2, OnButtonPress, new ButtonArgs("0, 0")), 0, 0);
            subGrid.AddChild(new Menu1Button(buttonTexture2, OnButtonPress, new ButtonArgs("1, 0")), 1, 0);
            subGrid.AddChild(new Menu1Button(buttonTexture2, OnButtonPress, new ButtonArgs("0, 1")), 0, 1);
            subGrid.AddChild(new Menu1Button(buttonTexture2, OnButtonPress, new ButtonArgs("1, 1")), 1, 1);

            Grid mainGrid = new Grid(new Margin(100, 0, 100, 0));
            mainGrid.PersistantHighlight = true;

            mainGrid.AddColumnDefinition(ColumnDefinition.Auto);
            mainGrid.AddColumnDefinition(ColumnDefinition.Auto);
            mainGrid.AddColumnDefinition(ColumnDefinition.Auto);
            mainGrid.AddColumnDefinition(ColumnDefinition.Auto);
            mainGrid.AddColumnDefinition(ColumnDefinition.Auto);
            mainGrid.AddColumnDefinition(ColumnDefinition.Fill);
            mainGrid.AddRowDefinition(RowDefinition.Auto);
            mainGrid.AddRowDefinition(RowDefinition.Auto);
            mainGrid.AddRowDefinition(RowDefinition.Auto);
            mainGrid.AddRowDefinition(RowDefinition.Auto);
            mainGrid.AddRowDefinition(RowDefinition.Fill);

            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("0, 0")), 0, 0);
            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("1, 0")), 1, 0);
            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("2, 0")), 2, 0);
            mainGrid.AddChild(new Image(imageTexture1), 3, 0);
            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("4, 0")), 4, 0);

            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("0, 1")), 0, 1);
            mainGrid.AddChild(subGrid, 1, 1);
            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("2, 1")), 2, 1);
            mainGrid.AddChild(new Menu1TextButton(bitmapFont, "Button", OnButtonPress, new ButtonArgs("3, 1")), 3, 1);
            mainGrid.AddChild(new Image(imageTexture1), 4, 1);

            mainGrid.AddChild(new Image(imageTexture1), 0, 2);
            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("1, 2")), 1, 2);
            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("2, 2")), 2, 2);
            mainGrid.AddChild(new Image(imageTexture1), 3, 2);
            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("4, 2")), 4, 2);

            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("0, 3")), 0, 3);
            mainGrid.AddChild(new Menu1TextButton(bitmapFont, "Button", OnButtonPress, new ButtonArgs("1, 3")), 1, 3);
            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("2, 3")), 2, 3);
            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("3, 3")), 3, 3);
            mainGrid.AddChild(new Menu1Button(buttonTexture1, OnButtonPress, new ButtonArgs("4, 3")), 4, 3);

            Item = mainGrid;
        }

		public void RandomGrid()
		{
			Random random = new Random();

			Grid subGrid1 = new Grid();
			subGrid1.AddColumnDefinition(ColumnDefinition.Auto);
			subGrid1.AddColumnDefinition(ColumnDefinition.Auto);
			subGrid1.AddRowDefinition(RowDefinition.Auto);
			subGrid1.AddRowDefinition(RowDefinition.Auto);

			Point subGrid1Location = randomLocation();

			for (int x = 0; x < 2; x++)
				for (int y = 0; y < 2; y++)
					subGrid1.AddChild(randomElement(x,y,true), x, y);

					Grid subGrid2 = new Grid();
			subGrid2.AddColumnDefinition(ColumnDefinition.Auto);
			subGrid2.AddColumnDefinition(ColumnDefinition.Auto);
			subGrid2.AddRowDefinition(RowDefinition.Auto);
			subGrid2.AddRowDefinition(RowDefinition.Auto);

			Point subGrid2Location = randomLocation();

			for (int x = 0; x < 2; x++)
				for (int y = 0; y < 2; y++)
					subGrid2.AddChild(randomElement(x, y, true), x, y);

			while (subGrid1Location == subGrid2Location)
				subGrid2Location = randomLocation();

			Grid mainGrid = new Grid(new Margin(100, 0, 100, 0));
			mainGrid.PersistantHighlight = true;

			mainGrid.AddColumnDefinition(ColumnDefinition.Auto);
			mainGrid.AddColumnDefinition(ColumnDefinition.Auto);
			mainGrid.AddColumnDefinition(ColumnDefinition.Auto);
			mainGrid.AddColumnDefinition(ColumnDefinition.Auto);
			mainGrid.AddColumnDefinition(ColumnDefinition.Auto);
			mainGrid.AddColumnDefinition(ColumnDefinition.Fill);
			mainGrid.AddRowDefinition(RowDefinition.Auto);
			mainGrid.AddRowDefinition(RowDefinition.Auto);
			mainGrid.AddRowDefinition(RowDefinition.Auto);
			mainGrid.AddRowDefinition(RowDefinition.Auto);
			mainGrid.AddRowDefinition(RowDefinition.Fill);

			for (int x = 0; x < 5; x++)
				for (int y = 0; y < 4; y++)
				{
					if (new Point(x, y) == subGrid1Location)
						mainGrid.AddChild(subGrid1, x, y);
					else if (new Point(x, y) == subGrid2Location)
						mainGrid.AddChild(subGrid2, x, y);
					else
					mainGrid.AddChild(randomElement(x, y, false), x, y);
				}

			Item = mainGrid;

			Point randomLocation()
			{
				return new Point(random.Next(0, 5), random.Next(0, 4));
			}

			UIElement randomElement(int x, int y, bool small)
			{
				int decider = random.Next(0, 3);
				switch(decider){
					case 0:
						return new Menu1Button(small?buttonTexture2:buttonTexture1, OnButtonPress, new ButtonArgs((x + ", " + y).ToString()));
					case 1:
						return new Menu1TextButton(bitmapFont, "Button", OnButtonPress, new ButtonArgs((x + ", " + y).ToString()));
					default:
						return new Image(small?imageTexture2:imageTexture1);
				}
			}
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
