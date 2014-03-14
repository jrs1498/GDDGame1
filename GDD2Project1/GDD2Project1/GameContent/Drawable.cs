using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GDD2Project1
{
    public class Drawable : ContentObject
    {
        protected Texture2D     _texture;
        protected Vector2       _origin;


        //-------------------------------------------------------------------------
        public Vector2 Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default Drawable constructor
        /// </summary>
        /// <param name="texture">The Drawable's texture</param>
        public Drawable(String name, Texture2D texture)
            : base(name)
        {
            _texture    = texture;
            _name       = name;
            _origin     = new Vector2(0, 0);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary Drawable constructor. This object should only be created by a
        /// GameContentManager from an XML file, using this constructor. See function
        /// GameContentManager.loadDrawable.
        /// </summary>
        /// <param name="name">This object's name, for reference purposes</param>
        /// <param name="texture">Texture used for drawing</param>
        /// <param name="origin">Origin used for drawing</param>
        public Drawable(
            String      name,
            Texture2D   texture,
            Vector2     origin)
            : base(name)
        {
            _texture    = texture;
            _origin     = origin;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Draw this Drawable
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used for drawing</param>
        /// <param name="position">Position to draw to</param>
        /// <param name="origin">Origin to draw in relation to</param>
        /// <param name="color">Color to apply to this texture</param>
        /// <param name="rotation">Z axis rotation to be applied</param>
        /// <param name="scale">Scale to be applied</param>
        public virtual void draw(
            SpriteBatch     spriteBatch, 
            Vector2         position, 
            Color           color,
            float           rotation,
            Vector2         scale)
        {
            spriteBatch.Draw(
                _texture,
                position,
                null,
                color,
                rotation,
                _origin,
                scale,
                SpriteEffects.None,
                0);
        }
    }
}
