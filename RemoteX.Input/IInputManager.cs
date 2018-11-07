using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Input
{
    public enum TouchMotionAction { Down, Up, Move}
    public delegate void TouchMotionHandler(ITouch touch, TouchMotionAction action);
    public interface IInputManager
    {
        event TouchMotionHandler OnTouchAction;
        ITouch[] Touches { get; }
    }
}
