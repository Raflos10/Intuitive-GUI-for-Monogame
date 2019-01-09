using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intuitive_GUI_for_Monogame.Items
{
    public class Margin
    {
        public int Left { get; private set; }
        public int Right { get; private set; }
        public int Top { get; private set; }
        public int Bottom { get; private set; }
        public static Margin Zero { get; private set; } = new Margin(0);

        public Margin(int left, int right, int top, int bottom)
        {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;
        }

        public Margin(int sides, int topAndBottom)
        {
            this.Left = sides;
            this.Right = sides;
            this.Top = topAndBottom;
            this.Bottom = topAndBottom;
        }

        public Margin(int margin)
        {
            this.Left = margin;
            this.Right = margin;
            this.Top = margin;
            this.Bottom = margin;
        }
    }
}
