using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using GameData;

namespace GDD2Project1
{
    public enum Direction
    {
        DIR_N,
        DIR_NE,
        DIR_E,
        DIR_SE,
        DIR_S,
        DIR_SW,
        DIR_W,
        DIR_NW,
        DIR_COUNT
    };

    public class GameLevelManager
    {
        public      const       String          PLAYER_NAME             = "playercharacter";

        public      const       int             UNIT_SIZE = 16;
        private     const       String          TILE_NAME_PREFIX        = "tile_";
        private     const       char            TILE_ROW_PREFIX         = 'r';
        private     const       char            TILE_COL_PREFIX         = 'c';
        public      const       int             TILE_SIZE               = 84;
        protected               GameTile[,]     _tiles;
        protected               int             _tileRows;
        protected               int             _tileCols;


        protected   GameContentManager                  _gameContentMgr;
        protected   Dictionary<String, GameObject>      _gameObjs;
        protected   Camera2D                            _camera;
        protected   Drawable                            _debugDrawable;


        //-------------------------------------------------------------------------
        public Camera2D Camera { get { return _camera; } }


        //-------------------------------------------------------------------------
        /// <summary>
        /// GameLevelManager constructor.
        /// </summary>
        /// <param name="gameContentMgr">Handles loading and saving of game data.</param>
        /// <param name="graphicsDevice">Provides values for screen dimensions.</param>
        public GameLevelManager(GameContentManager gameContentMgr, GraphicsDevice graphicsDevice)
        {
            // Initialize game content handler
            _gameContentMgr = gameContentMgr;

            // Initialize camera
            _camera = new Camera2D(this);
            _camera.Origin = new Vector2(
                graphicsDevice.Viewport.Width / 2,
                graphicsDevice.Viewport.Height / 2);

            // Initialize game objects
            _gameObjs = new Dictionary<string, GameObject>();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Initialize the 2D tile array, placing tiles in correct locations.
        /// </summary>
        /// <param name="rows">Number of rows</param>
        /// <param name="cols">Number of columns</param>
        public void initLevel(int rows, int cols)
        {
            // Initialize array size
            _tiles      = new GameTile[rows, cols];
            _tileRows   = rows;
            _tileCols   = cols;

            // Populate tile array
            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                {
                    String tileName =
                        TILE_NAME_PREFIX
                        + TILE_ROW_PREFIX + row
                        + TILE_COL_PREFIX + col;

                    GameNode tileNode   = new GameNode(this, tileName);
                    GameTile tile       = new GameTile(tileName, tileNode);
                    _tiles[row, col]    = tile;

                    tileNode.PositionIsometric = new Vector3(
                        row * TILE_SIZE,
                        0.0f,
                        col * TILE_SIZE);
                }

            // Set Camera's origin to the center of the level
            Vector2 halfpoint;
            halfpoint.X = _tileRows * TILE_SIZE / 2;
            halfpoint.Y = _tileCols * TILE_SIZE / 2;
            _camera.OriginIsometric = halfpoint;
        }

        /// <summary>
        /// Create a new level.
        /// </summary>
        /// <param name="rows">Number of rows.</param>
        /// <param name="cols">Number of columns.</param>
        /// <param name="drawable">Default tile drawable.</param>
        public void newLevel(int rows, int cols, String drawable)
        {
            // Initialize level
            initLevel(rows, cols);

            // Load default drawable
            Drawable tileDrawable = _gameContentMgr.loadDrawable(drawable);

            // Attach entities to tiles
            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                    _tiles[row, col].Entity = new Entity(tileDrawable);
        }

        /// <summary>
        /// Save the current state of this GameLevel as an XML file.
        /// </summary>
        /// <param name="path">File path to save this GameLevel.</param>
        public void saveLevel(String path)
        {
            // XML writer settings for serialization
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            // GameLevelData to hold state data
            GameLevelData data = new GameLevelData();

            // Save GameTiles
            data.NumRows    = _tileRows;
            data.NumCols    = _tileCols;
            data.Tiles      = new GameTileData[_tileRows * _tileCols];
            for (int row = 0; row < _tileRows; row++)
                for (int col = 0; col < _tileCols; col++)
                    data.Tiles[row * _tileCols + col] = _tiles[row, col].save();

            // Save GameObjects
            data.GameObjs = new GameObjectData[_gameObjs.Count];
            int i = 0;
            foreach (KeyValuePair<String, GameObject> entry in _gameObjs)
            {
                data.GameObjs[i] = entry.Value.save();
                i++;
            }

            // Write XML file
            using(XmlWriter writer = XmlWriter.Create(path + ".xml", settings))
                IntermediateSerializer.Serialize(writer, data, null);

            // Print success message to console
            Console.WriteLine("Current level saved to: " + path);
        }

        /// <summary>
        /// Load a GameLevel from an XML file.
        /// </summary>
        /// <param name="data">Serialized GameLevelData XML file.</param>
        public void loadLevel(String path)
        {
            // Load GameLevel data
            GameLevelData data = _gameContentMgr.loadLevelData(path);

            // Initialize level
            initLevel(data.NumRows, data.NumCols);

            // Load all GameTiles
            for (int row = 0; row < _tileRows; row++)
                for (int col = 0; col < _tileCols; col++)
                {
                    GameTileData    tileData        = data.Tiles[row * _tileCols + col];
                    GameTile        tile            = _tiles[row, col];
                    Entity          tileEntity      = new Entity(_gameContentMgr.loadDrawable(tileData.Drawable));
            
                    tile.Entity     = tileEntity;
                    tile.Active     = tileData.Active;
                    tile.Node.translate(0.0f, tileData.Elevation, 0.0f);
                }

            // Load all GameObjects
            foreach (GameObjectData gobjData in data.GameObjs)
            {
                GameObject  gobj        = null;

                switch (gobjData.ObjType)
                { 
                    case 0:     // GameObject
                        gobj = createGameObject<GameObject>(gobjData.Name, gobjData.Drawable);
                        break;

                    case 1:     // GameObjectMovable
                        gobj = createGameObject<GameObjectMovable>(gobjData.Name, gobjData.Drawable);
                        break;

                    case 2:     // GameCharacter
                        gobj = createGameObject<GameCharacter>(gobjData.Name, gobjData.Drawable);
                        break;
                }

                tileAtIndex(0, 0).Node.attachChildNode(gobj.Node);
                gobj.Node.translateTo(gobjData.Position);

                gobj.DirectionVector    = gobjData.Direction;
                gobj.Active             = gobjData.Active;
            }

            Console.WriteLine("Level loaded: " + path);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Apply one frame update to the entire GameLevel.
        /// </summary>
        /// <param name="dt">Precomputed delta time.</param>
        public void update(float dt)
        {
            // Update camera
            _camera.update(dt);

            // Perform all GameObject updates
            foreach (KeyValuePair<String, GameObject> entry in _gameObjs)
                entry.Value.update(null, dt);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Draw the entire GameLevel
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used for rendering</param>
        public void draw(SpriteBatch spriteBatch, float dt)
        {
            // Begin drawing according to this level's camera
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                null,
                null,
                null,
                null,
                _camera.Transformation);

            // Draw the array of cells, including their contents
            drawGraph(spriteBatch, dt);

            // End drawing
            spriteBatch.End();
        }

        /// <summary>
        /// Pass through all of the Root GameNode's children, drawing them in order
        /// according to the direction the camera is currently viewing. As this functions
        /// iterates through each GameNode, that node is drawn, and any of its children are drawn
        /// on top of it.
        /// </summary>
        protected void drawGraph(SpriteBatch spriteBatch, float dt)
        {
            switch (_camera.Dir)
            { 
                case Direction.DIR_N:
                case Direction.DIR_NE:
                    for (int col = 0; col < _tileCols; col++)
                        for (int row = _tileRows - 1; row >= 0; row--)
                            drawNode(spriteBatch, _tiles[row, col].Node, dt, true);
                    break;

                case Direction.DIR_E:
                case Direction.DIR_SE:
                    for (int row = _tileRows - 1; row >= 0; row--)
                        for (int col = _tileCols - 1; col >= 0; col--)
                            drawNode(spriteBatch, _tiles[row, col].Node, dt, true);
                    break;

                case Direction.DIR_S:
                case Direction.DIR_SW:
                    for (int col = _tileCols - 1; col >= 0; col--)
                        for (int row = 0; row < _tileRows; row++)
                            drawNode(spriteBatch, _tiles[row, col].Node, dt, true);
                    break;

                case Direction.DIR_W:
                case Direction.DIR_NW:
                    for (int row = 0; row < _tileRows; row++)
                        for (int col = 0; col < _tileCols; col++)
                            drawNode(spriteBatch, _tiles[row, col].Node, dt, true);
                    break;
            }
        }

        /// <summary>
        /// Issue a call to draw on a GameNode.
        /// </summary>
        /// <param name="node">GameNode to draw.</param>
        /// <param name="recursive">If true, all of this nodes children will be drawn.</param>
        protected void drawNode(SpriteBatch spriteBatch, GameNode node, float dt, bool recursive = true)
        {
            if (node == null)
                return;

            node.draw(spriteBatch, null, dt);

            if (recursive)
                foreach (KeyValuePair<String, GameNode> childEntry in node.Children)
                    drawNode(spriteBatch, childEntry.Value, dt, true);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Get tile GameNode at index.
        /// </summary>
        /// <param name="row">Row index</param>
        /// <param name="col">Column index</param>
        /// <returns></returns>
        public GameTile tileAtIndex(int row, int col)
        {
            if (    row < 0 || row >= _tileRows
                ||  col < 0 || col >= _tileCols)
                return null;

            return _tiles[row, col];
        }

        /// <summary>
        /// Get a tile GameNode from isometric coordinates.
        /// </summary>
        /// <param name="isoCoords">Isometric GameLevel coordinates.</param>
        /// <returns>Tile GameNode nearest the provided coordinates.</returns>
        public GameTile tileAtIsoCoords(Vector3 isoCoords)
        {
            // Map to graph indices
            isoCoords /= (float)TILE_SIZE;

            // Round to nearest index
            isoCoords.X = (int)Math.Round(isoCoords.X);
            isoCoords.Z = (int)Math.Round(isoCoords.Z);

            return tileAtIndex((int)isoCoords.X, (int)isoCoords.Z);
        }

        /// <summary>
        /// Get a tile GameNode and its index from isometric coordinates.
        /// </summary>
        /// <param name="isoCoords">Isometric GameLevel coordinates.</param>
        /// <param name="row">Outputs GameNode's row index.</param>
        /// <param name="col">Outputs GameNode's column index.</param>
        /// <returns></returns>
        public GameTile tileAtIsoCoords(Vector3 isoCoords, out int row, out int col)
        {
            // Map to graph indices
            isoCoords /= (float)TILE_SIZE;

            // Round to nearest index
            isoCoords.X = (int)Math.Round(isoCoords.X);
            isoCoords.Z = (int)Math.Round(isoCoords.Z);

            // Output row and col
            row = (int)isoCoords.X;
            col = (int)isoCoords.Z;

            return tileAtIndex(row, col);
        }

        /// <summary>
        /// Get a tile GameNode from screen coordinates.
        /// </summary>
        /// <param name="screenCoords">Screen coordinates.</param>
        /// <returns>Tile GameNode at the specified screen coordinates.</returns>
        public GameTile tileAtScreenCoords(Point screenCoords)
        {
            // TODO: needs to be able to grab elevated tiles

            // Generate coordinate vector so we can transform
            Vector2 vecScreenCoords = new Vector2(
                screenCoords.X,
                screenCoords.Y);

            // Transform into isometric coordinates
            vecScreenCoords = _camera.screenToIsometric(vecScreenCoords);

            // Coordinates as Vector3 so we can grab the index
            Vector3 isoCoords = new Vector3(
                vecScreenCoords.X,
                0.0f,
                vecScreenCoords.Y);

            return tileAtIsoCoords(isoCoords);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Creates a new GameObject and inserts it into our GameLevel.
        /// </summary>
        /// <typeparam name="T">Type of GameObject.</typeparam>
        /// <param name="name">Name of GameObject.</param>
        /// <param name="drawable">GameObject's drawable.</param>
        /// <returns>Newly created GameObject.</returns>
        public T createGameObject<T>(String name, String drawable)
            where T : GameObject
        {
            GameNode gobjNode = new GameNode(this, name);
            Drawable gobjDrawable = null;

            if (typeof(T) == typeof(GameObject) || typeof(T) == typeof(GameObjectMovable))
                gobjDrawable = _gameContentMgr.loadDrawable(drawable);
            else if (typeof(T) == typeof(GameCharacter))
                gobjDrawable = _gameContentMgr.loadDrawableAnimated(drawable);

            gobjNode.attachEntity(new Entity(gobjDrawable));
            T gobj = (T)Activator.CreateInstance(typeof(T), new object[] { name, gobjNode });

            _gameObjs.Add(gobj.Name, gobj);

            return gobj;
        }

        /// <summary>
        /// Return a GameObject previously added to this GameLevel
        /// </summary>
        /// <typeparam name="T">Type of GameObject</typeparam>
        /// <param name="name">Name of object previously created</param>
        /// <returns>Object corresponding to name</returns>
        public T getGameObject<T>(String name)
            where T : GameObject
        {
            // If the GameObject was never created, return null
            if (!_gameObjs.ContainsKey(name))
                return null;

            // If the GameObject is not of the type specified, return null
            if (!(_gameObjs[name] is T))
                return null;

            return _gameObjs[name] as T;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Get isometric direction from a specified rotation.
        /// </summary>
        /// <param name="radians">Rotation amount.</param>
        /// <returns>Isometric direction</returns>
        public static Direction directionViewFromAngle(float radians)
        {
            radians     +=  (float)Math.PI * 0.125f;
            float offs  =   (float)Math.PI * 0.25f;

            if (radians < offs * 1
                || radians > offs * 8)      return Direction.DIR_N;
            else if (radians < offs * 2)    return Direction.DIR_NW;
            else if (radians < offs * 3)    return Direction.DIR_W;
            else if (radians < offs * 4)    return Direction.DIR_SW;
            else if (radians < offs * 5)    return Direction.DIR_S;
            else if (radians < offs * 6)    return Direction.DIR_SE;
            else if (radians < offs * 7)    return Direction.DIR_E;
            else                            return Direction.DIR_NE;
        }

        /// <summary>
        /// Get an isometric vector pointing in the indicated view direction.
        /// </summary>
        /// <param name="dir">Direction to point towards.</param>
        /// <returns>Direction vector.</returns>
        public static Vector3 directionVectorFromView(Direction dir)
        {
            Vector3 v = Vector3.Zero;

            switch (dir)
            {
                case Direction.DIR_N:   v.Z = -1.0f;                    break;
                case Direction.DIR_NE:  v.X = 1.0f;     v.Z = -1.0f;    break;
                case Direction.DIR_E:   v.X = 1.0f;                     break;
                case Direction.DIR_SE:  v.X = 1.0f;     v.Z = 1.0f;     break;
                case Direction.DIR_S:   v.Z = 1.0f;                     break;
                case Direction.DIR_SW:  v.X = -1.0f;    v.Z = 1.0f;     break;
                case Direction.DIR_W:   v.X = -1.0f;                    break;
                case Direction.DIR_NW:  v.X = -1.0f;    v.Z = -1.0f;    break;
            }

            return v;
        }
    }
}
