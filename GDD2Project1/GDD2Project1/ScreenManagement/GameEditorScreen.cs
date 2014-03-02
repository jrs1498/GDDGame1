using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InputEventSystem;

namespace GDD2Project1
{
    public class GameEditorScreen : GameScreen
    {
        protected GUIManager        _guiMgr;

        protected EditMode          _editMode;

        protected List<GameObject>  _selection;
        protected bool              _grabbingSelection = false;

        public enum EditMode
        {
            EDITMODE_NONE,
            EDITMODE_TILE
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

            // Internal Funcs for easy GUI item creation
            Func<String, MenuItem> create_mi = (String text) =>
                {
                    MenuItem menuItem = new MenuItem(_screenMgr, _guiMgr);
                    menuItem.Text = text;
                    return menuItem;
                };
            Func<int, int, PopUpMenu> create_popup = (int width, int height) =>
                {
                    PopUpMenu menu = new PopUpMenu(_screenMgr, _guiMgr);
                    menu.Width = width;
                    menu.Height = height;
                    _guiMgr.Add(menu);
                    return menu;
                };

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
                }
                //-----------------------------------------------------------------
                {   // Save
                    MenuItem saveButton = create_mi("Save");
                    fileButton.Add(saveButton);
                }
                //-----------------------------------------------------------------
                {   // Save as
                    MenuItem saveAsButton = create_mi("Save as");
                    fileButton.Add(saveAsButton);
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

            MenuBar toolsBar = new MenuBar(_screenMgr, _guiMgr);
            toolsBar.Y = menuBar.Y + menuBar.Height;
            _guiMgr.Add(toolsBar);
            //---------------------------------------------------------------------
            {   // Select
                MenuItem selectButton = create_mi("Select");
                toolsBar.Add(selectButton);
            }
            //---------------------------------------------------------------------
            {   // Elevate
                MenuItem elevateButton = create_mi("Elevate");
                toolsBar.Add(elevateButton);
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
            base.injectMouseDown(e);
        }

        /// <summary>
        /// Local MouseUp event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override void injectMouseUp(InputEventSystem.MouseEventArgs e)
        {
            base.injectMouseUp(e);
        }

        /// <summary>
        /// Local MouseMove event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override void injectMouseMove(InputEventSystem.MouseEventArgs e)
        {
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

                case EditMode.EDITMODE_TILE:
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
    }
}
