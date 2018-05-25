using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using SkiaSharp.Views;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using RemoteX.SkiaComponent;
using SkiaBehaviour;

namespace RemoteX.Controller
{
    public class SkiaController : ControllerPage
    {
        SKCanvasView CanvasView;
        CanvasInfoProvider canvasInfoProvider;
        protected SkiaBehaviourEngine SkiaBehaviourEngine;
        bool firstFrame = true;
        bool exit = false;
        public SkiaController()
        {
            CanvasView = new SKCanvasView();
            CanvasView.PaintSurface += canvasView_PaintSurface;
            ControllerContentView = CanvasView;
            canvasInfoProvider = new CanvasInfoProvider();
            SkiaBehaviourEngine = new SkiaBehaviourEngine(canvasInfoProvider);
            exit = false;
            Device.StartTimer(TimeSpan.FromSeconds(1 / 60f), () => { CanvasView.InvalidateSurface(); return !exit; });

        }

        protected virtual void Update()
        {
        }

        protected virtual void Start()
        {
        }
        private void canvasView_Init(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvasInfoProvider.Canvas = canvas;
        }
        private void canvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvasInfoProvider.Canvas = canvas;
            canvas.Clear(SKColors.DarkBlue);
            if(firstFrame)
            {
                Start();
                firstFrame = false;
            }
            SkiaBehaviourEngine.Update();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            exit = true;
            SkiaBehaviourEngine.DestoryEngine();
        }
    }
}