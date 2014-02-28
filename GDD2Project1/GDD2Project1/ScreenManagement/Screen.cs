using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GDD2Project1.GUI;

namespace GDD2Project1
{
    public abstract class Screen
    {
        protected ScreenManager     _screenMgr;
        protected InterfaceRoot     _interface;
        protected String            _name;


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default Screen constructor
        /// </summary>
        /// <param name="screenMgr">Screen Manager containing this Screen</param>
        public Screen(ScreenManager screenMgr, String name)
        {
            _screenMgr  = screenMgr;
            _interface  = new InterfaceRoot();
            _name       = name;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary input checking method
        /// </summary>
        public virtual void pollInput()
        { 
            // TODO: Override this function in inherited class.
            // Used for checking user input
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary update function for a Screen
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public virtual void update(GameTime gameTime)
        { 
            // TODO: Override and add screen update functionality
            _interface.update(gameTime);
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
            _interface.draw(gameTime, spriteBatch);
        }
    }
}
