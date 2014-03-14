using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameData;

namespace GDD2Project1
{
    /// <summary>
    /// GameTile
    /// 
    /// Represents a tile within the GameLevel.
    /// </summary>
    public class GameTile : GameObject
    {


        //-------------------------------------------------------------------------
        /// <summary>
        /// GameTile constructor.
        /// </summary>
        /// <param name="node">This GameTile's node.</param>
        public GameTile(String name, GameNode node)
            : base(name, node)
        { 
        
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Save and return state data.
        /// </summary>
        /// <returns>Tile state data.</returns>
        public virtual new GameTileData save()
        {
            GameTileData data   = new GameTileData();
            data.Drawable       = _entityObj.Drawable.Name;
            data.Elevation      = _node.PositionIsometric.Y;
            data.Active         = _active;

            return data;
        }
    }
}
