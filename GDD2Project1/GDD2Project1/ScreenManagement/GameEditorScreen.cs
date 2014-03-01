using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowSystem;

namespace GDD2Project1
{
    public class GameEditorScreen : GameScreen
    {
        //-------------------------------------------------------------------------
        /// <summary>
        /// Default GameEditorScreen constructor
        /// </summary>
        /// <param name="screenMgr">ScreenManager containing this Screen</param>
        /// <param name="guiMgr">GUI Manager</param>
        /// <param name="name">Name of screen</param>
        public GameEditorScreen(ScreenManager screenMgr, GUIManager guiMgr, String name)
            : base(screenMgr, guiMgr, name)
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
            _guiMgr.Add(menuBar);

            return true;
        }
    }
}
