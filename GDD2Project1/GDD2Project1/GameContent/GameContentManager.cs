using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameData;

namespace GDD2Project1
{
    public class GameContentManager
    {
        // Directories
        private const String DIR_TEXTURES       = "textures\\";
        private const String DIR_DRAWABLES      = "drawables\\";
        private const String DIR_LEVELS         = "levels\\";

        // Name convention
        private const String TILE_PREFIX_ROW    = "r";
        private const String TILE_PREFIX_COL    = "c";

        protected ContentManager                    _contentMgr;

        protected Dictionary<String, Drawable>      _drawables;

        protected Dictionary<String, Texture2D>         _textures;
        protected Dictionary<String, ContentObject>     _objects;
        


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default GameContentManager constructor
        /// </summary>
        /// <param name="contentMgr">Used for loading game context (textures, sounds, etc)</param>
        public GameContentManager(ContentManager contentMgr)
        {
            _contentMgr     = contentMgr;

            // Initialize content dictionaries
            _textures       = new Dictionary<String, Texture2D>();
            _objects        = new Dictionary<String, ContentObject>();



            _drawables      = new Dictionary<String, Drawable>();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Completely clears this GameContentManager of all its loaded resources.
        /// This could be called when leaving a GameLevel and returning to the main menu,
        /// for example.
        /// </summary>
        public void unloadAll()
        {
            _drawables.Clear();

            // Drop all loaded data
            _contentMgr.Unload();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Loads an xml file containing level data, and returns the corresponding
        /// deserialized GameLevelData
        /// </summary>
        /// <param name="loadPath">Path to file</param>
        /// <returns>Deserialized XML level data</returns>
        public GameLevelData loadLevelData(String loadPath)
        {
            return _contentMgr.Load<GameLevelData>(loadPath);
        }





        //-------------------------------------------------------------------------
        /// <summary>
        /// Add a ContentObject to this ContentManager
        /// </summary>
        /// <param name="obj">Object to add</param>
        /// <returns>False if failed</returns>
        private bool addContentObject(ContentObject obj)
        {
            // Verify this object has not already been added
            if (_objects.ContainsKey(obj.Name))
                return false;

            // Add the object to ContentManager
            _objects.Add(obj.Name, obj);

            return true;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Load a texture from specified path.
        /// </summary>
        /// <param name="path">Path to texture file</param>
        /// <returns>Newly loaded texture</returns>
        private Texture2D loadTexture(String name)
        {
            // Verify this texture has not already been loaded
            if (_textures.ContainsKey(name))
                return _textures[name];

            // Load the texture and add it to ContentManager
            Texture2D texture = _contentMgr.Load<Texture2D>(DIR_TEXTURES + name);
            _textures.Add(name, texture);

            return texture;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Load a Drawable XML file.
        /// </summary>
        /// <param name="name">Name of XML Drawable file</param>
        /// <returns>Loaded Drawable</returns>
        public Drawable loadDrawable(String name)
        {
            // Verify this Drawable has not already been loaded
            if (_objects.ContainsKey(name))
                return _objects[name] as Drawable;

#if DEBUG
            Console.WriteLine("Loading Drawable: " + name);
#endif

            // Load DrawableData
            DrawableData    data        = _contentMgr.Load<DrawableData>(DIR_DRAWABLES + name);

            // Load new Drawable
            Drawable drawable = new Drawable(
                name,
                loadTexture(data.TextureName),
                data.Origin);

            // Add to ContentManager
            addContentObject(drawable);
            return drawable;
        }

        /// <summary>
        /// Load a DrawableAnimated XML file.
        /// </summary>
        /// <param name="name">Name of XML file</param>
        /// <returns>Newly loaded DrawableAnimated</returns>
        public DrawableAnimated loadDrawableAnimated(String name)
        { 
            // Verify this DrawableAnimated has not already been loaded
            if (_objects.ContainsKey(name))
                return _objects[name] as DrawableAnimated;

#if DEBUG
            Console.WriteLine("Loading DrawableAnimated: " + name);
#endif

            // Load data
            DrawableAnimatedData data = _contentMgr.Load<DrawableAnimatedData>(DIR_DRAWABLES + name);

            // Extract animation data
            int animCount = data.Animations.Count();
            Animation[] animations = new Animation[animCount];
            for (int i = 0; i < animCount; i++)
            {
                AnimationData animData = data.Animations[i];
                Animation anim = new Animation(
                    animData.Name,
                    animData.StartFrame,
                    animData.EndFrame,
                    animData.FrameTime,
                    animData.Looping);
                animations[i] = anim;
            }

            // Load new DrawableAnimated
            DrawableAnimated drawable = new DrawableAnimated(
                name,
                loadTexture(data.TextureName),
                data.Origin,
                data.FrameWidth,
                data.FrameHeight,
                data.FrameRows,
                data.FrameCols,
                animations);

            // Add to ContentManager
            addContentObject(drawable);
            return drawable;
        }

        public Entity createEntity(
            String drawableName,
            bool animated)
        {
            Drawable drawable;
            if (animated)
                drawable = loadDrawableAnimated(drawableName);
            else
                drawable = loadDrawable(drawableName);

            Entity entity = new Entity(drawable);
            return entity;
        }
    }
}
