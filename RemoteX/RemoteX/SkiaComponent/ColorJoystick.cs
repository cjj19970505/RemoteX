using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.SkiaComponent.Physics;
using SkiaSharp;
using Xamarin.Forms;

namespace RemoteX.SkiaComponent
{
    class ColorJoystick : Joystick
    {
        public override SKPoint Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                if (StartRegion == null)
                {
                    return;
                }
                (StartRegion as CircleCollider).Position = value;
            }
        }

        public ColorJoystick() : base()
        {
            StartRegion = new CircleCollider()
            {
                Radius = 200
            };
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
        public override void Draw(SKCanvas canvas)
        {
            float radius = (StartRegion as CircleCollider).Radius;
            canvas.DrawCircle(Position, radius, paint);
            if(Pressed)
            {
                float factor;
                
                if(Direction<0)
                {
                    factor = (Direction + 360) / 360;
                }
                else
                {
                    factor = (Direction) / 360;
                }
                System.Diagnostics.Debug.WriteLine(Distance);
                SKColor baseColor = new SKColor(255, (byte)(255*factor), 0);
                dragPaint.Color = baseColor;
                canvas.DrawCircle(Position, Distance, dragPaint);
            }

        }


    }
}
