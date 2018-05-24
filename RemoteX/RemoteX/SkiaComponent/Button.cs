using System;
using System.Collections.Generic;
using System.Text;
using SkiaBehaviour;
using SkiaSharp;
using Xamarin.Forms;
using RemoteX.Input;

namespace RemoteX.SkiaComponent
{
    public class Button:SkiaObject
    {
        protected IArea Area { get; set; }
        public Button(SkiaBehaviourEngine skiaBehaviourEngine):base(skiaBehaviourEngine)
        {
            
        }
    }
}
