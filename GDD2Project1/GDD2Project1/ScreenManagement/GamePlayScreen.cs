using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GDD2Project1.GUI;

namespace GDD2Project1
{
    public class GamePlayScreen : GameScreen
    {
        protected User _user;


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
            loadLevel(gameLevel);

            // Initialize the user
            initUser();

            // Load an example interface (should be deleted later)
            exampleInterface();

            return true;
        }

        /// <summary>
        /// Initialize the user, setting controllers and controller attributes.
        /// </summary>
        /// <returns>False if failed</returns>
        public virtual bool initUser()
        {
            // Initialize User
            _user = new User();

            // Create User's controllers
            _user.createController<CameraController>(
                _gameLevelMgr.Camera,
                "camController");
            _user.createController<CharacterController>(
                _gameLevelMgr.getCharacter("dudetwo"),
                "charController");
            
            // Set controller attributes
            _user.getController<CameraController>("camController").setCharacterTarget(
                _gameLevelMgr.getCharacter("dudetwo"),
                true);

            return true;
        }

        private void exampleInterface()
        {
            // Create two buttons
            _interface.createInterfaceObject<InterfaceButton>("mytestbutton");
            _interface.createInterfaceObject<InterfaceButton>("mytestbutton2");

            // Initialize the new buttons
            InterfaceButton button = _interface.getInterfaceObject<InterfaceButton>("mytestbutton");
            InterfaceButton button2 = _interface.getInterfaceObject<InterfaceButton>("mytestbutton2");
            button.X = 50;
            button.Y = 50;
            button2.X = 125;
            button2.Y = 50;
            button.Width = button2.Width = 50;
            button.Height = button2.Height = 30;
            
            // Give them some color and texture
            Texture2D buttonTexture = _screenMgr.Content.Load<Texture2D>("textures/interface/button");
            button.Texture = button2.Texture = buttonTexture;
            button.Color = Color.Red;
            button2.Color = Color.Green;

            // Hook up the button's clicked event to do something
            button.Clicked += delegate(InterfaceObject sender, EventArgs e)
            {
                Console.WriteLine(sender.Name + " was just clicked!");
            };

            button2.Clicked += delegate(InterfaceObject sender, EventArgs e)
            {
                Console.WriteLine(sender.Name + " was ALSO just clicked!");
            };
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary input checking method
        /// </summary>
        public override void pollInput()
        {
            _user.pollInput();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary update function for GamePlayScreen
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void update(GameTime gameTime)
        {
            float dt = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            // Update user and all of their controllers
            _user.update(dt);

            // Base updates GameLevel
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
            // TODO: Add HUD drawing after gamelevel (base)
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Load a new level
        /// </summary>
        /// <param name="gameLevel">Relative path to the GameLevel</param>
        public virtual void loadLevel(String gameLevel)
        {
            // TODO: Implement level loading from GameLevel file
            // Use _gameContentMgr to load content and pull content
            _gameLevelMgr = new GameLevelManager(_gameContentMgr, _screenMgr.GraphicsDevice);
        }
    }
}
