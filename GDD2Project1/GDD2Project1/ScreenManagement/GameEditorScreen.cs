using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using InputEventSystem;
using WindowSystem;
using GameData;

namespace GDD2Project1
{
    public enum EditorTool
    { 
        TOOL_NONE,
        TOOL_SELECT,
        TOOL_ELEVATE
    };

    /// <summary>
    /// GameEditorScreen
    /// 
    /// Presents the user with a level editor.
    /// </summary>
    public class GameEditorScreen : GameScreen
    {
        protected               GUIManager          _guiMgr;

        protected               EditorTool          _tool;
        protected               List<GameTile>      _selection;
        protected               bool                _grabbingSelection;
        protected               Color               _colorSelect;
        protected               Color               _colorDeselect;

        protected               bool                _elevating;
        protected               bool                _snapping;


        //-------------------------------------------------------------------------
        private EditorTool Tool
        {
            get { return _tool; }
            set
            {
                _tool = value;
                Console.WriteLine("TOOL: " + _tool);
            }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// GameEditorScreen constructor.
        /// </summary>
        /// <param name="screenMgr">ScreenManager containing this screen.</param>
        /// <param name="name">This screen's name.</param>
        public GameEditorScreen(ScreenManager screenMgr, String name)
            : base(screenMgr, name)
        {
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Initialize this GameEditorScreen.
        /// </summary>
        /// <returns>False if failed</returns>
        public override bool init()
        {
            _gameLevelMgr       = new GameLevelManager(_gameContentMgr, _screenMgr.GraphicsDevice);
            Tool                = EditorTool.TOOL_NONE;
            _selection          = new List<GameTile>();
            _grabbingSelection  = false;
            _colorSelect        = new Color(0.5f, 1.0f, 0.25f, 0.75f);
            _colorDeselect      = Color.White;

            _elevating          = false;
            _snapping           = false;

            return base.init();
        }

        /// <summary>
        /// Initialize the user.
        /// </summary>
        /// <returns>False if failed.</returns>
        protected override bool initUser()
        {
            if (!base.initUser())
                return false;

            _user.createController<CameraController>("camController", _gameLevelMgr.Camera)
                .setFreeLook(true);

            return true;
        }

        protected override bool initGUI()
        {
            _guiMgr = new GUIManager(_screenMgr);
            _screenMgr.Components.Add(_guiMgr);
            _guiMgr.Initialize();

            #region Shorthand GUI item creation
            // Create a MenuItem
            Func<UIComponent, String, MenuItem> create_mi =
                (UIComponent parent, String text) =>
                {
                    MenuItem mi             = new MenuItem(_screenMgr, _guiMgr);
                    mi.Text                 = text;

                    if          (parent is MenuBar)     (parent as MenuBar).Add(mi);
                    else if     (parent is MenuItem)    (parent as MenuItem).Add(mi);

                    return mi;
                };

            // Create a TextButton
            Func<UIComponent, String, int, int, int, int, TextButton> create_btn =
                (UIComponent parent, String text, int w, int h, int x, int y) =>
                {
                    TextButton btn = new TextButton(_screenMgr, _guiMgr);

                    btn.Text                = text;
                    btn.Width               = w;
                    btn.Height              = h;
                    btn.X                   = x;
                    btn.Y                   = y;

                    if          (parent is Dialog)      (parent as Dialog).Add(btn);
                    else if     (parent is Window)      (parent as Window).Add(btn);

                    return btn;
                };

            // Create a Dialog
            Func<String, int, int, int, int, Dialog> create_dialog =
                (String text, int w, int h, int x, int y) =>
                {
                    Dialog dialog = new Dialog(_screenMgr, _guiMgr);
                    _guiMgr.Add(dialog);

                    dialog.TitleText        = text;
                    dialog.Width            = w;
                    dialog.Height           = h;
                    dialog.X                = x;
                    dialog.Y                = y;
                    dialog.HasCloseButton   = false;

                    int bwidth              = 50;
                    int bheight             = 20;
                    int bxoffs              = 10;
                    int byoffs              = dialog.Height - 60;

                    // Ok button
                    TextButton btnOk = create_btn(
                        dialog, "Ok", bwidth, bheight, bxoffs, byoffs);
                    btnOk.Click += delegate(UIComponent sender)
                    {
                        dialog.DialogResult = DialogResult.OK;
                        dialog.CloseWindow();
                    };

                    // Cancel button
                    TextButton btnCancel = create_btn(
                        dialog, "Cancel", bwidth, bheight, bxoffs * 2 + bwidth, byoffs);
                    btnCancel.Click += delegate(UIComponent sender)
                    {
                        dialog.DialogResult = DialogResult.Cancel;
                        dialog.CloseWindow();
                    };

                    return dialog;
                };

            // Create a text box
            Func<UIComponent, String, int, int, int, TextBox> create_textbox =
                (UIComponent parent, String text, int w, int x, int y) =>
                {
                    TextBox textBox = new TextBox(_screenMgr, _guiMgr);

                    textBox.Width           = w;
                    textBox.X               = x;
                    textBox.Y               = y;

                    Label label             = new Label(_screenMgr, _guiMgr);
                    label.Text              = text;
                    label.Width             = 100;
                    label.Height            = 50;
                    label.X                 = x - label.Width;
                    label.Y                 = y + 5;

                    if (parent is Dialog)
                    {
                        (parent as Dialog).Add(textBox);
                        (parent as Dialog).Add(label);
                    }

                    return textBox;
                };
            #endregion

            {   // Main menu bar
                MenuBar menuBar = new MenuBar(_screenMgr, _guiMgr);
                _guiMgr.Add(menuBar);
                //-----------------------------------------------------------------
                {   // File
                    MenuItem fileButton = create_mi(menuBar, "File");
                    //-------------------------------------------------------------
                    {   // New 
                        MenuItem newButton = create_mi(fileButton, "New");
                        newButton.Click += delegate(UIComponent sender)
                        {
                            Dialog d = create_dialog("New", 300, 200, 100, 100);
                            TextBox rows = create_textbox(d, "Rows", 50, 150, 10);
                            TextBox cols = create_textbox(d, "Cols", 50, 150, 40);
                            TextBox tile = create_textbox(d, "Tile", 100, 150, 70);

                            d.Close += delegate(UIComponent dsender)
                            {
                                switch (d.DialogResult)
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

                    //-------------------------------------------------------------
                    {   // Save as
                        MenuItem saveAsButton = create_mi(fileButton, "Save as");
                        saveAsButton.Click += delegate(UIComponent sender)
                        {
                            Dialog d = create_dialog("Save as", 300, 200, 100, 100);
                            TextBox file = create_textbox(d, "Path", 200, 100, 50);

                            d.Close += delegate(UIComponent dsender)
                            {
                                switch (d.DialogResult)
                                {
                                    case DialogResult.Cancel:
                                        return;
                                    case DialogResult.OK:
                                        _gameLevelMgr.saveLevel(LEVEL_DIRECTORY + file.Text);
                                        return;
                                }
                            };
                        };
                    }
                    //-------------------------------------------------------------
                    {   // Load
                        MenuItem loadButton = create_mi(fileButton, "Load");
                        loadButton.Click += delegate(UIComponent sender)
                        {
                            Dialog d = create_dialog("Load", 300, 200, 100, 100);
                            TextBox file = create_textbox(d, "File", 200, 100, 50);

                            d.Close += delegate(UIComponent dsender)
                            {
                                switch (d.DialogResult)
                                {
                                    case DialogResult.Cancel:
                                        return;
                                    case DialogResult.OK:
                                        _gameLevelMgr.loadLevel("levels\\" + file.Text);
                                        return;
                                }
                            };
                        };
                    }
                    //-------------------------------------------------------------
                    {   // Quit to menu
                        MenuItem quitButton = create_mi(fileButton, "Quit to menu");
                        quitButton.Click += delegate(UIComponent sender)
                        {

                        };
                    }
                }
                //-----------------------------------------------------------------
                {   // Windows
                    MenuItem windows = create_mi(menuBar, "Windows");

                    Func<String, int, int, int, int, Window> create_win =
                        (String text, int w, int h, int x, int y) =>
                        {
                            Window win = new Window(_screenMgr, _guiMgr);
                            _guiMgr.Add(win);
                            win.Width = w;
                            win.Height = h;
                            win.X = x;
                            win.Y = y;
                            win.TitleText = text;

                            return win;
                        };

                    //-------------------------------------------------------------
                    {   // Tile browser
                        MenuItem tbrowser = create_mi(windows, "Tile Browser");
                        tbrowser.Click += delegate(UIComponent sender)
                        {
                            Window tbwin = create_win("Tile Browser", 300, 500, 400, 100);
                            //-----------------------------------------------------
                            {   // Tile buttons
                                TextButton stoneButton = create_btn(tbwin, "Stone", 60, 30, 10, 10);
                                stoneButton.Click += delegate(UIComponent bsender)
                                {
                                    Drawable tdrwble = _gameContentMgr.loadDrawable("tile_stone");
                                    foreach (GameTile tile in _selection) tile.Entity.Drawable = tdrwble;
                                };
                                TextButton grassButton = create_btn(tbwin, "Grass", 60, 30, 10, 50);
                                grassButton.Click += delegate(UIComponent bsender)
                                {
                                    Drawable tdrwble = _gameContentMgr.loadDrawable("tile_grass");
                                    foreach (GameTile tile in _selection) tile.Entity.Drawable = tdrwble;
                                };
                                TextButton rockButton = create_btn(tbwin, "Rock", 60, 30, 10, 90);
                                rockButton.Click += delegate(UIComponent bsender)
                                {
                                    Drawable tdrwble = _gameContentMgr.loadDrawable("tile_rock");
                                    foreach (GameTile tile in _selection) tile.Entity.Drawable = tdrwble;
                                };
                                TextButton sandButton = create_btn(tbwin, "Sand", 60, 30, 10, 130);
                                sandButton.Click += delegate(UIComponent bsender)
                                {
                                    Drawable tdrwble = _gameContentMgr.loadDrawable("tile_sand");
                                    foreach (GameTile tile in _selection) tile.Entity.Drawable = tdrwble;
                                };
                                TextButton stoneSandButton = create_btn(tbwin, "S-Sand", 60, 30, 10, 170);
                                stoneSandButton.Click += delegate(UIComponent bsender)
                                {
                                    Drawable tdrwble = _gameContentMgr.loadDrawable("tile_stonesand");
                                    foreach (GameTile tile in _selection) tile.Entity.Drawable = tdrwble;
                                };
                            }
                        };
                    }
                    //-------------------------------------------------------------
                    {   // Character browser
                        MenuItem chrbrowser = create_mi(windows, "Character Browser");
                        chrbrowser.Click += delegate(UIComponent sender)
                        {
                            Window chrbwin = create_win("Character Browser", 300, 500, 400, 100);
                        };
                    }
                    //-------------------------------------------------------------
                    {   // Object browser
                        MenuItem itembrowser = create_mi(windows, "Item Browser");
                        itembrowser.Click += delegate(UIComponent sender)
                        {
                            Window ibwin = create_win("Item Browser", 300, 500, 400, 100);
                            //-----------------------------------------------------
                            {   // Item buttons
                                TextButton tree1btn = create_btn(ibwin, "Tree1", 60, 30, 10, 10);
                                tree1btn.Click += delegate(UIComponent bsender)
                                {
                                    foreach (GameTile tile in _selection)
                                    {
                                        GameObject tree = _gameLevelMgr.createGameObject<GameObject>("tree" + _gameLevelMgr.GameObjectCount, "tree1");
                                        tile.Node.attachChildNode(tree.Node);
                                        tree.Node.translateTo(tile.Node.PositionIsometric);
                                    }
                                };
                            }
                        };
                    }
                }
                //-----------------------------------------------------------------
                {   // Tool bar
                    Window toolBar = new Window(_screenMgr, _guiMgr);
                    _guiMgr.Add(toolBar);

                    toolBar.HasCloseButton = false;
                    toolBar.Width = _screenMgr.GraphicsDevice.Viewport.Width;
                    toolBar.Height = 60;
                    toolBar.Y = menuBar.Y + menuBar.Height;
                    toolBar.Resizable = false;
                    toolBar.HasFullWindowMovableArea = false;
                    toolBar.TitleBarHeight = 4;

                    int btncount, btnx, btny, btnw, btnh;
                    btncount = 0;
                    btnx = 8;
                    btny = 4;
                    btnw = 60;
                    btnh = 24;
                    //-------------------------------------------------------------
                    {   // Deselect
                        TextButton noneButton = create_btn(toolBar, "None",
                            btnw, btnh, ((btnx + btnw) * btncount++), btny);
                        noneButton.Click += delegate(UIComponent sender)
                        {
                            Tool = EditorTool.TOOL_NONE;
                        };
                    }
                    //-------------------------------------------------------------
                    {   // Select
                        TextButton selectButton = create_btn(toolBar, "Select",
                            btnw, btnh, ((btnx + btnw) * btncount++), btny);
                        selectButton.Click += delegate(UIComponent sender)
                        {
                            Tool = EditorTool.TOOL_SELECT;
                        };
                    }
                    //-------------------------------------------------------------
                    {   // Zero
                        TextButton zeroButton = create_btn(toolBar, "Zero",
                            btnw, btnh, ((btnx + btnw) * btncount++), btny);
                        zeroButton.Click += delegate(UIComponent sender)
                        {
                            batchElevation(0.0f);
                        };
                    }
                    //-------------------------------------------------------------
                    {   // Elevate
                        TextButton elevateButton = create_btn(toolBar, "Elevate",
                            btnw, btnh, ((btnx + btnw) * btncount++), btny);
                        elevateButton.Click += delegate(UIComponent sender)
                        {
                            Tool = EditorTool.TOOL_ELEVATE;
                        };
                    }
                    //-------------------------------------------------------------
                    {   // Activate
                        TextButton activateButton = create_btn(toolBar, "Activate",
                            btnw, btnh, ((btnx + btnw) * btncount++), btny);
                        activateButton.Click += delegate(UIComponent sender)
                        {
                            batchActivate(true);
                        };
                    }
                    //-------------------------------------------------------------
                    {   // Deactivate
                        TextButton deactivateButton = create_btn(toolBar, "Deact",
                            btnw, btnh, ((btnx + btnw) * btncount++), btny);
                        deactivateButton.Click += delegate(UIComponent sender)
                        {
                            batchActivate(false);
                        };
                    }
                }
            }

            return base.initGUI();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// KeyDown event handler function.
        /// </summary>
        /// <param name="e">Keyboard event.</param>
        /// <returns>True if handled.</returns>
        public override bool injectKeyDown(KeyEventArgs e)
        {
            return base.injectKeyDown(e);
        }

        /// <summary>
        /// KeyUp event handler function.
        /// </summary>
        /// <param name="e">Keyboard event.</param>
        /// <returns>True if handled.</returns>
        public override bool injectKeyUp(KeyEventArgs e)
        {
            return base.injectKeyUp(e);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// MouseDown event handler function.
        /// </summary>
        /// <param name="e">Mouse event</param>
        /// <returns>True if handled</returns>
        public override bool injectMouseDown(MouseEventArgs e)
        {
            switch (e.Button)
            { 
                case MouseButtons.Left:
                    if (_tool != EditorTool.TOOL_NONE)
                    {
                        switch (_tool)
                        { 
                            case EditorTool.TOOL_SELECT:
                                beginTileSelection(e.Position);
                                return true;

                            case EditorTool.TOOL_ELEVATE:
                                beginElevation();
                                return true;
                        }
                    }
                    break;

                case MouseButtons.Right:
                    _user.getController<CameraController>("camController").RotateWithMouse = true;
                    return true;
            }

            return base.injectMouseDown(e);
        }

        /// <summary>
        /// MouseUp event handler function.
        /// </summary>
        /// <param name="e">Mouse event</param>
        /// <returns>True if handled</returns>
        public override bool injectMouseUp(MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (_tool != EditorTool.TOOL_NONE)
                    {
                        switch (_tool)
                        {
                            case EditorTool.TOOL_SELECT:
                                endTileSelection();
                                return true;

                            case EditorTool.TOOL_ELEVATE:
                                endElevation();
                                return true;
                        }
                    }
                    break;

                case MouseButtons.Right:
                    _user.getController<CameraController>("camController").RotateWithMouse = false;
                    return true;
            }

            return base.injectMouseUp(e);
        }

        /// <summary>
        /// MouseMove event handler function.
        /// </summary>
        /// <param name="e">Mouse event</param>
        /// <returns>True if handled</returns>
        public override bool injectMouseMove(MouseEventArgs e)
        {
            switch (_tool)
            { 
                case EditorTool.TOOL_SELECT:
                    if (_grabbingSelection)
                        updateTileSelection(e.Position);
                    return true;

                case EditorTool.TOOL_ELEVATE:
                    if (_elevating)
                        updateElevation(e);
                    return true;
            }

            return base.injectMouseMove(e);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary GameEditorScreen update function.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void update(GameTime gameTime)
        { 
            // Update GUI
            _guiMgr.Update(gameTime);

            // Base updates the rest
            base.update(gameTime);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary GameEditorScreen draw function.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteBatch">Draws textures.</param>
        public override void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Base draws the game level
            base.draw(gameTime, spriteBatch);

            // Draw GUI
            _guiMgr.Draw(gameTime);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Begin a click and drag tile selection.
        /// </summary>
        /// <param name="mousePosition">Current position of mouse.</param>
        protected void beginTileSelection(Point mousePosition)
        {
            GameTile originTile = _gameLevelMgr.tileAtScreenCoords(mousePosition);
            if (originTile != null)
            {
                clearSelection();
                _grabbingSelection = true;
                _selection.Add(originTile);
                updateTileSelection(mousePosition);
            }
        }

        /// <summary>
        /// Update selection to match mouse position.
        /// </summary>
        /// <param name="mousePosition">Current mouse position.</param>
        protected void updateTileSelection(Point mousePosition)
        {
            GameTile destTile = _gameLevelMgr.tileAtScreenCoords(mousePosition);
            if (destTile != null)
            {
                GameTile originTile = _selection[0];
                clearSelection();
                _selection.Add(originTile);

                if (destTile != originTile)
                {
                    int originX, originY, destX, destY;
                    _gameLevelMgr.tileAtIsoCoords(
                        originTile.Node.PositionIsometric, out originX, out originY);
                    _gameLevelMgr.tileAtIsoCoords(
                        destTile.Node.PositionIsometric, out destX, out destY);

                    int dx, dy, sx, sy;
                    dx = destX - originX;
                    dy = destY - originY;
                    sx = 1;
                    sy = 1;

                    if (dx != 0) sx = dx / Math.Abs(dx);
                    if (dy != 0) sy = dy / Math.Abs(dy);

                    switch (_gameLevelMgr.Camera.Dir)
                    { 
                        case Direction.DIR_N:
                        case Direction.DIR_NE:
                            for (int col = originY; col != destY + sy; col += sy)
                                for (int row = originX; row != destX + sx; row += sx)
                                {
                                    GameTile tile = _gameLevelMgr.tileAtIndex(row, col);
                                    if (tile != null && tile != originTile)
                                        _selection.Add(tile);
                                }
                            break;

                        case Direction.DIR_E:
                        case Direction.DIR_SE:
                            for (int row = destX; row != originX - sx; row -= sx)
                                for (int col = destY; col != originY - sy; col -= sy)
                                {
                                    GameTile tile = _gameLevelMgr.tileAtIndex(row, col);
                                    if (tile != null && tile != originTile)
                                        _selection.Add(tile);
                                }
                            break;

                        case Direction.DIR_S:
                        case Direction.DIR_SW:
                            for (int col = destY; col != originY - sy; col -= sy)
                                for (int row = originX; row != destX + sx; row += sx)
                                {
                                    GameTile tile = _gameLevelMgr.tileAtIndex(row, col);
                                    if (tile != null && tile != originTile)
                                        _selection.Add(tile);
                                }
                            break;

                        case Direction.DIR_W:
                        case Direction.DIR_NW:
                            for (int row = originX; row != destX + sx; row += sx)
                                for (int col = originY; col != destY + sy; col += sy)
                                {
                                    GameTile tile = _gameLevelMgr.tileAtIndex(row, col);
                                    if (tile != null && tile != originTile)
                                        _selection.Add(tile);
                                }
                            break;
                    }
                }

                foreach (GameTile tile in _selection)
                    tile.Entity.Color = _colorSelect;
            }
        }

        /// <summary>
        /// End click and drag tile selection.
        /// </summary>
        protected void endTileSelection()
        {
            _grabbingSelection = false;
        }

        /// <summary>
        /// Clear the tile selection
        /// </summary>
        protected void clearSelection()
        {
            foreach (GameTile tile in _selection)
                tile.Entity.Color = _colorDeselect;
            _selection.Clear();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Begin elevating according to mouse coordinates.
        /// </summary>
        protected void beginElevation()
        {
            _elevating = true;
        }

        /// <summary>
        /// Causes selected tiles to elevate according to mouse movement.
        /// </summary>
        /// <param name="e">Mouse event</param>
        protected void updateElevation(MouseEventArgs e)
        {
            if (_selection.Count == 0)
                return;

            if (!_snapping)
            {
                foreach (GameTile tileNode in _selection)
                    tileNode.Node.translate(
                        0.0f,
                        e.RelativePosition.Y / _gameLevelMgr.Camera.ScaleY / _gameLevelMgr.Camera.Zoom,
                        0.0f);
            }
            else
            { 
                
            }
        }

        /// <summary>
        /// Stop elevating according to mouse coordinates.
        /// </summary>
        protected void endElevation()
        {
            _elevating = false;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Set every tile in the selection to the specified elevation.
        /// </summary>
        /// <param name="elevation">Tile elevation</param>
        protected void batchElevation(float elevation)
        {
            foreach (GameTile tile in _selection)
            {
                float dy = elevation - tile.Node.PositionIsometric.Y;
                tile.Node.translate(0.0f, dy, 0.0f);
            }
        }

        /// <summary>
        /// Set the activation for all tiles in the selection.
        /// </summary>
        /// <param name="value">Activation value.</param>
        protected void batchActivate(bool value)
        {
            foreach (GameTile tile in _selection)
                tile.Active = value;
        }
    }
}
