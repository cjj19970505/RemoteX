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
        public SkiaObject GetSkiaObject(int index)
        {
            return skiaObjects[index];
        }
        public int SkiaObjectCount
        {
            get
            {
                return skiaObjects.Count;
            }
        }
        public SkiaBehaviourEngine(ICanvasInfoProvider canvasInfoProvider)
        {
            CanvasInfoProvider = canvasInfoProvider;
            skiaObjects = new List<SkiaObject>();
        }
        public SKPoint CanvasNormalizedToCanvas(SKPoint point)
        {
            SKRectI rect = CanvasInfoProvider.DeviceClipBounds;
            return new SKPoint(point.X * rect.Width, point.Y * rect.Height);
        }
        public SKPoint CanvasToCanvasNormalized(SKPoint point)
        {
            SKRectI rect = CanvasInfoProvider.DeviceClipBounds;
            return new SKPoint(point.X / rect.Width, point.Y / rect.Height);
        }

        public T Instantiate<T>() where T:SkiaObject, new()
        {
            T skiaObject = new T();
            skiaObjects.Add(skiaObject);
            skiaObject.InitByEngine(this);
            return skiaObject;
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

        public void Destory(SkiaObject skiaObject)
        {
            skiaObject.DestoryMark = true;
        }
        /// <summary>
        /// 想要整片画布都不显示的话要中止这个引擎
        /// </summary>
        public void DestoryEngine()
        {
            foreach(SkiaObject skiaObject in skiaObjects)
            {
                skiaObject.Destory();
            }
            skiaObjects.Clear();
        }


    }
}
