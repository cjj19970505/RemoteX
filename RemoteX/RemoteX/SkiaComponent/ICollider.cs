using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace RemoteX.SkiaComponent
{
    public interface ICollider
    {
        bool OverlapPoint(SKPoint point);
    }
}
