#region File Description
//-----------------------------------------------------------------------------
// File:      Label.cs
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
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace WindowSystem
{
    /// <summary>
    /// A graphical text label.
    /// </summary>
    public class Label : UIComponent
    {
        #region Default Properties
        private static string defaultFont = "Content/Fonts/DefaultFont";
        private static Color defaultColor = Color.Black;

        /// <summary>
        /// Sets the default font.
        /// </summary>
        /// <value>Must be a non-empty string.</value>
        public static string DefaultFont
        {
            set
            {
                Debug.Assert(value != null);
                Debug.Assert(value.Length > 0);
                defaultFont = value;
            }
        }

        /// <summary>
        /// Sets the default text colour.
        /// </summary>
        public static Color DefaultColor
        {
            set { defaultColor = value; }
        }
        #endregion

        #region Fields
        private string text;
        private char cursor;
        private bool isCursorShown;
        private Font font;
        private Color color;
        #endregion

        #region Properties
        /// <summary>
        /// Get/Set the label text.
        /// </summary>
        /// <value>Must not be null.</value>
        public string Text
        {
            get { return text; }
            set
            {
                Debug.Assert(value != null);

                this.text = value;
            }
        }

        /// <summary>
        /// Get/Set whether the cursor should be shown.
        /// </summary>
        public bool IsCursorShown
        {
            get { return isCursorShown; }
            set
            {
                if (value != this.isCursorShown)
                    this.isCursorShown = value;
            }
        }

        /// <summary>
        /// Sets the text font.
        /// </summary>
        /// <value>Must not be a valid path.</value>
        [SkinAttribute]
        public string Font
        {
            set { this.font = new Font(value, GUIManager.ContentManager); }
        }

        /// <summary>
        /// Get/Set the text colour.
        /// </summary>
        [SkinAttribute]
        public Color Color
        {
            get { return this.color; }
            set { this.color = value; }
        }

        /// <summary>
        /// Gets the font height.
        /// </summary>
        public int TextHeight
        {
            get
            {
                int result = 0;
                if (this.font != null)
                    result = this.font.LineSpacing;
                return result;
            }
        }

        /// <summary>
        /// Gets the width of current text.
        /// </summary>
        public int TextWidth
        {
            get
            {
                int result = 0;
                if (this.font != null)
                    result = (int)(this.font.MeasureString(Text).X + 1.0f);
                return result;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="game">The currently running Game object.</param>
        /// <param name="guiManager">GUIManager that this control is part of.</param>
        public Label(Game game, GUIManager guiManager)
            : base(game, guiManager)
        {
            this.text = string.Empty;
            this.cursor = '_';
            this.isCursorShown = false;

            #region Properties
            CanHaveFocus = false;
            #endregion

            #region Set Default Properties
            Color = defaultColor;
            #endregion
        }
        #endregion

        /// <summary>
        /// Load default font.
        /// </summary>
        protected override void LoadContent()
        {
            Font = defaultFont;

            base.LoadContent();
        }

        /// <summary>
        /// Draws the text and performs clipping. Clipping is implemented by
        /// rendering to a texture, which can be kept to draw each frame, until
        /// the control is invalidated, such as a text change, a resize, colour
        /// change, or if the graphics device becomes invalidated.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with.</param>
        /// <param name="parentScissor">The scissor region of the parent control.</param>
        protected override void DrawControl(SpriteBatch spriteBatch, Rectangle scissor)
        {
            // Make sure that the text must be drawn
            if (this.font != null && (this.text.Length > 0 || this.isCursorShown))
            {
                // Should the cursor be added?
                string text = this.text;

                if (this.isCursorShown)
                    text += this.cursor;

                this.font.Draw(text, spriteBatch, new Vector2(AbsolutePosition.X, AbsolutePosition.Y), this.color, scissor);
            }
        }

        /// <summary>
        /// Automatically resizes the label to fit the text.
        /// </summary>
        public void FitToText()
        {
            Width = TextWidth;
            Height = TextHeight;
        }
    }
}