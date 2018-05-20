using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace RemoteX.SkiaComponent
{
    interface ITransformer
    {
        SKPoint Position { get; set; }
        SKPoint Rotation { get; set; }
        SKPoint Scale { get; set; }
    }
}
