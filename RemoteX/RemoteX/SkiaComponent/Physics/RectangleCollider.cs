using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SkiaSharp;

namespace RemoteX.SkiaComponent.Physics
{
    class RectangleCollider : ICollider
    {
        public SKRect Rect { get; set; }
        public bool OverlapPoint(SKPoint point)
        {
            if(Rect.Contains(point))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
