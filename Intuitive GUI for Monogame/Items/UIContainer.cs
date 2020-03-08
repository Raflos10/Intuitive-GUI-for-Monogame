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

namespace Intuitive_GUI_for_Monogame.Items
{
	public abstract class UIContainer : Selectable
	{
		public abstract IReadOnlyCollection<ContainerEntry> Children { get; }
		public abstract Selectable SelectedElement { get; }
		public bool IsSelectable { get; protected set; } = false;

		public override void Highlight()
		{
			if (SelectedElement != null)
				SelectedElement.Highlight();
			base.Highlight();
		}

		public override void Unhighlight()
		{
			if (SelectedElement != null)
				SelectedElement.Unhighlight();
			base.Unhighlight();
		}

		public virtual void UnhighlightAll()
		{
			foreach(ContainerEntry entry in Children)
				switch (entry.Element)
				{
					case UIContainer container:
						container.ResetSelection();
						break;

					case Selectable selectable:
						selectable.Unhighlight();
						break;
				}
		}

		public override void MouseClick(Vector2 mouseGlobalPosition)
		{
			base.MouseClick(mouseGlobalPosition);

			SelectedElement?.MouseClick(mouseGlobalPosition);
		}

		public override void MouseRelease(Vector2 mouseGlobalPosition)
		{
			base.MouseRelease(mouseGlobalPosition);

			SelectedElement?.MouseRelease(mouseGlobalPosition);
		}

		/// <returns>True if within container bounds, false if not</returns>
		public abstract bool HandleSelectionChange(Menu.MenuInputs input);

		public abstract void HighlightFromOutside(Menu.MenuInputs input);

		// returns nearest number in array to "num"
		protected int GetClosestNumber(int num, int[] candidates)
		{
			int smallestDistance = Math.Abs(num - candidates[0]);
			int resultCandidate = candidates[0];
			foreach (int candidate in candidates)
			{
				int distance = Math.Abs(num - candidate);
				if (distance < smallestDistance)
				{
					smallestDistance = distance;
					resultCandidate = candidate;
				}
			}
			return resultCandidate;
		}

		public abstract void MouseUpdate(Vector2 mouseGlobalPosition);
	}

	public class ContainerEntry
	{
		public UIElement Element { get; }
		public ContainerEntry(UIElement element)
		{
			this.Element = element;
		}
	}
}
