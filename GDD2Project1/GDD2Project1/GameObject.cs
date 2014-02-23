using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GDD2Project1
{
    class GameObject
    {
        protected GameLevelManager _gameLevelMgr;

        protected Vector2
            _position,
            _origin;


        //-------------------------------------------------------------------------
        public virtual GameLevelManager GameLevelMgr
        {
            get { return _gameLevelMgr; }
        }

        public virtual Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public virtual Vector2 Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Common constructor code. All constructors should call this method.
        /// </summary>
        /// <param name="position">This GameObject's position</param>
        /// <param name="origin">GameObject's origin and pivot point</param>
        private void construct(GameLevelManager gameLevelMgr, Vector2 position, Vector2 origin)
        {
            _gameLevelMgr   = gameLevelMgr;
            _position       = position;
            _origin         = origin;
        }

        /// <summary>
        /// Default GameObject constructor
        /// </summary>
        public GameObject(GameLevelManager gameLevelMgr)
        {
            construct(gameLevelMgr, Vector2.Zero, Vector2.Zero);
        }

        /// <summary>
        /// Create a GameObject with specified position and origin
        /// </summary>
        /// <param name="position">GameObject's position</param>
        /// <param name="origin">GameObject's origin</param>
        public GameObject(GameLevelManager gameLevelMgr, Vector2 position, Vector2 origin)
        {
            construct(gameLevelMgr, position, origin);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Adjust this GameObject's position by some specified amount
        /// </summary>
        /// <param name="amount">Amount to translate</param>
        public virtual void translate(Vector2 amount)
        {
            _position += amount;
        }

        /// <summary>
        /// Adjust this GameObject's position by some specified amount
        /// </summary>
        /// <param name="x">X translation</param>
        /// <param name="y">Y translation</param>
        public virtual void translate(float x, float y)
        {
            translate(new Vector2(x, y));
        }
    }
}
