using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GDD2Project1
{
    class Character
    {
        protected GameNode          _node;
        protected DrawableAnimated  _drawable;
        protected Queue<GameNode>   _movePath;
        protected GameNode          _destination;
        protected Vector3           _moveDirection;     // Need to fix these confusing names
        protected Direction         _charDirection;
        protected CharacterState    _chrState;
        protected float             _moveSpeed;

        public enum CharacterState
        {
            CHRSTATE_IDLE,
            CHRSTATE_MOVING
        };

        //-------------------------------------------------------------------------
        public DrawableAnimated Drawable
        {
            get { return _drawable; }
            set { _drawable = value; }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Common constructor code. All Character constructors should
        /// call this function.
        /// </summary>
        /// <param name="drawable">This character's drawable</param>
        /// <param name="node">This character's node</param>
        public void construct(DrawableAnimated drawable, GameNode node)
        {
            _node       = node;
            _drawable   = drawable;

            _movePath   = new Queue<GameNode>();
            _moveSpeed  = 240.0f;

            setCharacterState(CharacterState.CHRSTATE_IDLE);
        }

        public Character(DrawableAnimated drawable)
        {
            construct(drawable, null);
        }

        public Character(DrawableAnimated drawable, GameNode node)
        {
            construct(drawable, node);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Subscribe to a Camera2D's DirectionChanged event
        /// </summary>
        /// <param name="cam">Camera2D to subscribe to</param>
        public void subscribeToCamera(Camera2D cam)
        {
            cam.DirectionChanged += new Camera2D.DirectionHandler(camDirectionChanged);
        }

        /// <summary>
        /// Called whenever a Camera2D fires its DirectionChanged event
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="e"></param>
        protected virtual void camDirectionChanged(Camera2D cam, EventArgs e)
        {
            setDirection();
            setCharacterAnimation();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Perform an update on this Character
        /// </summary>
        /// <param name="dt">Delta time</param>
        public void update(float dt)
        {
            switch (_chrState)
            { 
                case CharacterState.CHRSTATE_IDLE:
                    return;

                case CharacterState.CHRSTATE_MOVING:
                    updateMovement(dt);
                    break;
            }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Applies a position displacement according to the character's
        /// current state and move speed
        /// </summary>
        /// <param name="dt">Delta time</param>
        public void updateMovement(float dt)
        {
            Vector3 distance            = _destination.PositionIsometric - _node.PositionIsometric;
            distance.Y                  = 0.0f;
            Vector3 displacement        = _moveDirection * _moveSpeed * dt;
            
            // Check if we are going to overshoot or reach the destination
            if (Vector3H.MagnitudeSquared(displacement) >
                Vector3.DistanceSquared(_destination.PositionIsometric, _node.PositionIsometric))
            {
                // We have reached a new node
                _node.PositionIsometric = _destination.PositionIsometric;
                _node.Parent.detachChildNode(_node.Name);
                _destination.attachChildNode(_node);

                // Check for another destination in queue
                if (_movePath.Count > 0)
                {
                    _destination        = _movePath.Dequeue();
                    _moveDirection      = _destination.PositionIsometric - _node.PositionIsometric;
                    _moveDirection      = Vector3.Normalize(_moveDirection);
                    _moveDirection.Y    = 0.0f;
                    return;
                }
                // If not, then we're done here
                else
                {
                    setCharacterState(CharacterState.CHRSTATE_IDLE);
                    return;
                }
            }
            // If not, then apply the movement
            else
            {
                _node.PositionIsometric += displacement;

                GameNode currNode = _node.GameLevelMgr.getNodeFromIsometricCoordinates(_node.PositionIsometric);
                if (currNode != _node)
                {
                    _node.Parent.detachChildNode(_node.Name);
                    currNode.attachChildNode(_node);
                }
            }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary CharacterState handler function
        /// </summary>
        /// <param name="state">State to set this character to</param>
        public void setCharacterState(CharacterState state)
        {
            _chrState = state;

            setDirection();
            setCharacterAnimation();
        }

        /// <summary>
        /// Determine the Character's direction from its direction vector
        /// </summary>
        public void setDirection()
        {
            Vector2 dir = new Vector2(_moveDirection.X, _moveDirection.Z);
            dir = Vector2.Normalize(dir);
            Vector2 xAxis = new Vector2(1.0f, 0.0f);
            float cosTheta = Vector2.Dot(dir, xAxis);

            if (cosTheta < -0.707f)
                _charDirection = Direction.DIR_NW;
            else if (cosTheta > 0.707f)
                _charDirection = Direction.DIR_SE;
            else if (dir.Y > 0)
                _charDirection = Direction.DIR_SW;
            else
                _charDirection = Direction.DIR_NE;

            _charDirection = _node.GameLevelMgr.Camera.getRelativeDirection(_charDirection);
        }

        /// <summary>
        /// Sets the Character's animation according to state and direction
        /// </summary>
        public void setCharacterAnimation()
        {
            string anim = "";

            switch (_chrState)
            { 
                case CharacterState.CHRSTATE_IDLE:
                    switch (_charDirection)
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
                    switch (_charDirection)
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

            _drawable.setAnimation(anim);
        }

        //-------------------------------------------------------------------------
        /// <summary>
        /// Give this character a destination node, which they will
        /// begin moving towards until either they reach it, or the state changes.
        /// </summary>
        /// <param name="destination">Node to move to</param>
        public void moveToNode(GameNode destination)
        {
            if (destination == _node.Parent)
                return;

            _movePath = findPath(destination);
            _movePath.Enqueue(destination);

            // Set our initial destination
            _destination = _movePath.Dequeue();
            _moveDirection = _destination.PositionIsometric - _node.PositionIsometric;
            _moveDirection = Vector3.Normalize(_moveDirection);

            // Set character state to kick off movement
            setCharacterState(CharacterState.CHRSTATE_MOVING);
        }

        /// <summary>
        /// Dijkstra's algorithm to traverse the scene graph and find the
        /// shortest path to the destination node.
        /// </summary>
        /// <param name="node">Destination node</param>
        protected Queue<GameNode> findPath(GameNode destination)
        {
            Queue<GameNode> path = new Queue<GameNode>();

            // TODO: Implement pathfinding algorithm from
            // this character's node (_node) to the passed in destination

            // Use node.Neighbors to get an array of adjacent nodes.
            // This will return an array of length 4 at a maximum, and you must
            // check that each entry in the array is not null
            // EX:
            // foreach (GameNode node in _node.Neighbors)
            //     if (node != null)
            //         // Do stuff
            //
            // enqueue every node in the path into the path queue

            return path;
        }
    }
}
