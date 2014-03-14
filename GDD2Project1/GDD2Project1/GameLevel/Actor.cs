using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GDD2Project1
{
    /// <summary>
    /// Actor class represents anything with a position and orientation. This is not to say,
    /// however, that an actor class is a visible GameObject. A Camera, for example, needs
    /// to know only of its position.
    /// </summary>
    public class Actor
    {
        protected GameLevelManager      _gameLevelMgr;
        protected Vector2               _position;
        protected Vector2               _scale;
        protected float                 _rotation;


        //-------------------------------------------------------------------------
        public virtual      GameLevelManager        GameLevelMgr        { get { return _gameLevelMgr; } }
        public virtual      Vector2                 Position            { get { return _position; } }
        public virtual      Vector2                 Scale               { get { return _scale; } }
        public virtual      float                   Rotation            { get { return _rotation; } }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default GameObject constructor
        /// </summary>
        public Actor(GameLevelManager gameLevelMgr)
        {
            _gameLevelMgr   = gameLevelMgr;
            _position       = Vector2.Zero;
            _scale          = new Vector2(1.0f);
            _rotation       = 0.0f;
        }
    }
}
