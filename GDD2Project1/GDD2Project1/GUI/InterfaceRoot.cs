using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDD2Project1.GUI
{
    public class InterfaceRoot
    {
        protected Dictionary<String, InterfaceObject> _interfaceObjs;


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default InterfaceRoot constructor
        /// </summary>
        public InterfaceRoot()
        {
            _interfaceObjs = new Dictionary<string, InterfaceObject>();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Poll user input
        /// </summary>
        public virtual void pollInput()
        {
            if (InputManager.GetOneLeftClickDown())
                foreach (KeyValuePair<String, InterfaceObject> entry in _interfaceObjs)
                    if (entry.Value.containsCoordinates(InputManager.GetMouseLocation()))
                        entry.Value.onClicked();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Update all InterfaceObjects contained in this root.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public virtual void update(GameTime gameTime)
        {
            pollInput();

            foreach (KeyValuePair<String, InterfaceObject> entry in _interfaceObjs)
                entry.Value.update(gameTime);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Draw all InterfaceObjects contained in this root.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        /// <param name="spriteBatch">Draws textures</param>
        public virtual void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (KeyValuePair<String, InterfaceObject> entry in _interfaceObjs)
                entry.Value.draw(gameTime, spriteBatch);

            spriteBatch.End();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Create an InterfaceObject and add it to this root
        /// </summary>
        /// <typeparam name="T">Type of InterfaceObject</typeparam>
        /// <param name="name">Name of InterfaceObject</param>
        public virtual void createInterfaceObject<T>(String name)
            where T : InterfaceObject
        {
            T interfaceObj = (T)Activator.CreateInstance(typeof(T), null);
            interfaceObj.Name = name;
            _interfaceObjs.Add(name, interfaceObj);
        }

        /// <summary>
        /// Return a previously created InterfaceObject
        /// </summary>
        /// <typeparam name="T">Type of InterfaceObject</typeparam>
        /// <param name="name">Name of InterfaceObject</param>
        /// <returns>Requested Object, if it existed</returns>
        public virtual T getInterfaceObject<T>(String name)
            where T : InterfaceObject
        {
            if (!_interfaceObjs.ContainsKey(name))
                return null;

            return _interfaceObjs[name] as T;
        }
    }
}
