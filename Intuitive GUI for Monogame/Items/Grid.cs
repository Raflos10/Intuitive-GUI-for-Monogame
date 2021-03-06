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
    public class Grid : UIContainer
    {
        #region Read-Only Collections

        private List<Definition> _columnDefinitions = new List<Definition>();
        public IReadOnlyCollection<Definition> ColumnDefinitions
        {
            get { return _columnDefinitions.AsReadOnly(); }
        }
        private List<Definition> _rowDefinitions = new List<Definition>();
        public IReadOnlyCollection<Definition> RowDefinitions
        {
            get { return _rowDefinitions.AsReadOnly(); }
        }
        private Dictionary<Point, UIElement> _gridEntries = new Dictionary<Point, UIElement>();
        public override IReadOnlyCollection<UIElement> Children
        {
            get { return _gridEntries.Values.ToList().AsReadOnly(); }
        }

        #endregion

        private Segment[] columns, rows;

        #region Selectable Data

        private readonly Dictionary<int, List<int>> selectableRowsInColumn = new Dictionary<int, List<int>>();
        private readonly Dictionary<int, List<int>> selectableColumnsInRow = new Dictionary<int, List<int>>();
        private int firstSelectableColumn, firstSelectableRow, lastSelectableColumn, lastSelectableRow;

        private Point selection, primarySelection, ghostSelection;
        private bool primarySelectionSpecified;
        public override Selectable SelectedElement
        {
            get { return (Selectable)_gridEntries[selection]; }
        }

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
            ChangePrimarySelection(primarySelection.X, primarySelection.Y);
        }

        public void ChangePrimarySelection(int x, int y)
        {
            this.primarySelection = new Point(x, y);
            primarySelectionSpecified = true;
        }

        public void AddColumnDefinition(Definition columnDefinition)
        {
            _columnDefinitions.Add(columnDefinition);
        }

        public void AddRowDefinition(Definition rowDefinition)
        {
            _rowDefinitions.Add(rowDefinition);
        }

        public void AddChild(UIElement Element, int column, int row)
        {
            if (column >= ColumnDefinitions.Count)
                throw new Exception("Column index out of bounds.");
            if (row >= RowDefinitions.Count)
                throw new Exception("Row index out of bounds.");

            _gridEntries.Add(new Point(column, row), Element);

            if (Element is Selectable selectable)
                AddSelectable(selectable, column, row);
        }

        void AddSelectable(Selectable selectable, int column, int row)
        {
            // make sure it's not a container with nothing selectable inside
            if (selectable is UIContainer container)
                if (!container.IsSelectable)
                    return;

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

            if (!IsSelectable)
            {
                // the first added selectable child is the default selected
                if (!primarySelectionSpecified)
                    primarySelection = new Point(column, row);
                selection = primarySelection;
                ghostSelection = primarySelection;
                IsSelectable = true;
            }
        }

        public void BuildGrid(int containerWidth, int containerHeight)
        {
            Width = containerWidth - Margin.Left - Margin.Right;
            Height = containerHeight - Margin.Top - Margin.Bottom;

            columns = BuildDefinitions(ColumnDefinitions, true);
            rows = BuildDefinitions(RowDefinitions, false);

            foreach (KeyValuePair<Point, UIElement> gridEntry in _gridEntries)
            {
                gridEntry.Value.UpdateBounding(columns[gridEntry.Key.X], rows[gridEntry.Key.Y]);
                if (gridEntry.Value is Grid grid)
                    grid.BuildGrid(columns[gridEntry.Key.X].Length, rows[gridEntry.Key.Y].Length);
            }
        }

        private Segment[] BuildDefinitions(IReadOnlyCollection<Definition> definitions, bool column)
        {
            int[] lengths = new int[definitions.Count];

            // must list each fill definition
            HashSet<int> fillDefs = new HashSet<int>();
            int totalLength = 0;

            for (int i = 0; i < definitions.Count; i++)
            {
                switch (definitions.ElementAt(i).DefinitionType)
                {
                    case Definition.DefinitionTypes.Fixed:
                        lengths[i] = definitions.ElementAt(i).FixedLength;
                        totalLength += definitions.ElementAt(i).FixedLength;
                        // each fixed definition just adds to the overall length
                        break;

                    // take the length of the longest element in this segment collection
                    case Definition.DefinitionTypes.Auto:
                        int largest = 0;
                        // collection of each gridentry in this segment
                        Dictionary<Point, UIElement> elementsInThisSegment = _gridEntries.Where(kvp => column ? kvp.Key.X == i : kvp.Key.Y == i)
                                                                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                        foreach (UIElement element in elementsInThisSegment.Values)
                        {
                            // there is no way for a grid to be built without a fixed size
                            //TODO BuildGridVariableSize(out int width, out int height) returns a total width and height, fill columns/rows become zero
                            if (element is Grid grid)
                            {
                                //if (elementsInThisSegment.Values.Count == 1)
                                fillDefs.Add(i);
                                continue;
                            }

                            int lengthAndMargin = 0;
                            if (column)
                                lengthAndMargin = element.Width + element.Margin.Left + element.Margin.Right;
                            else
                                lengthAndMargin = element.Height + element.Margin.Top + element.Margin.Bottom;
                            if (lengthAndMargin > largest)
                                largest = lengthAndMargin;
                        }
                        lengths[i] = largest;
                        totalLength += largest;
                        break;

                    case Definition.DefinitionTypes.Fill:
                        fillDefs.Add(i);
                        break;
                }
            }

            // the length of: (whole segment minus all fixed and auto definitions), divided by the amount of fill definitions
            int fillLength = fillDefs.Count > 0 ? ((column ? Width : Height) - totalLength) / fillDefs.Count : 0;

            foreach (int i in fillDefs)
                lengths[i] = fillLength;

            totalLength = 0;
            Segment[] result = new Segment[lengths.Length];
            for (int i = 0; i < lengths.Length; i++)
            {
                result[i] = new Segment(totalLength + Margin.Left, lengths[i]);
                totalLength += lengths[i];
            }
            return result;
        }

        public void CenterElement(UIElement element)
        {
            Point location = _gridEntries.First(kvp => kvp.Value == element).Key;
            element.Margin = new Margin((columns[location.X].Length / 2) - (element.Width / 2),
                (rows[location.Y].Length / 2) - (element.Height / 2));
        }

        #region Selection With Mouse

        public override void MouseUpdate(Vector2 mouseGlobalPosition)
        {
            if (IsSelectable)
            {
                // the mouse is completely outside the grid
                if (!ContainsMouse(mouseGlobalPosition))
                {
                    if (Highlighted && !PersistantHighlight)
                    {
                        UnhighlightInternal();
                        Unhighlight();
                    }
                    return;
                }
                else if (!Highlighted)
                    Highlight();

                // mouse has not gone outside selected space
                if (SelectedElement.ContainsMouse(mouseGlobalPosition))
                {
                    SelectedElement.MouseUpdate(mouseGlobalPosition);
                    return;
                }

                // mouse is outside selected space but still inside grid

                Point mousePoint = GetMouseCoordinates(mouseGlobalPosition);

                // if mouse is over a selectable region
                if (_gridEntries.ContainsKey(mousePoint) && _gridEntries[mousePoint] is Selectable selectable)
                {
                    // if it's not a selectable container, don't do anything yet
                    if (selectable is UIContainer container && !container.IsSelectable)
                        return;

                    // if Persistant Highlight is on and the mouse isn't directly over the new selection, don't do anything yet
                    bool newSelectableContainsMouse = selectable.ContainsMouse(mouseGlobalPosition);
                    if (PersistantHighlight && !newSelectableContainsMouse)
                        return;

                    // definitely changing selection now
                    // previous element should unhighlight itself
                    SelectedElement.MouseUpdate(mouseGlobalPosition);

                    selection = mousePoint;
                    ghostSelection = selection;

                    // new element should highlight itself
                    selectable.MouseUpdate(mouseGlobalPosition);

                    return;
                }

                // mouse is in an unselectable area in the grid
                if (!PersistantHighlight && SelectedElement.Highlighted)
                    UnhighlightInternal();
            }
        }

        // finds which column and row the mouse is located inside
        Point GetMouseCoordinates(Vector2 mouseGlobalPosition)
        {
            Point result = new Point();
            Vector2 mouseLocalPosition = GetMouseLocalPosition(mouseGlobalPosition);

            for (int i = 0; i < columns.Length; i++)
            {
                if (mouseLocalPosition.X > columns[i].PostPosition)
                    continue;
                if (mouseLocalPosition.X > columns[i].PrePosition)
                    result.X = i;
                break;
            }

            for (int i = 0; i < rows.Length; i++)
            {
                if (mouseLocalPosition.Y > rows[i].PostPosition)
                    continue;
                if (mouseLocalPosition.Y > rows[i].PrePosition)
                    result.Y = i;
                break;
            }

            return result;
        }

        #endregion

        private void ChangeSelection(int column, int row)
        {
            Exception exception = new Exception("Tried to select a non-selectable location.");

            Point location = new Point(column, row);
            if (_gridEntries.ContainsKey(location))
            {
                if (_gridEntries[location] is Selectable)
                    selection = location;
                else
                    throw exception;
            }
            else
                throw exception;
        }

        public override void ResetSelection()
        {
            if (IsSelectable)
            {
                UnhighlightInternal();
                ChangeSelection(primarySelection.X, primarySelection.Y);
                Unhighlight();
            }
        }

        #region Keyboard Input

        public override void InputTrigger(Menu.MenuInputs input)
        {
            if (IsSelectable)
            {
                if (input == Menu.MenuInputs.OK)
                    SelectedElement.InputTrigger(input);
                else
                    HandleSelectionChange(input);
            }
        }

        // returns true if internal, false if out of bounds
        public override bool HandleSelectionChange(Menu.MenuInputs input)
        {
            UIContainer container = SelectedElement as UIContainer;
            if (container != null && container.HandleSelectionChange(input))
                return true;

            int columnPlus = input == Menu.MenuInputs.Left ? -1 : input == Menu.MenuInputs.Right ? 1 : 0;
            int rowPlus = input == Menu.MenuInputs.Up ? -1 : input == Menu.MenuInputs.Down ? 1 : 0;

            if (selection.X + columnPlus > lastSelectableColumn || selection.Y + rowPlus > lastSelectableRow ||
                selection.X + columnPlus < firstSelectableColumn || selection.Y + rowPlus < firstSelectableRow)
                return false;

            if (columnPlus != 0)
            {
                for (int i = selection.X + columnPlus; i <= lastSelectableColumn && i >= firstSelectableColumn; i += columnPlus)
                    if (selectableRowsInColumn.ContainsKey(i))
                    {
                        int selectableRow = GetClosestNumber(ghostSelection.Y, selectableRowsInColumn[i].ToArray());
                        container?.UnhighlightInternal();
                        SelectedElement.Unhighlight();
                        ghostSelection.X = i;
                        ChangeSelection(i, selectableRow);
                        container = SelectedElement as UIContainer;
                        container?.HighlightFromOutside(input);
                        SelectedElement.Highlight();
                        container?.HighlightInternal();
                        break;
                    }
            }
            else if (rowPlus != 0)
            {
                for (int i = selection.Y + rowPlus; i <= lastSelectableRow && i >= firstSelectableRow; i += rowPlus)
                    if (selectableColumnsInRow.ContainsKey(i))
                    {
                        int selectableColumn = GetClosestNumber(ghostSelection.X, selectableColumnsInRow[i].ToArray());
                        container?.UnhighlightInternal();
                        SelectedElement.Unhighlight();
                        ghostSelection.Y = i;
                        ChangeSelection(selectableColumn, i);
                        container = SelectedElement as UIContainer;
                        container?.HighlightFromOutside(input);
                        SelectedElement.Highlight();
                        container?.HighlightInternal();
                        break;
                    }
            }
            return true;
        }

        public override void HighlightFromOutside(Menu.MenuInputs input)
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

            foreach (KeyValuePair<Point, UIElement> keyValuePair in _gridEntries)
            {
                int x = columns[keyValuePair.Key.X].PrePosition;
                int y = rows[keyValuePair.Key.Y].PrePosition;
                Matrix transform = Matrix.CreateTranslation(x, y, 0f) * parentMatrix;
                keyValuePair.Value.UpdateTransformProperties(transform);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (UIElement element in Children)
                element.Draw(spriteBatch, gameTime);
        }
    }

    public struct Segment
    {
        public int PrePosition { get; }
        public int PostPosition { get; }
        public int Length { get; }
        public Segment(int position, int length)
        {
            this.PrePosition = position;
            this.Length = length;
            this.PostPosition = position + length;
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