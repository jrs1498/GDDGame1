using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GDD2Project1
{
    class Player
    {
        protected GameCharacter     _character;
        protected GameLevelManager  _gameLevelMgr;

        public Player(GameLevelManager gameLevelMgr, GameCharacter character)
        {
            _gameLevelMgr   = gameLevelMgr;
            _character      = character;
            
        }

        public void pollInput()
        {
            if (InputManager.GetOneLeftClickDown())
            {
                GameNode clickedNode =
                    _gameLevelMgr.getNodeFromScreenCoordinates(InputManager.GetMouseLocation());
                if (clickedNode != null)
                    _character.setDestination(clickedNode);
            }
        }
    }
}
