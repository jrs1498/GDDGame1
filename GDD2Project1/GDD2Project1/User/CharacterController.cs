using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputEventSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDD2Project1
{
    public class CharacterController : ActorController
    {
        List<GameObject> _adjTiles;
        GameObject _hoveringTile;

        //-------------------------------------------------------------------------
        /// <summary>
        /// Default CharacterController constructor.
        /// </summary>
        /// <param name="character">GameCharacter to be controlled</param>
        public CharacterController(GameCharacter character, String name)
            : base(character, name)
        {
            _adjTiles = new List<GameObject>();
        }


        //-------------------------------------------------------------------------
        public override void update(float dt)
        {
            foreach (GameObject tile in _adjTiles)
                if(_hoveringTile != null && tile != _hoveringTile)
                    tile.Color = Color.White;

            _adjTiles = (_actor as GameCharacter).GameLevelMgr.getAdjacentTiles(
                (_actor as GameCharacter).PositionIsometric, 200.0f);

            foreach (GameObject tile in _adjTiles)
                if (_hoveringTile != null && tile != _hoveringTile)
                    tile.Color = Color.Green;
        }


        //-------------------------------------------------------------------------
        public override bool injectMouseDown(MouseEventArgs e)
        {
            switch (e.Button)
            { 
                case MouseButtons.Left:
                    GameObject tile = _actor.GameLevelMgr.getTileFromScreenCoordinates(e.Position);
                    if (tile == null || !_adjTiles.Contains(tile))
                        return false;

                    (_actor as GameCharacter).setDestination(tile);
                    return true;

                default:
                    break;
            }

            return base.injectMouseDown(e);
        }


        //-------------------------------------------------------------------------
        public override bool injectMouseMove(MouseEventArgs e)
        {
            _hoveringTile = _actor.GameLevelMgr.getTileFromScreenCoordinates(e.Position);
            if (_adjTiles.Contains(_hoveringTile))
                _hoveringTile.Color = Color.Blue;

            return base.injectMouseMove(e);
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
