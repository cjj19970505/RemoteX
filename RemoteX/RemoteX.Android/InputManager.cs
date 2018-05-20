using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RemoteX.Input;
using RemoteXDataLibary.Mathf;

[assembly: Xamarin.Forms.Dependency(typeof(RemoteX.Droid.InputManager))]
namespace RemoteX.Droid
{
    class InputManager:Java.Lang.Object, IInputManager
    {
        private Dictionary<int, Touch> _Touches;
        public event TouchMotionHandler OnTouchAction;
        public InputManager()
        {
            _Touches = new Dictionary<int, Touch>();
        }

        public ITouch[] Touches
        {
            get
            {
                return _Touches.Values.ToArray();
            }
        }

        public bool OnTouch(MotionEvent e)
        {
            int pointerIndex = ((int)(e.Action & MotionEventActions.PointerIndexMask))>>((int)(MotionEventActions.PointerIndexShift));
            int pointerId = e.GetPointerId(pointerIndex);
            int down = (int)(e.ActionMasked & MotionEventActions.PointerDown);
            int up = (int)(e.ActionMasked & MotionEventActions.PointerUp);
            lock(_Touches)
            {
                switch (e.ActionMasked)
                {
                    case MotionEventActions.Down:
                    case MotionEventActions.PointerDown:
                        {
                            Touch touch = new Touch(pointerId);
                            touch.Position = new Vector2(e.GetX(pointerIndex), e.GetY(pointerIndex));
                            _Touches.Add(pointerId, touch);
                            OnTouchAction?.Invoke(touch, TouchMotionAction.Down);
                            break;
                        }
                    case MotionEventActions.Up:
                    case MotionEventActions.PointerUp:
                        {
                            Touch touch = _Touches[pointerId];
                            _Touches.Remove(pointerId);
                            OnTouchAction?.Invoke(touch, TouchMotionAction.Up);
                            break;
                        }
                    case MotionEventActions.Move:
                        {
                            foreach (var pair in _Touches)
                            {
                                int pIndex = e.FindPointerIndex(pair.Key);
                                Vector2 currentPos = new Vector2(e.GetX(pIndex), e.GetY(pIndex));
                                if (pair.Value.Position != currentPos)
                                {
                                    pair.Value.Position = currentPos;
                                    OnTouchAction?.Invoke(pair.Value, TouchMotionAction.Move);
                                }
                            }
                            break;
                        }
                }
            }
            return true;
        }
        private class Touch:ITouch
        {
            public List<Vector2> HistoryPosition { get; }
            public Vector2 Position { get; set; }
            public int Id { get; private set; }

            public Touch(int id)
            {
                this.Id = id;
                this.Position = Vector2.Zero;
            }

            public override string ToString()
            {
                return "[" + Id + " " + Position + "]";
            }
        }
    }
}