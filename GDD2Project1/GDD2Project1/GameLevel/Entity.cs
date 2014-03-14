using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDD2Project1
{
    /// <summary>
    /// Entity
    /// 
    /// This class represents any physical representation of an object within a
    /// GameLevel. An entity may be attached to a GameNode, and a GameNode may
    /// have one, and only one Entity attached to it.
    /// </summary>
    public class Entity
    {
        protected Drawable          _drawable;
        protected Color             _color;
        protected bool              _active;
        protected Direction         _directionView;
        protected AnimationState    _animationState;
        protected float             _stateTime;


        //-------------------------------------------------------------------------
        public      Drawable        Drawable        { get { return _drawable; } }
        public      Color           Color           { get { return _color; } set { _color = value; } }
        public      bool            Active          { get { return _active; } set { _active = value; } }
        public      AnimationState  AnimState
        {
            get { return _animationState; }
            set
            {
                if (_animationState != value)
                {
                    _animationState = value;
                    _stateTime = 0.0f;
                }
            }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Entity constructor.
        /// </summary>
        /// <param name="drawable">This Entity's drawable.</param>
        /// <param name="active">Initial active status of this Entity</param>
        public Entity(Drawable drawable, bool active = true)
        {
            _drawable           = drawable;
            _color              = Color.White;
            _active             = active;
            _directionView      = Direction.DIR_N;
            _animationState     = AnimationState.ANIMSTATE_IDLE;
            _stateTime          = 0.0f;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Draw this Entity.
        /// </summary>
        /// <param name="spriteBatch">Renders Texture2D.</param>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="dt">Precomputed delta time.</param>
        /// <param name="position">Screen render coordinates.</param>
        /// <param name="rotation">Rotation amount about Z axis.</param>
        /// <param name="scale">Scale amount.</param>
        public virtual void draw(
            SpriteBatch     spriteBatch,
            Camera2D        camera,
            GameTime        gameTime,
            float           dt,
            Vector2         position,
            float           rotation,
            Vector2         scale)
        {
            if (_active)
            {
                _stateTime += dt;
                Direction dir = camera.getRelativeDirectionView(_directionView);

                if (_drawable is DrawableAnimated)
                    (_drawable as DrawableAnimated).draw(
                        spriteBatch,
                        position,
                        _color,
                        rotation,
                        scale,
                        _stateTime,
                        _animationState,
                        dir);

                else
                    _drawable.draw(
                        spriteBatch,
                        position,
                        _color,
                        rotation,
                        scale);
            }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Update this Entity's view direction, which will indicate how it's drawn.
        /// </summary>
        public void updateDirectionView(Vector3 directionVector)
        {
            // Project as unit vector onto XZ plane
            directionVector.Y   = 0.0f;
            directionVector     = Vector3.Normalize(directionVector);

            // Grab cosine of angle between vector and -Z axis (north/south axis)
            float cosTheta      = Vector3.Dot(-Vector3.UnitZ, directionVector);
            if (Math.Abs(cosTheta) < 0.0005f) cosTheta = 0.000f;

            // Find angle and update direction
            float theta         = (float)Math.Acos(cosTheta);
            if (directionVector.X > 0.0f) theta = (float)(Math.PI + (Math.PI - theta));
            _directionView      = GameLevelManager.directionViewFromAngle(theta);
        }
    }
}
