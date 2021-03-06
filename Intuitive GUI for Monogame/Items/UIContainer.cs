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

namespace Intuitive_GUI_for_Monogame.Items
{
	public abstract class UIContainer : Selectable
	{
		public abstract IReadOnlyCollection<UIElement> Children { get; }
		public abstract Selectable SelectedElement { get; }
		public bool IsSelectable { get; protected set; } = false;

		public virtual void HighlightInternal()
		{
			if (IsSelectable && SelectedElement != null)
			{
				if (SelectedElement is UIContainer container && container.IsSelectable)
					container.HighlightInternal();
				SelectedElement.Highlight();
			}
		}

		public virtual void UnhighlightInternal()
		{
			if (IsSelectable && SelectedElement != null)
			{
				if (SelectedElement is UIContainer container && container.IsSelectable)
					container.UnhighlightInternal();
				SelectedElement.Unhighlight();
			}
		}

		public override void MouseClick(Vector2 mouseGlobalPosition)
		{
			base.MouseClick(mouseGlobalPosition);

			if (IsSelectable)
				SelectedElement?.MouseClick(mouseGlobalPosition);
		}

		public override void MouseRelease(Vector2 mouseGlobalPosition)
		{
			base.MouseRelease(mouseGlobalPosition);

			if (IsSelectable)
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
	}

	public class Definition
	{
		public enum DefinitionTypes { Fixed, Auto, Fill }
		public DefinitionTypes DefinitionType { get; set; }
		public int FixedLength { get; set; } = 0;

		public static Definition Auto { get; } = new Definition(DefinitionTypes.Auto);
		public static Definition Fill { get; } = new Definition(DefinitionTypes.Fill);

		/// <summary>
		/// Defines a length in a UIContainer
		/// </summary>
		/// <param name="fixedLength">The fixed length of the definition.</param>
		public Definition(int fixedLength)
		{
			DefinitionType = DefinitionTypes.Fixed;
			this.FixedLength = fixedLength;
		}

		/// <summary>
		/// Defines a length in a UIContainer
		/// </summary>
		/// <param name="definitionType">
		/// Fixed: A length you can specify here as an integer.
		/// Auto: Takes up only the space of its largest item. 
		/// Fill: Takes up all remaining space in the container, divided evenly between the other Fill definitions of this dimension.</param>
		public Definition(DefinitionTypes definitionType)
		{
			this.DefinitionType = definitionType;
		}
	}
}
