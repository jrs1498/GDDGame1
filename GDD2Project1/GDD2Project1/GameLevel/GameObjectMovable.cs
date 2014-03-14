using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameData;

namespace GDD2Project1
{
    /// <summary>
    /// GameObjectMovable
    /// 
    /// This class represents a GameObject which may move within the GameLevel,
    /// and will collide with other GameObjects.
    /// </summary>
    public class GameObjectMovable : GameObject
    {
        private const float STEP_HEIGHT = 40.0f;
        private const float MAX_DEPTH = 2000.0f;
        protected Vector3   _velocity;
        protected bool      _grounded;


        //-------------------------------------------------------------------------
        /// <summary>
        /// GameObjectMovable constructor.
        /// </summary>
        /// <param name="name">This GameObject's name.</param>
        /// <param name="node">This GameObject's GameNode.</param>
        public GameObjectMovable(String name, GameNode node)
            : base(name, node)
        {
            _velocity = new Vector3(0.0f, 0.0f, 0.0f);
            _grounded = false;

            // When the node changes parent tile, it should start falling
            node.ParentChange += delegate(NodeEventArgs e)
            {
                _grounded = false;
            };
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// GameObjectMovable update function.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="dt">Precomputed delta time.</param>
        public override void update(GameTime gameTime, float dt)
        {
            // If we aren't grounded, we should be falling
            if (!_grounded)
                _velocity.Y += (9.8f * dt * GameLevelManager.UNIT_SIZE);

            // Make sure there is a valid tile where we're trying to move
            checkForValidMove();

            // Apply velocity to GameNode
            if (_velocity != Vector3.Zero)
                _node.translate(_velocity * dt * GameLevelManager.UNIT_SIZE);

            // Check if we hane landed on our parent
            if (!_grounded)
                checkForLand();

            // Update base
            base.update(gameTime, dt);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Check whether or not this GameObject is allowed to move where it's trying
        /// to move. Modify velocity accordingly to make this move possible.
        /// </summary>
        protected void checkForValidMove()
        {
            // Grab what will be our parent
            GameNode nextParent = _node.GameLevelMgr.tileAtIsoCoords(
                _node.PositionIsometric + _velocity).Node;

            // If there won't be a new parent, do nothing
            if (nextParent == _node.Parent)
                return;

            // If the next parent doesn't exist, the GameObject is trying
            // to move outside the bounds of the GameLevel
            if (nextParent == null)
            {
                _velocity.X = 0.0f;
                _velocity.Z = 0.0f;
                return;
            }

            // If the next parent is inactive, then this indicates an empty
            // hole in the GameLevel. Should we fall through it?
            if (!nextParent.Entity.Active)
            {
                return;
            }

            // If the new parent is too high to reach, disallow a movement
            // to that tile
            float dy = _node.PositionIsometric.Y - nextParent.PositionIsometric.Y;
            if (dy > STEP_HEIGHT)
            {
                _velocity.X = 0.0f;
                _velocity.Z = 0.0f;
                return;
            }
        }


        /// <summary>
        /// Check if we have landed on the tile directly underneath this GameObject,
        /// and respond accordingly.
        /// </summary>
        protected void checkForLand()
        {
            if (_node.PositionIsometric.Y >= _node.Parent.PositionIsometric.Y
                && _node.Parent.Entity.Active)
            {
                float dy = _node.Parent.PositionIsometric.Y - _node.PositionIsometric.Y;
                _node.translate(0.0f, dy, 0.0f);
                _velocity.Y = 0.0f;
                _grounded = true;
            }
            else if (_node.PositionIsometric.Y > MAX_DEPTH)
            {
                respawn(_node.GameLevelMgr.tileAtIndex(0, 0).Node);
            }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Respawn this GameObject at the tile specified.
        /// </summary>
        /// <param name="parentTile">Tile where this GameObject will respawn.</param>
        public void respawn(GameNode parentTile)
        {
            _velocity = Vector3.Zero;
            _node.translateTo(parentTile.PositionIsometric - new Vector3(0.0f, 120.0f, 0.0f));
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Save and return the current state of this GameObject.
        /// </summary>
        /// <returns>State data.</returns>
        public override GameObjectData save()
        {
            GameObjectData data     = base.save();
            data.ObjType            = 1;

            return data;
        }
    }
}
