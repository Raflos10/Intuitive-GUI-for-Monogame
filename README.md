# Intuitive-GUI-for-Monogame
A WPF Inspired GUI for Monogame

# Information
This GUI is meant to be intuitive for both the developer and the user. WPF (Windows Presentation Foundation) uses a grid-based system to link elements together in a very clean way, and that is the main basis of this GUI. It also takes some inspiration from Flash in several ways. 
Another reason for the grid-based system is to make controller/keyboard support easy. This will be implemented later. 
Right now it's very basic, but will allow you to easily make grid-based menus that move, scale, and rotate all of their items as one. I encourage anyone to clone this project and make more useful UI elements that are missing, such as radio buttons, tab windows, listboxes, and so-on. 

# Steps to Getting Started
1. Download the latest [release](https://github.com/Raflos10/Intuitive-GUI-for-Monogame/releases) and reference it in your project.
2. Add a Using statement for Intuitive-GUI-for-Monogame in any class you plan to use it. 
3. In your Game1.cs (or extension of Game), create a new "MenuSystem" and add it to Components. 
This should look something like:
```
MenuSystem menuSystem = new MenuSystem()
Components.Add(menuSystem);
```
4. Make your menu classes extend "Menu"
5. To activate a menu, add it to MenuSystem.ActiveMenus
6. (optional) If your game changes resolution at all, make sure to copy your resolution matrix to MenuSystem.ResolutionMatrix
This is only used for getting the right mouse position at any resolution. 

## Constructing Menus and Grids

All menus can only contain one item. Grids can contain as many items as you like. So define a Grid for your menu. You can give it a margin if you like, which is a definition of the distance between the Grid and the edges of the Menu. 
Then once you have an instance of a Grid, you can use AddColumnDefinition and AddRowDefinition. There are three different types of definition. 
1. Fixed
   - A set number of pixels that does not change
2. Auto
   - Automatically increases size to fit the largest object in this specific row/column
3. Fill
   - Automatically increases size to fill any leftover space that the grid occupies (divided equally by the amount of definitions with this trait (Like Star in WPF)

To create a fixed definition, you can just type an integer in the declaration. 
```
grid.AddColumnDefinition(new ColumnDefinition(12));
```
For Auto and Fill, you can just specify by referencing them like this
```
grid.AddColumnDefinition(ColumnDefinition.Auto);
grid.AddRowDefinition(RowDefinition.Fill);
```
After declaring these, it's possible to add children elements to the Grid using "AddChild". You must specify which Column and Row the child element should occupy. The element must be a subclass of "UIItem". The first integer is the column index (starting with "0) and the second integer is the row index.
```
grid.AddChild(new Button(), 0, 1);
```
When these steps are completed, you can add the Grid to the Menu.Item. Make sure the Menu has a non-zero Width and a Height first. When adding the Grid to "Item", it is automatically built. 
(Optional) You can also make changes to the grid later and call BuildGrid, but you must specify the containment definitions of the grid. 
```
grid.BuildGrid(new ColumnDefinition(800), new RowDefinition(600));
```

## More about Grids
To center something inside a grid, it's easiest right now to have two empty columns/rows set as "fill". This way they fill evenly on either side of the item. 
Grids can be added to grids, so you can make some fairly complex menus. 

# Dependencies
Other than Monogame itself, there are no mandatory dependencies. 
### Optional
If you want to use Bitmap Fonts in your menus, you will need the optional .dll file which depends on [Monogame.Extended.dll](https://github.com/craftworkgames/MonoGame.Extended)

# Demo
In the demo you can right click to move the menu around, and use keys Q and E to rotate the menu to see how the UI items stay attached to the menu.
