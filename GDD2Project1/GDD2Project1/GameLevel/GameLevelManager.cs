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
        DIR_NE = 0,
        DIR_SE = 1,
        DIR_SW = 2,
        DIR_NW = 3,
        DIR_COUNT = 4
    };

    public class GameLevelManager
    {
        protected const String      TILE_NAME_PREFIX    = "tile_";
        protected const char        TILE_ROW_PREFIX     = 'r';
        protected const char        TILE_COL_PREFIX     = 'c';
        protected const int         TILE_SIZE = 84;
        protected GameObject[,]     _tiles;
        protected int               _tileRows;
        protected int               _tileCols;
        protected Vector2           _tileOrigin;

        protected GameContentManager _gameContentMgr;

        protected Camera2D          _camera;

        protected Dictionary<String, GameCharacter>     _characters;


        //-------------------------------------------------------------------------
        public int TileSize { get { return TILE_SIZE; } }

        public Camera2D Camera { get { return _camera; } }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default GameLevelManager constructor
        /// </summary>
        /// <param name="contentManager">ContentManager used for loading game data</param>
        public GameLevelManager(
            GameContentManager  gameContentMgr,
            GraphicsDevice      graphicsDevice)
        {
            _gameContentMgr = gameContentMgr;

            // Initialize camera
            _camera = new Camera2D(this);
            _camera.Origin = new Vector2(
                graphicsDevice.Viewport.Width / 2,
                graphicsDevice.Viewport.Height / 2);

            // Initialize dictionaries
            _characters = new Dictionary<string, GameCharacter>();

            // Initialize tile origin
            _tileOrigin = new Vector2(
                (float)Math.Sqrt(TILE_SIZE * TILE_SIZE) / 2,
                (TILE_SIZE * _camera.ScaleY));
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Save this GameLevel to the specified filename at the specified directory, relative to
        /// the current working directory.
        /// </summary>
        /// <param name="directory">Directory relative to CWD</param>
        /// <param name="filename">Filename to save as</param>
        public void saveLevel(String directory, String filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            GameLevelData data = new GameLevelData();
            data.NumRows = _tileRows;
            data.NumCols = _tileCols;
            data.Tiles = new GameObjectData[_tileRows * _tileCols];

            Func<GameObject, GameObjectData> save_obj = (GameObject gameObj) =>
                { return gameObj.saveGameObject(); };

            for (int x = 0; x < _tileRows; x++)
                for (int y = 0; y < _tileCols; y++)
                    data.Tiles[x * _tileCols + y] = save_obj(_tiles[x, y]);

            // Write the level
            using (XmlWriter writer = XmlWriter.Create(directory + filename + ".xml", settings))
                IntermediateSerializer.Serialize(writer, data, null);

            // Write a line to console
            Console.WriteLine("saved level " + directory + filename);
        }

        /// <summary>
        /// Load a GameLevel from specified file
        /// </summary>
        /// <param name="directory">Directory containing file</param>
        /// <param name="filename">Filename of level</param>
        public void loadLevel(String directory, String filename)
        {
            GameLevelData lvldata = _gameContentMgr.loadLevelData(directory + filename);
            initTileArray(lvldata.NumRows, lvldata.NumCols);

            // Pass through each tile, attaching drawables and applying elevation
            for (int row = 0; row < lvldata.NumRows; row++)
                for (int col = 0; col < lvldata.NumCols; col++)
                {
                    GameObjectData objData  = lvldata.Tiles[row * _tileCols + col];
                    Drawable tileDrwble     = _gameContentMgr.loadDrawable(objData.Drawable);
                    _tiles[row, col].attachDrawable(tileDrwble);
                    _tiles[row, col].translate(0.0f, objData.PositionIsometric.Y, 0.0f);
                }

            addTestContent();
        }

        /// <summary>
        /// Create a new level
        /// </summary>
        /// <param name="rows">Number of tile rows</param>
        /// <param name="cols">Number of tile columns</param>
        /// <param name="drawable">Default tile drawable texture</param>
        public void newLevel(int rows, int cols, String drawable)
        {
            initTileArray(rows, cols);

            Drawable tileDrwble = _gameContentMgr.loadDrawable(drawable);
            tileDrwble.Origin   = _tileOrigin;

            foreach (GameObject tile in _tiles)
                tile.attachDrawable(tileDrwble);
        }

        /// <summary>
        /// Initialize the 2D tile array, placing tiles in correct locations
        /// </summary>
        /// <param name="rows">Number of rows</param>
        /// <param name="cols">Number of columns</param>
        public void initTileArray(int rows, int cols)
        {
            _tiles = new GameObject[rows, cols];
            _tileRows = rows;
            _tileCols = cols;

            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                {
                    String tileName = ("r" + cols + "c" + rows);
                    GameObject tile = new GameObject(this, tileName);

                    tile.translate(
                        row * TILE_SIZE,
                        0.0f,
                        col * TILE_SIZE
                        );

                    _tiles[row, col] = tile;
                }
        }

        private void addTestContent()
        { 
            Drawable tree = _gameContentMgr.loadDrawable("tree1");

            GameObject obj1 = new GameObject(this, "obj1");
            GameObject obj2 = new GameObject(this, "obj2");

            obj1.attachDrawable(tree);
            obj2.attachDrawable(tree);

            GameObject tile1 = getTileAtIndex(5, 5);
            GameObject tile2 = getTileAtIndex(8, 8);

            tile1.attachChildNode(obj1);
            tile2.attachChildNode(obj2);

            obj1.PositionIsometric = tile1.PositionIsometric;
            obj2.PositionIsometric = tile2.PositionIsometric;

            tile1.Color = tile2.Color = Color.Green;



            DrawableAnimated player = _gameContentMgr.loadDrawableAnimated("playercharacter");

            GameCharacter character = new GameCharacter(this, "playercharacter");
            character.attachDrawable(player);
            _characters.Add(character.getName, character);

            GameObject tile3 = getTileAtIndex(0, 0);
            tile3.attachChildNode(character);

            character.PositionIsometric = tile3.PositionIsometric;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Apply one update to the entire GameLevel
        /// </summary>
        /// <param name="gameTime">Used to determine time delta</param>
        public void update(float dt)
        {
            // Update camera
            _camera.update(dt);

            // Update all character movement
            foreach (KeyValuePair<String, GameCharacter> entry in _characters)
                entry.Value.applyDisplacement(dt);
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
                case Direction.DIR_NE:
                    for (int row = 0; row < _tileRows; row++)
                        for (int col = 0; col < _tileCols; col++)
                            drawNode(spriteBatch, _tiles[row, col], dt, true);
                    break;

                case Direction.DIR_SE:
                    for (int col = _tileCols - 1; col >= 0; col--)
                        for (int row = 0; row < _tileRows; row++)
                            drawNode(spriteBatch, _tiles[row, col], dt, true);
                    break;

                case Direction.DIR_SW:
                    for (int row = _tileRows - 1; row >= 0; row--)
                        for (int col = _tileCols - 1; col >= 0; col--)
                            drawNode(spriteBatch, _tiles[row, col], dt, true);
                    break;

                case Direction.DIR_NW:
                    for (int col = 0; col < _tileCols; col++)
                        for (int row = _tileRows - 1; row >= 0; row--)
                            drawNode(spriteBatch, _tiles[row, col], dt, true);
                    break;
            }
            
        }

        /// <summary>
        /// Draws a GameNode's Drawable, if it contains one
        /// </summary>
        /// <param name="node">GameNode to draw</param>
        /// <param name="recursive">If true, all of this nodes children will be drawn</param>
        protected void drawNode(SpriteBatch spriteBatch, GameNode node, float dt, bool recursive = true)
        {
            if (node == null)
                return;

            if(!(node is GameObject))
                return;

            (node as GameObject).drawContents(spriteBatch, dt);

            if (recursive)
                foreach (KeyValuePair<String, GameNode> childEntry in node.getChildren)
                    drawNode(spriteBatch, childEntry.Value, dt, true);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Returns a previously created GameNode  through a specified index.
        /// The GameNode returned must have been previously created by the loadLevel
        /// function, and the index must be within the bounds of the GameLevel.
        /// </summary>
        /// <param name="x">X index</param>
        /// <param name="y">Y index</param>
        /// <returns>Game node at corresponding index</returns>
        public GameObject getTileAtIndex(int x, int y)
        {
            if (    x < 0 || x >= _tileRows
                ||  y < 0 || y >= _tileCols)
                return null;

            return _tiles[x, y];
        }

        /// <summary>
        /// Retrieve a tile GameNode from this level based on the
        /// specified isometric coordinates.
        /// </summary>
        /// <param name="isoCoords">Isometric coordinates</param>
        /// <returns>Tile GameNode at position</returns>
        public GameObject getTileFromIsometricCoordinates(Vector3 isoCoords)
        {
            // Map to graph indices
            isoCoords /= (float)TILE_SIZE;

            // Drop the fractional portion
            isoCoords.X -= isoCoords.X % 1.0f;
            isoCoords.Z -= isoCoords.Z % 1.0f;

            return getTileAtIndex((int)isoCoords.X, (int)isoCoords.Z);
        }

        /// <summary>
        /// Returns a node corresponding to the specified screen coordinates
        /// </summary>
        /// <param name="coordinates">Screen coordinates</param>
        /// <returns>GameNode at the location specified</returns>
        public GameObject getTileFromScreenCoordinates(Vector2 coordinates)
        {
            // TODO: needs to be able to grab elevated tiles

            coordinates = _camera.screenToIsometric(coordinates);
            Vector3 isoCoords = new Vector3(
                coordinates.X,
                0.0f, 
                coordinates.Y
                );

            return getTileFromIsometricCoordinates(isoCoords);
        }

        /// <summary>
        /// Returns a node corresponding to the specified screen coordinates
        /// </summary>
        /// <param name="coordinates">Screen coordinates</param>
        /// <returns>GameNode at the location specified</returns>
        public GameObject getTileFromScreenCoordinates(Point coordinates)
        {
            Vector2 vecCoordinates;
            vecCoordinates.X = coordinates.X;
            vecCoordinates.Y = coordinates.Y;
            return getTileFromScreenCoordinates(vecCoordinates);
        }

        /// <summary>
        /// Get the x and y index of a tile at the specified coordinates
        /// </summary>
        /// <param name="isoCoords">Isometric coordinates</param>
        /// <returns></returns>
        public Point getIndexFromPosition(Vector3 isoCoords)
        {
            isoCoords /= (float)TILE_SIZE;

            isoCoords.X -= isoCoords.X % 1.0f;
            isoCoords.Z -= isoCoords.Y % 1.0f;

            Point index;
            index.X = (int)isoCoords.X;
            index.Y = (int)isoCoords.Z;

            return index;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Remove a GameNode from this GameLevel
        /// </summary>
        /// <param name="node">Node to remove</param>
        public void destroyNode(GameNode node)
        {
            // Detach the node from its parent
            if (node.getParent != null)
                node.getParent.detachChildNode(node.getName);

            // If it's a Character, remove it from the dictionary
            if (node is GameCharacter)
                _characters.Remove(node.getName);
        }


        //-------------------------------------------------------------------------
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
            if (!_characters.ContainsKey(name))
                return null;

            // If the GameObject is not of the type specified, return null
            if (!(_characters[name] is T))
                return null;

            return _characters[name] as T;
        }
    }
}
