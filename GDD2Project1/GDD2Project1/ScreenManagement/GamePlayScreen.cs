﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WindowSystem;
using InputEventSystem;

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
            _gameLevelMgr.loadLevel("levels\\" + gameLevel);

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

            // Give the user control of their character
            GameCharacter playerChar = _gameLevelMgr.getGameObject<GameCharacter>(GameLevelManager.PLAYER_NAME);
            CharacterController charController = _user.createController<CharacterController>(
                "charController",
                playerChar);

            // Lock the camera onto the user's character
            CameraController camController = _user.createController<CameraController>(
                "camController",
                _gameLevelMgr.Camera);
            camController.setCharacterTarget(playerChar.Node, true);
            camController.RotateWithMouse = true;


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
