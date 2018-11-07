using SkiaBehaviour;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX.SkiaBehaviour.Tester
{
    class CanvasInfoProvider : ICanvasInfoProvider
    {
        public SKCanvas Canvas { get; set; }

        public SKRectI DeviceClipBounds { get; set; }
    }
}
