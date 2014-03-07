using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace GDD2Project1.GUI
{
    public class TextButton : InterfaceObject
    {
        private Texture2D _texture;
        private int _lineCount = 0;
        private String _text;
        private SpriteFont _font;
        private bool _lockWidth = true;
        private Color _color;

        

        public bool LockWidth
        {
            get { return _lockWidth; }
            set { _lockWidth = value; }
        }

        public TextButton(int x, int y, int width, int height, SpriteFont spriteFont, Texture2D texture)
            : base(x,y,width,height)
        {
            _texture = texture;
            _font = spriteFont;
            _text = "";
            _color = Color.White;

            onMouseOver += delegate()
            {
                _color = Color.Green;
            };

            onMouseLeave += delegate()
            {
                _color = Color.White;
            };
        }

        override public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(
                 _texture,
                 _rect, _color);

            spriteBatch.DrawString(_font, _text, new Vector2(_rect.X, _rect.Y), Color.Black);
        }

        // Will be used to draw only in the window
        public override void Draw(SpriteBatch spriteBatch, Color color, Window window)
        {
            
            spriteBatch.Draw(
                _texture,
                _rect,
                color);
            
            spriteBatch.DrawString(_font, _text, new Vector2(_rect.X, _rect.Y), Color.Black);
            

        }
        
        // Called when adding this element to a window
        // Adjusts its position to be properly placed in the window
        // Adjusts width to be the length of the window (minus the border) for easier clickability
        public override void addToWindow(Window window)
        {
            
            if (_rect.X < window.Border)
            {
                _rect.X = window.X + window.Border;
                _rect.Y += window.Y;
                if (_lockWidth)
                {
                    _rect.Width = window.Width - window.Border * 2;
                }
            }
            else
            {
                if (_lockWidth)
                {
                    _rect.Width = window.Width - window.Border * 2;
              
                }
                _rect.X += window.X;
                _rect.Y += window.Y;
            }

            // Wrap the text to the new width
            WrapText();
        }

        // Wraps the text based on the width
        private void WrapText()
        {
            String line = "";
            String endString = "";
            _text = _text.Replace("\n", "");
            // Create an array of words by spliting the original text
            // by spaces
            String[] words = _text.Split(' ');
            
            foreach (String word in words)
            {
                // If the length of the next word being added is more than the width
                // add that line to the end string with an end line attached to it
                if (_font.MeasureString(line + word).Length() > _rect.Width)
                {
                    endString += line + "\n";
                    line = "";
                    _lineCount += 1;
                }
                
                line += word + " ";
            }

            _lineCount += 1;

            _text = endString + line;

            // Calculate a new height of the clickable box since it changed with the word wrap
            findHeight(); 
        }

        // Sets the height of the rectangle to be the size of the text including all lines
        private void findHeight()
        {
            _rect.Height = (int)(_font.MeasureString(_text).Y);
        }

        public void changeText(String text)
        {
            _text = text;
            WrapText();
        }

    }
}
