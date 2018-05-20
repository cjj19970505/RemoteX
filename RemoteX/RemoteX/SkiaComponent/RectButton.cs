using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using RemoteX.SkiaComponent.Physics;

namespace RemoteX.SkiaComponent
{
    public class RectButton : Button
    {
        private SKSize _Size;
        public SKSize Size
        {
            get
            {
                return _Size;
            }
            set
            {
                _Size = value;
                SKRect rect = (Collider as RectangleCollider).Rect;
                rect.Size = value;
                (Collider as RectangleCollider).Rect = rect;
            }
        }
        public float Height { get; set; }
        
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
                SKRect rect = (Collider as RectangleCollider).Rect;
                rect.Left = value.X;
                rect.Top = value.Y;
                (Collider as RectangleCollider).Rect = rect;
            }
        }

        SKPaint blackFillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Black

        };
        public RectButton():base()
        {
            Collider = new RectangleCollider();
            Size = new SKSize(100, 100);
            Position = new SKPoint(0, 0);
        }

        public override void Draw(SKCanvas canvas)
        {
            SKRect rect = (Collider as RectangleCollider).Rect;
            canvas.DrawRect(rect, blackFillPaint);
        }

        protected override void OnPressed()
        {
            blackFillPaint.Color = SKColors.Red;
        }

        protected override void OnUp()
        {
            blackFillPaint.Color = SKColors.Black;
        }




        //public override 

    }
}
