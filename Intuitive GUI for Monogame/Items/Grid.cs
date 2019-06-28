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

        private readonly Dictionary<int, List<int>> selectableColumnsLocations = new Dictionary<int, List<int>>();
        private readonly Dictionary<int, List<int>> selectableRowsLocations = new Dictionary<int, List<int>>();
        private int firstSelectableColumn, firstSelectableRow, lastSelectableColumn, lastSelectableRow;

        #endregion

        private Point selection, primarySelection;
        public bool SelectionOutOfBounds { get; private set; } // make function?
        public Selectable SelectedItem { get; private set; }

        public bool SelectableGrid { get; private set; }
        private bool outOfBounds = false;
        public bool OutOfBounds
        {
            get
            {
                bool val = outOfBounds;
                outOfBounds = false;
                return val;
            }
            private set { outOfBounds = value; }
        }

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

            // if the item is Selectable, make sure its column and row are selectable
            // also store the selectable item's location in it's specific column/row
            if (!selectableColumnsLocations.ContainsKey(column))
            {
                if (firstSelectableColumn > column || selectableColumnsLocations.Count == 0)
                    firstSelectableColumn = column;
                if (lastSelectableColumn < column)
                    lastSelectableColumn = column;
                selectableColumnsLocations.Add(column, new List<int>());
                selectableColumnsLocations[column].Add(row);
            }
            if (!selectableRowsLocations.ContainsKey(row))
            {
                if (firstSelectableRow > row || selectableRowsLocations.Count == 0)
                    firstSelectableRow = row;
                if (lastSelectableRow < row)
                    lastSelectableRow = row;
                selectableRowsLocations.Add(row, new List<int>());
                selectableRowsLocations[row].Add(column);
            }

            if (!SelectableGrid)
            {
                // the first added selectable child is the default selected
                primarySelection = new Point(column, row);
                OnHighlight += HighlightThis;
                OnUnhighlight += UnhighlightThis;
                SelectableGrid = true;
            }
        }

        public void BuildGrid(ColumnDefinition containerColumnDefinition, RowDefinition containerRowDefinition)
        {
            if (containerColumnDefinition.DefinitionType == ColumnDefinition.DefinitionTypes.Fixed)
                Width = containerColumnDefinition.Width - Margin.Left - Margin.Right;

            if (containerRowDefinition.DefinitionType == RowDefinition.DefinitionTypes.Fixed)
                Height = containerRowDefinition.Height - Margin.Top - Margin.Bottom;

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
                                if (gridEntry.UIItem is Grid grid)
                                    grid.BuildGrid(_columnDefinitions[gridEntry.Column], _rowDefinitions[gridEntry.Row]);
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

            if (containerColumnDefinition.DefinitionType == ColumnDefinition.DefinitionTypes.Fixed)
            {
                int starWidth = starColumns.Count > 0 ? (Width - totalWidth) / starColumns.Count : 0;
                for (int i = 0; i < starColumns.Count; i++)
                    columnWidths[starColumns[i]] = starWidth;
            }
            else
                Width = totalWidth - Margin.Left - Margin.Right;

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
                                if (gridEntry.UIItem is Grid grid)
                                    grid.BuildGrid(_columnDefinitions[gridEntry.Column], _rowDefinitions[gridEntry.Row]);
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

            if (containerRowDefinition.DefinitionType == RowDefinition.DefinitionTypes.Fixed)
            {
                int starHeight = starRows.Count > 0 ? (Height - totalHeight) / starRows.Count : 0;
                for (int i = 0; i < starRows.Count; i++)
                    rowHeights[starRows[i]] = starHeight;
            }
            else
                Height = totalHeight - Margin.Top - Margin.Bottom;

            totalHeight = 0;
            rows = new Row[rowHeights.Length];
            for (int i = 0; i < rowHeights.Length; i++)
            {
                rows[i] = new Row(totalHeight + Margin.Top, rowHeights[i]);
                totalHeight += rowHeights[i];
            }

            #endregion
        }

        void HighlightThis(object sender, EventArgs e)
        {
            if (selectableColumnsLocations.Count == 0 && selectableRowsLocations.Count == 0)
                return;

            if (!gridEntryIndexByLocation.ContainsKey(new Point(selection.X, selection.Y)) ||
                GridEntries.ElementAt(gridEntryIndexByLocation[new Point(selection.X, selection.Y)]).UIItem is Selectable selectable)
                ChangeSelection(primarySelection.X, primarySelection.Y);
        }

        void UnhighlightThis(object sender, EventArgs e)
        {
            if (selectableColumnsLocations.Count == 0 && selectableRowsLocations.Count == 0)
                return;

            foreach (GridEntry gridEntry in GridEntries)
                if (gridEntry.UIItem is Selectable selectable)
                    selectable.Unselect();
        }

        public override void InputTrigger(Menu.MenuInputs input)
        {
            if (SelectedItem is Grid grid)
            {
                grid.InputTrigger(input);
                if (!grid.OutOfBounds)
                    return;
            }

            switch (input)
            {
                case Menu.MenuInputs.Up:
                    HandleSelectionChange(0, -1);
                    break;
                case Menu.MenuInputs.Down:
                    HandleSelectionChange(0, 1);
                    break;
                case Menu.MenuInputs.Left:
                    HandleSelectionChange(-1, 0);
                    break;
                case Menu.MenuInputs.Right:
                    HandleSelectionChange(1, 0);
                    break;
                case Menu.MenuInputs.OK:
                    SelectedItem.InputTrigger(input);
                    break;
            }
        }

        private void ChangeSelection(int column, int row)
        {
            Exception exception = new Exception("Tried to select a non-selectable location.");

            Point location = new Point(column, row);
            if (gridEntryIndexByLocation.ContainsKey(location))
            {
                if (SelectedItem != null)
                    SelectedItem.Unselect();
                if (GridEntries.ElementAt(gridEntryIndexByLocation[location]).UIItem is Selectable selectable)
                    SelectedItem = selectable;
                else
                    throw exception;
                SelectedItem.Select();
                selection = location;
            }
            else
                throw exception;
        }

        private void HandleSelectionChange(int columnPlus, int rowPlus)
        {
            if (selection.X + columnPlus > lastSelectableColumn || selection.Y + rowPlus > lastSelectableRow ||
                selection.X + columnPlus < firstSelectableColumn || selection.Y + rowPlus < firstSelectableRow)
                OutOfBounds = true;
            else
            {
                if (columnPlus != 0)
                {
                    for (int i = selection.X + columnPlus; i <= lastSelectableColumn && i >= firstSelectableColumn; i += columnPlus)
                        if (selectableColumnsLocations.ContainsKey(i))
                        {
                            ChangeSelection(i, selection.Y);
                            break;
                        }
                }
                else if (rowPlus != 0)
                {
                    for (int i = selection.Y + rowPlus; i <= lastSelectableRow && i >= firstSelectableRow; i += rowPlus)
                        if (selectableRowsLocations.ContainsKey(i))
                        {
                            ChangeSelection(selection.X, i);
                            break;
                        }
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

            //if (selectableColumns.Contains(Selection.Item1 + x) && selectableRows.Contains(Selection.Item2 + y))
            //{
            //    Selectable previousItem = (Selectable)GridEntries[selectableLocations[Selection]].UIItem;
            //    bool skip = false;
            //    if (previousItem.GetType() == typeof(Grid))
            //    {
            //        Grid previousGrid = (Grid)previousItem;
            //        if (previousGrid.SelectionOutOfBounds)
            //        {
            //            previousGrid.Selected = false;
            //            previousGrid.SelectionOutOfBounds = false;
            //        }
            //        else
            //            skip = true;
            //    }
            //    if (!skip)
            //    {
            //        previousItem.Selected = false;
            //        Selection = new Tuple<int, int>(Selection.Item1 + x, Selection.Item2 + y);
            //        Selectable nextItem = (Selectable)GridEntries[selectableLocations[Selection]].UIItem;
            //        nextItem.Selected = true;
            //    }
            //}
            //else
            //    SelectionOutOfBounds = true;
        }

        public override void MouseUpdate(MouseState mouseState, MouseState mouseStateLast, Vector2 virtualPosition)
        {
            foreach (GridEntry gridEntry in GridEntries)
                if (gridEntry.UIItem is Selectable selectable)
                    selectable.MouseUpdate(mouseState, mouseStateLast, virtualPosition);
        }

        public override void UpdateTransformProperties(Matrix parentMatrix)
        {
            base.UpdateTransformProperties(parentMatrix);

            for (int i = 0; i < GridEntries.Count; i++)
            {
                int x = columns[_gridEntries[i].Column].LeftPosition + GridEntries.ElementAt(i).UIItem.Margin.Left;
                int y = rows[_gridEntries[i].Row].TopPosition + GridEntries.ElementAt(i).UIItem.Margin.Top;
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
}