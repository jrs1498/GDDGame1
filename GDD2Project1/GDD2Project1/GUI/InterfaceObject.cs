using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDD2Project1.GUI
{
    public abstract class InterfaceObject
    {
        protected Rectangle     _rect;
        protected Texture2D     _texture;
        protected Color         _color;
        protected String        _name;


        //-------------------------------------------------------------------------
        public int X
        {
            get { return _rect.X; }
            set { _rect.X = value; }
        }
        public int Y
        {
            get { return _rect.Y; }
            set { _rect.Y = value; }
        }

        public int Right    { get { return _rect.Right; } }
        public int Left     { get { return _rect.Left; } }
        public int Top      { get { return _rect.Top; } }
        public int Bottom   { get { return _rect.Bottom; } }

        public int Width
        {
            get { return _rect.Width; }
            set { _rect.Width = value; }
        }
        public int Height
        {
            get { return _rect.Height; }
            set { _rect.Height = value; }
        }

        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }


        //-------------------------------------------------------------------------
        public delegate void InterfaceEventHandler(InterfaceObject sender, EventArgs e);
        public event InterfaceEventHandler Clicked;
        public EventArgs e = null;

        /// <summary>
        /// Fires Clicked event
        /// </summary>
        public virtual void onClicked()
        {
            if (Clicked != null)
                Clicked(this, e);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default InterfaceObject constructor
        /// </summary>
        public InterfaceObject()
        {

        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary InterfaceObject update function. Should check for things like mouse clicks.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public virtual void update(GameTime gameTime)
        { 
            
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary InterfaceObject draw function.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot for timing values</param>
        /// <param name="spriteBatch">Used to draw textures</param>
        public virtual void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _texture,
                _rect,
                _color);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Check whether or not this InterfaceObject is entirely enclosing the specified coordinates
        /// </summary>
        /// <param name="coordinates">Coordinates to check</param>
        /// <returns>True if enclosed</returns>
        public virtual bool containsCoordinates(Vector2 coordinates)
        {
            if (coordinates.X < Left)   return false;
            if (coordinates.X > Right)  return false;
            if (coordinates.Y < Top)    return false;
            if (coordinates.Y > Bottom) return false;
            return true;
        }
    }
}
