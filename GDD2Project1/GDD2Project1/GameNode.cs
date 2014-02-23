using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDD2Project1
{
    class GameNode : GameObject
    {
        protected GameNode[]                    _neighbors;
        protected Dictionary<String, GameNode>  _children;

        protected String        _name;
        protected int           _graphIndex;

        protected GameNode      _parent;

        protected Vector3       _positionIsometric;
        protected Vector2       _scale;
        protected float         _rotation;

        protected Drawable      _drawable;
        protected Color         _color;


        //-------------------------------------------------------------------------
        public String Name { get { return _name; } }
        public int GraphIndex { get { return _graphIndex; } set { _graphIndex = value; } }

        public GameNode Parent { get { return _parent; } }
        public Dictionary<String, GameNode> Children { get { return _children; } }
        public GameNode[] Neighbors { get { return _neighbors; } }

        public Vector3 PositionIsometric
        {
            get { return _positionIsometric; }
            set { _positionIsometric = value; }
        }
        public override Vector2 Position
        {
            get { return _position; }
        }
        public Vector2 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Common constructor code. Should be called by all constructors
        /// </summary>
        /// <param name="position">World position</param>
        /// <param name="origin">Origin used for draw location and pivot</param>
        private void construct(String name, Vector3 position, Vector2 origin)
        {
            _name                   = name;
            _positionIsometric      = position;
            _origin                 = origin;

            _scale                  = new Vector2(1.0f);
            _rotation               = 0;
            _color                  = Color.White;

            _neighbors              = new GameNode[4];
            _children               = new Dictionary<string, GameNode>();
        }

        /// <summary>
        /// Construct a default GameObject, using Vector2.Zero for position and origin
        /// </summary>
        public GameNode(GameLevelManager gameLevelMgr, String name)
            : base(gameLevelMgr)
        {
            construct(name, Vector3.Zero, Vector2.Zero);
        }

        /// <summary>
        /// Create a GameNode with specified position and origin
        /// </summary>
        /// <param name="position">World position</param>
        /// <param name="origin">Origin used for draw location and pivot</param>
        public GameNode(GameLevelManager gameLevelMgr, String name, Vector3 position, Vector2 origin)
            : base(gameLevelMgr)
        {
            construct(name, position, origin);
        }


        //-------------------------------------------------------------------------
        public override void translate(Vector2 amount)
        {
            translate(new Vector3(amount.X, 0.0f, amount.Y));
        }

        public override void translate(float x, float y)
        {
            translate(new Vector3(x, 0, y));
        }

        public virtual void translate(Vector3 amount)
        {
            _positionIsometric += amount;
            foreach (KeyValuePair<String, GameNode> entry in _children)
                entry.Value.translate(amount);
        }

        public virtual void translate(float x, float y, float z)
        {
            translate(new Vector3(x, y, z));
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Returns a a child node with the specified name. If no child
        /// is found, returns null
        /// </summary>
        /// <param name="name">Name of hashed child node</param>
        /// <returns>Child node specified by name. Null if does not exist</returns>
        public virtual GameNode getChildNode(String name)
        {
            if (_children.ContainsKey(name))
                return _children[name];
            return null;
        }

        /// <summary>
        /// Creates a child node, attaches it to this node, and returns that node
        /// </summary>
        /// <param name="name">Name of new node</param>
        /// <returns>Newly created child node</returns>
        public virtual GameNode createChildNode(String name)
        {
            GameNode node = new GameNode(_gameLevelMgr, name, _positionIsometric, _origin);
            node._parent = this;

            _children.Add(name, node);
            return node;
        }

        public virtual GameNode createChildNode(String name, Vector3 position, Vector2 origin)
        {
            GameNode node = new GameNode(_gameLevelMgr, name, position, origin);
            node._parent = this;

            _children.Add(name, node);
            return node;
        }

        /// <summary>
        /// Attach a preexisting GameNode to this GameNode
        /// </summary>
        /// <param name="node">GameNode to attach</param>
        public virtual void attachChildNode(GameNode node)
        {
            node._parent = this;
            _children.Add(node.Name, node);
        }

        /// <summary>
        /// Detach a specified child node and return it
        /// </summary>
        /// <param name="name">Name of the previously created node</param>
        /// <returns>Newly detached node</returns>
        public virtual GameNode detachChildNode(String name)
        {
            if (!_children.ContainsKey(name))
                return null;

            GameNode child = _children[name];
            _children.Remove(name);
            child._parent = null;

            return child;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Attach a neighboring GameNode to this GameNode.
        /// Neighboring GameNode's should only be node's adjacent to this one
        /// </summary>
        /// <param name="node">Neighboring node</param>
        /// <param name="index">Index location</param>
        public virtual void attachNeighbor(GameNode node, int index)
        {
            _neighbors[index] = node;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Attach a drawable object to this GameNode
        /// </summary>
        /// <param name="drawable">Drawable to attach</param>
        /// <returns>True if attach suceeded, false otherwise</returns>
        public bool attachDrawable(Drawable drawable)
        {
            if (_drawable != null)
                return false;

            _drawable = drawable;

            return true;
        }

        /// <summary>
        /// Detach this GameNode's drawable and return it
        /// </summary>
        /// <returns>Detached Drawabled</returns>
        public Drawable detachDrawable()
        {
            Drawable drawable = _drawable;
            _drawable = null;
            return drawable;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// If this GameNode contains a Drawable, it will be drawn
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used for drawing</param>
        public virtual void drawContents(SpriteBatch spriteBatch, float dt)
        {
            if (_drawable == null)
                return;

            _position = _gameLevelMgr.Camera.isometricToCartesian(_positionIsometric);

            _drawable.draw(
                spriteBatch,
                _position,
                _origin,
                _color,
                _rotation,
                _scale,
                dt);
        }
    }
}
