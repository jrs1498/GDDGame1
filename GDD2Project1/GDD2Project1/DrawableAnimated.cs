using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GDD2Project1
{
    class DrawableAnimated : Drawable
    {
        protected readonly int FRAME_WIDTH;
        protected readonly int FRAME_HEIGHT;
        protected readonly int TEXTURE_ROWS;
        protected readonly int TEXTURE_COLS;

        protected Dictionary<String, Animation> _animations;
        protected Animation _currentAnimation;

        //-------------------------------------------------------------------------
        public class Animation
        {
            private int _startFrame, _endFrame;
            private float _frameTime;
            private float _timeElapsed;
            private bool _loop;
            private bool _loopEnded = false;
            private int _currentFrame;

            public int StartFrame { get { return _startFrame; } }
            public int EndFrame { get { return _endFrame; } }
            public float FrameTime { get { return _frameTime; } }
            public bool Loop { get { return _loop; } }
            public int CurrentFrame { get { return _currentFrame; } }

            public Animation(int startFrame, int endFrame,
                float frameTime, bool loop = true)
            {
                _startFrame             = startFrame;
                _endFrame               = endFrame;
                _frameTime              = frameTime;
                _loop                   = loop;
            }

            public void addDeltaTime(float dt)
            {
                _timeElapsed += dt;
                while (_timeElapsed > _frameTime)
                {
                    _timeElapsed -= _frameTime;

                    if (!_loopEnded)
                    {
                        _currentFrame++;
                        if (_currentFrame > _endFrame)
                        {
                            if (!_loop) 
                            { 
                                _currentFrame = _endFrame;
                                _loopEnded = true;
                            }
                            else
                                _currentFrame = _startFrame;
                        }
                    }
                }
            }

            public void reset()
            {
                _timeElapsed = 0.0f;
                _loopEnded = false;
                _currentFrame = _startFrame;
            }
        }


        //-------------------------------------------------------------------------
        public DrawableAnimated(Texture2D texture, String name = "")
            : base(texture, name)
        {
            FRAME_WIDTH     = 78;
            FRAME_HEIGHT    = 128;
            TEXTURE_ROWS    = texture.Height / FRAME_HEIGHT;
            TEXTURE_COLS    = texture.Width / FRAME_WIDTH;

            _animations     = new Dictionary<String, Animation>();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Adds animation data to this drawable
        /// </summary>
        /// <param name="anim">Animation data</param>
        /// <param name="name">Name corresponding to this animation</param>
        public void addAnimation(Animation anim, String name)
        {
            _animations.Add(name, anim);
        }

        /// <summary>
        /// Set this Drawable's current animation state
        /// </summary>
        /// <param name="name">Name of the animation</param>
        public void setAnimation(String name)
        {
            if (!_animations.ContainsKey(name))
                return;

            _currentAnimation = _animations[name];
            _currentAnimation.reset();
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Draw this Drawable with its current animation
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used for drawing</param>
        /// <param name="position">Position to draw to</param>
        /// <param name="origin">Origin to draw in relation to</param>
        /// <param name="color">Color to apply to this texture</param>
        /// <param name="rotation">Z axis rotation to be applied</param>
        /// <param name="scale">Scale to be applied</param>
        public override void draw(
            SpriteBatch spriteBatch, 
            Vector2 position,
            Vector2 origin, 
            Color color, 
            float rotation, 
            Vector2 scale,
            float dt)
        {
            // There should be an animation state present, but if not,
            // draw the entire texture to show an obvious problem
            if (_currentAnimation == null)
                base.draw(spriteBatch, position, origin, color, rotation, scale, dt);
            else
            { 
                // Pass dt to the animation
                _currentAnimation.addDeltaTime(dt);
                int frame = _currentAnimation.CurrentFrame;

                Rectangle frameSource;
                frameSource.Width   = FRAME_WIDTH;
                frameSource.Height  = FRAME_HEIGHT;

                frameSource.X       = (frame % TEXTURE_COLS) * FRAME_WIDTH;
                frameSource.Y       = ((frame - (frame % TEXTURE_COLS)) / TEXTURE_COLS) * FRAME_HEIGHT;

                spriteBatch.Draw(
                    _texture,
                    position,
                    frameSource,
                    color,
                    rotation,
                    origin,
                    scale,
                    SpriteEffects.None,
                    0);

            }
        }
    }
}
