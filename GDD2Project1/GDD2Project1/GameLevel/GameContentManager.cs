using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GDD2Project1
{
    public class GameContentManager
    {
        protected ContentManager                    _contentMgr;

        protected Dictionary<String, Drawable>      _drawables;


        //-------------------------------------------------------------------------
        /// <summary>
        /// Default GameContentManager constructor
        /// </summary>
        /// <param name="contentMgr">Used for loading game context (textures, sounds, etc)</param>
        public GameContentManager(ContentManager contentMgr)
        {
            // Primary initialization
            _contentMgr     = contentMgr;

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
        /// Load a Drawable into memory so that it may be used in a GameLevel.
        /// </summary>
        /// <typeparam name="T">Type of Drawable</typeparam>
        /// <param name="directory">Relative directory containing drawable's texture</param>
        /// <param name="filename">Drawable's filename</param>
        public T loadDrawable<T>(String directory, String filename)
            where T : Drawable
        {
            // If the drawable has already been loaded, simple return it
            if (_drawables.ContainsKey(filename))
                return _drawables[filename] as T;

            // Otherwise, load it, and then return it
            Texture2D texture   = _contentMgr.Load<Texture2D>(directory + filename);
            T drawable          = (T)Activator.CreateInstance(typeof(T), new object[] { texture, filename });
            _drawables.Add(filename, drawable);

            return drawable;
        }

        /// <summary>
        /// Retrieve a previously loaded Drawable
        /// </summary>
        /// <typeparam name="T">Type of Drawable</typeparam>
        /// <param name="filename">Filename of loaded Drawable, not including extension</param>
        /// <returns>Drawable corresponding to filename</returns>
        public T getDrawable<T>(String filename)
            where T : Drawable
        {
            if (!_drawables.ContainsKey(filename))
                return null;

            return _drawables[filename] as T;
        }
    }
}
