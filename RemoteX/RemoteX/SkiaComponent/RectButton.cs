using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaBehaviour;
using RemoteX;
using RemoteX.Service;
using Xamarin.Forms;

namespace RemoteX.SkiaComponent
{
    class RectButton:Button
    {
        protected override IArea Area
        {
            get
            {
                return new RectArea(RectTransform.Rect);
            }
        }
        SKPaint blackFillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Black
        };
        protected override void OnButtonPressed()
        {
            blackFillPaint.Color = SKColors.Red;
            IVibrator vibrator = DependencyService.Get<IVibrator>();
            vibrator.Vibrate();
            
        }
        protected override void OnButtonUp()
        {
            blackFillPaint.Color = SKColors.Black;
        }
        public override void Draw()
        {
            base.Draw();
            SKCanvas canvas = SkiaBehaviourEngine.CanvasInfoProvider.Canvas;
            canvas.DrawRect(RectTransform.Rect, blackFillPaint);
        }

    }
}
