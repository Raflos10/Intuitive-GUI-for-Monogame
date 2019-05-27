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

        // <column, row>, GridEntries index
        public Dictionary<Tuple<int, int>, int> selectableLocations = new Dictionary<Tuple<int, int>, int>();
        public List<int?> selectableColumns = new List<int?>();
        public List<int?> selectableRows = new List<int?>();
        public Tuple<int, int> Selection, PrimarySelection;
        public bool SelectionOutOfBounds { get; private set; }
        public Selectable SelectedItem { get; private set; }
        private bool usingMouse = true;

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

        public void AddColumnDefinition(ColumnDefinition columnDefinition)
        {
            _columnDefinitions.Add(columnDefinition);
        }

        public void AddRowDefinition(RowDefinition rowDefinition)
        {
            _rowDefinitions.Add(rowDefinition);
        }

        public void AddChild(UIItem uiItem, int column, int row)
        {
            _gridEntries.Add(new GridEntry(uiItem, column, row));

            ////maybe move this to BuildGrid()
            //if (uiItem.GetType().IsSubclassOf(typeof(Selectable)))
            //{
            //    //make sure it isn't a grid with only non-selectables inside
            //    bool addSelectable = true;
            //    if (uiItem is Grid grid)
            //        if (grid.selectableColumns.Count == 0 && grid.selectableRows.Count == 0)
            //            addSelectable = false;

            //    if (addSelectable)
            //    {
            //        while (selectableColumns.Count <= column)
            //            selectableColumns.Add(null);
            //        while (selectableRows.Count <= row)
            //            selectableRows.Add(null);
            //        selectableColumns[column] = column;
            //        selectableRows[row] = row;

            //        selectableLocations.Add(new Tuple<int, int>(column, row), GridEntries.Count - 1);
            //    }
            //}
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
                        for (int j = 0; j < _gridEntries.Count; j++)
                        {
                            if (_gridEntries[j].Column == i)
                            {
                                if (_gridEntries[j].UIItem is Grid grid)
                                    grid.BuildGrid(_columnDefinitions[_gridEntries[j].Column], _rowDefinitions[_gridEntries[j].Row]);
                                int widthAndMargin = _gridEntries[j].UIItem.Width + _gridEntries[j].UIItem.Margin.Left + _gridEntries[j].UIItem.Margin.Right;
                                if (widthAndMargin > largest)
                                    largest = widthAndMargin;
                            }
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
                        for (int j = 0; j < _gridEntries.Count; j++)
                        {
                            if (_gridEntries[j].Row == i)
                            {
                                if (_gridEntries[j].UIItem is Grid grid)
                                    grid.BuildGrid(_columnDefinitions[_gridEntries[j].Column], _rowDefinitions[_gridEntries[j].Row]);
                                int heightAndMargin = _gridEntries[j].UIItem.Height + _gridEntries[j].UIItem.Margin.Top + _gridEntries[j].UIItem.Margin.Bottom;
                                if (heightAndMargin > largest)
                                    largest = heightAndMargin;
                            }
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

            ////get the global points for each edge of the column/row
            //int[] BuildEdges(int[] definitions, bool column)
            //{
            //    //this array holds the size of each area in the column/row
            //    int[] sizes = new int[definitions.Length];

            //    //this is just to figure out how much of the grid is left for the auto size areas
            //    int currentSize = 0;
            //    //list of the indexes of the auto size areas
            //    List<int> autoSizes = new List<int>();
            //    //get all the specific sizes, and get the indexes for auto size areas
            //    for (int i = 0; i < definitions.Length; i++)
            //    {
            //        if (definitions[i] > 0)
            //        {
            //            sizes[i] = definitions[i];
            //            currentSize += definitions[i];
            //        }
            //        else
            //            autoSizes.Add(i);
            //    }

            //    //only necessary if we have auto size areas
            //    if (autoSizes.Count > 0)
            //    {
            //        //total size of this grid in our current dimension
            //        int gridSize = column ? Width : Height;

            //        //the size of all autosize areas (all equal sizes determined by overall grid size)
            //        int autoSize = (gridSize - currentSize) / autoSizes.Count;

            //        //fill in the missing sizes with the auto size
            //        for (int i = 0; i < autoSizes.Count; i++)
            //            sizes[autoSizes[i]] = autoSize;
            //    }

            //    //result array
            //    int[] res = new int[definitions.Length + 1];
            //    //using this int again for another loop
            //    currentSize = 0;
            //    //the grid's location
            //    int location = column ? (int)Position.X : (int)Position.Y;
            //    //convert the sizes to global points
            //    for (int i = 0; i < res.Length; i++)
            //    {
            //        res[i] = location + currentSize;
            //        if (i < sizes.Length)
            //            currentSize += sizes[i];
            //    }

            //    return res;
            //}

            //bool hasSelection = false;
            //int lowestColumn = 0, lowestRow = 0;
            //for (int i = 0; i < GridEntries.Count; i++)
            //{
            //    //if (GridEntries[i].UIItem.GetType() == typeof(Grid))
            //    //{
            //    //    Grid subGrid = (Grid)GridEntries[i].UIItem;
            //    //    int x = columnEdges[GridEntries[i].Column];
            //    //    int y = rowEdges[GridEntries[i].Row];
            //    //    subGrid.Rectangle = new Rectangle(x, y, columnEdges[GridEntries[i].Column + 1] - x, rowEdges[GridEntries[i].Row + 1] - y);
            //    //    subGrid.BuildGrid();
            //    //}
            //    //else
            //    //{
            //    //    Rectangle originalRect = GridEntries[i].UIItem.Rectangle;
            //    //    GridEntries[i].UIItem.Rectangle = new Rectangle(columnEdges[GridEntries[i].Column], rowEdges[GridEntries[i].Row],
            //    //        originalRect.Width, originalRect.Height);
            //    //}

            //    if (GridEntries[i].UIItem.GetType().IsSubclassOf(typeof(Selectable)))
            //    {
            //        Selectable selItem = (Selectable)GridEntries[i].UIItem;

            //        if (hasSelection)
            //        {
            //            if (GridEntries[i].Column < lowestColumn)
            //                lowestColumn = GridEntries[i].Column;
            //            if (GridEntries[i].Row < lowestRow)
            //                lowestRow = GridEntries[i].Row;
            //        }
            //        else
            //        {
            //            lowestColumn = GridEntries[i].Column;
            //            lowestRow = GridEntries[i].Row;
            //            OnSelect += Grid_OnSelect;
            //            hasSelection = true;
            //        }
            //    }
            //}
            //if (hasSelection)
            //{
            //    PrimarySelection = new Tuple<int, int>(lowestColumn, lowestRow);
            //    Selection = PrimarySelection;
            //    OnDeselect += Grid_OnDeselect;
            //}
        }

        //private void Grid_OnSelect(object sender, EventArgs e)
        //{
        //    //given key not in dictionary
        //    Selectable selectableItem = (Selectable)GridEntries[selectableLocations[Selection]].UIItem;
        //    selectableItem.Selected = true;
        //}

        //private void Grid_OnDeselect(object sender, EventArgs e)
        //{
        //    for (int i = 0; i < GridEntries.Count; i++)
        //    {
        //        Selectable selItem = GridEntries[i].UIItem as Selectable;
        //        if (selItem != null)
        //            selItem.Selected = false;
        //        Selection = PrimarySelection;
        //    }
        //}

        public override void InputTrigger(Menu.MenuInputs input)
        {
            //if (!usingMouse)
            //    switch (input)
            //    {
            //        case Menu.MenuInputs.Up:
            //            HandleSelectionChange(0, -1);
            //            break;
            //        case Menu.MenuInputs.Down:
            //            HandleSelectionChange(0, 1);
            //            break;
            //        case Menu.MenuInputs.Left:
            //            HandleSelectionChange(-1, 0);
            //            break;
            //        case Menu.MenuInputs.Right:
            //            HandleSelectionChange(1, 0);
            //            break;
            //        case Menu.MenuInputs.OK:
            //            Selectable selectedItem = (Selectable)GridEntries[selectableLocations[Selection]].UIItem;
            //            selectedItem.InputTrigger(input);
            //            break;
            //    }
            //else
            //{
            //    if (GridEntries[selectableLocations[Selection]].UIItem is Selectable selectable)
            //        selectable.Selected = true;
            //    usingMouse = false;
            //}
        }

        //private void HandleSelectionChange(int x = 0, int y = 0)
        //{
        //    if (selectableColumns.Contains(Selection.Item1 + x) && selectableRows.Contains(Selection.Item2 + y))
        //    {
        //        Selectable previousItem = (Selectable)GridEntries[selectableLocations[Selection]].UIItem;
        //        bool skip = false;
        //        if (previousItem.GetType() == typeof(Grid))
        //        {
        //            Grid previousGrid = (Grid)previousItem;
        //            if (previousGrid.SelectionOutOfBounds)
        //            {
        //                previousGrid.Selected = false;
        //                previousGrid.SelectionOutOfBounds = false;
        //            }
        //            else
        //                skip = true;
        //        }
        //        if (!skip)
        //        {
        //            previousItem.Selected = false;
        //            Selection = new Tuple<int, int>(Selection.Item1 + x, Selection.Item2 + y);
        //            Selectable nextItem = (Selectable)GridEntries[selectableLocations[Selection]].UIItem;
        //            nextItem.Selected = true;
        //        }
        //    }
        //    else
        //        SelectionOutOfBounds = true;
        //}

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
        public UIItem UIItem { get; private set; }
        public int Column { get; private set; }
        public int Row { get; private set; }

        public GridEntry(UIItem uiItem, int column, int row)
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