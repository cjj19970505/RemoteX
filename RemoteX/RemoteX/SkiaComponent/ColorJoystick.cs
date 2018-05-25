using System;
using System.Collections.Generic;
using System.Text;
using SkiaBehaviour;
using SkiaSharp;

namespace RemoteX.SkiaComponent
{
    class ColorJoystick:Joystick
    {
        protected override IArea StartRegion
        {
            get
            {
                return new CircleArea(Math.Min(RectTransform.Rect.Width/2, RectTransform.Rect.Height / 2), RectTransform.Rect.Mid());
            }
        }
        SKPaint paint = new SKPaint()
        {
            Color = SKColors.AliceBlue,
            Style = SKPaintStyle.Fill
        };
        SKPaint dragPaint = new SKPaint()
        {
            Style = SKPaintStyle.Fill
        };

        protected override void OnJoystickMove()
        {
        }

        protected override void OnJoystickPressed()
        {
        }

        protected override void OnJoystickUp()
        {
        }

        public override void Draw()
        {

            float radius = ((CircleArea)StartRegion).Radius;
            SKCanvas canvas = SkiaBehaviourEngine.CanvasInfoProvider.Canvas;
            canvas.DrawCircle(((CircleArea)StartRegion).Position, radius, paint);
            if (Pressed)
            {
                float factor;

                if (Direction < 0)
                {
                    factor = (Direction + 360) / 360;
                }
                else
                {
                    factor = (Direction) / 360;
                }
                SKColor baseColor = new SKColor(255, (byte)(255 * factor), 0);
                dragPaint.Color = baseColor;
                canvas.DrawCircle(((CircleArea)StartRegion).Position, Distance, dragPaint);
            }
        }
    }
}
