using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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

    class GameLevelManager
    {
        protected ContentManager    _contentManager;

        protected Camera2D          _camera;
        protected CameraController  _cameraController;

        protected GameNode          _rootNode;
        protected const int         TILE_SIZE = 84;
        protected int               _numRows;
        protected int               _numCols;
        protected const String      TILE_NAME_PREFIX = "t";
        protected const String      TILE_NUMBER_SEPARATOR = "T";
        protected Vector2           _tileOrigin;

        protected Player                        _player;
        protected Dictionary<String, Drawable>  _drawables;
        protected Dictionary<String, GameCharacter> _characters;

        protected int _displayWidth;
        protected int _displayHeight;


        //-------------------------------------------------------------------------
        public Camera2D Camera
        {
            get { return _camera; }
        }

        public GameNode Root
        {
            get { return _rootNode; }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default GameLevelManager constructor
        /// </summary>
        /// <param name="contentManager">ContentManager used for loading game data</param>
        public GameLevelManager(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _contentManager = contentManager;
            _displayWidth = graphicsDevice.Viewport.Width;
            _displayHeight = graphicsDevice.Viewport.Height;

            // Initialize camera
            _camera = new Camera2D(this);
            _camera.Origin = new Vector2(
                _displayWidth / 2,
                _displayHeight / 2);
            _camera.RotationZ = (float)(Math.PI / 4);
            _camera.RotationX = (float)(Math.PI / 6);
            _cameraController = new CameraController(_camera);

            // Initialize dictionaries
            _drawables = new Dictionary<string, Drawable>();
            _characters = new Dictionary<string, GameCharacter>();

            // Load an example level
            loadLevel(11, 11, "textures/tiles/rocktile");
        }


        //-------------------------------------------------------------------------
        public void loadLevel(int rows, int cols, String tilePath)
        {
            // Set rows and columns for this level
            _numRows = rows;
            _numCols = cols;

            _rootNode = new GameNode(this, "root");
            Drawable defaultTile = createDrawable<Drawable>(tilePath, "tile");

            // Grab the center
            Vector2 nodeOffset = Vector2.Zero;
            nodeOffset.X = (cols / 2) * TILE_SIZE;
            nodeOffset.Y = (rows / 2) * TILE_SIZE;

            _tileOrigin = new Vector2(TILE_SIZE / 2);

            for (int x = 0; x < rows; x++)
                for (int y = 0; y < cols; y++)
                {
                    Vector3 position;
                    position.X = ((rows - 1) - y) * TILE_SIZE;
                    position.Z = x * TILE_SIZE;

                    position.X -= nodeOffset.X;
                    position.Y = 0;
                    position.Z -= nodeOffset.Y;

                    GameObject tile = createGameObject(
                        TILE_NAME_PREFIX + x + TILE_NUMBER_SEPARATOR + y, defaultTile, _rootNode);
                    tile.GraphIndex = x * cols + y;
                    tile.Origin = _tileOrigin;
                    tile.PositionIsometric = position;
                }

            generateNeighbors();

            // Test stuff=-------------

            GameNode dudesNode = getNodeFromIndex(3, 3).createChildNode("dudesNode");
            dudesNode.Origin = new Vector2(20, 120);

            // Test character
            DrawableAnimated.Animation walkSW = new DrawableAnimated.Animation(16, 19, 0.132f, true);
            DrawableAnimated.Animation walkSE = new DrawableAnimated.Animation(20, 23, 0.132f, true);
            DrawableAnimated.Animation walkNW = new DrawableAnimated.Animation(24, 27, 0.132f, true);
            DrawableAnimated.Animation walkNE = new DrawableAnimated.Animation(28, 31, 0.132f, true);
                                                                                         
            DrawableAnimated.Animation idleSW = new DrawableAnimated.Animation(16, 16, 0.132f, false);
            DrawableAnimated.Animation idleSE = new DrawableAnimated.Animation(20, 20, 0.132f, false);
            DrawableAnimated.Animation idleNW = new DrawableAnimated.Animation(24, 24, 0.132f, false);
            DrawableAnimated.Animation idleNE = new DrawableAnimated.Animation(28, 28, 0.132f, false);

            DrawableAnimated drawable = createDrawable<DrawableAnimated>("textures/characters/character", "character");
            drawable.addAnimation(walkSW, "walkSW");
            drawable.addAnimation(walkSE, "walkSE");
            drawable.addAnimation(walkNW, "walkNW");
            drawable.addAnimation(walkNE, "walkNE");

            drawable.addAnimation(idleSW, "idleSW");
            drawable.addAnimation(idleSE, "idleSE");
            drawable.addAnimation(idleNW, "idleNW");
            drawable.addAnimation(idleNE, "idleNE");

            GameCharacter anotherDude = createCharacter("dudetwo", drawable,
                getNodeFromIndex(3, 3));
            anotherDude.Origin = new Vector2(20, 120);
            

            _player = new Player(this, anotherDude);


            // Move some tiles
            getNodeFromIndex(0, 0).translate(new Vector3(0.0f, -250.0f, 0.0f));
            getNodeFromIndex(1, 0).translate(new Vector3(0.0f, -200.0f, 0.0f));
            getNodeFromIndex(2, 0).translate(new Vector3(0.0f, -150.0f, 0.0f));
            getNodeFromIndex(10, 0).translate(new Vector3(0.0f, -50.0f, 0.0f));
            getNodeFromIndex(9, 0).translate(new Vector3(0.0f, -25.0f, 0.0f));
            getNodeFromIndex(8, 0).translate(new Vector3(0.0f, -75.0f, 0.0f));

            // Remove some tiles
            _rootNode.detachChildNode(getNodeFromIndex(4, 4).getName);
            _rootNode.detachChildNode(getNodeFromIndex(5, 4).getName);
            _rootNode.detachChildNode(getNodeFromIndex(6, 4).getName);
            _rootNode.detachChildNode(getNodeFromIndex(6, 5).getName);
            _rootNode.detachChildNode(getNodeFromIndex(6, 6).getName);
            _rootNode.detachChildNode(getNodeFromIndex(6, 7).getName);
            _rootNode.detachChildNode(getNodeFromIndex(7, 7).getName);
            _rootNode.detachChildNode(getNodeFromIndex(6, 8).getName);
            _rootNode.detachChildNode(getNodeFromIndex(7, 8).getName);
            _rootNode.detachChildNode(getNodeFromIndex(8, 8).getName);
            _rootNode.detachChildNode(getNodeFromIndex(9, 8).getName);
            _rootNode.detachChildNode(getNodeFromIndex(9, 9).getName);
            _rootNode.detachChildNode(getNodeFromIndex(10, 9).getName);
            _rootNode.detachChildNode(getNodeFromIndex(10, 10).getName);
            _rootNode.detachChildNode(getNodeFromIndex(9, 10).getName);
            _rootNode.detachChildNode(getNodeFromIndex(5, 5).getName);
            _rootNode.detachChildNode(getNodeFromIndex(4, 5).getName);

            // end test stuff-----------

        }

        /// <summary>
        /// Pass through the node graph and generate neighbors for all nodes.
        /// Neighbors are generated based on which nodes are adjacent.
        /// </summary>
        protected void generateNeighbors()
        {
            for (int x = 0; x < _numRows; x++)
                for (int y = 0; y < _numCols; y++)
                {
                    GameNode node = getNodeFromIndex(x, y);

                    int indxTOP = y + 1;
                    if (indxTOP >= 0) node.attachNeighbor(getNodeFromIndex(x, indxTOP), 0);

                    int indxBOTTOM = y - 1;
                    if (indxBOTTOM < _numRows) node.attachNeighbor(getNodeFromIndex(x, indxBOTTOM), 1);

                    int indxLEFT = x + 1;
                    if (indxLEFT < _numCols) node.attachNeighbor(getNodeFromIndex(indxLEFT, y), 2);

                    int indxRIGHT = x - 1;
                    if (indxRIGHT >= 0) node.attachNeighbor(getNodeFromIndex(indxRIGHT, y), 3);
                }
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Apply one update to the entire GameLevel
        /// </summary>
        /// <param name="gameTime">Used to determine time delta</param>
        public void update(GameTime gameTime)
        {
            // Get delta time in seconds
            float dt = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            // Poll user input
            pollInput();

            // Update camera
            _cameraController.update(dt);
            _camera.update(dt);

            // Update all character movement
            foreach (KeyValuePair<String, GameCharacter> entry in _characters)
                entry.Value.applyDisplacement(dt);
        }

        /// <summary>
        /// Primary game level function to poll user input
        /// </summary>
        protected void pollInput()
        {
            _player.pollInput();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Draw the entire GameLevel
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used for rendering</param>
        public void draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Get delta time in seconds
            float dt = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

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
            drawNodeGraph(spriteBatch, dt);

            // End drawing
            spriteBatch.End();
        }

        /// <summary>
        /// Pass through the array of cells, drawing each node correctly, according to
        /// what angle the camera is currently viewing at.
        /// </summary>
        public void drawNodeGraph(SpriteBatch spriteBatch, float dt)
        {
            // Draw the nodes according to the camera's direction
            switch (_camera.Dir)
            { 
                case Direction.DIR_NE:
                    for (int x = 0; x < _numRows; x++)
                        for (int y = _numCols - 1; y >= 0; y--)
                            drawNode(spriteBatch, getNodeFromIndex(x, y), dt, true);
                    break;

                case Direction.DIR_SE:
                    for (int x = _numRows - 1; x >= 0; x--)
                        for (int y = _numCols - 1; y >= 0; y--)
                            drawNode(spriteBatch, getNodeFromIndex(x, y), dt, true);
                    break;

                case Direction.DIR_SW:
                    for (int x = _numRows - 1; x >= 0; x--)
                        for (int y = 0; y < _numCols; y++)
                            drawNode(spriteBatch, getNodeFromIndex(x, y), dt, true);
                    break;

                case Direction.DIR_NW:
                    for (int x = 0; x < _numRows; x++)
                        for (int y = 0; y < _numCols; y++)
                            drawNode(spriteBatch, getNodeFromIndex(x, y), dt, true);
                    break;
            }
        }

        /// <summary>
        /// Draws a GameNode's Drawable, if it contains one
        /// </summary>
        /// <param name="node">GameNode to draw</param>
        /// <param name="recursive">If true, all of this nodes children will be drawn</param>
        public void drawNode(SpriteBatch spriteBatch, GameNode node, float dt, bool recursive = true)
        {
            if (node == null)
                return;

            if(!(node is GameObject))
                return;

            GameObject go = node as GameObject;
            go.drawContents(spriteBatch, dt);

            if (recursive)
                foreach (KeyValuePair<String, GameNode> childEntry in node.getChildren)
                    drawNode(spriteBatch, childEntry.Value, dt, true);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Creates and returns a new Drawable, which may then be attached to a GameNode
        /// </summary>
        /// <typeparam name="T">Type of Drawable to create. Must be type Drawable</typeparam>
        /// <param name="texturePath">Path to texture file</param>
        /// <returns></returns>
        public T createDrawable<T>(String texturePath, String name) where T : Drawable
        {
            Texture2D texture = _contentManager.Load<Texture2D>(texturePath);
            T drawable = (T)Activator.CreateInstance(typeof(T), new object[]{texture, name});

            _drawables.Add(name, drawable);

            return drawable;
        }

        /// <summary>
        /// Destroy a previously created Drawable
        /// </summary>
        /// <param name="name">Name of drawable to destroy</param>
        public void destroyDrawable(String name)
        {
            if (!_drawables.ContainsKey(name))
                return;
            _drawables.Remove(name);
        }


        //-------------------------------------------------------------------------
        public GameObject createGameObject(String name, Drawable drawable, GameNode parent)
        {
            GameObject gameObject = new GameObject(this, name);
            parent.attachChildNode(gameObject);
            gameObject.attachDrawable(drawable);
            return gameObject;
        }

        public GameCharacter createCharacter(String name, DrawableAnimated drawable, GameNode parent)
        {
            GameCharacter character = new GameCharacter(this, name, drawable);
            character.subscribeToCamera(_camera);
            parent.attachChildNode(character);
            _characters.Add(name, character);
            character.PositionIsometric = parent.PositionIsometric;
            return character;
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
        public GameNode getNodeFromIndex(int x, int y)
        {
            if (    x < 0 || x > _numCols
                ||  y < 0 || y > _numRows)
                return null;

            return _rootNode.getChildNode(TILE_NAME_PREFIX + x + TILE_NUMBER_SEPARATOR + y);
        }

        /// <summary>
        /// Returns a node corresponding to the specified screen coordinates
        /// </summary>
        /// <param name="coordinates">Screen coordinates</param>
        /// <returns>GameNode at the location specified</returns>
        public GameNode getNodeFromScreenCoordinates(Vector2 coordinates)
        {
            // TODO: needs to be able to grab elevated tiles

            coordinates = _camera.screenToIsometric(coordinates);
            Vector3 isoCoords = new Vector3(coordinates.X, 0.0f, coordinates.Y);

            return getNodeFromIsometricCoordinates(isoCoords);
        }

        /// <summary>
        /// Retrieve a tile GameNode from this level based on the
        /// specified isometric coordinates.
        /// </summary>
        /// <param name="isoCoords">Isometric coordinates</param>
        /// <returns>Tile GameNode at position</returns>
        public GameNode getNodeFromIsometricCoordinates(Vector3 isoCoords)
        {
            // Map to graph indices
            isoCoords.X += _tileOrigin.X;
            isoCoords.Z += _tileOrigin.Y;
            isoCoords /= (float)TILE_SIZE;
            isoCoords.X += _numCols / 2;
            isoCoords.Z += _numRows / 2;

            // Drop the fractional portion
            isoCoords.X -= isoCoords.X % 1.0f;
            isoCoords.Z -= isoCoords.Z % 1.0f;

            // If we don't have a valid index, return null
            if (isoCoords.X < 0 || isoCoords.X >= _numCols
                || isoCoords.Z < 0 || isoCoords.Z >= _numRows)
                return null;

            isoCoords.X = (_numCols - 1) - isoCoords.X;

            return getNodeFromIndex((int)isoCoords.Z, (int)isoCoords.X);
        }
    }
}
