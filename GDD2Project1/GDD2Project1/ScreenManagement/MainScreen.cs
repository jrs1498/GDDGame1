using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GDD2Project1.GUI;
using InputEventSystem;

namespace GDD2Project1
{
    public class MainScreen : Screen
    {
        private const string FIRST_LEVEL = "testlevel";
        private InterfaceManager _interfaceMgr;

        //-------------------------------------------------------------------------
        public MainScreen(ScreenManager screenMgr, String name)
            : base(screenMgr, name)
        {
        }


        //-------------------------------------------------------------------------
        public override bool init()
        {
            if (!base.init())
                return false;

            return true;
        }

        protected override bool initGUI()
        {
            _interfaceMgr = new InterfaceManager(_screenMgr);

            //---------------------------------------------------------------------
            {   // Play Game
                TextButton playGame = _interfaceMgr.create<TextButton>(100, 100, 300, 150);
                playGame.changeText("Play Game");
                playGame.onClick += delegate()
                {
                    GamePlayScreen gameScreen = 
                        _screenMgr.createScreen<GamePlayScreen>("gamePlayScreen", null, true, true);
                    gameScreen.init(FIRST_LEVEL);
                };

            }
            //---------------------------------------------------------------------
            {   // Level Editor
                TextButton levelEditor = _interfaceMgr.create<TextButton>(100, 200, 300, 150);
                levelEditor.changeText("Level Editor");
                levelEditor.onClick += delegate()
                {
                    Screen editorScreen =
                        _screenMgr.createScreen<GameEditorScreen>("editorScreen", null, true, true);
                    editorScreen.init();
                };

            }
            //---------------------------------------------------------------------
            {   // Quit
                TextButton quit = _interfaceMgr.create<TextButton>(100, 300, 300, 150);
                quit.changeText("Quit Game");
                quit.onClick += delegate()
                {
                    _screenMgr.Exit();
                };
            }

            return true;
        }


        //-------------------------------------------------------------------------
        public override bool injectMouseMove(MouseEventArgs e)
        {
            _interfaceMgr.injectMouseMove(e.Position);

            return base.injectMouseMove(e);
        }

        public override bool injectMouseDown(MouseEventArgs e)
        {
            _interfaceMgr.injectMouseDown(e.Position);

            return base.injectMouseDown(e);
        }


        //-------------------------------------------------------------------------
        public override void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.draw(gameTime, spriteBatch);

            _interfaceMgr.draw(spriteBatch);
        }
    }
}
