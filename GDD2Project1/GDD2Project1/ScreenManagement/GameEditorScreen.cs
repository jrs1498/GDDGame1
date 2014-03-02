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
        protected const String LEVEL_DIRECTORY = "..\\..\\..\\..\\GDD2Project1Content\\levels\\";

        protected GUIManager        _guiMgr;

        protected EditMode          _editMode;
        protected Tool              _tool = Tool.TOOL_NONE;

        protected List<GameObject>  _selection;
        protected bool              _grabbingSelection = false;

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
            setEditMode(EditMode.EDITMODE_NONE);
            _selection = new List<GameObject>();
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
        public override void injectKeyDown(KeyEventArgs e)
        {

            base.injectKeyDown(e);
        }

        /// <summary>
        /// Local KeyUp event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override void injectKeyUp(KeyEventArgs e)
        {
            base.injectKeyUp(e);
        }

        /// <summary>
        /// Local MouseDown event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override void injectMouseDown(MouseEventArgs e)
        {
            switch (e.Button)
            { 
                case MouseButtons.Left:
                    if (_tool == Tool.TOOL_SELECT)
                    {
                        clearSelection();
                        _selection.Add(_gameLevelMgr.getTileFromScreenCoordinates(e.Position));
                        _grabbingSelection = true;
                    }
                    break;
            }

            base.injectMouseDown(e);
        }

        /// <summary>
        /// Local MouseUp event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override void injectMouseUp(InputEventSystem.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (_tool == Tool.TOOL_SELECT)
                        _grabbingSelection = false;
                    break;
            }

            base.injectMouseUp(e);
        }

        /// <summary>
        /// Local MouseMove event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override void injectMouseMove(InputEventSystem.MouseEventArgs e)
        {
            if (_grabbingSelection)
                grabSelection(e.Position);

            base.injectMouseMove(e);
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

            // Update based on EditMode
            switch (_editMode)
            { 
                case EditMode.EDITMODE_NONE:
                    break;

                case EditMode.EDITMODE_TERRAIN:
                    break;
            }

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


        //-------------------------------------------------------------------------
        /// <summary>
        /// Grab a selection of tiles. This will be called only when the mouse moves, so we will
        /// grab a completely new selection based on the origin in this function.
        /// </summary>
        protected void grabSelection(Point mousePosition)
        {
            // Grab the origin tile and clear the selection
            GameObject origin = _selection[0];
            clearSelection();

            // Grab the tile the mouse is hovering over
            GameObject mouseTile = _gameLevelMgr.getTileFromScreenCoordinates(mousePosition);

            // Grab start and end index
            Point originIndex = _gameLevelMgr.getIndexFromPosition(origin.PositionIsometric);
            Point mouseIndex = _gameLevelMgr.getIndexFromPosition(mouseTile.PositionIsometric);

            origin.Color = Color.Red;
            mouseTile.Color = Color.Red;
        }

        /// <summary>
        /// Function handler for clearing the selection
        /// </summary>
        protected void clearSelection()
        {
            _selection.Clear();
        }
    }
}
