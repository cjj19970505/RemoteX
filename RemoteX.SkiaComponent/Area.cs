using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace RemoteX.SkiaComponent
{
    public interface IArea
    {
        bool IsOverlapPoint(SKPoint point);
    }

    public struct RectArea : IArea
    {
        public SKRect Rect { get; set; }
        public RectArea(SKRect rect)
        {
            Rect = rect;
        }
        public bool IsOverlapPoint(SKPoint point)
        {
            if(Rect.Contains(point))
            {
                return true;
            }
            return false;
        }
    }

    public struct CircleArea:IArea
    {
        public float Radius { get; set; }
        public SKPoint Position { get; set; }
        public CircleArea(float radius, SKPoint position)
        {
            Radius = radius;
            Position = position;
        }

        public bool IsOverlapPoint(SKPoint point)
        {
            SKPoint delta = point - Position;
            float magsqrt = delta.X * delta.X + delta.Y * delta.Y;
            if (magsqrt < Radius*Radius)
            {
                return true;
            }
            return false;
        }
    }
}
