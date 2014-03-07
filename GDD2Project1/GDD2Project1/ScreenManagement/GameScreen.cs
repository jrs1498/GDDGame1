using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InputEventSystem;
using WindowSystem;

namespace GDD2Project1
{
    public abstract class GameScreen : Screen
    {
        protected const String LEVEL_DIRECTORY = "..\\..\\..\\..\\GDD2Project1Content\\levels\\";

        protected GameContentManager    _gameContentMgr;
        protected GameLevelManager      _gameLevelMgr;
        protected User                  _user;


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default GameScreen constructor.
        /// </summary>
        /// <param name="screenMgr">ScreenManager containing this GameScreen</param>
        /// <param name="name">Name of this GameScreen</param>
        public GameScreen(ScreenManager screenMgr, String name)
            : base(screenMgr, name)
        {
            // Initialize GameContentManager, for content loading / unloading
            _gameContentMgr = new GameContentManager(screenMgr.Content);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Screen initialization function. Should handle setting up the screen,
        /// members, displaying initial dialogs, etc.
        /// </summary>
        /// <returns>False if failed</returns>
        public override bool init()
        {
            if (!initUser())
                return false;

            if (!base.init())
                return false;

            return true;
        }

        /// <summary>
        /// Primary user initialization function. This is where the User class should
        /// be instantiated, hooked up with controllers, etc. This function contains
        /// general user initialization code, but it should be overridden in inherited
        /// classes to implement specific user initialization. Inherited classes should
        /// call base.initUser() before any specific initialization is done.
        /// </summary>
        /// <returns>False if failed</returns>
        protected virtual bool initUser()
        {
            _user = new User();

            return true;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Local KeyDown event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override void injectKeyDown(KeyEventArgs e)
        {
            _user.injectKeyDown(e);
        }

        /// <summary>
        /// Local KeyUp event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override void injectKeyUp(KeyEventArgs e)
        {
            _user.injectKeyUp(e);
        }

        /// <summary>
        /// Local MouseDown event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override void injectMouseDown(MouseEventArgs e)
        {
            _user.injectMouseDown(e);
        }

        /// <summary>
        /// Local MouseUp event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override void injectMouseUp(InputEventSystem.MouseEventArgs e)
        {
            _user.injectMouseUp(e);
        }

        /// <summary>
        /// Local MouseMove event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override void injectMouseMove(InputEventSystem.MouseEventArgs e)
        {
            _user.injectMouseMove(e);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary update function for a GameScreen
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void update(GameTime gameTime)
        {
            float dt = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            // Update GameLevel and User
            _gameLevelMgr.update(dt);
            _user.update(dt);

            // Base updates interface
            base.update(gameTime);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary draw function for a GameScreen
        /// </summary>
        /// <param name="gameTime">Provides a snapshot for timing values</param>
        /// <param name="spriteBatch">Draws textures</param>
        public override void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float dt = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            _gameLevelMgr.draw(spriteBatch, dt);
            base.draw(gameTime, spriteBatch);
        }
    }
}
