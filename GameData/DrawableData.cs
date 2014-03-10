using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameData
{
    public class DrawableData
    {
        public String TextureName;
        public Vector2 Origin;

        public DrawableData()
        { }

        public DrawableData(
            String pTextureName, 
            Vector2 pOrigin)
        {
            TextureName     = pTextureName;
            Origin          = pOrigin;
        }
    }

    public class DrawableAnimatedData : DrawableData
    {
        public AnimationData[] Animations;
        public int FrameWidth;
        public int FrameHeight;
        public int FrameRows;
        public int FrameCols;

        public DrawableAnimatedData()
        { }

        public DrawableAnimatedData(
            String pTextureName, 
            Vector2 pOrigin,
            int pFrameWidth, 
            int pFrameHeight, 
            int pFrameRows, 
            int pFrameCols,
            AnimationData[] pAnimations)
            : base(pTextureName, pOrigin)
        {
            FrameWidth      = pFrameWidth;
            FrameHeight     = pFrameHeight;
            FrameRows       = pFrameRows;
            FrameCols       = pFrameCols;
            Animations      = pAnimations;
        }
    }

    public class AnimationData
    {
        public String   Name;
        public int      StartFrame;
        public int      EndFrame;
        public float    FrameTime;
        public bool     Looping;

        public AnimationData()
        { }

        public AnimationData(
            String  pName,
            int     pStartFrame,
            int     pEndFrame,
            float   pFrameTime,
            bool    pLooping)
        {
            Name            = pName;
            StartFrame      = pStartFrame;
            EndFrame        = pEndFrame;
            FrameTime       = pFrameTime;
            Looping         = pLooping;
        }
    }

    public class DrawablePackageData
    {
        public DrawableData[] _drawables;

        public DrawablePackageData()
        { }
    }
}
