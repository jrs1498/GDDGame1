using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDD2Project1
{
    public abstract class GameScreen : Screen
    {
        protected GameContentManager    _gameContentMgr;
        protected GameLevelManager      _gameLevelMgr;


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
        /// Primary update function for a GameScreen
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void update(GameTime gameTime)
        {
            float dt = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            _gameLevelMgr.update(dt);
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
