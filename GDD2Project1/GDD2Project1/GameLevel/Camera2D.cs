using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GDD2Project1
{
    public class Camera2D : Actor
    {
        protected Matrix
            _transformation,
            _transformationInverse,
            _transformationIsometric,
            _transformationIsometricInverse;

        protected float
            _rotationX,
            _rotationZ,
            _scaleY,
            _zoom;

        protected Vector2
            _origin,
            _originIsometric;

        protected Direction _dir;


        //-------------------------------------------------------------------------
        public Matrix Transformation
        {
            get { return _transformation; }
        }

        public float RotationZ
        {
            get { return _rotationZ; }
            set 
            { 
                _rotationZ = (float)(value % (Math.PI * 2));
                if (_rotationZ < 0.0f)
                    _rotationZ += (float)(Math.PI * 2);
                updateDirection();
            }
        }

        public float RotationX
        {
            get { return _rotationX; }
            set { _rotationX = value; }
        }

        public float ScaleY
        {
            get { return _scaleY; }
        }

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        public Vector2 Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        public Vector2 OriginIsometric
        {
            get { return _originIsometric; }
            set { _originIsometric = value; }
        }

        public Direction Dir
        {
            get { return _dir; }
        }


        //-------------------------------------------------------------------------
        public Camera2D(GameLevelManager gameLevelMgr)
            : base(gameLevelMgr)
        {
            _rotationZ = 0.0f;
            _rotationX = (float)(Math.PI / 6.0f);
            _zoom = 0.5f;
            _originIsometric = Vector2.Zero;
            update(0.0f);
        }


        //-------------------------------------------------------------------------
        public void update(float dt)
        {
            updateTransformation();
            updateTransformationIsometric();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Update the direction enum, which indicates where the camera
        /// is pointing
        /// </summary>
        protected void updateDirection()
        {
            _dir = GameLevelManager.directionViewFromAngle(_rotationZ);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Update this Camera's view transformation matrix.
        /// This is necessary any time the camera changes position
        /// </summary>
        protected virtual void updateTransformation()
        {
            _transformation =
                Matrix.CreateScale(_zoom)
                * Matrix.CreateTranslation(new Vector3(-_position.X, -_position.Y, 0))
                * Matrix.CreateTranslation(new Vector3(_origin.X, _origin.Y, 0));

            _transformationInverse = Matrix.Invert(_transformation);
        }

        /// <summary>
        /// Update this Camera's isometric transformation matrix.
        /// This is necessary any time the Camera's angles changes.
        /// </summary>
        protected virtual void updateTransformationIsometric()
        {
            _scaleY = 1.0f - (float)Math.Sin(_rotationX);

            _transformationIsometric =
                Matrix.CreateTranslation(-_originIsometric.X, -_originIsometric.Y, 0.0f)
                * Matrix.CreateRotationZ(_rotationZ)
                * Matrix.CreateScale(1.0f, _scaleY, 1.0f);

            _transformationIsometricInverse =
                Matrix.Invert(_transformationIsometric);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Apply a translation to this Camera's position.
        /// </summary>
        /// <param name="amount">Translation amount.</param>
        public virtual void translate(Vector2 amount)
        {
            _position += amount;
        }

        /// <summary>
        /// Apply a translation to this Camera's position.
        /// </summary>
        /// <param name="x">X translation amount.</param>
        /// <param name="y">Y translation amount.</param>
        public virtual void translate(float x, float y)
        {
            translate(new Vector2(x, y));
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Retrieve cartesian (2D) coordinates corresponding to the Camera's current
        /// isometric viewing angles.
        /// </summary>
        /// <param name="isometricCoordinates">Isometric coordinates to convert</param>
        /// <returns>2D cartesian coordinates</returns>
        public Vector2 isometricToCartesian(Vector3 isometricCoordinates)
        {
            Vector2 cartesianCoordinates;
            cartesianCoordinates.X = isometricCoordinates.X;
            cartesianCoordinates.Y = isometricCoordinates.Z;
            cartesianCoordinates = Vector2.Transform(cartesianCoordinates, _transformationIsometric);
            cartesianCoordinates.Y += (isometricCoordinates.Y * _scaleY);

            return cartesianCoordinates;
        }

        /// <summary>
        /// Retrieve isometric coordinates from screen coordinates
        /// </summary>
        /// <param name="screenCoordinates">Screen coordinates</param>
        /// <returns>Isometric coordinates</returns>
        public Vector2 screenToIsometric(Vector2 screenCoordinates)
        {
            screenCoordinates = Vector2.Transform(screenCoordinates, _transformationInverse);
            screenCoordinates = Vector2.Transform(screenCoordinates, _transformationIsometricInverse);
            return screenCoordinates;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Returns a direction relative to this Camera's direction
        /// </summary>
        /// <param name="dir">Relative direction</param>
        /// <returns>Adjusted direction</returns>
        public Direction getRelativeDirectionView(Direction dir)
        {
            return (Direction)(((int)dir + (int)Direction.DIR_COUNT - (int)_dir) % (int)Direction.DIR_COUNT);
        }

        /// <summary>
        /// Get a direction vector relative to the current orientation of the Camera.
        /// </summary>
        /// <param name="dir">Direction vector.</param>
        /// <returns>Relative direction vector.</returns>
        public Vector3 getRelativeDirectionVector(Vector3 dir)
        {
            Matrix mat  = Matrix.Identity;
            mat         = Matrix.CreateRotationY(_rotationZ);
            dir         = Vector3.Transform(dir, mat);
            dir         = Vector3.Normalize(dir);

            if (Math.Abs(dir.X) < 0.005f) dir.X = 0;
            if (Math.Abs(dir.Y) < 0.005f) dir.Y = 0;
            if (Math.Abs(dir.Z) < 0.005f) dir.Z = 0;

            return dir;
        }
    }
}
