using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.SkiaComponent
{
    class KeyboardJoystick:Joystick
    {
        List<byte> pressingKey;
        public float DistanceSegment { get; set; }
        
        public KeyboardJoystick(float distanceSegment):base()
        {
            pressingKey = new List<byte>();
            DistanceSegment = distanceSegment;
        }

        protected override void OnJoystickMove()
        {
        }





    }
}
