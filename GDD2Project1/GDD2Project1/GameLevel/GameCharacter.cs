using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameData;

namespace GDD2Project1
{
    /// <summary>
    /// GameCharacter
    /// 
    /// This class represents any character existing in a GameLevel.
    /// </summary>
    public class GameCharacter : GameObjectMovable
    {
        protected float         _moveSpeedSlow;
        protected float         _moveSpeedFast;
        protected float         _jumpSpeed;


        //-------------------------------------------------------------------------
        public override Vector3 DirectionVector
        {
            get { return _directionVector; }
            set
            {
                base.DirectionVector = value;
                if (_entityObj.AnimState != AnimationState.ANIMSTATE_IDLE)
                {
                    _velocity.X = _directionVector.X * _moveSpeedSlow;
                    _velocity.Z = _directionVector.Z * _moveSpeedSlow;
                }
            }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default GameCharacter constructor.
        /// </summary>
        /// <param name="gameLevelMgr">This Character's GameLevelManager</param>
        /// <param name="name">Character's name</param>
        /// <param name="drawable">Character's DrawableAnimated</param>
        public GameCharacter(String name, GameNode node)
            : base(name, node)
        {
            _moveSpeedSlow      = 26.0f;
            _moveSpeedFast      = 60.0f;
            _jumpSpeed          = 60.0f;
            setState(AnimationState.ANIMSTATE_IDLE);
        }


        //-------------------------------------------------------------------------
        public override void update(GameTime gameTime, float dt)
        {


            // Base updates core functionality
            base.update(gameTime, dt);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Set this GameCharacter's Entity AnimationState, which sets the GameCharacter's
        /// appearance and behavior.
        /// </summary>
        /// <param name="state">GameCharacter's AnimationState</param>
        public void setState(AnimationState state)
        {
            if (_entityObj.AnimState != state)
            {
                _entityObj.AnimState = state;

                switch (state)
                {
                    case AnimationState.ANIMSTATE_IDLE:
                        _velocity = Vector3.Zero;
                        break;

                    case AnimationState.ANIMSTATE_MOVE_SLOW:
                        _velocity.X = _directionVector.X * _moveSpeedSlow;
                        _velocity.Z = _directionVector.Z * _moveSpeedSlow;
                        break;

                    case AnimationState.ANIMSTATE_MOVE_FAST:
                        _velocity.X = _directionVector.X * _moveSpeedFast;
                        _velocity.Z = _directionVector.Z * _moveSpeedFast;
                        break;

                    //case AnimationState.ANIMSTATE_JUMP:
                    //    break;
                }
            }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Cause this character to perform a jump. The character will jump
        /// to a height corresponding to its _jumpHeight value.
        /// </summary>
        public void jump()
        {
            if (_grounded)
            {
                _velocity.Y -= _jumpSpeed;
                _grounded = false;
            }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Save the current state of this GameObject.
        /// </summary>
        /// <returns>State data.</returns>
        public override GameObjectData save()
        {
            GameObjectData data     = base.save();
            data.ObjType            = 2;

            return data;
        }
    }
}
