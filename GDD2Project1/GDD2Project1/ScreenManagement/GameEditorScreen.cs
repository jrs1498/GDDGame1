using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using InputEventSystem;
using GameData;

namespace GDD2Project1
{
    public class GameEditorScreen : GameScreen
    {
        protected GUIManager        _guiMgr;

        protected EditMode          _editMode;
        protected Tool              _tool = Tool.TOOL_NONE;

        protected List<GameObject>  _selection;

        protected Color             _colorSelected = new Color(255, 150, 150, 200);
        protected Color             _colorUnselected = Color.White;
        protected bool              _grabbingSelection = false;

        protected float             _elevateSnap = 20.0f;
        protected float             _elevateIncr = 1.0f;
        protected float             _elevateCurrent = 20.0f;

        protected GameObject        _selectedTile;

        protected bool              _elevating      = false;
        protected bool              _snapping       = false;
        protected float             _snapHeight;

        public enum EditMode
        {
            EDITMODE_NONE,
            EDITMODE_TERRAIN
        };

        public enum Tool
        {
            TOOL_NONE,
            TOOL_SELECT,
            TOOL_ELEVATE
        };


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default GameEditorScreen constructor
        /// </summary>
        /// <param name="screenMgr">ScreenManager containing this Screen</param>
        /// <param name="guiMgr">GUI Manager</param>
        /// <param name="name">Name of screen</param>
        public GameEditorScreen(ScreenManager screenMgr, String name)
            : base(screenMgr, name)
        {
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Screen initialization function. Should handle setting up the screen,
        /// members, displaying initial dialogs, etc.
        /// </summary>
        /// <returns>False if failed</returns>
        public override bool init()
        {
            // Initialize GameLevelManager
            _gameLevelMgr = new GameLevelManager(_gameContentMgr, _screenMgr.GraphicsDevice);

            // Initialize tools
            setTool(Tool.TOOL_NONE);
            _selection = new List<GameObject>();
            _snapHeight = _gameLevelMgr.TileSize;

            // Base initializes user
            if (!base.init())
                return false;

            return true;
        }

        /// <summary>
        /// Initialize user
        /// </summary>
        /// <returns>False if failed</returns>
        protected override bool initUser()
        {
            if (!base.initUser())
                return false;

            _user.createController<CameraController>(_gameLevelMgr.Camera, "camController")
                .setFreeLook(true);

            return true;
        }

        /// <summary>
        /// Initialize editor interface
        /// </summary>
        /// <returns>Flase if failed</returns>
        protected override bool initGUI()
        {
            // Initialize WindowSystem
            _guiMgr = new GUIManager(_screenMgr);
            _screenMgr.Components.Add(_guiMgr);
            _guiMgr.Initialize();

            #region Shorthand GUI Creation
            Func<String, MenuItem> create_mi =
                (String text) =>
                {
                    MenuItem menuItem = new MenuItem(_screenMgr, _guiMgr);
                    menuItem.Text = text;
                    return menuItem;
                };

            Func<String, int, int, int, int, TextButton> create_button =
                (String text, int w, int h, int x, int y) =>
                {
                    TextButton button = new TextButton(_screenMgr, _guiMgr);
                    button.Text = text;
                    button.Width = w;
                    button.Height = h;
                    button.X = x;
                    button.Y = y;
                    return button;
                };

            Func<String, int, int, Dialog> create_dialog =
                (String text, int w, int h) =>
                {
                    Dialog dialog = new Dialog(_screenMgr, _guiMgr);
                    _guiMgr.Add(dialog);
                    dialog.TitleText = text;
                    dialog.Width = w;
                    dialog.Height = h;
                    dialog.X = 100;
                    dialog.Y = 50;
                    dialog.HasCloseButton = false;

                    int buttonWidth = 50;
                    int buttonHeight = 20;
                    int buttonXoffset = 10;
                    int buttonYoffset = dialog.Height - 60;

                    // Ok button
                    TextButton buttonOk = create_button
                        ("Ok", buttonWidth, buttonHeight, buttonXoffset, buttonYoffset);
                    buttonOk.Click += delegate(UIComponent sender)
                    {
                        dialog.DialogResult = DialogResult.OK;
                        dialog.CloseWindow();
                    };
                    dialog.Add(buttonOk);

                    // Cancel button
                    TextButton buttonCancel = create_button
                        ("Cancel", buttonWidth, buttonHeight, buttonXoffset * 2 + buttonWidth, buttonYoffset);
                    buttonCancel.Click += delegate(UIComponent sender)
                    {
                        dialog.DialogResult = DialogResult.Cancel;
                        dialog.CloseWindow();
                    };
                    dialog.Add(buttonCancel);

                    return dialog;
                };

            Func<String, int, int, int, Dialog, TextBox> create_textbox =
                (String label, int w, int x, int y, Dialog d) =>
                {
                    TextBox textBox = new TextBox(_screenMgr, _guiMgr);
                    textBox.Width = w;
                    textBox.X = x;
                    textBox.Y = y;

                    Label textLabel = new Label(_screenMgr, _guiMgr);
                    textLabel.Text = label;
                    textLabel.Width = 100;
                    textLabel.Height = 50;
                    textLabel.X = x - textLabel.Width;
                    textLabel.Y = y + 5;

                    d.Add(textBox);
                    d.Add(textLabel);

                    return textBox;
                };
            #endregion

            {
                MenuBar menuBar = new MenuBar(_screenMgr, _guiMgr);
                _guiMgr.Add(menuBar);

                //---------------------------------------------------------------------
                {   // File
                    MenuItem fileButton = create_mi("File");
                    menuBar.Add(fileButton);
                    //-----------------------------------------------------------------
                    {   // New
                        MenuItem newButton = create_mi("New");
                        fileButton.Add(newButton);

                        newButton.Click += delegate(UIComponent sender)
                        {
                            Dialog dialog = create_dialog("New", 300, 200);
                            TextBox rows = create_textbox("Rows", 50, 150, 10, dialog);
                            TextBox cols = create_textbox("Cols", 50, 150, 40, dialog);
                            TextBox tile = create_textbox("Tile", 100, 150, 70, dialog);

                            dialog.Close += delegate(UIComponent csender)
                            {
                                switch (dialog.DialogResult)
                                {
                                    case DialogResult.Cancel:
                                        return;
                                    case DialogResult.OK:
                                        int numRows = Convert.ToInt32(rows.Text);
                                        int numCols = Convert.ToInt32(cols.Text);
                                        _gameLevelMgr.newLevel(numRows, numCols, tile.Text);
                                        return;
                                }
                            };
                        };
                    }
                    //-----------------------------------------------------------------
                    {   // Save as
                        MenuItem saveAsButton = create_mi("Save as");
                        fileButton.Add(saveAsButton);

                        saveAsButton.Click += delegate(UIComponent sender)
                        {
                            Dialog saveAsDialog = create_dialog("Save as", 300, 200);
                            TextBox name = create_textbox("Name", 200, 100, 50, saveAsDialog);

                            saveAsDialog.Close += delegate(UIComponent csender)
                            {
                                switch (saveAsDialog.DialogResult)
                                {
                                    case DialogResult.Cancel:
                                        return;
                                    case DialogResult.OK:
                                        _gameLevelMgr.saveLevel(LEVEL_DIRECTORY, name.Text);
                                        return;
                                }
                            };
                        };
                    }
                    //-----------------------------------------------------------------
                    {   // Load
                        MenuItem loadButton = create_mi("Load");
                        fileButton.Add(loadButton);

                        loadButton.Click += delegate(UIComponent sender)
                        {
                            Dialog loadDialog = create_dialog("Load", 300, 200);
                            TextBox name = create_textbox("Name", 200, 100, 50, loadDialog);

                            loadDialog.Close += delegate(UIComponent csender)
                            {
                                switch (loadDialog.DialogResult)
                                {
                                    case DialogResult.Cancel:
                                        return;
                                    case DialogResult.OK:
                                        _gameLevelMgr.loadLevel("levels\\", name.Text);
                                        return;
                                }
                            };
                        };
                    }
                    //-----------------------------------------------------------------
                    {   // Quit to menu
                        MenuItem quitButton = create_mi("Quit to menu");
                        fileButton.Add(quitButton);

                        quitButton.Click += delegate(UIComponent sender)
                        {
                            Screen mainScreen =
                                _screenMgr.createScreen<MainScreen>("mainScreen", null, true, true);
                            mainScreen.init();
                        };
                    }
                }
                //---------------------------------------------------------------------
                {   // Windows
                    MenuItem windowsButton = create_mi("Windows");
                    menuBar.Add(windowsButton);
                    //-----------------------------------------------------------------
                    {   // Tools
                        MenuItem tools = create_mi("Tools");
                        windowsButton.Add(tools);
                    }
                    //-----------------------------------------------------------------
                    {   // Content browser
                        MenuItem contentBrowser = create_mi("Content browser");
                        windowsButton.Add(contentBrowser);
                    }
                }
                //---------------------------------------------------------------------
                {   // Edit
                    MenuItem editButton = create_mi("Edit");
                    menuBar.Add(editButton);
                    //---------------------------------------------------------------------
                    {   // Select
                        MenuItem selectButton = create_mi("Select");
                        editButton.Add(selectButton);

                        selectButton.Click += delegate(UIComponent sender)
                        {
                            _tool = Tool.TOOL_SELECT;
                        };
                    }
                }
            }

            {
                Window toolBar = new Window(_screenMgr, _guiMgr);
                _guiMgr.Add(toolBar);
                toolBar.Width = 600;
                toolBar.Height = 72;
                toolBar.Y = 50;
                toolBar.HasCloseButton = false;
                toolBar.Resizable = false;
                toolBar.HasFullWindowMovableArea = false;
                //---------------------------------------------------------------------
                {   // Select
                    TextButton select = create_button("select", 60, 40, 0, 0);
                    toolBar.Add(select);

                    select.Click += delegate(UIComponent sender)
                    {
                        setTool(Tool.TOOL_SELECT);
                    };
                }
                //---------------------------------------------------------------------
                {   // Clear select
                    TextButton clear = create_button("clear", 60, 40, 70, 0);
                    toolBar.Add(clear);

                    clear.Click += delegate(UIComponent sender)
                    {
                        clearSelection();
                        endTileSelection();
                    };
                }
                //---------------------------------------------------------------------
                {   // Elevate 
                    TextButton elevate = create_button("elevate", 60, 40, 140, 0);
                    toolBar.Add(elevate);

                    elevate.Click += delegate(UIComponent sender)
                    {
                        setTool(Tool.TOOL_ELEVATE);
                    };
                }
            }

            return true;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Unload any data that will not be useful after this screen is no longer active
        /// </summary>
        /// <returns>False if failed</returns>
        protected override bool unload()
        {
            if (_guiMgr != null)
                _screenMgr.Components.Remove(_guiMgr);

            return base.unload();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Local KeyDown event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override bool injectKeyDown(KeyEventArgs e)
        {
            // Ctrl hotkeys
            if (e.Control)
            {
                switch (e.Key)
                { 
                    case Keys.S:
                        setTool(Tool.TOOL_SELECT);
                        return true;

                    case Keys.E:
                        setTool(Tool.TOOL_ELEVATE);
                        return true;

                    case Keys.D:
                        clearSelection();
                        endTileSelection();
                        return true;

                    default:
                        break;
                }
            }

            return base.injectKeyDown(e);
        }

        /// <summary>
        /// Local KeyUp event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override bool injectKeyUp(KeyEventArgs e)
        {
            // Ctrl hotkeys
            if (e.Control)
            {
                switch (e.Key)
                { 
                    case Keys.S:
                        return true;

                    case Keys.E:
                        return true;

                    case Keys.D:
                        return true;
                }
            }

            return base.injectKeyUp(e);
        }

        /// <summary>
        /// Local MouseDown event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override bool injectMouseDown(MouseEventArgs e)
        {
            switch (e.Button)
            { 
                case MouseButtons.Left:
                    if (_tool != Tool.TOOL_NONE)
                    {
                        switch (_tool)
                        { 
                            case Tool.TOOL_SELECT:
                                beginTileSelection(e.Position);
                                return true;

                            case Tool.TOOL_ELEVATE:
                                _elevating = true;
                                return true;

                            default:
                                break;
                        }
                    }
                    break;
            }

            return base.injectMouseDown(e);
        }

        /// <summary>
        /// Local MouseUp event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override bool injectMouseUp(InputEventSystem.MouseEventArgs e)
        {
            switch (e.Button)
            { 
                case MouseButtons.Left:
                    switch (_tool)
                    { 
                        case Tool.TOOL_SELECT:
                            endTileSelection();
                            return true;

                        case Tool.TOOL_ELEVATE:
                            _elevating = false;
                            return true;
                    }
                    break;
            }

            return base.injectMouseUp(e);
        }

        /// <summary>
        /// Local MouseMove event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override bool injectMouseMove(InputEventSystem.MouseEventArgs e)
        {
            switch (_tool)
            { 
                case Tool.TOOL_SELECT:
                    if (_grabbingSelection)
                        updateTileSelection(e.Position);
                    return true;

                case Tool.TOOL_ELEVATE:
                    if (_elevating)
                        updateElevation(e);
                    return true;
            }

            return base.injectMouseMove(e);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary GameEditorScreen update function
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void update(GameTime gameTime)
        {
            // Update GUI
            _guiMgr.Update(gameTime);

            // Base updates GameLevel and User
            base.update(gameTime);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Draw this GameEditorScreen and its GUI
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        /// <param name="spriteBatch">Draws textures</param>
        public override void draw(GameTime gameTime, SpriteBatch spriteBatch)
        { 
            // Base draws gameLevel
            base.draw(gameTime, spriteBatch);

            // Draw GUI
            _guiMgr.Draw(gameTime);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Set the current edit mode, which determines how this screen runs its updates
        /// </summary>
        /// <param name="editMode">New edit mode</param>
        protected void setEditMode(EditMode editMode)
        {
            _editMode = editMode;
        }

        /// <summary>
        /// Set the terrain editing tool
        /// </summary>
        /// <param name="tool">Tool to use</param>
        protected void setTool(Tool tool)
        {
            setEditMode(EditMode.EDITMODE_TERRAIN);
            _tool = tool;

            Console.WriteLine("Tool: " + _tool);
        }


        //-------------------------------------------------------------------------
        protected void beginTileSelection(Point mousePosition)
        {
            Console.WriteLine("Tile selection started");
            clearSelection();
            GameObject originTile = _gameLevelMgr.getTileFromScreenCoordinates(mousePosition);
            if (originTile != null)
            {
                _grabbingSelection = true;
                _selection.Add(originTile);
                updateTileSelection(mousePosition);
            }
        }

        protected void updateTileSelection(Point mousePosition)
        {
            GameObject destTile = _gameLevelMgr.getTileFromScreenCoordinates(mousePosition);
            if (destTile != null)
            {
                GameObject originTile = _selection[0];
                clearSelection();
                _selection.Add(originTile);

                if (destTile != originTile)
                {
                    Point originIndex = _gameLevelMgr.getIndexFromPosition(originTile.PositionIsometric);
                    Point destIndex = _gameLevelMgr.getIndexFromPosition(destTile.PositionIsometric);

                    int diffx, diffy, signx, signy;

                    diffx = destIndex.X - originIndex.X;
                    diffy = destIndex.Y - originIndex.Y;
                    signx = 1;
                    signy = 1;

                    if (diffx != 0) signx = diffx / Math.Abs(diffx);
                    if (diffy != 0) signy = diffy / Math.Abs(diffy);

                    switch (_gameLevelMgr.Camera.Dir)
                    {
                        case Direction.DIR_NE:
                            for (int row = originIndex.X; row != destIndex.X + signx; row += signx)
                                for (int col = originIndex.Y; col != destIndex.Y + signy; col += signy)
                                {
                                    GameObject tile = _gameLevelMgr.getTileAtIndex(row, col);
                                    if (tile != null && tile != originTile)
                                        _selection.Add(tile);
                                }
                            break;
                    }
                }

                foreach (GameObject tile in _selection)
                    tile.Color = _colorSelected;

                Console.WriteLine("Updated selection: " + _selection.Count + " tiles");
            }
        }

        protected void endTileSelection()
        {
            Console.WriteLine("Tile selection ended");
            _grabbingSelection = false;
        }

        protected void clearSelection()
        {
            foreach (GameObject tile in _selection)
                tile.Color = _colorUnselected;
            _selection.Clear();
        }

        protected void updateElevation(MouseEventArgs e)
        {
            if (_selection.Count == 0)
                return;

            if (!_snapping)
            {
                foreach (GameObject tile in _selection)
                    tile.translate(0.0f, e.RelativePosition.Y
                        / _gameLevelMgr.Camera.ScaleY
                        / _gameLevelMgr.Camera.Zoom,
                        0.0f);
            }
            else
            { 
                // Don't quite know how to make snapping work yet
            }
        }
    }
}
