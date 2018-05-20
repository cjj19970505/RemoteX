using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.Input;
using Xamarin.Forms;
using SkiaSharp;
using RemoteX.SkiaComponent.Physics;

namespace RemoteX.SkiaComponent
{
    public class Joystick : SkiaComponent
    {
        protected IInputManager InputManager;

        /// <summary>
        /// 这个时候在摇杆上的Touch
        /// </summary>
        protected ITouch OnTouch;

        private float _Direction;

        /// <summary>
        /// 用角度表示
        /// </summary>
        public float Direction
        {
            get
            {
                return _Direction;
            }
            private set
            {
                _Direction = value;
            }
        }

        private float _Distance;
        public float Distance
        {
            get
            {
                return _Distance;
            }
            private set
            {
                _Distance = value;
            }
        }

        private ICollider _StartRegion;
        public ICollider StartRegion
        {
            get
            {
                return _StartRegion;
            }
            protected set
            {
                _StartRegion = value;
            }
        }

        private SKPoint startPos;

        public bool Pressed
        {
            get
            {
                if(OnTouch != null)
                {
                    return true;
                }
                return false;
            }
        }

        public Joystick()
        {
            InputManager = DependencyService.Get<IInputManager>();
            InputManager.OnTouchAction += onTouchEvent;
        }

        void onTouchEvent(ITouch touch, TouchMotionAction action)
        {
            if (action == TouchMotionAction.Down)
            {
                if (OnTouch == null && StartRegion.OverlapPoint(touch.Position))
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
        protected virtual void OnJoystickMove()
        {

        }
        protected virtual void OnJoystickPressed()
        {

        }
        protected virtual void OnJoystickUp()
        {

        }
        public virtual void Update()
        {

        }

    }
}
