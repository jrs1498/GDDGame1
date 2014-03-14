using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputEventSystem;
using Microsoft.Xna.Framework;

namespace GDD2Project1
{
    /// <summary>
    /// UserController
    /// 
    /// Provides a user input interface to control objects in this application.
    /// </summary>
    public abstract class UserController
    {
        protected String _name;


        //-------------------------------------------------------------------------
        /// <summary>
        /// UserController constructor.
        /// </summary>
        /// <param name="name">This controller's name.</param>
        public UserController(String name)
        {
            _name = name;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// KeyDown input event handler function.
        /// </summary>
        /// <param name="e">KeyEvent arguments.</param>
        /// <returns>True if handled.</returns>
        public virtual bool injectKeyDown(KeyEventArgs e)
        {
            return false;
        }

        /// <summary>
        /// KeyUp input event handler function.
        /// </summary>
        /// <param name="e">KeyEvent arguments.</param>
        /// <returns>True if handled.</returns>
        public virtual bool injectKeyUp(KeyEventArgs e)
        {
            return false;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// MouseDown input event handler function.
        /// </summary>
        /// <param name="e">MouseEvent arguments.</param>
        /// <returns>True if handled.</returns>
        public virtual bool injectMouseDown(MouseEventArgs e)
        {
            return false;
        }

        /// <summary>
        /// MouseUp input event handler function.
        /// </summary>
        /// <param name="e">MouseEvent arguments.</param>
        /// <returns>True if handled.</returns>
        public virtual bool injectMouseUp(MouseEventArgs e)
        {
            return false;
        }

        /// <summary>
        /// MouseMove input event handler function.
        /// </summary>
        /// <param name="e">MouseEvent arguments.</param>
        /// <returns>True if handled.</returns>
        public virtual bool injectMouseMove(MouseEventArgs e)
        {
            return false;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// UserController update function. Applies controller influence on an object.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="dt">Precomputed delta time.</param>
        public virtual void update(GameTime gameTime, float dt)
        {
            return;
        }
    }
}
