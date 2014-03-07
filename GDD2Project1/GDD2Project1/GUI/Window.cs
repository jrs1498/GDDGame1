using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDD2Project1.GUI
{
    public class Window : InterfaceObject
    {
        private int _border = 10;
        private String _label;
        private List<InterfaceObject> _interfaceObjects;
        private SpriteFont _spriteFont;
        private bool _scrollBarEnabled = true;

        private Texture2D _texture;

        public int Border { get { return _border; } set { _border = value; } }
        public bool ScrollBarEnabled { get { return _scrollBarEnabled; } set { _scrollBarEnabled = value; } }
        public List<InterfaceObject> InterfaceObjects { get { return _interfaceObjects; } }
        public String Label { get { return _label; } set { _label = value; } }
        

        public Window(int x, int y, int width, int height, SpriteFont sf, Texture2D texture)
            : base(x, y, width, height)
        {
            _interfaceObjects = new List<InterfaceObject>();
            _texture = texture;
            _spriteFont = sf;
            _label = "";
        }

        override public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(
                 _texture,
                 _rect,
                 color);
            spriteBatch.DrawString(_spriteFont, _label, new Vector2(_rect.X, _rect.Y), Color.Blue);

            for (int i = 0; i < _interfaceObjects.Count; i++)
            {
                _interfaceObjects[i].Draw(spriteBatch, Color.Green, this);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Color color, Window window)
        {
            return;
        }

        public void Add(InterfaceObject interfaceObject)
        {
            interfaceObject.addToWindow(this);
            _interfaceObjects.Add(interfaceObject);
        }

        override public void addToWindow(Window win)
        {
            return;
        }

       
    }
}
