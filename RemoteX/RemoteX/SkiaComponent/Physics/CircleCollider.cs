using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using RemoteX.SkiaComponent;

namespace RemoteX.SkiaComponent.Physics
{
    class CircleCollider : ICollider
    {
        public SKPoint Position { get; set; }
        public float Radius { get; set; }
        public bool OverlapPoint(SKPoint point)
        {
            if((point-Position).Magnitude()<Radius)
            {
                return true;
            }
            return false;
        }
    }
}
