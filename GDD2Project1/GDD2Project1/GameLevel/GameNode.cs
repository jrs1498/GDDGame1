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
    /// GameNode class represents an arbitrary node existing within the GameLevel.
    /// A GameNode may have neighbors (nodes adjacent to it) and children (nodes
    /// connected to it).
    /// 
    /// This class also takes care of managing an isometric position, as well
    /// as a screen position cache.
    /// </summary>
    public class GameNode : Actor
    {
        protected GameNode                      _parent;
        protected Dictionary<String, GameNode>  _children;

        protected String                        _name;

        protected Vector3                       _positionIsometric;
        protected Vector2                       _scale;
        protected float                         _rotation;


        //-------------------------------------------------------------------------
        public delegate void NodeEventHandler(GameNode sender, GameNode child, EventArgs e);
        public event NodeEventHandler ChildAttached;
        public event NodeEventHandler ChildDetached;
        public EventArgs e = null;


        //-------------------------------------------------------------------------
        /// <summary>
        /// Common constructor code. Should be called by all constructors
        /// </summary>
        /// <param name="position">World position</param>
        /// <param name="origin">Origin used for draw location and pivot</param>
        private void construct(String name, Vector3 position)
        {
            _name                   = name;
            _positionIsometric      = position;

            _scale                  = new Vector2(1.0f);
            _rotation               = 0;

            _children               = new Dictionary<string, GameNode>();
        }

        /// <summary>
        /// Construct a default GameObject, using Vector2.Zero for position and origin
        /// </summary>
        public GameNode(GameLevelManager gameLevelMgr, String name)
            : base(gameLevelMgr)
        {
            construct(name, Vector3.Zero);
        }

        /// <summary>
        /// Create a GameNode with specified position and origin
        /// </summary>
        /// <param name="position">World position</param>
        /// <param name="origin">Origin used for draw location and pivot</param>
        public GameNode(GameLevelManager gameLevelMgr, String name, Vector3 position)
            : base(gameLevelMgr)
        {
            construct(name, position);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Save this GameNode
        /// </summary>
        /// <returns>Data corresponding to GameNode</returns>
        public GameNodeData saveGameNode()
        {
            GameNodeData data = new GameNodeData();
            data.Name = _name;
            data.PositionIsometric = _positionIsometric;
            return data;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Subscribe to a node to receive events when that node attaches a new child.
        /// </summary>
        /// <param name="node">Node to receive events from</param>
        public void subscribeToNode(GameNode node)
        {
            node.ChildAttached += new GameNode.NodeEventHandler(parentAttachedChild);
            node.ChildDetached += new GameNode.NodeEventHandler(parentDetachedChild);
        }

        /// <summary>
        /// Unsubscribe from a node's attached event handler
        /// </summary>
        /// <param name="node">Node to unsubscribe from</param>
        public void unsubscribeFromNode(GameNode node)
        {
            node.ChildAttached -= parentAttachedChild;
            node.ChildDetached -= parentDetachedChild;
        }

        /// <summary>
        /// This function is fired whenever one of this node's subscriptions attaches a child node.
        /// </summary>
        /// <param name="sender">Node which attached a child</param>
        /// <param name="child">The child which was attached</param>
        /// <param name="e">Event args</param>
        protected virtual void parentAttachedChild(GameNode sender, GameNode child, EventArgs e)
        {
            Console.WriteLine(_name + " (received event: Parent Attached Child) Sender: " + sender.getName + " Child: " + child.getName);
        }

        /// <summary>
        /// This function is fired whenever one of this node's subscriptions detaches a child node.
        /// </summary>
        /// <param name="sender">Node which detached a child</param>
        /// <param name="child">The child which was attached</param>
        /// <param name="e">Event args</param>
        protected virtual void parentDetachedChild(GameNode sender, GameNode child, EventArgs e)
        {
            Console.WriteLine(_name + " (received event: Parent Detached Child) Sender: " + sender.getName + " Child: " + child.getName);
        }



        //-------------------------------------------------------------------------
        /// <summary>
        /// Return this GameNode's parent
        /// </summary>
        public GameNode getParent
        {
            get { return _parent; }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Returns this GameNode's name
        /// </summary>
        public String getName
        {
            get { return _name; }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Get / Set this GameNode's isometric position. These coordinates
        /// correspond to the GameNode's world position.
        /// </summary>
        public virtual Vector3 PositionIsometric
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


        //-------------------------------------------------------------------------
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
        /// <param name="x">X translation</param>
        /// <param name="y">Y translation</param>
        /// <param name="z">Z translation</param>
        public virtual void translate(float x, float y, float z)
        {
            translate(new Vector3(x, y, z));
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
            return createChildNode(name, Vector3.Zero);
        }

        /// <summary>
        /// Creates a child node, attaches it to this node, and returns that node.
        /// </summary>
        /// <param name="name">Name of new node</param>
        /// <param name="position">Node's position</param>
        /// <param name="origin">Node's origin</param>
        /// <returns>Newly created child node</returns>
        public virtual GameNode createChildNode(String name, Vector3 position)
        {
            GameNode node = new GameNode(_gameLevelMgr, name, position);

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

            // Fire event
            if (ChildDetached != null)
                ChildDetached(this, child, e);

            return child;
        }
    }
}
