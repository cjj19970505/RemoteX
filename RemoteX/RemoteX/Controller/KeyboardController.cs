using System;
using System.Collections.Generic;
using System.Text;
using SkiaBehaviour;
using SkiaSharp;
using RemoteX.SkiaComponent;
using RemoteX.Data.Mathf;
using RemoteX.Input;

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

            /*
            RectButton rectButton = SkiaBehaviourEngine.Instantiate<RectButton>();
            rectButton.RectTransform.AnchorMin = new SKPoint(0, 0);
            rectButton.RectTransform.AnchorMax = new SKPoint(0, 0);
            rectButton.RectTransform.OffsetMin = new SKPoint(33.49999f, 33);
            rectButton.RectTransform.OffsetMax = new SKPoint(143.9f, 80);*/

            RectButton rectButton2 = SkiaBehaviourEngine.Instantiate<RectButton>();
            rectButton2.RectTransform.AnchorMin = new SKPoint(1, 0);
            rectButton2.RectTransform.AnchorMax = new SKPoint(1, 0);
            rectButton2.RectTransform.OffsetMin = new SKPoint(-148.9f, 27.79999f);
            rectButton2.RectTransform.OffsetMax = new SKPoint(-38.49998f, 74.79999f);
            RectButton rectButton3 = SkiaBehaviourEngine.Instantiate<RectButton>();
            rectButton3.RectTransform.AnchorMin = new SKPoint(0.5f, 1);
            rectButton3.RectTransform.AnchorMax = new SKPoint(0.5f, 1);
            rectButton3.RectTransform.OffsetMin = new SKPoint(-47.8f, -102.4f);
            rectButton3.RectTransform.OffsetMax = new SKPoint(62.59999f, -55.40002f);
        }

        private Vector2 _PrePos;

        protected override void Update()
        {
            base.Update();
            //SkiaBehaviourEngine.GetSkiaInputManager().OnSkiaTouchAction += _HandleSkiaTouchAction;
            SkiaInputManager skiaInputManager = SkiaBehaviourEngine.GetSkiaInputManager();
            skiaInputManager.OnSkiaTouchAction += _OnSkiaTouchAction;
            SkiaTouch[] touches = skiaInputManager.Touches;
            if (touches.Length > 0)
            {
                
            }

        }

        private void _OnSkiaTouchAction(SkiaTouch skiaTouch, TouchMotionAction action)
        {
            throw new NotImplementedException();
        }
    }
}
