using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GDD2Project1
{
    public class User
    {
        protected Dictionary<String, ActorController> _controllers;


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default User constructor
        /// </summary>
        public User()
        { 
            
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary input function. All input functionality should be handled
        /// here. After checking for any user-specific input, this function
        /// should then check for input within any of the contained tools.
        /// </summary>
        public void pollInput()
        {
            if (_controllers != null)
                foreach (KeyValuePair<String, ActorController> entry in _controllers)
                    entry.Value.pollInput();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary User update function. This should update any user specific data,
        /// and then continue by updating all contained controllers
        /// </summary>
        /// <param name="dt">Delta time</param>
        public virtual void update(float dt)
        {
            if (_controllers != null)
                foreach (KeyValuePair<String, ActorController> entry in _controllers)
                    entry.Value.update(dt);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Create and add an ActorController to this User's controller inventory.
        /// These controllers give the user access to a variety of objects within
        /// the GameLevel. Anything that extends from Actor may have a controller class.
        /// </summary>
        /// <typeparam name="T">Type of controller</typeparam>
        /// <param name="name">Name of controllers</param>
        /// <returns>False if failed</returns>
        public virtual bool createController<T>(Actor actor, String name)
            where T : ActorController
        {
            if (_controllers == null)
                _controllers = new Dictionary<string, ActorController>();
            else if (_controllers.ContainsKey(name))
                return false;

            T controller = (T)Activator.CreateInstance(typeof(T), new object[] { actor, name });
            _controllers.Add(name, controller);

            return true;
        }

        /// <summary>
        /// Get a previously created controller from this User.
        /// If the specified name does not correspond to a currently active
        /// controller, null will be returned.
        /// </summary>
        /// <typeparam name="T">Type of Controller</typeparam>
        /// <param name="name">Name of Controller</param>
        /// <returns>Controller specified by name and type</returns>
        public virtual T getController<T>(String name)
            where T : ActorController
        {
            if (!_controllers.ContainsKey(name))
                return null;

            return _controllers[name] as T;
        }

        /// <summary>
        /// Destroy a previously created ActorController
        /// </summary>
        /// <param name="name">Name of ActorController to destroy</param>
        /// <returns>False if failed</returns>
        public virtual bool destroyController(String name)
        {
            if (_controllers == null)
                return false;
            else if (!_controllers.ContainsKey(name))
                return false;

            _controllers.Remove(name);

            return true;
        }
    }
}
