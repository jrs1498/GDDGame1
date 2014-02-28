using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// Base user input function. Every controller has a pollInput function which
        /// should be called in the User's update function.
        /// </summary>
        public virtual void pollInput()
        { 
            // TODO: Override this method in inherited classes and
            // provide controller functionality.
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
