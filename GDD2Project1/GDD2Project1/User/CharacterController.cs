using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using InputEventSystem;

namespace GDD2Project1
{
    /// <summary>
    /// CharacterController
    /// 
    /// Provides GameCharacter controller interface.
    /// </summary>
    public class CharacterController : UserController
    {
        protected GameCharacter     _character;
        protected Vector3           _moveDirection;


        //-------------------------------------------------------------------------
        protected Vector3 MoveDirection
        {
            get { return _moveDirection; }
            set
            {
                _moveDirection = value;
                if (_moveDirection == Vector3.Zero)
                    _character.setState(AnimationState.ANIMSTATE_IDLE);
                else
                {
                    _character.DirectionVector =
                        _character.Node.GameLevelMgr.Camera.getRelativeDirectionVector(_moveDirection);
                    _character.setState(AnimationState.ANIMSTATE_MOVE_SLOW);
                }
            }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// CharacterController constructor.
        /// </summary>
        /// <param name="name">Name of this controller.</param>
        /// <param name="character">Character controlled by this controller.</param>
        public CharacterController(String name, GameCharacter character)
            : base(name)
        {
            _character = character;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// KeyDown input event handler function.
        /// </summary>
        /// <param name="e">KeyEvent arguments.</param>
        /// <returns>True if handled.</returns>
        public override bool injectKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            { 
                case Keys.W:
                    MoveDirection += GameLevelManager.directionVectorFromView(Direction.DIR_N);
                    break;

                case Keys.A:
                    MoveDirection += GameLevelManager.directionVectorFromView(Direction.DIR_W);
                    break;

                case Keys.S:
                    MoveDirection += GameLevelManager.directionVectorFromView(Direction.DIR_S);
                    break;

                case Keys.D:
                    MoveDirection += GameLevelManager.directionVectorFromView(Direction.DIR_E);
                    break;

                case Keys.Space:
                    _character.jump();
                    break;
            }

            return base.injectKeyDown(e);
        }

        /// <summary>
        /// KeyUp input event handler function.
        /// </summary>
        /// <param name="e">KeyEvent arguments.</param>
        /// <returns>True if handled.</returns>
        public override bool injectKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.W:
                    MoveDirection -= GameLevelManager.directionVectorFromView(Direction.DIR_N);
                    break;

                case Keys.A:
                    MoveDirection -= GameLevelManager.directionVectorFromView(Direction.DIR_W);
                    break;

                case Keys.S:
                    MoveDirection -= GameLevelManager.directionVectorFromView(Direction.DIR_S);
                    break;

                case Keys.D:
                    MoveDirection -= GameLevelManager.directionVectorFromView(Direction.DIR_E);
                    break;
            }

            return base.injectKeyUp(e);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// MouseDown input event handler function.
        /// </summary>
        /// <param name="e">MouseEvent arguments.</param>
        /// <returns>True if handled.</returns>
        public override bool injectMouseDown(MouseEventArgs e)
        {
            return base.injectMouseDown(e);
        }

        /// <summary>
        /// MouseUp input event handler function.
        /// </summary>
        /// <param name="e">MouseEvent arguments.</param>
        /// <returns>True if handled.</returns>
        public override bool injectMouseUp(MouseEventArgs e)
        {
            return base.injectMouseUp(e);
        }

        /// <summary>
        /// MouseMove input event handler function.
        /// </summary>
        /// <param name="e">MouseEvent arguments.</param>
        /// <returns>True if handled.</returns>
        public override bool injectMouseMove(InputEventSystem.MouseEventArgs e)
        {
            return base.injectMouseMove(e);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// UserController update function. Applies controller influence on an object.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="dt">Precomputed delta time.</param>
        public override void update(GameTime gameTime, float dt)
        {
            if (_moveDirection != Vector3.Zero)
            {
                Vector3 dir = _character.Node.GameLevelMgr.Camera.getRelativeDirectionVector(_moveDirection);
                _character.DirectionVector = dir;
            }

            return;
        }
    }
}
