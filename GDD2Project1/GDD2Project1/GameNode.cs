using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDD2Project1
{
    /// <summary>
    /// GameNode class represents an arbitrary node existing within the GameLevel.
    /// A GameNode may have neighbors (nodes adjacent to it) and children (nodes
    /// connected to it).
    /// 
    /// This class also takes care of managing an isometric position, as well
    /// as a screen position cache.
    /// </summary>
    class GameNode : Actor
    {
        protected GameNode                      _parent;
        protected GameNode[]                    _neighbors;
        protected Dictionary<String, GameNode>  _children;

        protected String                        _name;
        protected int                           _graphIndex;

        protected Vector3                       _positionIsometric;
        protected Vector2                       _scale;
        protected float                         _rotation;


        //-------------------------------------------------------------------------
        public delegate void AttachedEventHandler(GameNode sender, GameNode child, EventArgs e);
        public event AttachedEventHandler ChildAttached;
        public EventArgs e = null;


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
        /// <summary>
        /// Subscribe to a node to receive events when that node attaches a new child.
        /// </summary>
        /// <param name="node">Node to receive events from</param>
        public void subscribeToNode(GameNode node)
        {
            node.ChildAttached += new GameNode.AttachedEventHandler(parentAttachedChild);
        }

        /// <summary>
        /// Unsubscribe from a node's attached event handler
        /// </summary>
        /// <param name="node">Node to unsubscribe from</param>
        public void unsubscribeFromNode(GameNode node)
        {
            node.ChildAttached -= parentAttachedChild;
        }

        /// <summary>
        /// This function is fired whenever one of this node's subscriptions attaches a child node.
        /// </summary>
        /// <param name="sender">Node which attached a child</param>
        /// <param name="child">The child which was attached</param>
        /// <param name="e">Event args</param>
        protected virtual void parentAttachedChild(GameNode sender, GameNode child, EventArgs e)
        {
            // TODO: This function needs to tell this node to respond to the event.
            // find out what kind of node child is.
            // For example, if node is an enemy, then the player should probably die,
            // because they can't both be on the same tile.
            // If child is a consumable, however, or if this is a consumable and child is a character,
            // then the consumable should be eaten by the character
        }



        //-------------------------------------------------------------------------
        /// <summary>
        /// Return this GameNode's parent
        /// </summary>
        public GameNode getParent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Returns this GameNode's neighbor array.
        /// This array will hold a maximum of four adjacent nodes
        /// and each node MAY be null. When iterating, you must
        /// check that each node is not null.
        /// </summary>
        public GameNode[] getNeighbors
        {
            get { return _neighbors; }
        }

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
        /// Returns this GameNode's name
        /// </summary>
        public String getName
        {
            get { return _name; }
        }

        /// <summary>
        /// Get / Set this GameNode's graph index. This index is used
        /// for graph traversal purposes, i.e. pathfinding.
        /// </summary>
        public int GraphIndex
        {
            get { return _graphIndex; }
            set { _graphIndex = value; }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Get / Set this GameNode's isometric position. These coordinates
        /// correspond to the GameNode's world position.
        /// </summary>
        public Vector3 PositionIsometric
        {
            get { return _positionIsometric; }
            set { _positionIsometric = value; }
        }

        /// <summary>
        /// Returns this GameNode's screen position vector.
        /// </summary>
        public override Vector2 Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Get / Set this GameNode's scale factor. This scale is applied to this node,
        /// as well as all of its children nodes. (not yet functional)
        /// </summary>
        public Vector2 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        /// <summary>
        /// Get / Set this GameNode's rotation amount. This rotation is applied
        /// to this GameNode, as well as all children nodes. (not yet functional)
        /// </summary>
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        /// <summary>
        /// Translate this GameNode by some specified amount.
        /// Vector's X and Y coordinates correspond to isometric
        /// X and Z coordinates.
        /// </summary>
        /// <param name="amount">Amount to translate</param>
        public override void translate(Vector2 amount)
        {
            translate(new Vector3(amount.X, 0.0f, amount.Y));
        }

        /// <summary>
        /// Translate this node by some specified amount.
        /// X and Z coordinates coorespond to isometric coordinates.
        /// </summary>
        /// <param name="x">X translation</param>
        /// <param name="z">Z translation</param>
        public override void translate(float x, float z)
        {
            translate(new Vector3(x, 0, z));
        }

        /// <summary>
        /// Translate this GameNode by some specified amount.
        /// </summary>
        /// <param name="amount">Amount to translate</param>
        public virtual void translate(Vector3 amount)
        {
            _positionIsometric += amount;
            foreach (KeyValuePair<String, GameNode> entry in _children)
                entry.Value.translate(amount);
        }

        /// <summary>
        /// Translate this GameNode by some specified amount.
        /// </summary>
        /// <param name="x">X translation</param>
        /// <param name="y">Y translation</param>
        /// <param name="z">Z translation</param>
        public virtual void translate(float x, float y, float z)
        {
            translate(new Vector3(x, y, z));
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Returns this GameNode's children dictionary
        /// </summary>
        public Dictionary<String, GameNode> getChildren
        {
            get { return _children; }
        }

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
            return createChildNode(name, Vector3.Zero, Vector2.Zero);
        }

        /// <summary>
        /// Creates a child node, attaches it to this node, and returns that node.
        /// </summary>
        /// <param name="name">Name of new node</param>
        /// <param name="position">Node's position</param>
        /// <param name="origin">Node's origin</param>
        /// <returns>Newly created child node</returns>
        public virtual GameNode createChildNode(String name, Vector3 position, Vector2 origin)
        {
            GameNode node = new GameNode(_gameLevelMgr, name, position, origin);

            attachChildNode(node);

            return node;
        }

        /// <summary>
        /// Attach a preexisting GameNode to this GameNode
        /// </summary>
        /// <param name="node">GameNode to attach</param>
        public virtual void attachChildNode(GameNode node)
        {
            node._parent = this;
            _children.Add(node.getName, node);

            // Fire event
            if (ChildAttached != null)
                ChildAttached(this, node, e);

            // Subscribe
            node.subscribeToNode(this);
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

            // Detach the node
            GameNode child = _children[name];
            _children.Remove(name);
            child._parent = null;

            // Unsubscribe
            child.unsubscribeFromNode(this);

            return child;
        }
    }
}
