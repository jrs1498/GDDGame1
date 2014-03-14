using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GDD2Project1
{
    public enum AnimationState
    { 
        ANIMSTATE_IDLE,
        ANIMSTATE_MOVE_SLOW,
        ANIMSTATE_MOVE_FAST,
        ANIMSTATE_JUMP,
        ANIMSTATE_ATTACK,
        ANIMSTATE_HURT
    };

    public class DrawableAnimated : Drawable
    {
        #region Animation identifiers
        public const String ANIM_IDLE_N             = "idleN";
        public const String ANIM_IDLE_NE            = "idleNE";
        public const String ANIM_IDLE_E             = "idleE";
        public const String ANIM_IDLE_SE            = "idleSE";
        public const String ANIM_IDLE_S             = "idleS";
        public const String ANIM_IDLE_SW            = "idleSW";
        public const String ANIM_IDLE_W             = "idleW";
        public const String ANIM_IDLE_NW            = "idleNW";

        public const String ANIM_MOVE_SLOW_N        = "moveslowN";
        public const String ANIM_MOVE_SLOW_NE       = "moveslowNE";
        public const String ANIM_MOVE_SLOW_E        = "moveslowE";
        public const String ANIM_MOVE_SLOW_SE       = "moveslowSE";
        public const String ANIM_MOVE_SLOW_S        = "moveslowS";
        public const String ANIM_MOVE_SLOW_SW       = "moveslowSW";
        public const String ANIM_MOVE_SLOW_W        = "moveslowW";
        public const String ANIM_MOVE_SLOW_NW       = "moveslowNW";

        public const String ANIM_MOVE_FAST_N        = "movefastN";
        public const String ANIM_MOVE_FAST_NE       = "movefastNE";
        public const String ANIM_MOVE_FAST_E        = "movefastE";
        public const String ANIM_MOVE_FAST_SE       = "movefastSE";
        public const String ANIM_MOVE_FAST_S        = "movefastS";
        public const String ANIM_MOVE_FAST_SW       = "movefastSW";
        public const String ANIM_MOVE_FAST_W        = "movefastW";
        public const String ANIM_MOVE_FAST_NW       = "movefastNW";

        public const String ANIM_JUMP_N             = "jumpN";
        public const String ANIM_JUMP_NE            = "jumpNE";
        public const String ANIM_JUMP_E             = "jumpE";
        public const String ANIM_JUMP_SE            = "jumpSE";
        public const String ANIM_JUMP_S             = "jumpS";
        public const String ANIM_JUMP_SW            = "jumpSW";
        public const String ANIM_JUMP_W             = "jumpW";
        public const String ANIM_JUMP_NW            = "jumpNW";

        public const String ANIM_ATTACK_N           = "attackN";
        public const String ANIM_ATTACK_NE          = "attackNE";
        public const String ANIM_ATTACK_E           = "attackE";
        public const String ANIM_ATTACK_SE          = "attackSE";
        public const String ANIM_ATTACK_S           = "attackS";
        public const String ANIM_ATTACK_SW          = "attackSW";
        public const String ANIM_ATTACK_W           = "attackW";
        public const String ANIM_ATTACK_NW          = "attackNW";

        public const String ANIM_HURT_N             = "hurtN";
        public const String ANIM_HURT_NE            = "hurtNE";
        public const String ANIM_HURT_E             = "hurtE";
        public const String ANIM_HURT_SE            = "hurtSE";
        public const String ANIM_HURT_S             = "hurtS";
        public const String ANIM_HURT_SW            = "hurtSW";
        public const String ANIM_HURT_W             = "hurtW";
        public const String ANIM_HURT_NW            = "hurtNW";

        public const String ANIM_DEFAULT            = ANIM_IDLE_NE;
        #endregion

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
            AnimationState  state,
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
        protected Animation getAnimation(AnimationState state, Direction direction)
        {
            String anim = ANIM_DEFAULT;

            switch (state)
            { 
                case AnimationState.ANIMSTATE_IDLE:
                    switch (direction)
                    {
                        case Direction.DIR_N:       anim = ANIM_IDLE_N;         break;
                        case Direction.DIR_NE:      anim = ANIM_IDLE_NE;        break;
                        case Direction.DIR_E:       anim = ANIM_IDLE_E;         break;
                        case Direction.DIR_SE:      anim = ANIM_IDLE_SE;        break;
                        case Direction.DIR_S:       anim = ANIM_IDLE_S;         break;
                        case Direction.DIR_SW:      anim = ANIM_IDLE_SW;        break;
                        case Direction.DIR_W:       anim = ANIM_IDLE_W;         break;
                        case Direction.DIR_NW:      anim = ANIM_IDLE_NW;        break;
                    }
                    break;

                case AnimationState.ANIMSTATE_MOVE_SLOW:
                    switch (direction)
                    {
                        case Direction.DIR_N:       anim = ANIM_MOVE_SLOW_N;    break;
                        case Direction.DIR_NE:      anim = ANIM_MOVE_SLOW_NE;   break;
                        case Direction.DIR_E:       anim = ANIM_MOVE_SLOW_E;    break;
                        case Direction.DIR_SE:      anim = ANIM_MOVE_SLOW_SE;   break;
                        case Direction.DIR_S:       anim = ANIM_MOVE_SLOW_S;    break;
                        case Direction.DIR_SW:      anim = ANIM_MOVE_SLOW_SW;   break;
                        case Direction.DIR_W:       anim = ANIM_MOVE_SLOW_W;    break;
                        case Direction.DIR_NW:      anim = ANIM_MOVE_SLOW_NW;   break;
                    }
                    break;

                case AnimationState.ANIMSTATE_MOVE_FAST:
                    switch (direction)
                    {
                        case Direction.DIR_N:       anim = ANIM_MOVE_FAST_N;    break;
                        case Direction.DIR_NE:      anim = ANIM_MOVE_FAST_NE;   break;
                        case Direction.DIR_E:       anim = ANIM_MOVE_FAST_E;    break;
                        case Direction.DIR_SE:      anim = ANIM_MOVE_FAST_SE;   break;
                        case Direction.DIR_S:       anim = ANIM_MOVE_FAST_S;    break;
                        case Direction.DIR_SW:      anim = ANIM_MOVE_FAST_SW;   break;
                        case Direction.DIR_W:       anim = ANIM_MOVE_FAST_W;    break;
                        case Direction.DIR_NW:      anim = ANIM_MOVE_FAST_NW;   break;
                    }
                    break;

                case AnimationState.ANIMSTATE_JUMP:
                    switch (direction)
                    {
                        case Direction.DIR_N:       anim = ANIM_JUMP_N;         break;
                        case Direction.DIR_NE:      anim = ANIM_JUMP_NE;        break;
                        case Direction.DIR_E:       anim = ANIM_JUMP_E;         break;
                        case Direction.DIR_SE:      anim = ANIM_JUMP_SE;        break;
                        case Direction.DIR_S:       anim = ANIM_JUMP_S;         break;
                        case Direction.DIR_SW:      anim = ANIM_JUMP_SW;        break;
                        case Direction.DIR_W:       anim = ANIM_JUMP_W;         break;
                        case Direction.DIR_NW:      anim = ANIM_JUMP_NW;        break;
                    }

                    break;

                case AnimationState.ANIMSTATE_ATTACK:
                    switch (direction)
                    {
                        case Direction.DIR_N:       anim = ANIM_ATTACK_N;       break;
                        case Direction.DIR_NE:      anim = ANIM_ATTACK_NE;      break;
                        case Direction.DIR_E:       anim = ANIM_ATTACK_E;       break;
                        case Direction.DIR_SE:      anim = ANIM_ATTACK_SE;      break;
                        case Direction.DIR_S:       anim = ANIM_ATTACK_S;       break;
                        case Direction.DIR_SW:      anim = ANIM_ATTACK_SW;      break;
                        case Direction.DIR_W:       anim = ANIM_ATTACK_W;       break;
                        case Direction.DIR_NW:      anim = ANIM_ATTACK_NW;      break;
                    }
                    break;

                case AnimationState.ANIMSTATE_HURT:
                    switch (direction)
                    {
                        case Direction.DIR_N:       anim = ANIM_HURT_N;         break;
                        case Direction.DIR_NE:      anim = ANIM_HURT_NE;        break;
                        case Direction.DIR_E:       anim = ANIM_HURT_E;         break;
                        case Direction.DIR_SE:      anim = ANIM_HURT_SE;        break;
                        case Direction.DIR_S:       anim = ANIM_HURT_S;         break;
                        case Direction.DIR_SW:      anim = ANIM_HURT_SW;        break;
                        case Direction.DIR_W:       anim = ANIM_HURT_W;         break;
                        case Direction.DIR_NW:      anim = ANIM_HURT_NW;        break;
                    }
                    break;
            }

            if (!_animations.ContainsKey(anim))
                return _animations[ANIM_DEFAULT];
            return _animations[anim];
        }
    }
}
