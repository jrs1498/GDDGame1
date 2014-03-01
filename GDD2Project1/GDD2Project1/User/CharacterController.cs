using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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


        ////-------------------------------------------------------------------------
        ///// <summary>
        ///// Primary input functionality. This is where the User may control the GameCharacter
        ///// </summary>
        //public override void pollInput()
        //{
        //    if (InputManager.GetOneLeftClickDown())
        //    {
        //        GameNode clickedNode =
        //            getGameLevelMgr().getTileFromScreenCoordinates(InputManager.GetMouseLocation());
        //        if (clickedNode != null)
        //            getCharacter().setDestination(clickedNode);
        //    }
        //}
    }
}
