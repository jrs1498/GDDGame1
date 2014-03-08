using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GDD2Project1
{
    public class Animation : ContentObject
    {
        private int     _startFrame;
        private int     _endFrame;
        private int     _numFrames;
        private float   _frameTime;
        private float   _animTime;
        private bool    _looping;


        //-------------------------------------------------------------------------
        /// <summary>
        /// Primary constructor for Animation. This object should only be created by
        /// GameContentManager from XML data, contained within DrawableAnimated data.
        /// </summary>
        /// <param name="startFrame">First frame of the animation</param>
        /// <param name="endFrame">Last frame of the animation</param>
        /// <param name="frameTime">Seconds between frames</param>
        /// <param name="looping">Does this animation loop?</param>
        public Animation(
            String  name,
            int     startFrame,
            int     endFrame,
            float   frameTime,
            bool    looping)
            : base(name)
        {
            _startFrame     = startFrame;
            _endFrame       = endFrame;
            _numFrames      = (_endFrame - _startFrame) + 1;
            _frameTime      = frameTime;
            _animTime       = _numFrames * _frameTime;
            _looping        = looping;
        }


        //-------------------------------------------------------------------------
        /// <summary>
        /// Get the source rectangle for the current frame of an animation sequence.
        /// </summary>
        /// <param name="frameWidth">Width of a frame</param>
        /// <param name="frameHeight">Height of a frame</param>
        /// <param name="frameRows">Number of frame rows in the texture</param>
        /// <param name="frameCols">Number of frame columns in the texture</param>
        /// <param name="time">Time in animation</param>
        /// <returns>Source rectangle corresponding to the frame</returns>
        public Rectangle getFrame(
            int     frameWidth,
            int     frameHeight,
            int     frameRows,
            int     frameCols,
            float   time)
        {
            Rectangle frame;
            frame.Width     = frameWidth;
            frame.Height    = frameHeight;

            if (!_looping)
            {
                if (time > _frameTime)
                    time = _frameTime;
            } 
            else 
                time %= _frameTime;

            int frameindex = (int)Math.Floor(((float)(time / _frameTime) * _numFrames));
            if (frameindex >= _numFrames)
                frameindex = _numFrames - 1;
            frameindex += _startFrame;

            frame.X = (frameindex % frameCols) * frameWidth;
            frame.Y = ((frameindex - (frameindex % frameCols)) / frameCols) * frameHeight;

            return frame;
        }
    }
}
