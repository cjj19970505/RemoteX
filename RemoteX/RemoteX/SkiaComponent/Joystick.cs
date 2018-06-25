using System;
using System.Collections.Generic;
using System.Text;
using SkiaBehaviour;
using SkiaSharp;
using RemoteX.Input;
using Xamarin.Forms;

namespace RemoteX.SkiaComponent
{
    abstract class Joystick:SkiaObject, ISkiaInputComponent
    {
        protected SkiaTouch OnSkiaTouch { get; private set; }
        protected virtual IArea StartRegion { get; }
        public float Distance { get; private set; }

        /// <summary>
        /// In Degree
        /// </summary>
        public float Direction { get; private set; }
        public bool Pressed
        {
            get
            {
                if (OnSkiaTouch != null)
                {
                    return true;
                }
                return false;
            }
        }

        public int InputHeightLevel
        {
            get
            {
                return 2;
            }
        }

        public IArea FirstTouchArea
        {
            get
            {
                return StartRegion;
            }
        }

        SKPoint startPos;
        protected override void Init()
        {
            SkiaBehaviourEngine.GetSkiaInputManager().OnSkiaTouchAction += handleSkiaTouchAction;
            
        }

        private void handleSkiaTouchAction(SkiaTouch skiaTouch, TouchMotionAction action)
        {
            if(skiaTouch.HeightLevel != InputHeightLevel)
            {
                return;
            }
            ITouch touch = skiaTouch.Touch;
            CanvasInfoProvider canvasInfoProvider = SkiaBehaviourEngine.CanvasInfoProvider as CanvasInfoProvider;
            if (action == TouchMotionAction.Down)
            {
                if (OnSkiaTouch == null && StartRegion.IsOverlapPoint(canvasInfoProvider.DeviceToCanvasPoint(touch.Position)))
                {
                    OnSkiaTouch = skiaTouch;
                    Distance = 0;
                    Direction = 0;
                    startPos = touch.Position;
                    OnJoystickPressed();
                }
            }
            else if (action == TouchMotionAction.Up)
            {
                if (OnSkiaTouch == skiaTouch)
                {
                    Distance = 0;
                    Direction = 0;
                    OnSkiaTouch = null;
                    OnJoystickUp();
                }
            }
            else if (action == TouchMotionAction.Move)
            {
                if (OnSkiaTouch == skiaTouch)
                {
                    SKPoint currentPos = touch.Position;
                    float distance = (currentPos - startPos).Magnitude();
                    float degree = (float)(Math.Atan2((currentPos - startPos).Y, (currentPos - startPos).X) * (180 / Math.PI));
                    this.Distance = distance;
                    this.Direction = degree;
                    OnJoystickMove();
                }
            }

        }

        protected abstract void OnJoystickPressed();

        protected abstract void OnJoystickUp();

        protected abstract void OnJoystickMove();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SkiaBehaviourEngine.GetSkiaInputManager().OnSkiaTouchAction -= handleSkiaTouchAction;
        }

    }
}
