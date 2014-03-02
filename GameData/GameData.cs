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
        public int numRows;
        public int numCols;

        public GameLevelData(int rows, int cols)
        {
            numRows = rows;
            numCols = cols;
        }
    }


}
