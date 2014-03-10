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
    /// GameObject class represent an actual object in the GameLevel.
    /// </summary>
    public class GameObject : GameNode
    {
        protected Drawable      _drawable;
        protected Color         _color;
        protected Vector3       _vecDirection;
        protected Direction     _isoDirection;
        protected bool          _active = true;


        //-------------------------------------------------------------------------
        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default GameObject constructor.
        /// </summary>
        /// <param name="gameLevelMgr"></param>
        /// <param name="name"></param>
        public GameObject(GameLevelManager gameLevelMgr, String name)
            : base(gameLevelMgr, name)
        {
            // Defaults
            _color = Color.White;
            _vecDirection = new Vector3(1.0f, 0.0f, 0.0f);
            _isoDirection = Direction.DIR_SW;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Save this GameObject
        /// </summary>
        /// <returns>Data corresponding to GameObject</returns>
        public GameObjectData saveGameObject()
        {
            GameObjectData data = new GameObjectData();
            data.Name = _name;
            data.PositionIsometric = _positionIsometric;
            data.Drawable = _drawable.Name;
            data.Active = _active;

            data.Children = new GameObjectData[_children.Count];
            int i = 0;
            foreach (KeyValuePair<String, GameNode> entry in _children)
            {
                if (entry.Value is Consumable)
                {
                    data.Children[i] = (entry.Value as Consumable).saveConsumable();
                }
                else if (entry.Value is GameObject)
                {
                    data.Children[i] = (entry.Value as GameObject).saveGameObject();
                }

                i++;
            }

            return data;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Draws this GameObject's attached Drawable, if it exists. If there is no
        /// Drawable attached, then this function will immediately return.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used for drawing</param>
        /// <param name="dt">Delta time</param>
        public virtual void drawContents(SpriteBatch spriteBatch, float dt)
        {
            if (_drawable == null || !_active)
                return;

            // This line can be optimized.
            // _position only needs to update if the GameObject moved, or
            // if the camera moved.
            _position = _gameLevelMgr.Camera.isometricToCartesian(_positionIsometric);

            _drawable.draw(
                spriteBatch,
                _position,
                _color,
                _rotation,
                _scale,
                dt);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Set / Get isometric position vector. Overridden to ensure that when
        /// a GameObject is moved, it will select the correct parent tile.
        /// </summary>
        public override Vector3 PositionIsometric
        {
            get { return base.PositionIsometric; }
            set
            {
                base.PositionIsometric = value;
                grabParentTile();
            }
        }

        /// <summary>
        /// Translate this GameNode by some specified amount.
        /// A GameObject should be moved only through this method.
        /// </summary>
        /// <param name="amount">Amount to translate</param>
        public override void translate(Vector3 amount)
        {
            base.translate(amount);
            grabParentTile();
        }

        /// <summary>
        /// Translate this GameObject to the specified position
        /// </summary>
        /// <param name="position">Destination</param>
        public virtual void translateTo(Vector3 position)
        {
            translate(position - _positionIsometric);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Attaches a Drawable to this GameObject. A GameObject may have only one Drawable
        /// at a time, and if this GameObject is currently holding a Drawable, it must be
        /// detached before a different Drawable may be attached.
        /// </summary>
        /// <param name="drawable">Drawable to attach</param>
        /// <returns>True if Drawable is now attached</returns>
        public virtual bool attachDrawable(Drawable drawable)
        {
            if (_drawable != null)
                return false;

            _drawable = drawable;

            return true;
        }

        /// <summary>
        /// Detaches and returns this GameObject's Drawable. This has the potential
        /// to return a null Drawable, which would happen in the case that there is no Drawable
        /// currently attached to this GameObject.
        /// </summary>
        /// <returns>Detached Drawable</returns>
        public Drawable detachDrawable()
        {
            Drawable drawable = _drawable;
            _drawable = null;
            return drawable;
        }

        /// <summary>
        /// Get this GameObject's drawable
        /// </summary>
        /// <returns>GameObject's drawable</returns>
        public Drawable getDrawable()
        {
            return _drawable;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Get / Set this GameObject's drawing color.
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Updates this GameObject's isometric (viewing) direction, according to its
        /// direction vector and the GameLevel's camera's viewing angle.
        /// This function should be called any time the GameObject changes direction,
        /// or any time the camera changing viewing angle.
        /// </summary>
        protected void updateDirection()
        {
            // Grab the cosine of our direction vector and the x axis
            Vector2 dir     = new Vector2(_vecDirection.X, _vecDirection.Z);
            dir             = Vector2.Normalize(dir);
            float cosTheta  = Vector2.Dot(dir, Vector2.UnitX);

            // Determine what way this GameObject is facing
            if (cosTheta < -0.707f)
                _isoDirection = Direction.DIR_NW;
            else if (cosTheta > 0.707f)
                _isoDirection = Direction.DIR_SE;
            else if (dir.Y > 0)
                _isoDirection = Direction.DIR_SW;
            else
                _isoDirection = Direction.DIR_NE;
        }

        /// <summary>
        /// Find out which parent tile node should be holding onto this node.
        /// If it needs to change, then this function will change it.
        /// </summary>
        protected void grabParentTile()
        {
            if (_parent == null)
                return;

            GameNode parentTile = _gameLevelMgr.getTileFromIsometricCoordinates(_positionIsometric);

            if (parentTile == null)
                return;

            if (parentTile == _parent)
                return;

            _parent.detachChildNode(_name);
            parentTile.attachChildNode(this);
        }
    }
}
