using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameData
{
    //-------------------------------------------------------------------------
    public class GameLevelData
    {
        public int              NumRows;
        public int              NumCols;
        public GameTileData[]   Tiles;
        public GameObjectData[] GameObjs;

        public GameLevelData()
        { }
    }

    //-------------------------------------------------------------------------
    public class GameObjectData
    {
        public  int             ObjType;
        public  String          Name;
        public  String          Drawable;
        public  Vector3         Position;
        public  Vector3         Direction;
        public  bool            Active;

        public GameObjectData()
        { }
    }

    //-------------------------------------------------------------------------
    public class GameTileData
    {
        public  String          Drawable;
        public  float           Elevation;
        public  bool            Active;

        public GameTileData()
        { }
    }
}
