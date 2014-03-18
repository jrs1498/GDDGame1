using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InputEventSystem;
using Microsoft.Xna.Framework.Input;

namespace GDD2Project1
{
    /// <summary>
    /// The ScreenManager class is the root for anything in the game.
    /// This class controls which screen is currently being updated and drawn.
    /// </summary>
    public class ScreenManager : Game
    {
        private bool                            _windowHasFocus;

        private GraphicsDeviceManager           _graphics;
        private SpriteBatch                     _spriteBatch;

        private Dictionary<String, Screen>      _screens;
        private String                          _currentScreen;

        private InputEvents                     _input;
        private const int                       MAX_PRESSED_KEYS = 8;
        private List<Keys>                      _pressedKeys;

        protected Vector2                       _mouseCoordinates;

        private const int INITIAL_WINDOW_WIDTH  = 1280;
        private const int INITIAL_WINDOW_HEIGHT = 720;


        //-------------------------------------------------------------------------
        public bool WindowHasFocus { get { return _windowHasFocus; } }
        public InputEvents InputSystem { get { return _input; } }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default ScreenManager constructor
        /// </summary>
        public ScreenManager()
        {
            _windowHasFocus                         = true;

            _graphics                               = new GraphicsDeviceManager(this);
            Content.RootDirectory                   = "Content";

            _input                                  = new InputEvents(this);
            Components.Add(this._input);

            IsFixedTimeStep                         = false;
            IsMouseVisible                          = true;

            _graphics.PreferredBackBufferWidth      = INITIAL_WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight     = INITIAL_WINDOW_HEIGHT;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Game initialization
        /// </summary>
        protected override void Initialize()
        {
            // Initialize buffered input event handlers
            _input.KeyDown      += new KeyDownHandler(ReceiveKeyDown);
            _input.KeyUp        += new KeyUpHandler(ReceiveKeyUp);
            _input.MouseDown    += new MouseDownHandler(ReceiveMouseDown);
            _input.MouseUp      += new MouseUpHandler(ReceiveMouseUp);
            _input.MouseMove    += new MouseMoveHandler(ReceiveMouseMove);

            _pressedKeys        = new List<Keys>();

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

            // Create initial screen
            createScreen<MainScreen>("mainScreen", new object[] { this }, true);
            getScreen<MainScreen>("mainScreen").init();
            
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
        /// Primary update function
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        protected override void Update(GameTime gameTime)
        {
            // Update input
            _input.Update(gameTime);

            // Update Screen
            Screen screen = getCurrentScreen();
            if (screen != null)
                screen.update(gameTime);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary drawing function
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Screen screen = getCurrentScreen();
            if (screen != null)
                screen.draw(gameTime, _spriteBatch);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Root KeyDown event handler. This function needs to inject this event
        /// into its current screen, which will further inject that event to its components
        /// </summary>
        /// <param name="e">Key event arguments</param>
        protected virtual void ReceiveKeyDown(KeyEventArgs e)
        {
            // Check if we can press another key
            if (_pressedKeys.Count >= MAX_PRESSED_KEYS) return;
            if (_pressedKeys.Contains(e.Key))           return;

            // Add key to our pressed keys
            _pressedKeys.Add(e.Key);

            // Fire the event
            Screen screen = getCurrentScreen();
            if (screen != null)
                screen.injectKeyDown(e);
        }

        /// <summary>
        /// Root KeyUp event handler. This function needs to inject this event
        /// into its current screen, which will further inject that event to its components
        /// </summary>
        /// <param name="e">Key event arguments</param>
        protected virtual void ReceiveKeyUp(KeyEventArgs e)
        {
            // Remove key from our pressed keys
            if (!_pressedKeys.Remove(e.Key)) return;

            // Fire the event
            Screen screen = getCurrentScreen();
            if (screen != null)
                screen.injectKeyUp(e);
        }

        /// <summary>
        /// Check if a specified key is currently being held down
        /// </summary>
        /// <param name="key">Key to check</param>
        public virtual bool IsKeyDown(Keys key)
        {
            return _pressedKeys.Contains(key);
        }

        /// <summary>
        /// Root MouseDown event handler. This function listens for the event,
        /// and then passes the event down to its current screen and that screens
        /// components.
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
        protected virtual void ReceiveMouseDown(MouseEventArgs e)
        {
            Screen screen = getCurrentScreen();
            if (screen != null)
                screen.injectMouseDown(e);
        }

        /// <summary>
        /// Root MouseUp event handler. This function listens for the event,
        /// and then passes the event down to its current screen and that screens
        /// components.
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
        protected virtual void ReceiveMouseUp(MouseEventArgs e)
        {
            Screen screen = getCurrentScreen();
            if (screen != null)
                screen.injectMouseUp(e);
        }

        /// <summary>
        /// Root MouseMoved event handler. This function listens for the event,
        /// and then passes the event down to its current screen and that screens
        /// components.
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
        protected virtual void ReceiveMouseMove(MouseEventArgs e)
        {
            // Update mouse coordinates
            _mouseCoordinates.X = e.Position.X;
            _mouseCoordinates.Y = e.Position.Y;

            Screen screen = getCurrentScreen();
            if (screen != null)
                screen.injectMouseMove(e);
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
        public virtual T createScreen<T>(String name, object[] args = null, bool setToCurrent = false, bool destroyCurrent = true) 
            where T : Screen
        {
            if (_screens.ContainsKey(name))
                return null;
            
            T screen = (T)Activator.CreateInstance(typeof(T), new object[]{this, name});
            _screens.Add(name, screen);

            if (destroyCurrent)
                destroyScreen(_currentScreen);

            if (setToCurrent)
                setCurrentScreen(name);

            return screen;
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
            if (name == null)
                return null;

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
            if (name == null)
                return false;

            if (!_screens.ContainsKey(name))
                return false;

            _screens.Remove(name);
            if (_currentScreen == name)
                _currentScreen = null;

            return true;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Get the current known coordinates of the mouse cursor
        /// </summary>
        public Vector2 MouseCoordinates
        {
            get { return _mouseCoordinates; }
        }
    }
}
