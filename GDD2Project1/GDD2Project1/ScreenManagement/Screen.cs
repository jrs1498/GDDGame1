using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using InputEventSystem;

namespace GDD2Project1
{
    public abstract class Screen
    {
        protected ScreenManager     _screenMgr;
        protected String            _name;


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default Screen constructor
        /// </summary>
        /// <param name="screenMgr">Screen Manager containing this Screen</param>
        public Screen(ScreenManager screenMgr, String name)
        {
            _screenMgr  = screenMgr;
            _name       = name;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Screen initialization function. Should handle setting up the screen,
        /// members, displaying initial dialogs, etc.
        /// </summary>
        /// <returns>False if failed</returns>
        public virtual bool init()
        {
            if (!initGUI())
                return false;

            return true;
        }

        /// <summary>
        /// Each screen will implement its own GUI. This function should be overridden
        /// by inherited classes, and should contain all GUI initialization.
        /// <returns>False is failed</returns>
        /// </summary>
        protected virtual bool initGUI()
        {
            return true;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Unload any data that will not be useful after this screen is no longer active
        /// </summary>
        /// <returns>False if failed</returns>
        protected virtual bool unload()
        {
            return true;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Local KeyDown event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public virtual void injectKeyDown(KeyEventArgs e)
        {
        }

        /// <summary>
        /// Local KeyUp event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public virtual void injectKeyUp(KeyEventArgs e)
        {
        }

        /// <summary>
        /// Local MouseDown event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
        public virtual void injectMouseDown(MouseEventArgs e)
        {
        }

        /// <summary>
        /// Local MouseUp event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
        public virtual void injectMouseUp(MouseEventArgs e)
        {
        }

        /// <summary>
        /// Local MouseMove event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
        public virtual void injectMouseMove(MouseEventArgs e)
        {
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary update function for a Screen
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public virtual void update(GameTime gameTime)
        { 
            // TODO: Override and add screen update functionality
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary draw function for a Screen
        /// </summary>
        /// <param name="gameTime">Provides a snapshot for timing values</param>
        /// <param name="spriteBatch">Draws textures</param>
        public virtual void draw(GameTime gameTime, SpriteBatch spriteBatch)
        { 
            // TODO: Override and add screen draw functionality
        }
    }
}
