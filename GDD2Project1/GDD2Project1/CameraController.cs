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
        protected ShiftTarget   _shiftTarget = ShiftTarget.SHIFT0;
        protected float
            _shiftOffset        = (float)Math.PI * 0.25f,
            _shiftAmount        = (float)Math.PI * 0.5f,
            _zoomTarget         = 1.0f,
            _zoomMin            = 0.5f,
            _zoomMax            = 2.0f,
            _adjustCoeff        = 4.0f,
            _adjustTolerance    = 0.005f;
        protected bool
            _shifting           = true,
            _zooming            = true;

        public enum ShiftTarget
        {
            SHIFT0 = 0,
            SHIFT1 = 1,
            SHIFT2 = 2,
            SHIFT3 = 3
        };


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default CameraController constructor. Provides clean movement for
        /// a Camera2D.
        /// </summary>
        /// <param name="camera">Camera to control</param>
        public CameraController(Camera2D camera)
        {
            _camera = camera;
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

            if (_shifting)
                updateShift(dt);

            if (_zooming)
                updateZoom(dt);
        }

        /// <summary>
        /// Provide the player with input control of the camera
        /// </summary>
        protected void pollUserInput()
        {
            if (InputManager.GetOneKeyPressDown(Keys.Right))
            {
                int shiftTarget = (int)_shiftTarget + 1;
                shiftTarget %= 4;
                shift((ShiftTarget)shiftTarget);
            }

            if (InputManager.GetOneKeyPressDown(Keys.OemMinus))
                zoom(0.5f);
            if (InputManager.GetOneKeyPressDown(Keys.OemPlus))
                zoom(1.0f);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Shift the camera to a new rotation target. This target indicates
        /// which rotation to move to, and is not relative to the camera's
        /// current positioning
        /// </summary>
        public void shift(ShiftTarget shiftTarget)
        {
            _shiftTarget = shiftTarget;
            _shifting = true;
        }

        /// <summary>
        /// If the camera is currently shifting to a new target, this function
        /// handles the camera's movement
        /// </summary>
        /// <param name="dt">Delta time</param>
        protected void updateShift(float dt)
        {
            float targetRadians = _shiftOffset + ((int)_shiftTarget * _shiftAmount);
            float diff = Math.Abs(targetRadians - _camera.RotationZ);

            if (diff < _adjustTolerance)
            {
                _camera.RotationZ = targetRadians;
                _shifting = false;
                return;
            }

            diff *= _adjustCoeff;
            diff *= dt;
            _camera.RotationZ += diff;
        }

        //-------------------------------------------------------------------------
        /// <summary>
        /// Gives the camera a target to zoom to
        /// </summary>
        /// <param name="amount">Amount for the target zoom</param>
        public void zoom(float amount)
        {
            if (amount < _zoomMin)
                amount = _zoomMin;
            else if (amount > _zoomMax)
                amount = _zoomMax;

            _zoomTarget = amount;
            _zooming = true;
        }

        /// <summary>
        /// When the Camera is in the process of zooming, this
        /// function handles its movement
        /// </summary>
        /// <param name="dt">Delta time</param>
        protected void updateZoom(float dt)
        {
            float diff = _zoomTarget - _camera.Zoom;

            if (diff < _adjustTolerance)
            {
                _camera.Zoom = _zoomTarget;
                _zooming = false;
                return;
            }

            diff *= _adjustCoeff;
            diff *= dt;
            _camera.Zoom += diff;
        }
    }
}
