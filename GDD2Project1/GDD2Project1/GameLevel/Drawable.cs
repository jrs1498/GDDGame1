using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GDD2Project1
{
    public class Drawable
    {
        protected Texture2D     _texture;
        protected String        _name;


        //-------------------------------------------------------------------------
        public String Name { get { return _name; } }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default Drawable constructor
        /// </summary>
        /// <param name="texture">The Drawable's texture</param>
        public Drawable(Texture2D texture, String name)
        {
            _texture    = texture;
            _name       = name;
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
            Vector2         origin,
            Color           color,
            float           rotation,
            Vector2         scale,
            float           dt)
        {
            spriteBatch.Draw(
                _texture,
                position,
                null,
                color,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0);
        }
    }
}
