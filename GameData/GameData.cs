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
    public class GameLevelData
    {
        public int NumRows;
        public int NumCols;
        public GameObjectData[] Tiles;

        public GameLevelData()
        { }
    }


    /// <summary>
    /// Class used for saving a GameNode
    /// </summary>
    public class GameNodeData
    {
        public String Name;
        public Vector3 PositionIsometric;

        public GameNodeData()
        { }
    }


    /// <summary>
    /// Class used for saving a GameObject
    /// </summary>
    public class GameObjectData : GameNodeData
    {
        public String Drawable;

        public GameObjectData()
        { }
    }


}
