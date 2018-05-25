using System;
using System.Collections.Generic;
using System.Text;
using SkiaBehaviour;
using SkiaSharp;
using RemoteX.Input;
using Xamarin.Forms;

namespace RemoteX.SkiaComponent
{
    abstract class Joystick:SkiaObject
    {
        protected ITouch OnTouch { get; private set; }
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
                if (OnTouch != null)
                {
                    return true;
                }
                return false;
            }
        }
        SKPoint startPos;
        protected override void Init()
        {
            DependencyService.Get<IInputManager>().OnTouchAction += handleTouchAction;
        }
        void handleTouchAction(ITouch touch, TouchMotionAction action)
        {
            CanvasInfoProvider canvasInfoProvider = SkiaBehaviourEngine.CanvasInfoProvider as CanvasInfoProvider;
            if (action == TouchMotionAction.Down)
            {
                if (OnTouch == null && StartRegion.IsOverlapPoint(canvasInfoProvider.DeviceToCanvasPoint(touch.Position)))
                {
                    OnTouch = touch;
                    Distance = 0;
                    Direction = 0;
                    startPos = touch.Position;
                    OnJoystickPressed();
                }
            }
            else if (action == TouchMotionAction.Up)
            {
                if (OnTouch == touch)
                {
                    Distance = 0;
                    Direction = 0;
                    OnTouch = null;
                    OnJoystickUp();
                }
            }
            else if (action == TouchMotionAction.Move)
            {
                if (OnTouch == touch)
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
            DependencyService.Get<IInputManager>().OnTouchAction -= handleTouchAction;
        }

    }
}
