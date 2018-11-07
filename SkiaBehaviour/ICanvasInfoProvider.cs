using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace SkiaBehaviour
{
    /// <summary>
    /// 需要各个平台自己定义的组建
    /// </summary>
    public interface ICanvasInfoProvider
    {
        SKCanvas Canvas { get; }
        SKRectI DeviceClipBounds { get;}

        /// <summary>
        /// May requiring DPI to be correctly set
        /// </summary>
        //SKMatrix CanvasPixelSpaceToCanvasPhysicsSpaceMatrix { get; }
        //SKMatrix ComponentPixelSpaceToIdealPixelSpaceMatrix { get; }
        //SKMatrix AppPixelSpaceToComponentPixelSpaceMatrix { get; }



    }
}
