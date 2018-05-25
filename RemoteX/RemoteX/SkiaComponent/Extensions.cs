using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
