using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDD2Project1
{
    /// <summary>
    /// GameObject class represent an actual object in the GameLevel.
    /// </summary>
    class GameObject : GameNode
    {
        protected Drawable      _drawable;
        protected Color         _color;
        protected Vector3       _vecDirection;
        protected Direction     _isoDirection;


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default GameObject constructor.
        /// </summary>
        /// <param name="gameLevelMgr"></param>
        /// <param name="name"></param>
        public GameObject(GameLevelManager gameLevelMgr, String name)
            : base(gameLevelMgr, name)
        {
            // Defaults
            _color = Color.White;
            _vecDirection = new Vector3(1.0f, 0.0f, 0.0f);
            _isoDirection = Direction.DIR_SW;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Draws this GameObject's attached Drawable, if it exists. If there is no
        /// Drawable attached, then this function will immediately return.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used for drawing</param>
        /// <param name="dt">Delta time</param>
        public virtual void drawContents(SpriteBatch spriteBatch, float dt)
        {
            if (_drawable == null)
                return;

            // This line can be optimized.
            // _position only needs to update if the GameObject moved, or
            // if the camera moved.
            _position = _gameLevelMgr.Camera.isometricToCartesian(_positionIsometric);

            _drawable.draw(
                spriteBatch,
                _position,
                _origin,
                _color,
                _rotation,
                _scale,
                dt);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Attaches a Drawable to this GameObject. A GameObject may have only one Drawable
        /// at a time, and if this GameObject is currently holding a Drawable, it must be
        /// detached before a different Drawable may be attached.
        /// </summary>
        /// <param name="drawable">Drawable to attach</param>
        /// <returns>True if Drawable is now attached</returns>
        public virtual bool attachDrawable(Drawable drawable)
        {
            if (_drawable != null)
                return false;

            _drawable = drawable;

            return true;
        }

        /// <summary>
        /// Detaches and returns this GameObject's Drawable. This has the potential
        /// to return a null Drawable, which would happen in the case that there is no Drawable
        /// currently attached to this GameObject.
        /// </summary>
        /// <returns>Detached Drawable</returns>
        public Drawable detachDrawable()
        {
            Drawable drawable = _drawable;
            _drawable = null;
            return drawable;
        }

        /// <summary>
        /// Get this GameObject's drawable
        /// </summary>
        /// <returns>GameObject's drawable</returns>
        public Drawable getDrawable()
        {
            return _drawable;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Get / Set this GameObject's drawing color.
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Subscribe this GameObject to the GameLevel's Camera2D. This causes the
        /// camDirectionChanged() function to fire every time the camera's isometric
        /// position state is updated. This is used to preserve the GameObject's drawing
        /// angle when the camera rotates to a new viewing angle.
        /// </summary>
        /// <param name="cam">Camera2D to subscribe to</param>
        public void subscribeToCamera(Camera2D cam)
        {
            cam.DirectionChanged += new Camera2D.DirectionHandler(camDirectionChanged);
        }

        /// <summary>
        /// This function fires whenever the Camera2D's isometric viewing angle changes.
        /// This is where the logic should go to tell the GameObject to change its
        /// viewing angle to match that of the Camera's
        /// </summary>
        /// <param name="cam">Camera2D which fired the event</param>
        /// <param name="e">Event args</param>
        protected virtual void camDirectionChanged(Camera2D cam, EventArgs e)
        {
            updateDirection();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Updates this GameObject's isometric (viewing) direction, according to its
        /// direction vector and the GameLevel's camera's viewing angle.
        /// This function should be called any time the GameObject changes direction,
        /// or any time the camera changing viewing angle.
        /// </summary>
        protected void updateDirection()
        {
            // Grab the cosine of our direction vector and the x axis
            Vector2 dir = new Vector2(_vecDirection.X, _vecDirection.Z);
            dir = Vector2.Normalize(dir);
            float cosTheta = Vector2.Dot(dir, Vector2.UnitX);

            Direction tempDir;

            // Determine what way this GameObject is facing
            if (cosTheta < -0.707f)
                tempDir = Direction.DIR_NW;
            else if (cosTheta > 0.707f)
                tempDir = Direction.DIR_SE;
            else if (dir.Y > 0)
                tempDir = Direction.DIR_SW;
            else
                tempDir = Direction.DIR_NE;

            // Now update the direction relative to the camera's viewing angle
            tempDir = _gameLevelMgr.Camera.getRelativeDirection(tempDir);
            _isoDirection = tempDir;
        }
    }
}
