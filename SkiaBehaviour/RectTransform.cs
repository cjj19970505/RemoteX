using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace SkiaBehaviour
{
    /// <summary>
    /// 照抄Unity的
    /// </summary>
    public class RectTransform: ISkiaBehaviourEngineProvider
    {
        public SkiaBehaviourEngine SkiaBehaviourEngine { get; private set; }
        public SkiaObject SkiaObject { get; private set; }
        /// <summary>
        /// Currently Useless
        /// </summary>
        public SKPoint AnchoredPosition;
        /// <summary>
        /// Currently Useless
        /// </summary>
        public SKPoint Pivot;
        /// <summary>
        /// Normalized
        /// </summary>
        public SKPoint AnchorMax;
        /// <summary>
        /// Normalized
        /// </summary>
        public SKPoint AnchorMin;
        public SKPoint OffsetMax;
        public SKPoint OffsetMin;

        public RectTransform(SkiaObject skiaObject)
        {
            this.SkiaObject = skiaObject;
            SkiaBehaviourEngine = SkiaObject.SkiaBehaviourEngine;
        }
        
        public SKRect Rect
        {
            get
            {
                SKPoint canvasAnchorMax = SkiaBehaviourEngine.CanvasNormalizedToCanvas(AnchorMax);
                SKPoint canvasAnchorMin = SkiaBehaviourEngine.CanvasNormalizedToCanvas(AnchorMin);
                SKPoint maxCorner = canvasAnchorMax + OffsetMax;
                SKPoint minCorner = canvasAnchorMin + OffsetMin;
                SKRect rect = new SKRect(minCorner.X, minCorner.Y, maxCorner.X, maxCorner.Y);
                return rect;
            }
        }
    }
}
