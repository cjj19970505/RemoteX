using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using RemoteX.SkiaComponent.Physics;
using SkiaSharp;

namespace RemoteX.SkiaComponent
{
    class CircleButton:Button
    {
        private float _Radius;

        public float Radius
        {
            get
            {
                return _Radius;
            }
            set
            {
                _Radius = value;
                if(Collider == null)
                {
                    return;
                }
                (Collider as CircleCollider).Radius = _Radius;
            }
        }
        public override SKPoint Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                if(Collider == null)
                {
                    return;
                }
                (Collider as CircleCollider).Position = Position;
            }
        }

        SKBitmap buttonPic;
        public CircleButton():base()
        {
            Collider = new CircleCollider();
            string resourceID = "RemoteX.UI.Icon.JoystickButton_Up.png";
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                buttonPic = SKBitmap.Decode(skStream);
            }
        }

        SKPaint blackFillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Black,
        };


        public override void Draw(SKCanvas canvas)
        {
            canvas.DrawCircle(Position, Radius, blackFillPaint);
            SKRect buttonRect = new SKRect(Position.X - Radius, Position.Y - Radius, Position.X + Radius, Position.Y + Radius);
            canvas.DrawBitmap(buttonPic, buttonRect);
        }
        protected override void OnPressed()
        {
            base.OnPressed();
            blackFillPaint.Color = SKColors.Red;
        }

        protected override void OnUp()
        {
            base.OnUp();
            blackFillPaint.Color = SKColors.Black;
        }
    }
}
