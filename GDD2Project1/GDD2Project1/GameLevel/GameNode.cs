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
    /// GameNode
    /// 
    /// This class represents an arbitrary node existing within the GameLevel.
    /// a GameNode may have a single parent GameNode, and may have multiple
    /// children GameNodes.
    /// </summary>
    public class GameNode : Actor
    {
        protected String                        _name;
        protected GameNode                      _parent;
        protected Dictionary<String, GameNode>  _children;
        protected Vector3                       _positionIsometric;
        protected Entity                        _entity;


        //-------------------------------------------------------------------------
        public              String                          Name                { get { return _name; } }
        public              GameNode                        Parent              { get { return _parent; } }
        public              Dictionary<String, GameNode>    Children            { get { return _children; } }
        public              Vector3                         PositionIsometric   { get { return _positionIsometric; } set { _positionIsometric = value; } }
        public              Entity                          Entity              { get { return _entity; } }


        //-------------------------------------------------------------------------
        /// <summary>
        /// GameNode constructor.
        /// </summary>
        /// <param name="gameLevelMgr">GameLevelManager containing this GameNode.</param>
        /// <param name="name">Name of this GameNode.</param>
        /// <param name="active">Initial active status.</param>
        public GameNode(GameLevelManager gameLevelMgr, String name)
            : base(gameLevelMgr)
        {
            _name                   = name;
            _parent                 = null;
            _children               = new Dictionary<string, GameNode>();
            _positionIsometric      = Vector3.Zero;
            _entity                 = null;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Draw the contents of this GameNode.
        /// </summary>
        /// <param name="spriteBatch">Renders Texture2D.</param>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="dt">Precomputed delta time.</param>
        public void draw(SpriteBatch spriteBatch, GameTime gameTime, float dt)
        {
            if (_entity == null)
                return;

            _position = _gameLevelMgr.Camera.isometricToCartesian(_positionIsometric);

            _entity.draw(
                spriteBatch,
                _gameLevelMgr.Camera,
                gameTime,
                dt,
                _position,
                _rotation,
                _scale);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Translate this GameNode by some specified amount.
        /// </summary>
        /// <param name="amount">Amount to translate</param>
        public void translate(Vector3 amount)
        {
            if (amount == Vector3.Zero
                || float.IsNaN(amount.X)
                || float.IsNaN(amount.Y)
                || float.IsNaN(amount.Z))
                return;

            // Apply displacement
            _positionIsometric += amount;

            // Make all children follow along
            foreach (KeyValuePair<String, GameNode> entry in _children)
                entry.Value.translate(amount);

            // See if we need to update parent tile
            checkForNewParentTile();
        }

        /// <summary>
        /// Translate this GameNode by some specified amount.
        /// </summary>
        /// <param name="x">X translation</param>
        /// <param name="y">Y translation</param>
        /// <param name="z">Z translation</param>
        public void translate(float x, float y, float z)
        {
            translate(new Vector3(x, y, z));
        }

        /// <summary>
        /// Translate this node to a destination.
        /// </summary>
        /// <param name="destination">Destination</param>
        public void translateTo(Vector3 destination)
        {
            translate(destination - _positionIsometric);
        }

        /// <summary>
        /// Translate this node to a destination.
        /// </summary>
        /// <param name="x">X destination.</param>
        /// <param name="y">Y destination.</param>
        /// <param name="z">Z destination.</param>
        public void translateTo(float x, float y, float z)
        {
            translateTo(new Vector3(x, y, z));
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Returns a a child node with the specified name. If no child
        /// is found, returns null
        /// </summary>
        /// <param name="name">Name of hashed child node</param>
        /// <returns>Child node specified by name. Null if does not exist</returns>
        public GameNode getChildNode(String name)
        {
            if (_children.ContainsKey(name))
                return _children[name];
            return null;
        }

        /// <summary>
        /// Attach a preexisting GameNode to this GameNode.
        /// </summary>
        /// <param name="node">GameNode to attach.</param>
        public void attachChildNode(GameNode node)
        {
            // Form the parent-child relationship
            node._parent = this;
            _children.Add(node.Name, node);

            // Tell current children about the new child
            NodeEventArgs e = new NodeEventArgs(this, node);
            if (ChildAttached != null)
                ChildAttached(e);

            // Tell the new child about future children
            node.subscribe(this);
        }

        /// <summary>
        /// Detach a specified child node and return it
        /// </summary>
        /// <param name="name">Name of the previously created node</param>
        /// <returns>Newly detached node</returns>
        public GameNode detachChildNode(String name)
        {
            // Grab the child
            GameNode child = getChildNode(name);
            if (child == null)
                return null;

            // Remove the parent-child relationship
            _children.Remove(name);
            child._parent = null;

            // Stop telling this child about future children
            child.unsubscribe(this);

            // Tell all children about the node we just detached
            NodeEventArgs e = new NodeEventArgs(this, child);
            if (ChildDetached != null)
                ChildDetached(e);

            // Pass the detached node to caller
            return child;
        }

        /// <summary>
        /// Verify that this GameNode's parent tile is the tile directly underneath it.
        /// This functionality is essential for corrent draw order of objects.
        /// </summary>
        private void checkForNewParentTile()
        {
            // Node must already have a parent
            if (_parent == null)
                return;

            GameNode newParent = _gameLevelMgr.tileAtIsoCoords(_positionIsometric).Node;

            // If no node was found, we can't do anything
            if (newParent == null)
                return;

            // If we do have a parent, make sure we didn't grab the same one
            if (_parent == newParent)
                return;

            // We have a new parent, make the switch
            _parent.Entity.Color = Color.White;
            _parent.detachChildNode(_name);
            newParent.attachChildNode(this);
            _parent.Entity.Color = Color.Gold;

            // Fire parent changed event
            NodeEventArgs e = new NodeEventArgs(this);
            if (ParentChange != null)
                ParentChange(e);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Attach an entity to this GameNode.
        /// </summary>
        /// <param name="entity">Entity to attach.</param>
        public void attachEntity(Entity entity)
        {
            _entity = entity;
        }


        //-------------------------------------------------------------------------
        public delegate void        NodeEventHandler(NodeEventArgs e);
        private event               NodeEventHandler    ChildAttached;
        private event               NodeEventHandler    ChildDetached;
        public event                NodeEventHandler    ParentChange;


        //-------------------------------------------------------------------------
        /// <summary>
        /// Subscribe to a node to receive events when that node attaches a new child.
        /// </summary>
        /// <param name="node">Node to receive events from</param>
        private void subscribe(GameNode node)
        {
            node.ChildAttached += new GameNode.NodeEventHandler(parentAttachedChild);
            node.ChildDetached += new GameNode.NodeEventHandler(parentDetachedChild);
        }

        /// <summary>
        /// Unsubscribe from a node's attached event handler
        /// </summary>
        /// <param name="node">Node to unsubscribe from</param>
        private void unsubscribe(GameNode node)
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
        private void parentAttachedChild(NodeEventArgs e)
        {
            Console.WriteLine(_name + " (received event: Parent Attached Child) Sender: " + e.Sender.Name + " Child: " + e.Child.Name);
        }

        /// <summary>
        /// This function is fired whenever one of this node's subscriptions detaches a child node.
        /// </summary>
        /// <param name="sender">Node which detached a child</param>
        /// <param name="child">The child which was attached</param>
        /// <param name="e">Event args</param>
        private void parentDetachedChild(NodeEventArgs e)
        {
            Console.WriteLine(_name + " (received event: Parent Detached Child) Sender: " + e.Sender.Name + " Child: " + e.Child.Name);
        }
    }



    //-----------------------------------------------------------------------------
    /// <summary>
    /// NodeEventArgs
    /// 
    /// This class represents arguments passed when a GameNode fires an event.
    /// </summary>
    public class NodeEventArgs : EventArgs
    {
        private     GameNode    _sender;
        private     GameNode    _child;


        //-------------------------------------------------------------------------
        public      GameNode        Sender      { get { return _sender; } }
        public      GameNode        Child       { get { return _child; } }


        //-------------------------------------------------------------------------
        /// <summary>
        /// NodeEventArgs primary constructor. This constructor should populate
        /// all event variables.
        /// </summary>
        /// <param name="sender">Sender GameNode</param>
        /// <param name="child">Sender's child which may have triggered the event</param>
        public NodeEventArgs(
            GameNode sender,
            GameNode child = null)
        {
            _sender = sender;
            _child = child;
        }
    }
}
