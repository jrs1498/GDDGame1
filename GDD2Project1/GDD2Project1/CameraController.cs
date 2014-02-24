using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GDD2Project1
{
    class CameraController
    {

        protected Camera2D      _camera;

        protected const float   BASE_OFFSET         = (float)(Math.PI * 0.25);
        protected const float   ROTATION_INTERVAL   = (float)(Math.PI * 0.5f);
        protected const int     MAX_ROTATIONS       = (int)((Math.PI * 2) / ROTATION_INTERVAL + 1);
        protected const float   ZOOM_INTERVAL       = 0.2f;
        protected const float   MIN_ZOOM            = 0.4f;
        protected const float   MAX_ZOOM            = 0.8f;
        protected const float   TOLERANCE           = 0.005f;
        protected const float   SMOOTH_FACTOR       = 4.0f;

        protected int           _rotationIntervalTarget;
        protected float         _rotationTarget;
        protected float         _zoomTarget;

        protected bool          _rotating;
        protected bool          _zooming;


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default CameraController constructor. Provides clean movement for
        /// a Camera2D.
        /// </summary>
        /// <param name="camera">Camera to control</param>
        public CameraController(Camera2D camera)
        {
            _camera = camera;
            setRotationIntervalTarget(2);
            setZoomTarget(MIN_ZOOM);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// This update loop applies whatever influence to
        /// the camera this controller is currently having
        /// </summary>
        /// <param name="dt">Delta time</param>
        public void update(float dt)
        {
            pollUserInput();

            if (_rotating)
                applyRotation(dt);

            if (_zooming)
                applyZoom(dt);
        }

        /// <summary>
        /// Provide the player with input control of the camera
        /// </summary>
        protected void pollUserInput()
        {
            if (InputManager.GetOneKeyPressDown(Keys.Right))
                setRotationIntervalTarget(_rotationIntervalTarget + 1);
            if (InputManager.GetOneKeyPressDown(Keys.Left))
                setRotationIntervalTarget(_rotationIntervalTarget - 1);

            if (InputManager.GetOneKeyPressDown(Keys.OemPlus))
                setZoomTarget(_zoomTarget + ZOOM_INTERVAL);
            if (InputManager.GetOneKeyPressDown(Keys.OemMinus))
                setZoomTarget(_zoomTarget - ZOOM_INTERVAL);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Set the target rotation interval for the camera
        /// </summary>
        /// <param name="target">Target rotation interval</param>
        public void setRotationIntervalTarget(int target)
        {
            _rotationIntervalTarget = (target % MAX_ROTATIONS);

            if (_rotationIntervalTarget < 0)
                _rotationIntervalTarget = MAX_ROTATIONS + _rotationIntervalTarget;

            _rotationTarget = (_rotationIntervalTarget * ROTATION_INTERVAL) + BASE_OFFSET;
            _rotating = true;
        }

        /// <summary>
        /// Rotate the camera towards its target based on its speed and
        /// the elapsed time
        /// </summary>
        /// <param name="dt">Delta time</param>
        protected void applyRotation(float dt)
        {
            float diff = _rotationTarget - _camera.RotationZ;

            // Apply the rotation
            _camera.RotationZ += diff * SMOOTH_FACTOR * dt;

            /*
             * TODO: When the camera reaches either extreme of its rotation,
             * it performs a 135 degree rotation instead of a 45 degree rotation.
             * Need to implement a way for the controller to know that it should
             * make the smaller of the two rotations.
             * Don't modify the Camera class, that's what this function is for.
             * Determine the amount of rotation to apply to the camera, as done above.
             */

            // Snap to our target if we've reached it
            if (Math.Abs(diff) < TOLERANCE)
            {
                _camera.RotationZ = _rotationTarget;
                _rotating = false;
                return;
            }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Set the target zoom amount for the camera
        /// </summary>
        /// <param name="target">Target zoom</param>
        public void setZoomTarget(float target)
        {
            if (target > MAX_ZOOM)
                target = MAX_ZOOM;
            else if (target < MIN_ZOOM)
                target = MIN_ZOOM;

            _zoomTarget = target;
            _zooming = true;
        }

        /// <summary>
        /// Adjust the Camera's zoom over time, gradually bringing it
        /// closer to the target zoom amount
        /// </summary>
        /// <param name="dt">Delta time</param>
        protected void applyZoom(float dt)
        {
            float diff      = _zoomTarget - _camera.Zoom;

            _camera.Zoom += diff * SMOOTH_FACTOR * dt;

            // Did we reach our target?
            if (Math.Abs(diff) < TOLERANCE)
            {
                _camera.Zoom = _zoomTarget;
                _zooming = false;
            }
        }
    }
}
