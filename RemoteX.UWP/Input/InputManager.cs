using RemoteX.Data.Mathf;
using RemoteX.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.Devices.Input;


namespace RemoteX.UWP.Input
{
    public class InputManager : IInputManager
    {
        public ITouch[] Touches => throw new NotImplementedException();
        public event TouchMotionHandler OnTouchAction;

        public UIElement TouchHandleElement { get; }

        private List<Touch> _Touches;
        public float EpxToPxCoefficient = 1;

        public InputManager(UIElement touchHandleElement)
        {
            _Touches = new List<Touch>();
            TouchHandleElement = touchHandleElement;
            touchHandleElement.PointerPressed += TouchHandleElement_PointerPressed;
            touchHandleElement.PointerMoved += TouchHandleElement_PointerMoved;
            touchHandleElement.PointerReleased += TouchHandleElement_PointerReleased;
        }

        private void TouchHandleElement_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var touch = _GetExistTouchFromUwpPointer(e.Pointer);
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
            {
                return;
            }
            if (touch == null)
            {
                throw new NotImplementedException();
            }
            _Touches.Remove(touch);
            OnTouchAction?.Invoke(touch, TouchMotionAction.Up);
        }

        private void TouchHandleElement_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            
            var touch = _GetExistTouchFromUwpPointer(e.Pointer);
            if(e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
            {
                return;
            }
            if(touch == null)
            {
                throw new NotImplementedException();
            }
            var newPosition = e.GetCurrentPoint(null).Position.ToVector2() * EpxToPxCoefficient;
            touch.Position = newPosition;
            OnTouchAction?.Invoke(touch, TouchMotionAction.Move);
        }

        private void TouchHandleElement_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            
            var pointer = e.Pointer;
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
            {
                return;
            }
            Touch touch = new Touch((int)pointer.PointerId)
            {
                Position = e.GetCurrentPoint(null).Position.ToVector2()*EpxToPxCoefficient,
                
                UwpPointer = pointer
            };
            System.Diagnostics.Debug.WriteLine(touch.Position);
            _Touches.Add(touch);
            
            OnTouchAction?.Invoke(touch, TouchMotionAction.Down);
        }

        private Touch _GetExistTouchFromUwpPointer(Pointer pointer)
        {
            foreach(var touch in _Touches)
            {
                if(touch.Id == pointer.PointerId)
                {
                    return touch;
                }
            }
            return null;
        }

        private class Touch : ITouch
        {
            public List<Vector2> HistoryPosition => throw new NotImplementedException();

            public int Id { get; private set; }

            public Vector2 Position { get; set; }

            public Pointer UwpPointer { get; set; }
            

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
