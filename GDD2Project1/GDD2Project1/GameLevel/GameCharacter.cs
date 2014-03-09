using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDD2Project1
{
    /// <summary>
    /// CharacterState enum keeps track of this character's behavior.
    /// </summary>
    public enum CharacterState
    {
        CHRSTATE_IDLE,
        CHRSTATE_MOVING
    };


    //-----------------------------------------------------------------------------
    public class GameCharacter : GameObject
    {
        protected float                 _moveSpeed;
        protected CharacterState        _chrState;
        protected GameNode              _destination;
        protected Queue<GameNode>       _movePath;
        protected float                 _stateTime;


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default GameCharacter constructor.
        /// </summary>
        /// <param name="gameLevelMgr">This Character's GameLevelManager</param>
        /// <param name="name">Character's name</param>
        /// <param name="drawable">Character's DrawableAnimated</param>
        public GameCharacter(GameLevelManager gameLevelMgr, String name)
            : base(gameLevelMgr, name)
        {
            _movePath = new Queue<GameNode>();
            _moveSpeed = 240.0f;
        }


        //-------------------------------------------------------------------------
        public override bool attachDrawable(Drawable drawable)
        {
            if (!base.attachDrawable(drawable))
                return false;

            setCharacterState(CharacterState.CHRSTATE_IDLE);

            return true;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Draw this GameCharacter's drawable according to its animation data.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used for drawing sprites</param>
        /// <param name="dt">Delta time</param>
        public override void drawContents(SpriteBatch spriteBatch, float dt)
        {
            if (_drawable == null)
                return;

            _stateTime += dt;

            if (_drawable is DrawableAnimated)
            {
                (_drawable as DrawableAnimated).draw(
                    spriteBatch,
                    _gameLevelMgr.Camera.isometricToCartesian(_positionIsometric),
                    _color,
                    _rotation,
                    _scale,
                    _stateTime,
                    _chrState,
                    _gameLevelMgr.Camera.getRelativeDirection(_isoDirection));

                return;
            }

            base.drawContents(spriteBatch, dt);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Gives this GameCharacter a destination to move towards. This puts the character
        /// into a move state, and will continue until the destination changes, or until
        /// that destination is reached.
        /// </summary>
        /// <param name="destination">Node to move to</param>
        public void setDestination(GameNode destination)
        {
            if (destination == _parent)
                return;

            // Find our path
            _movePath.Enqueue(destination);

            // Set initial destination
            _destination    = _movePath.Dequeue();
            _vecDirection   = _destination.PositionIsometric - _positionIsometric;
            _vecDirection   = Vector3.Normalize(_vecDirection);

            // Set character state to kick off the movement
            setCharacterState(CharacterState.CHRSTATE_MOVING);
        }

        /// <summary>
        /// This function handles a GameCharacter's in-game movement. It should not be called by
        /// the Character itself, but it should be called by a GameLevel's update function.
        /// This will apply a displacement to this character based on the current character state,
        /// vector direction and move speed.
        /// <param name="dt">Delta time</param>
        public void applyDisplacement(float dt)
        {
            switch (_chrState)
            { 
                case CharacterState.CHRSTATE_IDLE:
                    return;

                case CharacterState.CHRSTATE_MOVING:
                    moveTowardsDestination(dt);
                    return;
            }
        }

        /// <summary>
        /// Update this GameCharacter's position, moving them closer to their destination.
        /// If their destination is reached, then the character state will revert back to
        /// the default state and their movement will cease.
        /// </summary>
        /// <param name="dt">Delta time</param>
        protected void moveTowardsDestination(float dt)
        {
            Vector3 distance            = _destination.PositionIsometric - _positionIsometric;
            distance.Y                  = 0.0f;
            Vector3 displacement        = _vecDirection * _moveSpeed * dt;

            // Check if we are going to overshoot or reach our destination
            if (Vector3H.MagnitudeSquared(displacement) >
                Vector3.DistanceSquared(_destination.PositionIsometric, _positionIsometric))
            {
                // We have reached our destination
                translateTo(_destination.PositionIsometric);

                // Check if there's another destination in queue
                if (_movePath.Count > 0)
                {
                    _destination = _movePath.Dequeue();
                    _vecDirection = _destination.PositionIsometric - _positionIsometric;
                    _vecDirection = Vector3.Normalize(_vecDirection);
                    _vecDirection.Y = 0.0f;
                    return;
                }
                // If not, then out work is done here
                else
                {
                    setCharacterState(CharacterState.CHRSTATE_IDLE);
                    return;
                }
            }
            // If not, then apply this character's movement
            else
            {
                translate(displacement);
            }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Handles all CharacterState changing logic. This function should be called
        /// once every time the character's state is changed.
        /// </summary>
        /// <param name="state"></param>
        public void setCharacterState(CharacterState state)
        {
            _chrState = state;
            _stateTime = 0;

            updateDirection();
        }
    }
}
