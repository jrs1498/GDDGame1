using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GDD2Project1
{
    /// <summary>
    /// This class represents any items that may be consumed by a character.
    /// </summary>
    class Consumable : GameObject
    {
        protected ConsumableType    _type;
        protected int               _amount;

        //-------------------------------------------------------------------------
        /// <summary>
        /// A consumable's type represents how it will respond to an event.
        /// </summary>
        public enum ConsumableType
        {
            TYPE_POWER,
            TYPE_HEALTH
        };


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default Consumable constructor
        /// </summary>
        /// <param name="gameLevelMgr">GameLevelManager containing this node</param>
        /// <param name="name">This Consumable's name</param>
        public Consumable(GameLevelManager gameLevelMgr, String name, ConsumableType type, int amount)
            : base(gameLevelMgr, name)
        {
            _type = type;
            _amount = amount;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Amount is an arbitrary value, which only has meaning in context of the
        /// ConsumableType. This can represent power, health, money, etc.
        /// </summary>
        public int setAmount
        {
            set { _amount = value; }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// This function is fired whenever one of this node's subscriptions attaches a child node.
        /// </summary>
        /// <param name="sender">Node which attached a child</param>
        /// <param name="child">The child which was attached</param>
        /// <param name="e">Event args</param>
        protected override void parentAttachedChild(GameNode sender, GameNode child, EventArgs e)
        {
            base.parentAttachedChild(sender, child, e);

            switch(_type)
            {
                case ConsumableType.TYPE_POWER:
                    Console.WriteLine("Player just received " + _amount + " power");
                    break;

                case ConsumableType.TYPE_HEALTH:
                    Console.WriteLine("Player just received a health consumable! " + _amount + " health");
                    break;
            }

            _gameLevelMgr.destroyNode(this);
        }

        /// <summary>
        /// This function is fired whenever one of this node's subscriptions detaches a child node.
        /// </summary>
        /// <param name="sender">Node which detached a child</param>
        /// <param name="child">The child which was attached</param>
        /// <param name="e">Event args</param>
        protected override void parentDetachedChild(GameNode sender, GameNode child, EventArgs e)
        {
            base.parentDetachedChild(sender, child, e);
        }
    }
}
