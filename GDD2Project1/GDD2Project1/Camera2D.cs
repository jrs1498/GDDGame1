using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GDD2Project1
{
    class Camera2D : Actor
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

        protected float
            _dirN = 0,
            _dirS = (float)Math.PI,
            _dirE = (float)Math.PI * 0.5f,
            _dirW = (float)Math.PI * 1.5f;

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
                _rotationZ = value;
                if (_rotationZ >= Math.PI * 2)
                    _rotationZ = 0;
                else if (_rotationZ < 0)
                    _rotationZ = 0;

                updateDirection();
            }
        }

        public float RotationX
        {
            get { return _rotationX; }
            set { _rotationX = value; }
        }

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        public Direction Dir
        {
            get { return _dir; }
        }


        //-------------------------------------------------------------------------
        public delegate void DirectionHandler(Camera2D cam, EventArgs e);
        public event DirectionHandler DirectionChanged;
        public EventArgs e = null;


        //-------------------------------------------------------------------------
        public Camera2D(GameLevelManager gameLevelMgr)
            : base(gameLevelMgr)
        {
            _rotationZ = (float)(Math.PI / 2.0f);
            _rotationX = (float)(Math.PI / 6.0f);
            _zoom = 0.5f;
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
            Direction dir;

            if (_rotationZ > _dirN && _rotationZ < _dirE)
                dir = Direction.DIR_NE;
            else if (_rotationZ > _dirE && _rotationZ < _dirS)
                dir = Direction.DIR_SE;
            else if (_rotationZ > _dirS && _rotationZ < _dirW)
                dir = Direction.DIR_SW;
            else
                dir = Direction.DIR_NW;

            if (_dir != dir)
            {
                _dir = dir;
                if (DirectionChanged != null)
                    DirectionChanged(this, e);
            }
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
                Matrix.CreateRotationZ(_rotationZ)
                * Matrix.CreateScale(1.0f, _scaleY, 1.0f);

            _transformationIsometricInverse =
                Matrix.Invert(_transformationIsometric);
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
        public Direction getRelativeDirection(Direction dir)
        {
            return (Direction)(((int)dir + (int)_dir) % (int)Direction.DIR_COUNT);
        }
    }
}
