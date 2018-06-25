﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using SkiaBehaviour;

namespace RemoteX.SkiaComponent
{
    static class Extensions
    {
        public static float Magnitude(this SKPoint self)
        {
            return (float)Math.Sqrt(self.X * self.X + self.Y * self.Y);
        }

        public static SKPoint Mid(this SKRect self)
        {
            return new SKPoint(self.MidX, self.MidY);
        }

        public static SkiaInputManager GetSkiaInputManager(this SkiaBehaviourEngine self)
        {
            for(int i = 0; i < self.SkiaObjectCount; i++)
            {
                SkiaObject skiaObject = self.GetSkiaObject(i);
                if(skiaObject is SkiaInputManager)
                {
                    return skiaObject as SkiaInputManager;
                }
            }
            return null;
        }
    }
}
