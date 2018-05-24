using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkiaBehaviour
{
    public class SkiaBehaviourEngine
    {
        public ICanvasInfoProvider CanvasInfoProvider { get; private set; }

        public List<SkiaObject> skiaObjects;

        public SkiaBehaviourEngine(ICanvasInfoProvider canvasInfoProvider)
        {
            CanvasInfoProvider = canvasInfoProvider;
        }

        public SKPoint CanvasNormalizedToCanvas(SKPoint point)
        {
            SKRectI rect = CanvasInfoProvider.Canvas.DeviceClipBounds;
            return new SKPoint(point.X * rect.Width, point.Y * rect.Height);
        }

        public SKPoint CanvasToCanvasNormalized(SKPoint point)
        {
            SKRectI rect = CanvasInfoProvider.Canvas.DeviceClipBounds;
            return new SKPoint(point.X / rect.Width, point.Y / rect.Height);
        }

        public void Update()
        {
            foreach(SkiaObject skiaObject in skiaObjects)
            {
                skiaObject.Update();
            }

            foreach (SkiaObject skiaObject in skiaObjects)
            {
                skiaObject.Draw();
            }
        }
    }
}
