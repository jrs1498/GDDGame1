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
    /// GameObject
    /// 
    /// This class acts as a controller for a GameNode, providing a GameObject
    /// specific interface and behavior.
    /// </summary>
    public class GameObject
    {
        protected String        _name;
        protected GameNode      _node;
        protected Entity        _entityObj;
        protected Vector3       _directionVector;
        protected bool          _active;


        //-------------------------------------------------------------------------
        public              String          Name            { get { return _name; } }
        public              GameNode        Node            { get { return _node; } }
        public              Entity          Entity
        {
            get { return _entityObj; }
            set
            {
                _entityObj          = value;
                _entityObj.Active   = _active;
                _node.attachEntity(value);
            }
        }
        public virtual      Vector3         DirectionVector
        {
            get { return _directionVector; }
            set
            {
                value = Vector3.Normalize(value);
                _directionVector = value;
                _entityObj.updateDirectionView(value);
            }
        }

        public              float           X               { get { return _node.PositionIsometric.X; } }
        public              float           Y               { get { return _node.PositionIsometric.Y; } }
        public              float           Z               { get { return _node.PositionIsometric.Z; } }

        public              bool            Active
        {
            get { return _active; }
            set
            {
                _active = value;
                if (_entityObj != null)
                    _entityObj.Active = value;
            }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// GameObject constructor.
        /// </summary>
        /// <param name="node">GameNode controlled by this GameObject.</param>
        public GameObject(String name, GameNode node)
        {
            _name               = name;
            _node               = node;
            _entityObj          = node.Entity;
            Active              = true;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// GameObject update function.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="dt">Precomputed delta time.</param>
        public virtual void update(GameTime gameTime, float dt)
        {
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Save and return the current state of this GameObject.
        /// </summary>
        /// <returns>GameObject state data.</returns>
        public virtual GameObjectData save()
        {
            GameObjectData data     = new GameObjectData();
            data.ObjType            = 0;
            data.Name               = _name;
            data.Drawable           = _entityObj.Drawable.Name;
            data.Position           = _node.PositionIsometric;
            data.Direction          = _directionVector;
            data.Active             = _active;

            return data;
        }
    }
}
