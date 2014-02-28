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

    public class GameLevelManager
    {
        protected GameContentManager _gameContentMgr;

        protected Camera2D          _camera;

        protected GameNode          _rootNode;
        protected const int         TILE_SIZE = 84;
        protected int               _numRows;
        protected int               _numCols;
        protected const String      TILE_NAME_PREFIX = "r";
        protected const String      TILE_NUMBER_SEPARATOR = "c";
        protected Vector2           _tileOrigin;

        protected Dictionary<String, GameCharacter>     _characters;

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
        public GameLevelManager(GameContentManager gameContentMgr, GraphicsDevice graphicsDevice)
        {
            _gameContentMgr = gameContentMgr;
            _displayWidth = graphicsDevice.Viewport.Width;
            _displayHeight = graphicsDevice.Viewport.Height;

            // Initialize camera
            _camera = new Camera2D(this);
            _camera.Origin = new Vector2(
                _displayWidth / 2,
                _displayHeight / 2);

            // Initialize dictionaries
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

            _gameContentMgr.loadDrawable<Drawable>("textures/tiles/", "rocktile");
            Drawable defTile = _gameContentMgr.getDrawable<Drawable>("rocktile");

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
                        TILE_NAME_PREFIX + x + TILE_NUMBER_SEPARATOR + y, defTile, _rootNode);

                    tile.GraphIndex = x * cols + y;
                    tile.Origin = _tileOrigin;
                    tile.PositionIsometric = position;
                }

            // Test stuff=-------------

            // Test character
            DrawableAnimated.Animation walkSW = new DrawableAnimated.Animation(16, 19, 0.132f, true);
            DrawableAnimated.Animation walkSE = new DrawableAnimated.Animation(20, 23, 0.132f, true);
            DrawableAnimated.Animation walkNW = new DrawableAnimated.Animation(24, 27, 0.132f, true);
            DrawableAnimated.Animation walkNE = new DrawableAnimated.Animation(28, 31, 0.132f, true);
                                                                                         
            DrawableAnimated.Animation idleSW = new DrawableAnimated.Animation(16, 16, 0.132f, false);
            DrawableAnimated.Animation idleSE = new DrawableAnimated.Animation(20, 20, 0.132f, false);
            DrawableAnimated.Animation idleNW = new DrawableAnimated.Animation(24, 24, 0.132f, false);
            DrawableAnimated.Animation idleNE = new DrawableAnimated.Animation(28, 28, 0.132f, false);

            _gameContentMgr.loadDrawable<DrawableAnimated>("textures/characters/", "character");
            DrawableAnimated drawable = _gameContentMgr.getDrawable<DrawableAnimated>("character");
            drawable.addAnimation(walkSW, "walkSW");
            drawable.addAnimation(walkSE, "walkSE");
            drawable.addAnimation(walkNW, "walkNW");
            drawable.addAnimation(walkNE, "walkNE");

            drawable.addAnimation(idleSW, "idleSW");
            drawable.addAnimation(idleSE, "idleSE");
            drawable.addAnimation(idleNW, "idleNW");
            drawable.addAnimation(idleNE, "idleNE");

            GameCharacter anotherDude = createCharacter("dudetwo", drawable,
                getTileFromIndex(3, 3));
            anotherDude.Origin = new Vector2(20, 120);


            // Tell the camera controller to track our player's movement

            // Move some tiles
            getTileFromIndex(0, 0).translate(new Vector3(0.0f, -250.0f, 0.0f));
            getTileFromIndex(0, 1).translate(new Vector3(0.0f, -200.0f, 0.0f));
            getTileFromIndex(1, 0).translate(new Vector3(0.0f, -200.0f, 0.0f));
            getTileFromIndex(1, 1).translate(new Vector3(0.0f, -180.0f, 0.0f));
            getTileFromIndex(1, 2).translate(new Vector3(0.0f, -140.0f, 0.0f));
            getTileFromIndex(0, 2).translate(new Vector3(0.0f, -120.0f, 0.0f));
            getTileFromIndex(2, 1).translate(new Vector3(0.0f, -110.0f, 0.0f));
            getTileFromIndex(3, 1).translate(new Vector3(0.0f, -60.0f, 0.0f));
            getTileFromIndex(3, 0).translate(new Vector3(0.0f, -80.0f, 0.0f));
            getTileFromIndex(2, 2).translate(new Vector3(0.0f, -70.0f, 0.0f));
            getTileFromIndex(1, 3).translate(new Vector3(0.0f, -40.0f, 0.0f));
            getTileFromIndex(2, 3).translate(new Vector3(0.0f, -20.0f, 0.0f));
            getTileFromIndex(2, 0).translate(new Vector3(0.0f, -150.0f, 0.0f));
            getTileFromIndex(10, 0).translate(new Vector3(0.0f, -50.0f, 0.0f));
            getTileFromIndex(9, 0).translate(new Vector3(0.0f, -25.0f, 0.0f));
            getTileFromIndex(8, 0).translate(new Vector3(0.0f, -75.0f, 0.0f));
            getTileFromIndex(5, 6).translate(new Vector3(0.0f, -75.0f, 0.0f));
            getTileFromIndex(5, 7).translate(new Vector3(0.0f, -35.0f, 0.0f));
            getTileFromIndex(4, 7).translate(new Vector3(0.0f, -15.0f, 0.0f));

            // Remove some tiles
            _rootNode.detachChildNode(getTileFromIndex(3, 5).getName);
            _rootNode.detachChildNode(getTileFromIndex(3, 6).getName);
            _rootNode.detachChildNode(getTileFromIndex(3, 7).getName);
            _rootNode.detachChildNode(getTileFromIndex(4, 6).getName);
            _rootNode.detachChildNode(getTileFromIndex(4, 4).getName);
            _rootNode.detachChildNode(getTileFromIndex(5, 4).getName);
            _rootNode.detachChildNode(getTileFromIndex(6, 4).getName);
            _rootNode.detachChildNode(getTileFromIndex(6, 5).getName);
            _rootNode.detachChildNode(getTileFromIndex(6, 6).getName);
            _rootNode.detachChildNode(getTileFromIndex(6, 7).getName);
            _rootNode.detachChildNode(getTileFromIndex(7, 7).getName);
            _rootNode.detachChildNode(getTileFromIndex(6, 8).getName);
            _rootNode.detachChildNode(getTileFromIndex(7, 8).getName);
            _rootNode.detachChildNode(getTileFromIndex(8, 8).getName);
            _rootNode.detachChildNode(getTileFromIndex(9, 8).getName);
            _rootNode.detachChildNode(getTileFromIndex(9, 9).getName);
            _rootNode.detachChildNode(getTileFromIndex(10, 9).getName);
            _rootNode.detachChildNode(getTileFromIndex(10, 10).getName);
            _rootNode.detachChildNode(getTileFromIndex(9, 10).getName);
            _rootNode.detachChildNode(getTileFromIndex(5, 5).getName);
            _rootNode.detachChildNode(getTileFromIndex(4, 5).getName);

            // Plant some trees
            _gameContentMgr.loadDrawable<Drawable>("textures/decorative/", "tree");
            Drawable tree = _gameContentMgr.getDrawable<Drawable>("tree");


            GameObject tree1 = createGameObject("tree1", tree, getTileFromIndex(8, 0));
            tree1.Origin = new Vector2(80, 180);
            GameObject tree2 = createGameObject("tree2", tree, getTileFromIndex(2, 10));
            tree2.Origin = new Vector2(80, 180);
            GameObject tree3 = createGameObject("tree3", tree, getTileFromIndex(0, 0));
            tree3.Origin = new Vector2(80, 180);
            GameObject tree4 = createGameObject("tree4", tree, getTileFromIndex(2, 0));
            tree4.Origin = new Vector2(80, 180);
            GameObject tree5 = createGameObject("tree5", tree, getTileFromIndex(5, 6));
            tree5.Origin = new Vector2(80, 180);

            // Lay down some consumables
            _gameContentMgr.loadDrawable<Drawable>("textures/consumables/", "consumable");
            Drawable consumable = _gameContentMgr.getDrawable<Drawable>("consumable");


            Consumable c1 = createConsumable("cnsmble1", consumable, getTileFromIndex(8, 1), Consumable.ConsumableType.TYPE_POWER, 100);
            c1.Origin = new Vector2(20, 40);
            Consumable c2 = createConsumable("cnsmble2", consumable, getTileFromIndex(3, 10), Consumable.ConsumableType.TYPE_HEALTH, 3000);
            c2.Origin = new Vector2(20, 40);
            Consumable c3 = createConsumable("cnsmble3", consumable, getTileFromIndex(1, 8), Consumable.ConsumableType.TYPE_HEALTH, 10000);
            c3.Origin = new Vector2(20, 40);
            Consumable c4 = createConsumable("cnsmble4", consumable, getTileFromIndex(10, 8), Consumable.ConsumableType.TYPE_POWER, 5432);
            c4.Origin = new Vector2(20, 40);
            Consumable c5 = createConsumable("cnsmble5", consumable, getTileFromIndex(7, 6), Consumable.ConsumableType.TYPE_HEALTH, 800);
            c5.Origin = new Vector2(20, 40);
            Consumable c6 = createConsumable("cnsmble6", consumable, getTileFromIndex(1, 1), Consumable.ConsumableType.TYPE_POWER, 100);
            c6.Origin = new Vector2(20, 40);

            // end test stuff-----------

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
                            drawNode(spriteBatch, getTileFromIndex(x, y), dt, true);
                    break;

                case Direction.DIR_SE:
                    for (int x = _numRows - 1; x >= 0; x--)
                        for (int y = _numCols - 1; y >= 0; y--)
                            drawNode(spriteBatch, getTileFromIndex(x, y), dt, true);
                    break;

                case Direction.DIR_SW:
                    for (int x = _numRows - 1; x >= 0; x--)
                        for (int y = 0; y < _numCols; y++)
                            drawNode(spriteBatch, getTileFromIndex(x, y), dt, true);
                    break;

                case Direction.DIR_NW:
                    for (int x = 0; x < _numRows; x++)
                        for (int y = 0; y < _numCols; y++)
                            drawNode(spriteBatch, getTileFromIndex(x, y), dt, true);
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

            (node as GameObject).drawContents(spriteBatch, dt);

            if (recursive)
                foreach (KeyValuePair<String, GameNode> childEntry in node.getChildren)
                    drawNode(spriteBatch, childEntry.Value, dt, true);
        }


        //-------------------------------------------------------------------------
        public GameObject createGameObject(String name, Drawable drawable, GameNode parent)
        {
            GameObject gameObject = new GameObject(this, name);
            parent.attachChildNode(gameObject);
            gameObject.attachDrawable(drawable);
            gameObject.PositionIsometric = parent.PositionIsometric;
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

        public Consumable createConsumable(String name, Drawable drawable, GameNode parent, Consumable.ConsumableType type, int amount)
        {
            Consumable consumable = new Consumable(this, name, type, amount);
            parent.attachChildNode(consumable);
            consumable.attachDrawable(drawable);
            consumable.PositionIsometric = parent.PositionIsometric;
            return consumable;
        }

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
        /// Get a previously created character
        /// </summary>
        /// <param name="name">Character name</param>
        /// <returns>Character at specified name index</returns>
        public GameCharacter getCharacter(String name)
        {
            if (!(_characters.ContainsKey(name)))
                return null;

            return _characters[name];
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
        public GameNode getTileFromIndex(int x, int y)
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
        public GameNode getTileFromScreenCoordinates(Vector2 coordinates)
        {
            // TODO: needs to be able to grab elevated tiles

            coordinates = _camera.screenToIsometric(coordinates);
            Vector3 isoCoords = new Vector3(coordinates.X, 0.0f, coordinates.Y);

            return getTileFromIsometricCoordinates(isoCoords);
        }

        /// <summary>
        /// Retrieve a tile GameNode from this level based on the
        /// specified isometric coordinates.
        /// </summary>
        /// <param name="isoCoords">Isometric coordinates</param>
        /// <returns>Tile GameNode at position</returns>
        public GameNode getTileFromIsometricCoordinates(Vector3 isoCoords)
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

            return getTileFromIndex((int)isoCoords.Z, (int)isoCoords.X);
        }
    }
}
