using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputEventSystem;
using Microsoft.Xna.Framework;

namespace GDD2Project1
{
    public class CharacterController : ActorController
    {


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default CharacterController constructor.
        /// </summary>
        /// <param name="character">GameCharacter to be controlled</param>
        public CharacterController(GameCharacter character, String name)
            : base(character, name)
        { 
            
        }


        //-------------------------------------------------------------------------
        public override void injectMouseDown(MouseEventArgs e)
        {
            switch (e.Button)
            { 
                case MouseButtons.Left:
                    GameObject tile = _actor.GameLevelMgr.getTileFromScreenCoordinates(e.Position);
                    if (tile == null)
                        return;

                    (_actor as GameCharacter).setDestination(tile);
                    tile.Color = Color.Green;
                    break;
            }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Shorthand function to return the inherited actor as a GameCharacter
        /// </summary>
        /// <returns>GameCharacter controlled by this controller</returns>
        protected virtual GameCharacter getCharacter()
        {
            return _actor as GameCharacter;
        }

        /// <summary>
        /// Shorthand function returning the controlled GameCharacter's
        /// GameLevelManager
        /// </summary>
        /// <returns>GameLevelManager containing the GameCharacter</returns>
        protected virtual GameLevelManager getGameLevelMgr()
        {
            return getCharacter().GameLevelMgr;
        }
    }
}
