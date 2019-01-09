//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;

//using MonoGame.Extended.BitmapFonts;

//namespace Library_Game.UI.Items
//{
//    public class Label : UIItem
//    {
//        private BitmapFont font;
//        string text;

//        public Vector2 Size { get; set; }
//        public Color Color { get; set; }

//        public Label(string text, BitmapFont font, Color color)
//        {
//            this.font = font;
//            this.text = text;
//            this.Color = color;
//            this.Rectangle = new Rectangle(0, 0, (int)Size.X, (int)Size.Y);
//        }

//        public void NewText(string text)
//        {
//            //TODO
//        }

//        public override void Update(GameTime gameTime)
//        {
//        }

//        public override void Draw(GameTime gameTime)
//        {
//            Game1.SpriteBatch.DrawString(font, text, new Vector2(Rectangle.X, Rectangle.Y), Color);
//        }
//    }
//}
