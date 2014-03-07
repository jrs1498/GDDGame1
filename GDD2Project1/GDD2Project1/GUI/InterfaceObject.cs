using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDD2Project1.GUI
{
    public delegate void UIEventHandler();

    public abstract class InterfaceObject
    {
        
        protected Rectangle _rect;
        public int X { get { return _rect.X; } set { _rect.X = value; } }
        public int Y { get { return _rect.Y; } set { _rect.Y = value; } }
        public int Width { get { return _rect.Width; } set { _rect.Width = value; } }
        public int Height { get { return _rect.Height; } set { _rect.Height = value; } }

        private bool mouseHovering = false;

        public event UIEventHandler onClick;
        public event UIEventHandler onMouseOver;
        public event UIEventHandler onMouseLeave;

        public InterfaceObject(int x, int y, int width, int height)
        {
            _rect.X = x;
            _rect.Y = y;
            _rect.Width = width;
            _rect.Height = height;
            
        }

        abstract public void addToWindow(Window window);

        abstract public void Draw(SpriteBatch spriteBatch, Color color);

        abstract public void Draw(SpriteBatch spriteBatch, Color color, Window window);

        public void CheckMouseOver(Point mouseCoords)
        {
            if (mouseHovering == false)
            {
                if (_rect.Contains(mouseCoords))
                {
                    mouseHovering = true;
                    if (onMouseOver != null)
                        onMouseOver();
                }
            }
        }

        public void CheckMouseLeave(Point mouseCoords)
        {
            if (mouseHovering)
            {
                if (!_rect.Contains(mouseCoords))
                {
                    mouseHovering = false;
                    if (onMouseLeave != null)
                        onMouseLeave();
                }
            }
        }

        public void CheckMouseClick(Point mouseCoords)
        {
            if (mouseHovering)
            {
                if (_rect.Contains(mouseCoords))
                {
                    if (onClick != null)
                        onClick();
                }
            }
        }
    }
}
