using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GDD2Project1
{
    public class DrawableAnimated : Drawable
    {
        // Animation identifiers
        public const String ANIM_IDLE_N      = "idleN";
        public const String ANIM_IDLE_NE     = "idleNE";
        public const String ANIM_IDLE_E      = "idleE";
        public const String ANIM_IDLE_SE     = "idleSE";
        public const String ANIM_IDLE_S      = "idleS";
        public const String ANIM_IDLE_SW     = "idleSW";
        public const String ANIM_IDLE_W      = "idleW";
        public const String ANIM_IDLE_NW     = "idleNW";

        public const String ANIM_MOVE_N      = "moveN";
        public const String ANIM_MOVE_NE     = "moveNE";
        public const String ANIM_MOVE_E      = "moveE";
        public const String ANIM_MOVE_SE     = "moveSE";
        public const String ANIM_MOVE_S      = "moveS";
        public const String ANIM_MOVE_SW     = "moveSW";
        public const String ANIM_MOVE_W      = "moveW";
        public const String ANIM_MOVE_NW     = "moveNW";

        public const String ANIM_DEFAULT     = ANIM_IDLE_N;

        // Frame attributes
        protected readonly int FRAME_WIDTH;
        protected readonly int FRAME_HEIGHT;
        protected readonly int FRAME_ROWS;
        protected readonly int FRAME_COLS;

        protected Dictionary<String, Animation> _animations;


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary DrawableAnimated constructor. This object should only be created
        /// by a GameContentManager from an XML file, using this constructor. See function
        /// GameContentManager.loadDrawableAnimated.
        /// </summary>
        /// <param name="name">This object's name, for reference purposes</param>
        /// <param name="texture">Texture used for drawing</param>
        /// <param name="origin">Origin used for drawing</param>
        /// <param name="frameWidth">Width of a single frame</param>
        /// <param name="frameHeight">Height of a single frame</param>
        /// <param name="frameRows">Total frame rows in the texture</param>
        /// <param name="frameCols">Total frame columns in the texture</param>
        /// <param name="animations">This Drawable's animation data</param>
        public DrawableAnimated(
            String      name,
            Texture2D   texture,
            Vector2     origin,
            int         frameWidth,
            int         frameHeight,
            int         frameRows,
            int         frameCols,
            Animation[] animations)
            : base(name, texture, origin)
        {
            FRAME_WIDTH     = frameWidth;
            FRAME_HEIGHT    = frameHeight;
            FRAME_ROWS      = frameRows;
            FRAME_COLS      = frameCols;

            _animations = new Dictionary<string, GDD2Project1.Animation>();
            foreach (GDD2Project1.Animation anim in animations)
                _animations.Add(anim.Name, anim);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Draws this DrawableAnimated. Using the supplied information, the correct frame
        /// is determined entirely in this class. Time must be a time representing how much
        /// time the containing object has been in its current state.
        /// </summary>
        /// <param name="spriteBatch">Used for drawing textures</param>
        /// <param name="position">Position to draw to</param>
        /// <param name="color">Color to draw with</param>
        /// <param name="rotation">Rotation amount</param>
        /// <param name="scale">Scale amount</param>
        /// <param name="time">Time in current animation</param>
        /// <param name="state">Current state</param>
        /// <param name="direction">Current direction</param>
        public virtual void draw(
            SpriteBatch     spriteBatch,
            Vector2         position,
            Color           color,
            float           rotation,
            Vector2         scale,
            float           time,
            CharacterState  state,
            Direction       direction)
        {
            Rectangle frame = getAnimation(state, direction).getFrame(
                FRAME_WIDTH,
                FRAME_HEIGHT,
                FRAME_ROWS,
                FRAME_COLS,
                time);

            spriteBatch.Draw(
                _texture,
                position,
                frame,
                color,
                rotation,
                _origin,
                scale,
                SpriteEffects.None,
                0.0f);
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Determines the appropriate animation according to the current state and
        /// direction of the object in question.
        /// </summary>
        /// <param name="state">State of the object containing this DrawableAnimated</param>
        /// <param name="direction">Direction of the object containing this DrawableAnimated</param>
        /// <returns>Appropriate animation</returns>
        protected Animation getAnimation(
            CharacterState  state,
            Direction       direction)
        {
            String anim = ANIM_DEFAULT;

            switch (state)
            { 
                case CharacterState.CHRSTATE_IDLE:
                    switch (direction)
                    { 
                        case Direction.DIR_NE:
                            anim = ANIM_IDLE_NE;
                            break;

                        case Direction.DIR_SE:
                            anim = ANIM_IDLE_SE;
                            break;

                        case Direction.DIR_SW:
                            anim = ANIM_IDLE_SW;
                            break;

                        case Direction.DIR_NW:
                            anim = ANIM_IDLE_NW;
                            break;
                    }
                    break;

                case CharacterState.CHRSTATE_MOVING:
                    switch (direction)
                    {
                        case Direction.DIR_NE:
                            anim = ANIM_MOVE_NE;
                            break;

                        case Direction.DIR_SE:
                            anim = ANIM_MOVE_SE;
                            break;

                        case Direction.DIR_SW:
                            anim = ANIM_MOVE_SW;
                            break;

                        case Direction.DIR_NW:
                            anim = ANIM_MOVE_NW;
                            break;
                    }
                    break;
            }

            return _animations[anim];
        }
    }
}
