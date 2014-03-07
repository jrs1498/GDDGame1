using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDD2Project1.GUI
{
    public class InterfaceManager
    {
        private ScreenManager _screenMgr;
        private List<InterfaceObject> _interfaceObjs;
        private SpriteFont _interfaceFont;
        private Texture2D _noAlphaPixel;


        //-------------------------------------------------------------------------
        public InterfaceManager(ScreenManager screenMgr)
        {
            _interfaceObjs = new List<InterfaceObject>();
            _interfaceFont = screenMgr.Content.Load<SpriteFont>("Fonts/DefaultFont");
            _screenMgr = screenMgr;
            _noAlphaPixel = new Texture2D(_screenMgr.GraphicsDevice, 1, 1);
            _noAlphaPixel.SetData<Color>(new Color[1] { Color.White });
        }


        //-------------------------------------------------------------------------
        public void injectMouseMove(Point mouseCoords)
        {
            foreach (InterfaceObject obj in _interfaceObjs)
            {
                obj.CheckMouseOver(mouseCoords);
                obj.CheckMouseLeave(mouseCoords);
            }
        }

        public void injectMouseDown(Point mouseCoords)
        {
            foreach (InterfaceObject obj in _interfaceObjs)
                obj.CheckMouseClick(mouseCoords);
        }


        //-------------------------------------------------------------------------
        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            foreach (InterfaceObject obj in _interfaceObjs)
                obj.Draw(spriteBatch, Color.White);
            spriteBatch.End();
        }


        //-------------------------------------------------------------------------
        public T create<T>(int x, int y, int width, int height)
            where T : InterfaceObject
        {
            T obj = (T)Activator.CreateInstance(typeof(T), new object[] { x, y, width, height, _interfaceFont, _noAlphaPixel });
            _interfaceObjs.Add(obj);

            return obj;
        }
    }
}
