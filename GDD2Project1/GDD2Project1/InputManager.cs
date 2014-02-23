using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace GDD2Project1
{
    static class InputManager
    {
        #region Attributes
        private static MouseState _mStatePrev;
        private static MouseState _mStateCurr;
        private static KeyboardState _kbStatePrev;
        private static KeyboardState _kbStateCurr;
        #endregion

        /// <summary>
        /// Get the most recent input
        /// </summary>
        public static void UpdateInput()
        {
            if (_mStateCurr != null)
                _mStatePrev = _mStateCurr;
            _mStateCurr = Mouse.GetState();

            if (_kbStateCurr != null)
                _kbStatePrev = _kbStateCurr;
            _kbStateCurr = Keyboard.GetState();
        }

        #region Keyboard Functions
        /// <summary>
        /// Check if a key is current held down
        /// </summary>
        public static bool GetKeyDown(Keys key)
        {
            if (_kbStateCurr.IsKeyDown(key))
                return true;
            return false;
        }

        /// <summary>
        /// Check if a key was press on the current frame
        /// </summary>
        public static bool GetOneKeyPressDown(Keys key)
        {
            if (_kbStateCurr.IsKeyDown(key) && _kbStatePrev.IsKeyUp(key))
                return true;
            return false;
        }

        /// <summary>
        /// Check if a key was released on the current frame
        /// </summary>
        public static bool GetOneKeyPressUp(Keys key)
        {
            if (_kbStateCurr.IsKeyUp(key) && _kbStatePrev.IsKeyDown(key))
                return true;
            return false;
        }

        /// <summary>
        /// Returns an array of all keys currently pressed
        /// </summary>
        public static Keys GetPressedKey()
        {
            Keys[] keys = _kbStateCurr.GetPressedKeys();
            if (keys.Count() > 0)
                return keys[0];

            return Keys.None;
        }
        #endregion

        #region Mouse Functions
        public static Vector2 GetMouseLocation()
        {
            return new Vector2(_mStateCurr.X, _mStateCurr.Y);
        }

        /// <summary>
        /// Returns true if the left mouse button is being held down
        /// </summary>
        public static bool IsLeftMouseDown()
        {
            if (_mStateCurr.LeftButton == ButtonState.Pressed)
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if the right mouse button is being held down
        /// </summary>
        public static bool IsRightMouseDown()
        {
            if (_mStateCurr.RightButton == ButtonState.Pressed)
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if the left mouse has been pressed down on this frame
        /// </summary>
        public static bool GetOneLeftClickDown()
        {
            if (_mStateCurr.LeftButton == ButtonState.Pressed && _mStatePrev.LeftButton == ButtonState.Released)
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if the right mouse has been pressed down on this frame
        /// </summary>
        public static bool GetOneRightClickDown()
        {
            if (_mStateCurr.RightButton == ButtonState.Pressed && _mStatePrev.RightButton == ButtonState.Released)
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if the left mouse has been released on this frame
        /// </summary>
        public static bool GetOneLeftClickUp()
        {
            if (_mStateCurr.LeftButton == ButtonState.Released && _mStatePrev.LeftButton == ButtonState.Pressed)
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if the right mouse has been released on this frame
        /// </summary>
        public static bool GetOneRightClickUp()
        {
            if (_mStateCurr.RightButton == ButtonState.Released && _mStatePrev.RightButton == ButtonState.Pressed)
                return true;
            return false;
        }
        #endregion
    }
}