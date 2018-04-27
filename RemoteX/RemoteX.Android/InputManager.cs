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
using RemoteX.Mathf;

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
            //int pointerId = (event.getAction()&MotionEvent.ACTION_POINTER_ID_MASK)>>> MotionEvent.ACTION_POINTER_ID_SHIFT; 
            int pointerId = ((int)(e.Action & MotionEventActions.PointerIndexMask))>>((int)(MotionEventActions.PointerIndexShift));
            int down = (int)(e.ActionMasked & MotionEventActions.PointerDown);
            int up = (int)(e.ActionMasked & MotionEventActions.PointerUp);
            //System.Diagnostics.Debug.WriteLine("[" + pointerId + ": DOWN::" + down + ", UP::" + up + "]");
            //Touch touch;
            //System.Diagnostics.Debug.WriteLine(e.GetPointerId(pointerId) + " " + e.ActionMasked);
            
            switch(e.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    {
                        Touch touch = new Touch(pointerId);
                        _Touches.Add(pointerId, touch);
                        //System.Diagnostics.Debug.WriteLine(pointerId + " " + e.ActionMasked);
                        break;
                    }
                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                    {
                        _Touches.Remove(pointerId);
                        //System.Diagnostics.Debug.WriteLine(pointerId + " " + "Up");
                        break;
                    }
                case MotionEventActions.Move:
                    {
                        e.
                        break;
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