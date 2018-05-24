using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using SkiaSharp.Views;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using RemoteX.SkiaComponent;

namespace RemoteX.Controller
{
    public class SkiaController : ControllerPage
    {
        SKCanvasView CanvasView;
        public SkiaController()
        {
            CanvasView = new SKCanvasView();
            CanvasView.PaintSurface += canvasView_PaintSurface;
            ControllerContentView = CanvasView;
            Device.StartTimer(TimeSpan.FromSeconds(1 / 60f), () => { CanvasView.InvalidateSurface(); return true; });

        }

        private void canvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
        }
	}
}