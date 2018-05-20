using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using RemoteX.Input;

namespace RemoteX.SkiaComponent
{
    public class SkiaComponent:ITransformer
    {
        public virtual SKPoint Position { get; set; }
        public SKPoint Rotation { get; set; }
        public SKPoint Scale { get; set; }

        public SkiaComponent()
        {
        }

        /// <summary>
        /// call这个来绘制
        /// </summary>
        public virtual void Draw(SKCanvas canvas)
        {
            
        }


    }
}
