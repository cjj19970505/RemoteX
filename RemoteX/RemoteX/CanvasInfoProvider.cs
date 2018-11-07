using System;
using System.Collections.Generic;
using System.Text;
using SkiaBehaviour;
using SkiaSharp;
namespace RemoteX
{
    public class CanvasInfoProvider : ICanvasInfoProvider
    {
        SKCanvas _Canvas;
        public SKCanvas Canvas
        {
            get
            {
                return _Canvas;
            }
            set
            {
                this._Canvas = value;
                DeviceClipBounds = value.DeviceClipBounds;
            }
        }
        public SKRectI DeviceClipBounds { get; private set; }
        public CanvasInfoProvider()
        {
            
        }
        public SKPoint CanvasNormalizedToCanvas(SKPoint point)
        {
            SKRectI rect = _Canvas.DeviceClipBounds;
            return new SKPoint(point.X * rect.Width, point.Y * rect.Height);
        }

        public SKPoint CanvasToCanvasNormalized(SKPoint point)
        {
            SKRectI rect = _Canvas.DeviceClipBounds;
            return new SKPoint(point.X/rect.Width, point.Y/rect.Height);
        }

        public SKPoint DeviceToCanvasPoint(SKPoint point)
        {
            return point;
        }
    }
}
