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
	public class Grid : Selectable
	{
		#region Read-Only Lists

		private List<ColumnDefinition> _columnDefinitions = new List<ColumnDefinition>();
		public IReadOnlyCollection<ColumnDefinition> ColumnDefinitions
		{
			get { return _columnDefinitions.AsReadOnly(); }
		}
		private List<RowDefinition> _rowDefinitions = new List<RowDefinition>();
		public IReadOnlyCollection<RowDefinition> RowDefinitions
		{
			get { return _rowDefinitions.AsReadOnly(); }
		}

		private List<GridEntry> _gridEntries = new List<GridEntry>();
		public IReadOnlyCollection<GridEntry> GridEntries
		{
			get { return _gridEntries.AsReadOnly(); }
		}

		#endregion

		private Column[] columns;
		private Row[] rows;

		private readonly Dictionary<Point, int> gridEntryIndexByLocation = new Dictionary<Point, int>();

		#region Selectable Data

		private readonly Dictionary<int, List<int>> selectableRowsInColumn = new Dictionary<int, List<int>>();
		private readonly Dictionary<int, List<int>> selectableColumnsInRow = new Dictionary<int, List<int>>();
		private int firstSelectableColumn, firstSelectableRow, lastSelectableColumn, lastSelectableRow;

		private Point selection, primarySelection, ghostSelection;
		private bool primarySelectionSpecified;
		public Selectable SelectedItem
		{
			get { return (Selectable)GridEntries.ElementAt(gridEntryIndexByLocation[new Point(selection.X, selection.Y)]).UIItem; }
		}

		public bool SelectableGrid { get; private set; }

		EventHandler OutOfBoundsSelection;

		#endregion

		/// <summary>
		/// Grid for holding multiple UI Items.
		/// </summary>
		/// <param name="margin">
		/// The size of the margin area surrounding this grid.
		/// </param>
		public Grid(Margin margin = null)
		{
			this.Margin = margin ?? Margin.Zero;
		}

		/// <summary>
		/// Grid for holding multiple UI Items.
		/// </summary>
		/// <param name="margin">
		/// The size of the margin area surrounding this grid.
		/// </param>
		/// <param name="primarySelection">
		/// The first item to be selected when using a keyboard or gamepad. Defaults to the first child added to the grid. 
		/// </param>
		public Grid(Point primarySelection, Margin margin = null)
		{
			this.Margin = margin ?? Margin.Zero;
			this.primarySelection = primarySelection;
			primarySelectionSpecified = true;
		}

		public void AddColumnDefinition(ColumnDefinition columnDefinition)
		{
			_columnDefinitions.Add(columnDefinition);
		}

		public void AddRowDefinition(RowDefinition rowDefinition)
		{
			_rowDefinitions.Add(rowDefinition);
		}

		public void AddChild(UIElement uiItem, int column, int row)
		{
			if (column >= ColumnDefinitions.Count)
				throw new Exception("Column index out of bounds.");
			if (row >= RowDefinitions.Count)
				throw new Exception("Row index out of bounds.");

			_gridEntries.Add(new GridEntry(uiItem, column, row));
			gridEntryIndexByLocation.Add(new Point(column, row), GridEntries.Count - 1);

			if (uiItem is Selectable selectable)
				AddSelectable(selectable, column, row);
		}

		void AddSelectable(Selectable selectable, int column, int row)
		{
			// make sure it's not a grid with nothing selectable inside
			if (selectable is Grid grid)
				if (!grid.SelectableGrid)
					return;
				else
					grid.OutOfBoundsSelection += ReturnFromInnerGrid;

			// if the item is Selectable, make sure its column and row are selectable
			// also store the selectable item's location in it's specific column/row
			if (!selectableRowsInColumn.ContainsKey(column))
			{
				if (firstSelectableColumn > column || selectableRowsInColumn.Count == 0)
					firstSelectableColumn = column;
				if (lastSelectableColumn < column)
					lastSelectableColumn = column;
				selectableRowsInColumn.Add(column, new List<int>());
			}
			selectableRowsInColumn[column].Add(row);

			if (!selectableColumnsInRow.ContainsKey(row))
			{
				if (firstSelectableRow > row || selectableColumnsInRow.Count == 0)
					firstSelectableRow = row;
				if (lastSelectableRow < row)
					lastSelectableRow = row;
				selectableColumnsInRow.Add(row, new List<int>());
			}
			selectableColumnsInRow[row].Add(column);

			if (!SelectableGrid)
			{
				// the first added selectable child is the default selected
				if (!primarySelectionSpecified)
					primarySelection = new Point(column, row);
				selection = primarySelection;
				ghostSelection = primarySelection;
				//OnHighlight += HighlightChild;
				//OnUnhighlight += UnhighlightChild;
				OnSwitchInputMethod += (sender, args) => UnhighlightInternal();
				SelectableGrid = true;
			}
		}

		public void ChangePrimarySelection(Point location)
		{
			primarySelection = location;
		}

		public EventHandler debugEvent;
		public int debug;

		public void BuildGrid(int containerWidth, int containerHeight)
		{
			Width = containerWidth - Margin.Left - Margin.Right;
			Height = containerHeight - Margin.Top - Margin.Bottom;

			// this grid's
			int[] columnWidths, rowHeights;

			#region Columns

			columnWidths = new int[_columnDefinitions.Count];

			List<int> starColumns = new List<int>();
			int totalWidth = 0;

			for (int i = 0; i < _columnDefinitions.Count; i++)
			{
				switch (_columnDefinitions[i].DefinitionType)
				{
					case ColumnDefinition.DefinitionTypes.Fixed:
						columnWidths[i] = _columnDefinitions[i].Width;
						totalWidth += _columnDefinitions[i].Width;
						break;

					case ColumnDefinition.DefinitionTypes.Auto:
						int largest = 0;
						foreach (GridEntry gridEntry in GridEntries)
							if (gridEntry.Column == i)
							{
								// there is no way for a grid to be built without a fixed size, so change it to fill
								//TODO BuildGridVariableSize(out int width, out int height) returns a total width and height, fill columns/rows become zero
								if (gridEntry.UIItem is Grid grid)
								{
									if (!starColumns.Contains(i))
										starColumns.Add(i);
									continue;
								}
								int widthAndMargin = gridEntry.UIItem.Width + gridEntry.UIItem.Margin.Left + gridEntry.UIItem.Margin.Right;
								if (widthAndMargin > largest)
									largest = widthAndMargin;
							}
						columnWidths[i] = largest;
						totalWidth += largest;
						break;

					case ColumnDefinition.DefinitionTypes.Fill:
						starColumns.Add(i);
						break;
				}
			}

			int starWidth = starColumns.Count > 0 ? (Width - totalWidth) / starColumns.Count : 0;
			for (int i = 0; i < starColumns.Count; i++)
				columnWidths[starColumns[i]] = starWidth;

			totalWidth = 0;
			columns = new Column[columnWidths.Length];
			for (int i = 0; i < columnWidths.Length; i++)
			{
				columns[i] = new Column(totalWidth + Margin.Left, columnWidths[i]);
				totalWidth += columnWidths[i];
			}

			#endregion

			#region Rows

			rowHeights = new int[_rowDefinitions.Count];

			List<int> starRows = new List<int>();
			int totalHeight = 0;

			for (int i = 0; i < _rowDefinitions.Count; i++)
			{
				switch (_rowDefinitions[i].DefinitionType)
				{
					case RowDefinition.DefinitionTypes.Fixed:
						rowHeights[i] = _rowDefinitions[i].Height;
						totalHeight += _rowDefinitions[i].Height;
						break;

					case RowDefinition.DefinitionTypes.Auto:
						int largest = 0;
						foreach (GridEntry gridEntry in GridEntries)
							if (gridEntry.Row == i)
							{
								// there is no way for a grid to be built without a fixed size, so change it to fill
								//TODO BuildGridVariableSize(out int width, out int height) returns a total width and height, fill columns/rows become zero
								if (gridEntry.UIItem is Grid grid)
								{
									if (!starRows.Contains(i))
										starRows.Add(i);
									continue;
								}
								int heightAndMargin = gridEntry.UIItem.Height + gridEntry.UIItem.Margin.Top + gridEntry.UIItem.Margin.Bottom;
								if (heightAndMargin > largest)
									largest = heightAndMargin;
							}
						rowHeights[i] = largest;
						totalHeight += largest;
						break;

					case RowDefinition.DefinitionTypes.Fill:
						starRows.Add(i);
						break;
				}
			}

			int starHeight = starRows.Count > 0 ? (Height - totalHeight) / starRows.Count : 0;
			for (int i = 0; i < starRows.Count; i++)
				rowHeights[starRows[i]] = starHeight;

			totalHeight = 0;
			rows = new Row[rowHeights.Length];
			for (int i = 0; i < rowHeights.Length; i++)
			{
				rows[i] = new Row(totalHeight + Margin.Top, rowHeights[i]);
				totalHeight += rowHeights[i];
			}

			#endregion

			foreach (GridEntry gridEntry in GridEntries)
			{
				gridEntry.UIItem.UpdateBounding(columns[gridEntry.Column], rows[gridEntry.Row]);
				if (gridEntry.UIItem is Grid grid)
					grid.BuildGrid(columns[gridEntry.Column].Width, rows[gridEntry.Row].Height);
			}
		}

		public void HighlightInternal()
		{
			if (SelectedItem is Grid grid)
				grid.HighlightInternal();
			else
				SelectedItem?.Highlight();
			Highlight();
		}

		public void UnhighlightInternal()
		{
			if (SelectedItem is Grid grid)
				grid.UnhighlightInternal();
			else
				SelectedItem?.Unhighlight();
			Unhighlight();
		}

		void UnhighlightAll()
		{
			foreach (GridEntry gridEntry in GridEntries)
				switch (gridEntry.UIItem)
				{
					case Grid grid:
						grid.ResetSelection();
						break;

					case Selectable selectable:
						selectable.Unhighlight();
						break;
				}
		}

		#region Selection With Mouse

		public override void MouseClick(Vector2 mouseGlobalPosition)
		{
			base.MouseClick(mouseGlobalPosition);

			if (SelectableGrid)
				SelectedItem.MouseClick(mouseGlobalPosition);
		}

		public override void MouseRelease(Vector2 mouseGlobalPosition)
		{
			base.MouseRelease(mouseGlobalPosition);

			if (SelectableGrid)
				SelectedItem.MouseRelease(mouseGlobalPosition);
		}

		public override void MouseUpdate(Vector2 mouseGlobalPosition)
		{
			if (SelectableGrid)
			{
				//base.MouseUpdate(mouseGlobalPosition);

				Vector2 mouseLocalPosition = GetMouseLocalPosition(mouseGlobalPosition);

				// mouse has not gone outside selected space
				if (columns[selection.X].LeftPosition <= mouseLocalPosition.X && columns[selection.X].RightPosition >= mouseLocalPosition.X &&
					rows[selection.Y].TopPosition <= mouseLocalPosition.Y && rows[selection.Y].BottomPosition >= mouseLocalPosition.Y)
				{
					SelectedItem.MouseUpdate(mouseGlobalPosition);

					return;
				}

				// mouse is currently outside selected space

				if (!ContainsMouse(mouseGlobalPosition))
				{
					UnhighlightInternal();
					return;
				}

				Point mousePoint = selection;

				for (int i = 0; i < columns.Length; i++)
					if (columns[i].RightPosition >= mouseLocalPosition.X && columns[i].LeftPosition <= mouseLocalPosition.X)
					{
						mousePoint.X = i;
						break;
					}
				for (int i = 0; i < rows.Length; i++)
					if (rows[i].BottomPosition >= mouseLocalPosition.Y && rows[i].TopPosition <= mouseLocalPosition.Y)
					{
						mousePoint.Y = i;
						break;
					}

				// if mouse is over a selectable region
				if (gridEntryIndexByLocation.ContainsKey(mousePoint) &&
					GridEntries.ElementAt(gridEntryIndexByLocation[mousePoint]).UIItem is Selectable selectable)
				{
					// if it's not a selectable grid, don't do anything yet
					if (selectable is Grid grid && !grid.SelectableGrid)
						return;

					// if Persistant Highlight is on and the mouse isn't directly over the new selection, don't do anything yet
					if (PersistantHighlight && selectable.StrictBoundingBox && !selectable.ContainsMouse(mouseGlobalPosition))
						return;
					else if (SelectedItem.Highlighted)
						SelectedItem.Unhighlight();

					selection = mousePoint;
					ghostSelection = selection;

					SelectedItem.MouseUpdate(mouseGlobalPosition);
				}
				else if (!PersistantHighlight && SelectedItem.Highlighted)
					SelectedItem.Unhighlight();
			}
		}

		#endregion

		private void ChangeSelection(int column, int row)
		{
			Exception exception = new Exception("Tried to select a non-selectable location.");

			Point location = new Point(column, row);
			if (gridEntryIndexByLocation.ContainsKey(location))
			{
				if (GridEntries.ElementAt(gridEntryIndexByLocation[location]).UIItem is Selectable selectable)
					selection = location;
				else
					throw exception;
			}
			else
				throw exception;
		}

		public override void ResetSelection()
		{
			if (SelectableGrid)
			{
				UnhighlightAll();
				ChangeSelection(primarySelection.X, primarySelection.Y);
			}
		}

		#region Keyboard Input

		public override void InputTrigger(Menu.MenuInputs input)
		{
			if (SelectableGrid)
			{
				if (input == Menu.MenuInputs.OK)
					SelectedItem.InputTrigger(input);
				else if (SelectedItem is Grid grid)
					grid.InputTrigger(input);
				else
					HandleSelectionChange(input);
			}
		}

		// returns nearest number in array to "num"
		int GetClosestNumber(int num, int[] candidates)
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

		private void HandleSelectionChange(Menu.MenuInputs input)
		{
			int columnPlus = input == Menu.MenuInputs.Left ? -1 : input == Menu.MenuInputs.Right ? 1 : 0;
			int rowPlus = input == Menu.MenuInputs.Up ? -1 : input == Menu.MenuInputs.Down ? 1 : 0;

			if (selection.X + columnPlus > lastSelectableColumn || selection.Y + rowPlus > lastSelectableRow ||
				selection.X + columnPlus < firstSelectableColumn || selection.Y + rowPlus < firstSelectableRow)
				OutOfBoundsSelection?.Invoke(this, new OutOfBoundsSelectionEventArgs(input));
			else
			{
				if (columnPlus != 0)
				{
					for (int i = selection.X + columnPlus; i <= lastSelectableColumn && i >= firstSelectableColumn; i += columnPlus)
						if (selectableRowsInColumn.ContainsKey(i))
						{
							int selectableRow = GetClosestNumber(ghostSelection.Y, selectableRowsInColumn[i].ToArray());
							if (GridEntries.ElementAt(gridEntryIndexByLocation[new Point(i, selectableRow)]).UIItem is Grid grid)
								grid.EnterFromOutside(columnPlus > 0 ? Menu.MenuInputs.Right : Menu.MenuInputs.Left);
							UnhighlightInternal();
							ghostSelection.X = i;
							ChangeSelection(i, selectableRow);
							HighlightInternal();
							break;
						}
				}
				else if (rowPlus != 0)
				{
					for (int i = selection.Y + rowPlus; i <= lastSelectableRow && i >= firstSelectableRow; i += rowPlus)
						if (selectableColumnsInRow.ContainsKey(i))
						{
							int selectableColumn = GetClosestNumber(ghostSelection.X, selectableColumnsInRow[i].ToArray());
							if (GridEntries.ElementAt(gridEntryIndexByLocation[new Point(selectableColumn, i)]).UIItem is Grid grid)
								grid.EnterFromOutside(rowPlus > 0 ? Menu.MenuInputs.Down : Menu.MenuInputs.Up);
							UnhighlightInternal();
							ghostSelection.Y = i;
							ChangeSelection(selectableColumn, i);
							HighlightInternal();
							break;
						}
				}
			}
		}

		void ReturnFromInnerGrid(object sender, EventArgs e)
		{
			if (e is OutOfBoundsSelectionEventArgs args)
				HandleSelectionChange(args.InputDirection);
		}

		public void EnterFromOutside(Menu.MenuInputs input)
		{
			switch (input)
			{
				case Menu.MenuInputs.Left:
					selection = new Point(lastSelectableColumn,
						GetClosestNumber(selection.Y, selectableRowsInColumn[lastSelectableColumn].ToArray()));
					break;

				case Menu.MenuInputs.Right:
					selection = new Point(firstSelectableColumn,
						GetClosestNumber(selection.Y, selectableRowsInColumn[firstSelectableColumn].ToArray()));
					break;

				case Menu.MenuInputs.Up:
					selection = new Point(GetClosestNumber(selection.X,
						selectableColumnsInRow[lastSelectableRow].ToArray()), lastSelectableRow);
					break;

				case Menu.MenuInputs.Down:
					selection = new Point(GetClosestNumber(selection.X,
						selectableColumnsInRow[firstSelectableRow].ToArray()), firstSelectableRow);
					break;
			}
			ghostSelection = selection;
		}

		#endregion

		public override void UpdateTransformProperties(Matrix parentMatrix)
		{
			base.UpdateTransformProperties(parentMatrix);

			for (int i = 0; i < GridEntries.Count; i++)
			{
				int x = columns[_gridEntries[i].Column].LeftPosition;
				int y = rows[_gridEntries[i].Row].TopPosition;
				Matrix transform = Matrix.CreateTranslation(x, y, 0f) * parentMatrix;
				_gridEntries[i].UIItem.UpdateTransformProperties(transform);
			}
		}

		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			for (int i = 0; i < GridEntries.Count; i++)
				_gridEntries[i].UIItem?.Draw(spriteBatch, gameTime);
		}
	}

	public class GridEntry
	{
		public UIElement UIItem { get; private set; }
		public int Column { get; private set; }
		public int Row { get; private set; }

		public GridEntry(UIElement uiItem, int column, int row)
		{
			this.UIItem = uiItem;
			this.Column = column;
			this.Row = row;
		}
	}

	public class ColumnDefinition
	{
		public enum DefinitionTypes { Fixed, Auto, Fill }
		public DefinitionTypes DefinitionType { get; set; }
		public int Width { get; set; } = 0;

		public static ColumnDefinition Auto { get; private set; } = new ColumnDefinition(DefinitionTypes.Auto);
		public static ColumnDefinition Fill { get; private set; } = new ColumnDefinition(DefinitionTypes.Fill);

		/// <summary>
		/// Defines a column in a grid.
		/// </summary>
		/// <param name="width">The fixed width of the column.</param>
		public ColumnDefinition(int width)
		{
			DefinitionType = DefinitionTypes.Fixed;
			this.Width = width;
		}

		/// <summary>
		/// Defines a column in a grid.
		/// </summary>
		/// <param name="definitionType">
		/// Fixed: A width you can specify here as an integer.
		/// Auto: Takes up only the space of its widest item. 
		/// Fill: Takes up all remaining space on the grid, divided evenly between any other columns of this type.</param>
		public ColumnDefinition(DefinitionTypes definitionType)
		{
			this.DefinitionType = definitionType;
		}
	}

	public class RowDefinition
	{
		public enum DefinitionTypes { Fixed, Auto, Fill }
		public DefinitionTypes DefinitionType { get; set; }
		public int Height { get; set; } = 0;

		public static RowDefinition Auto { get; private set; } = new RowDefinition(DefinitionTypes.Auto);
		public static RowDefinition Fill { get; private set; } = new RowDefinition(DefinitionTypes.Fill);

		/// <summary>
		/// Defines a row in a grid.
		/// </summary>
		/// <param name="height">The fixed height of the row.</param>
		public RowDefinition(int height)
		{
			DefinitionType = DefinitionTypes.Fixed;
			this.Height = height;
		}

		/// <summary>
		/// Defines a row in a grid.
		/// </summary>
		/// <param name="definitionType">
		/// Fixed: A height you can specify here as an integer.
		/// Auto: Takes up only the space of its tallest item. 
		/// Fill: Takes up all remaining space on the grid, divided evenly between any other rows of this type.</param>
		public RowDefinition(DefinitionTypes definitionType)
		{
			this.DefinitionType = definitionType;
		}
	}

	public class Column
	{
		public int LeftPosition { get; set; }
		public int RightPosition { get; set; }
		public int Width { get; private set; }

		public Column(int leftPosition, int width)
		{
			this.LeftPosition = leftPosition;
			this.Width = width;
			this.RightPosition = leftPosition + width;
		}
	}

	public class Row
	{
		public int TopPosition { get; set; }
		public int BottomPosition { get; set; }
		public int Height { get; private set; }

		public Row(int topPosition, int height)
		{
			this.TopPosition = topPosition;
			this.Height = height;
			this.BottomPosition = topPosition + height;
		}
	}

	public class OutOfBoundsSelectionEventArgs : EventArgs
	{
		public Menu.MenuInputs InputDirection { get; private set; }
		public OutOfBoundsSelectionEventArgs(Menu.MenuInputs inputDirection)
		{
			this.InputDirection = inputDirection;
		}
	}
}