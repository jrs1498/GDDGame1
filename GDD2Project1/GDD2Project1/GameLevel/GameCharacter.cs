using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDD2Project1
{
    public class GameCharacter : GameObject
    {
        protected float                 _moveSpeed;
        protected CharacterState        _chrState;
        protected GameNode              _destination;
        protected Queue<GameNode>       _movePath;


        //-------------------------------------------------------------------------
        /// <summary>
        /// CharacterState enum keeps track of this character's behavior.
        /// </summary>
        public enum CharacterState
        {
            CHRSTATE_IDLE,
            CHRSTATE_MOVING
        };


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default GameCharacter constructor.
        /// </summary>
        /// <param name="gameLevelMgr">This Character's GameLevelManager</param>
        /// <param name="name">Character's name</param>
        /// <param name="drawable">Character's DrawableAnimated</param>
        public GameCharacter(GameLevelManager gameLevelMgr, String name, Drawable drawable)
            : base(gameLevelMgr, name)
        {
            attachDrawable(drawable);

            _movePath = new Queue<GameNode>();
            _moveSpeed = 240.0f;

            setCharacterState(CharacterState.CHRSTATE_IDLE);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Draw this GameCharacter's drawable according to its animation data.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used for drawing sprites</param>
        /// <param name="dt">Delta time</param>
        public override void drawContents(SpriteBatch spriteBatch, float dt)
        {
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
            //_movePath = findPath(destination);
            _movePath.Enqueue(destination);

            // Set initial destination
            _destination    = _movePath.Dequeue();
            _vecDirection   = _destination.PositionIsometric - _positionIsometric;
            _vecDirection   = Vector3.Normalize(_vecDirection);

            // Set character state to kick off the movement
            setCharacterState(CharacterState.CHRSTATE_MOVING);
        }

        /// <summary>
        /// Given a destination, this function finds the shortest path from
        /// the GameCharacter's current parent node (_parent) to the destination
        /// specified. This path is then returned as a queue to the setDestination()
        /// function, which tells the character to begin walking this path.
        /// </summary>
        /// <param name="destination">Character's destination</param>
        /// <returns></returns>
        protected Queue<GameNode> findPath(GameNode destination)
        {
            Queue<GameNode> path = new Queue<GameNode>();

            // TODO: 
            // implement a pathfinding algorithm, and return that path
            // as a queue of game nodes.
            // 
            // Use GameNode.getNeighbors to get an array of adjacent nodes.
            // This will return an array of 4, with potential to contain null nodes,
            // so make sure you check if(node != null) before doing anything with it.

            return path;
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
                _positionIsometric = _destination.PositionIsometric;

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
        /// This function is fired any time the camera's isometric viewing angle changes.
        /// This function should then preserve the GameCharacter's isometric direction
        /// so we are looking at the correct side no matter what angle the camera is looking.
        /// </summary>
        /// <param name="cam">Camera which fired the event</param>
        /// <param name="e">Event args</param>
        protected override void camDirectionChanged(Camera2D cam, EventArgs e)
        {
            // Base calls updatePosition()
            base.camDirectionChanged(cam, e);

            updateCharacterAnimation();
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

            updateDirection();
            updateCharacterAnimation();
        }

        /// <summary>
        /// Sets this GameCharacter's drawable's animation to match the current
        /// character state.
        /// </summary>
        protected void updateCharacterAnimation()
        {
            string anim = "";

            switch (_chrState)
            { 
                case CharacterState.CHRSTATE_IDLE:
                    switch (_isoDirection)
                    { 
                        case Direction.DIR_NE:
                            anim = "idleNE";
                            break;
                        case Direction.DIR_SE:
                            anim = "idleSE";
                            break;
                        case Direction.DIR_SW:
                            anim = "idleSW";
                            break;
                        case Direction.DIR_NW:
                            anim = "idleNW";
                            break;
                    }
                    break;

                case CharacterState.CHRSTATE_MOVING:
                    switch (_isoDirection)
                    {
                        case Direction.DIR_NE:
                            anim = "walkNE";
                            break;
                        case Direction.DIR_SE:
                            anim = "walkSE";
                            break;
                        case Direction.DIR_SW:
                            anim = "walkSW";
                            break;
                        case Direction.DIR_NW:
                            anim = "walkNW";
                            break;
                    }
                    break;
            }

            if (anim != "")
            {
                DrawableAnimated drawable = _drawable as DrawableAnimated;
                drawable.setAnimation(anim);
            }
        }
    }
}
