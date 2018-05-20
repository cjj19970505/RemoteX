using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace RemoteX.SkiaComponent
{
    public static class Extensions
    {
        public static float Magnitude(this SKPoint self)
        {
            return (float)Math.Sqrt(self.X * self.X + self.Y * self.Y);
        }
    }
}
