using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WindowSystem;

namespace GDD2Project1
{
    public class GamePlayScreen : GameScreen
    {
        //-------------------------------------------------------------------------
        /// <summary>
        /// Default GamePlayScreen constructor
        /// </summary>
        /// <param name="screenMgr">ScreenManager containing this GamePlayScreen</param>
        /// <param name="name">Name of this GamePlayScreen</param>
        public GamePlayScreen(ScreenManager screenMgr, String name)
            : base(screenMgr, name)
        { 
            
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Initialize this GamePlayScreen by loading an initial level and all defaults.
        /// </summary>
        /// <param name="gameLevel">Relative path to the GameLevel file to be loaded</param>
        /// <returns>False if init has already been called</returns>
        public virtual bool init(String gameLevel)
        {
            // Check if this GamePlayScreen has already been initialized
            if (_gameLevelMgr != null)
                return false;

            // Load the level passed in
            _gameLevelMgr = new GameLevelManager(_gameContentMgr, _screenMgr.GraphicsDevice);
            _gameLevelMgr.loadLevel("levels\\", gameLevel, true);

            // Initialize the user
            initUser();

            return true;
        }

        /// <summary>
        /// Initialize the user, setting controllers and controller attributes.
        /// </summary>
        /// <returns>False if failed</returns>
        protected override bool initUser()
        {
            // Initialize User
            if (!base.initUser())
                return false;

            // Grab the player's character from GameLevel
            GameCharacter playerChar = 
                _gameLevelMgr.getGameObject<GameCharacter>(PLAYER_CHARACTER);

            // Create User's controllers
            CameraController camController = _user.createController<CameraController>(
                _gameLevelMgr.Camera,
                "camController");

            CharacterController charController = _user.createController<CharacterController>(
                playerChar, "charController");
            //camController.setCharacterTarget(playerChar, true);

            return true;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary update function for GamePlayScreen
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void update(GameTime gameTime)
        {
            float dt = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            // Base updates GameLevel and User
            base.update(gameTime);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary draw function for GamePlayScreen
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        /// <param name="spriteBatch">Draws textures</param>
        public override void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.draw(gameTime, spriteBatch);
            // TODO: Implement HUD drawing
        }
    }
}
