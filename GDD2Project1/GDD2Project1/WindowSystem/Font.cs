#region File Description
//-----------------------------------------------------------------------------
// File:      Font.cs
// Namespace: WindowSystem
// Author:    Aaron MacDougall
//-----------------------------------------------------------------------------
#endregion

#region License
//-----------------------------------------------------------------------------
// Copyright (c) 2007, Aaron MacDougall
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice,
//   this list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of Aaron MacDougall nor the names of its contributors may
//   be used to endorse or promote products derived from this software without
//   specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace WindowSystem
{
    /// <summary>
    /// A re-implementation of SpriteFont that allows text clipping. This
    /// allows the Window System to perform better without the need for a
    /// scissor rectangle for each label. It is compatible with .spritefont
    /// files.
    /// </summary>
    public class Font
    {
        #region Fields
        private SpriteFont spriteFont;
        private List<char> characterMap;
        private List<Rectangle> croppingData;
        private List<Rectangle> glyphData;
        private List<Vector3> kerning;
        private int lineSpacing;
        private float spacing;
        private Texture2D textureValue;
        #endregion

        #region Properties
        public int LineSpacing
        {
            get { return this.spriteFont.LineSpacing; }
        }
        #endregion

        public Font(string fontName, ContentManager content)
        {
            SpriteFont spriteFont = content.Load<SpriteFont>(fontName);
            LoadData(spriteFont);

            this.spriteFont = spriteFont;
        }

        public Font(SpriteFont spriteFont)
        {
            LoadData(spriteFont);

            this.spriteFont = spriteFont;
        }

        private void LoadData(SpriteFont spriteFont)
        {
            // Get the SpriteFont type
            Type type = typeof(SpriteFont);

            // Get private fields and put them in our own fields
            FieldInfo fieldInfo = type.GetField("characterMap", BindingFlags.NonPublic | BindingFlags.Instance);
            this.characterMap = (List<char>) fieldInfo.GetValue(spriteFont);

            fieldInfo = type.GetField("croppingData", BindingFlags.NonPublic | BindingFlags.Instance);
            this.croppingData = (List<Rectangle>)fieldInfo.GetValue(spriteFont);

            fieldInfo = type.GetField("glyphData", BindingFlags.NonPublic | BindingFlags.Instance);
            this.glyphData = (List<Rectangle>)fieldInfo.GetValue(spriteFont);

            fieldInfo = type.GetField("kerning", BindingFlags.NonPublic | BindingFlags.Instance);
            this.kerning = (List<Vector3>)fieldInfo.GetValue(spriteFont);

            fieldInfo = type.GetField("lineSpacing", BindingFlags.NonPublic | BindingFlags.Instance);
            this.lineSpacing = (int)fieldInfo.GetValue(spriteFont);

            fieldInfo = type.GetField("spacing", BindingFlags.NonPublic | BindingFlags.Instance);
            this.spacing = (float)fieldInfo.GetValue(spriteFont);

            fieldInfo = type.GetField("textureValue", BindingFlags.NonPublic | BindingFlags.Instance);
            this.textureValue = (Texture2D)fieldInfo.GetValue(spriteFont);
        }

        public Vector2 MeasureString(string text)
        {
            return this.spriteFont.MeasureString(text);
        }

        public void Draw(string text, SpriteBatch spriteBatch, Vector2 position, Color color, Rectangle scissor)
        {
            bool newLine = true;
            Vector2 overallPosition = Vector2.Zero;
            Rectangle source;
            Rectangle destination;
            Vector2 charPosition;

            for (int i = 0; i < text.Length; i++)
            {
                // Work out current character
                char character = text[i];
                int characterIndex = this.GetCharacterIndex(character);

                Vector3 charKerning = this.kerning[characterIndex];

                if (newLine)
                    charKerning.X = Math.Max(charKerning.X, 0f);
                else
                    overallPosition.X += this.spacing;

                overallPosition.X += charKerning.X;

                source = this.glyphData[characterIndex];

                charPosition = overallPosition;
                charPosition.X += this.croppingData[characterIndex].X;
                charPosition.Y += this.croppingData[characterIndex].Y;

                charPosition += position;

                destination = new Rectangle((int)charPosition.X, (int)charPosition.Y, source.Width, source.Height);

                if (GUIManager.PerformClipping(ref scissor, ref source, ref destination))
                    spriteBatch.Draw(this.textureValue, destination, source, Color.Black);
                
                newLine = false;
                overallPosition.X += charKerning.Y + charKerning.Z;
            }
        }

        private int GetCharacterIndex(char character)
        {
            int num2 = 0;
            int num3 = this.characterMap.Count - 1;

            while (num2 <= num3)
            {
                int num = num2 + ((num3 - num2) >> 1);
                char ch = this.characterMap[num];
                if (ch == character)
                {
                    return num;
                }
                if (ch < character)
                {
                    num2 = num + 1;
                }
                else
                {
                    num3 = num - 1;
                }
            }

            return -1;
        }

    }
}