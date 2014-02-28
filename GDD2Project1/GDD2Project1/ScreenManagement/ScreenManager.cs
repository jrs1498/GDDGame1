using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDD2Project1
{
    /// <summary>
    /// The ScreenManager class is the root for anything in the game.
    /// This class controls which screen is currently being updated and drawn.
    /// </summary>
    public class ScreenManager : Game
    {
        GraphicsDeviceManager                   _graphics;
        SpriteBatch                             _spriteBatch;

        private Dictionary<String, Screen>      _screens;
        private String                          _currentScreen;

        private const int INITIAL_WINDOW_WIDTH  = 1280;
        private const int INITIAL_WINDOW_HEIGHT = 720;


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default ScreenManager constructor
        /// </summary>
        public ScreenManager()
        {
            _graphics                               = new GraphicsDeviceManager(this);
            Content.RootDirectory                   = "Content";

            _graphics.PreferredBackBufferWidth      = INITIAL_WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight     = INITIAL_WINDOW_HEIGHT;

            IsMouseVisible                          = true;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Game initialization
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Load game content / screens / etc
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch    = new SpriteBatch(GraphicsDevice);
            _screens        = new Dictionary<String, Screen>();

            // Load a GamePlayScreen
            createScreen<GamePlayScreen>("gameScreen", true);
            getScreen<GamePlayScreen>("gameScreen").init("exampleLevel");
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Stub for unloading any content
        /// </summary>
        protected override void UnloadContent()
        {
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Pass the command down through all contained objects to check for user input
        /// </summary>
        protected virtual void PollInput(Screen currentScreen)
        {
            InputManager.UpdateInput();
            currentScreen.pollInput();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary update function
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        protected override void Update(GameTime gameTime)
        {
            Screen screen = getCurrentScreen();
            if (screen != null)
            {
                // Poll input
                PollInput(screen);

                if (screen != null)
                    screen.update(gameTime);
            }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary drawing function
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen
            GraphicsDevice.Clear(Color.White);

            Screen screen = getCurrentScreen();
            if (screen != null)
                screen.draw(gameTime, _spriteBatch);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Set the current screen. This screen must have previously been created by
        /// the ScreenManager.
        /// </summary>
        /// <param name="screen">Name of the previously created screen</param>
        /// <returns>False if no screen found</returns>
        protected virtual bool setCurrentScreen(String screen)
        {
            if (!_screens.ContainsKey(screen))
                return false;

            _currentScreen = screen;

            return true;
        }

        /// <summary>
        /// Safe function for getting the current active screen being presented
        /// to the user. This may return null, so perform a null check before any actions.
        /// </summary>
        /// <returns>Current Screen</returns>
        protected virtual Screen getCurrentScreen()
        {
            if (_currentScreen == null)
                return null;

            if (!_screens.ContainsKey(_currentScreen))
                return null;

            return _screens[_currentScreen];
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Create and add a new Screen to this ScreenManager. The newly created
        /// Screen may then be set to the current screen with a call to setCurrentScreen.
        /// The current screen contains everything that is presented to the user.
        /// </summary>
        /// <typeparam name="T">Type of screen</typeparam>
        /// <param name="args">Constructor arguments corresponding to Screen type</param>
        /// <param name="name">Screen's name, used for referencing</param>
        protected virtual bool createScreen<T>(String name, bool setToCurrent = false) 
            where T : Screen
        {
            if (_screens.ContainsKey(name))
                return false;

            T screen = (T)Activator.CreateInstance(typeof(T), new object[]{this, name});
            _screens.Add(name, screen);

            if (setToCurrent)
                setCurrentScreen(name);

            return true;
        }

        /// <summary>
        /// Safe function for retrieving a Screen from this GameManager.
        /// Screen is returned as typeof T. This function may potentially
        /// return a null value, in the case that the name specified does not
        /// correspond to a previously created Screen, or that Screen has been
        /// destroyed.
        /// </summary>
        /// <typeparam name="T">Type of Screen to be returned</typeparam>
        /// <param name="name">Name of previously created screen</param>
        /// <returns>Screen corresponding to name, as typeof T</returns>
        protected virtual T getScreen<T>(String name)
            where T : Screen
        {
            if (!_screens.ContainsKey(name))
                return null;

            return _screens[name] as T;
        }

        /// <summary>
        /// Destroys a previously created screen, freeing it from memory
        /// </summary>
        /// <param name="name">Name of the previously created screen</param>
        /// <returns>False if the screen did not exist</returns>
        protected virtual bool destroyScreen(String name)
        {
            if (!_screens.ContainsKey(name))
                return false;

            _screens.Remove(name);
            if (_currentScreen == name)
                _currentScreen = null;

            return true;
        }
    }
}
