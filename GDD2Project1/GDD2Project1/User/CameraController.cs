using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using InputEventSystem;

namespace GDD2Project1
{
    public class CameraController : ActorController
    {
        protected const float   BASE_OFFSET         = (float)(Math.PI * 0.25);
        protected const float   ROTATION_INTERVAL   = (float)(Math.PI * 0.5f);
        protected const int     MAX_ROTATIONS       = (int)((Math.PI * 2) / ROTATION_INTERVAL + 1);
        protected const float   ZOOM_INTERVAL       = 0.2f;
        protected const float   MIN_ZOOM            = 0.4f;
        protected const float   MAX_ZOOM            = 1.0f;
        protected const float   TOLERANCE           = 0.005f;
        protected const float   SMOOTH_FACTOR       = 6.0f;

        protected GameNode      _nodeTarget;
        protected bool          _followNodeTarget;

        protected int           _rotationIntervalTarget;
        protected float         _rotationTarget;
        protected float         _zoomTarget;

        protected bool          _rotating;
        protected bool          _zooming;

        protected bool          _freeLook           = false;
        protected float         _moveSpeed          = 200.0f;
        protected Vector2       _velocity           = Vector2.Zero;


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default CameraController constructor. Provides clean movement for
        /// a Camera2D.
        /// </summary>
        /// <param name="camera">Camera to control</param>
        public CameraController(Camera2D camera, String name)
            : base(camera, name)
        {
            setRotationIntervalTarget(0);
            setZoomTarget(MIN_ZOOM);
            setFreeLook(false);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Shorthand function for returning the inherited actor as a Camera2D
        /// </summary>
        /// <returns>Camera2D controlled by this controller</returns>
        public Camera2D getCamera()
        {
            return _actor as Camera2D;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Local KeyDown event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override bool injectKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            { 
                case Keys.Right:
                    setRotationIntervalTarget(_rotationIntervalTarget + 1);
                    return true;

                case Keys.Left:
                    setRotationIntervalTarget(_rotationIntervalTarget - 1);
                    return true;

                case Keys.OemPlus:
                    setZoomTarget(_zoomTarget + ZOOM_INTERVAL);
                    return true;

                case Keys.OemMinus:
                    setZoomTarget(_zoomTarget - ZOOM_INTERVAL);
                    return true;

                case Keys.W:
                    _velocity.Y -= _moveSpeed;
                    return true;

                case Keys.S:
                    _velocity.Y += _moveSpeed;
                    return true;

                case Keys.A:
                    _velocity.X -= _moveSpeed;
                    return true;

                case Keys.D:
                    _velocity.X += _moveSpeed;
                    return true;
            }

            return base.injectKeyDown(e);
        }

        /// <summary>
        /// Local KeyUp event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override bool injectKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.W:
                    _velocity.Y += _moveSpeed;
                    return true;

                case Keys.S:
                    _velocity.Y -= _moveSpeed;
                    return true;

                case Keys.A:
                    _velocity.X += _moveSpeed;
                    return true;

                case Keys.D:
                    _velocity.X -= _moveSpeed;
                    return true;
            }

            return base.injectKeyUp(e);
        }

        /// <summary>
        /// Local MouseDown event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override bool injectMouseDown(MouseEventArgs e)
        {
            return base.injectMouseDown(e);
        }

        /// <summary>
        /// Local MouseUp event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override bool injectMouseUp(MouseEventArgs e)
        {
            return base.injectMouseUp(e);
        }

        /// <summary>
        /// Local MouseMove event handler. This function should inject this event
        /// to any components that check for input.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        public override bool injectMouseMove(MouseEventArgs e)
        {
            return base.injectMouseMove(e);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// This update loop applies whatever influence to
        /// the camera this controller is currently having
        /// </summary>
        /// <param name="dt">Delta time</param>
        public override void update(float dt)
        {
            if (_rotating)
                applyRotation(dt);

            if (_zooming)
                applyZoom(dt);

            if (_nodeTarget != null)
            {
                if (_followNodeTarget)
                    applyFollowTarget(dt);
            }
            else if (_freeLook)
                applyVelocity(dt);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Give this controller a GameNode to follow
        /// </summary>
        /// <param name="target">Target for this controller to follow</param>
        /// <param name="follow">Whether or not we are initially following the target</param>
        public void setCharacterTarget(GameNode target, bool follow = true)
        {
            _nodeTarget = target;
            setFollowTarget(follow);
        }

        /// <summary>
        /// If this controller already has a target, this function specifies whether or not that
        /// target is currently being followed by this controller's camera.
        /// </summary>
        /// <param name="value">Follow</param>
        public void setFollowTarget(bool value)
        {
            _followNodeTarget = value;
        }

        /// <summary>
        /// If we are following out target node, this function updates the
        /// camera's position to follow that node.
        /// </summary>
        /// <param name="dt">Delta time</param>
        protected void applyFollowTarget(float dt)
        {
            Vector2 nodePos = (_actor as Camera2D).isometricToCartesian(_nodeTarget.PositionIsometric);
            if (getCamera().Position == nodePos)
                return;

            getCamera().Position = nodePos;

            if (!_zooming)
                setZoomTarget(1.0f);
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
            float diff = _rotationTarget - getCamera().RotationZ;

            // If we reach the extremes of either rotation,
            // add or subtract a full rotation so we don't do
            // a 135 degree rotation
            if (diff > Math.PI)
                diff -= ((float)Math.PI * 2);
            else if (diff < -Math.PI)
                diff += ((float)Math.PI * 2);

            // Apply the rotation
            getCamera().RotationZ += diff * SMOOTH_FACTOR * dt;

            // Snap to our target if we've reached it
            if (Math.Abs(diff) < TOLERANCE)
            {
                getCamera().RotationZ = _rotationTarget;
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
            float diff = _zoomTarget - getCamera().Zoom;

            getCamera().Zoom += diff * SMOOTH_FACTOR * dt;

            // Did we reach our target?
            if (Math.Abs(diff) < TOLERANCE)
            {
                getCamera().Zoom = _zoomTarget;
                _zooming = false;
            }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Set whether or not this camera is in free look mode
        /// </summary>
        /// <param name="value">Obvious</param>
        public void setFreeLook(bool value)
        {
            _velocity = Vector2.Zero;
            _freeLook = value;
        }

        /// <summary>
        /// If the Camera is in free look mode, apply the velocity vector to it.
        /// </summary>
        /// <param name="dt">Delta time</param>
        protected void applyVelocity(float dt)
        {
            if (_velocity == Vector2.Zero)
                return;

            getCamera().translate(_velocity * dt);
        }
    }
}
