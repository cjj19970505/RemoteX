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
        RectButton button;
        CircleButton circleButton;
        ColorJoystick joyStick;

        public SkiaController()
        {
            CanvasView = new SKCanvasView();
            CanvasView.PaintSurface += canvasView_PaintSurface;
            ControllerContentView = CanvasView;
            CanvasView.InvalidateSurface();
            
            button = new RectButton()
            {
                Position = new SKPoint(100, 100),
                Size = new SKSize(200, 100)
            };
            circleButton = new CircleButton()
            {
                Position = new SKPoint(500, 500),
                Radius = 200
            };
            joyStick = new ColorJoystick()
            {
                Position = new SKPoint(1000, 500)
            };

            Device.StartTimer(TimeSpan.FromSeconds(1 / 60f), () => { CanvasView.InvalidateSurface(); return true; });

        }

        private void canvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.White);
            button.Update();
            circleButton.Update();
            joyStick.Update();
            button.Draw(canvas);
            circleButton.Draw(canvas);
            joyStick.Draw(canvas);
        }
	}
}