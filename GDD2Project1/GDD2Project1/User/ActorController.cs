﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputEventSystem;

namespace GDD2Project1
{
    public abstract class ActorController
    {
        protected Actor     _actor;
        protected String    _name;


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default ActorController class
        /// </summary>
        /// <param name="actor">Actor which will be controlled by this controller.</param>
        public ActorController(Actor actor, String name)
        {
            _actor = actor;
            _name = name;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Local KeyDown event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public virtual bool injectKeyDown(KeyEventArgs e)
        {
            return false;
        }

        /// <summary>
        /// Local KeyUp event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public virtual bool injectKeyUp(KeyEventArgs e)
        {
            return false;
        }

        /// <summary>
        /// Local MouseDown event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public virtual bool injectMouseDown(MouseEventArgs e)
        {
            return false;
        }

        /// <summary>
        /// Local MouseUp event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public virtual bool injectMouseUp(MouseEventArgs e)
        {
            return false;
        }

        /// <summary>
        /// Local MouseMove event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public virtual bool injectMouseMove(MouseEventArgs e)
        {
            return false;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary ActorController functionality. This function needs implementation
        /// in inherited classes.
        /// </summary>
        /// <param name="dt">Delta time</param>
        public virtual void update(float dt)
        { 
            // TODO: Override this method in inherited classes. This method should
            // generally apply any updates to the actor it's controllering in this function.
            // For example, if an inherited CameraController class is moving the camera,
            // then this function would apply that velocity to the camera
        }
    }
}
