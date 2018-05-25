using System;
using System.Collections.Generic;
using System.Text;
using SkiaBehaviour;
using SkiaSharp;
using RemoteX.SkiaComponent;
namespace RemoteX.Controller
{
    class KeyboardController:SkiaController
    {
        protected override void Start()
        {
            RectButton rectButton = SkiaBehaviourEngine.Instantiate<RectButton>();
            rectButton.RectTransform.AnchorMax = new SKPoint(0, 0);
            rectButton.RectTransform.AnchorMin = new SKPoint(0, 0);
            rectButton.RectTransform.OffsetMax = new SKPoint(1000, 1000);
            rectButton.RectTransform.OffsetMin = new SKPoint(500, 250);

            ColorJoystick colorJoystick = SkiaBehaviourEngine.Instantiate<ColorJoystick>();
            colorJoystick.RectTransform.AnchorMax = new SKPoint(0, 0);
            colorJoystick.RectTransform.AnchorMin = new SKPoint(0, 0);
            colorJoystick.RectTransform.OffsetMax = new SKPoint(2000, 500);
            colorJoystick.RectTransform.OffsetMin = new SKPoint(500, 250);
        }
    }
}
