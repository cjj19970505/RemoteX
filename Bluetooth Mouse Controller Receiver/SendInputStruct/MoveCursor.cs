using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;

namespace Bluetooth_Mouse_Controller_Receiver
{
    public class MoveCursor
    {
        [DllImport("User32.dll")]
        public extern static uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        INPUT[] InputArray = new INPUT[1];
        uint sendinput;

        private int x = 0, y = 0;
        Point _MousePosition;

        public bool isAbsolute = true;

        private uint getFlags()
        {
            uint flags = 0;
            if (isAbsolute)
            {
                flags |= 0x8000;
            }
            return flags;
        }
        public MoveCursor(bool isAbsolute = true)
        {
            this.isAbsolute = isAbsolute;
            InputArray[0].mi.dwFlags = 0x8000;
            sendinput = SendInput(1, InputArray, Marshal.SizeOf<INPUT>(InputArray[0]));
        }

        public Point Position
        {
            get { return this._MousePosition; }
            set { this._MousePosition = value; x = _MousePosition.X; y = _MousePosition.Y; }
        }

        public Point MoveTo(Point MousePosition)
        {
            //设置InputArray数组参数
            InputArray[0].type = 0;
            InputArray[0].mi.dx = MousePosition.X;
            InputArray[0].mi.dy = MousePosition.Y;
            InputArray[0].mi.dwFlags = 0x0001 | getFlags();

            //移动光标
            sendinput = SendInput(1, InputArray, Marshal.SizeOf<INPUT>(InputArray[0]));

            //返回鼠标位置
            return MousePosition;
        }

        public void LeftDown()
        {
            InputArray[0].mi.dwFlags = 0x0002;
            sendinput = SendInput(1, InputArray, Marshal.SizeOf<INPUT>(InputArray[0]));
        }

        public void LeftUp()
        {
            InputArray[0].mi.dwFlags = 0x0004;
            sendinput = SendInput(1, InputArray, Marshal.SizeOf<INPUT>(InputArray[0]));
        }

        public void LeftClick()
        {
            LeftDown();
            LeftUp();
        }

        public void RightDown()
        {
            InputArray[0].mi.dwFlags = 0x0008;
            sendinput = SendInput(1, InputArray, Marshal.SizeOf<INPUT>(InputArray[0]));
        }

        public void RightUp()
        {
            InputArray[0].mi.dwFlags = 0x0010;
            sendinput = SendInput(1, InputArray, Marshal.SizeOf<INPUT>(InputArray[0]));
        }

        public void RightClick()
        {
            RightDown();
            RightUp();
        }

        public void MiddleDown()
        {
            InputArray[0].mi.dwFlags = 0x0020;
            sendinput = SendInput(1, InputArray, Marshal.SizeOf<INPUT>(InputArray[0]));
        }

        public void MiddleUp()
        {
            InputArray[0].mi.dwFlags = 0x0040;
            sendinput = SendInput(1, InputArray, Marshal.SizeOf<INPUT>(InputArray[0]));
        }

        public void MiddleClick()
        {
            MiddleDown();
            MiddleUp();
        }

        public void MouseWheel(uint distance)
        {
            InputArray[0].mi.dwFlags = 0x0080;
            InputArray[0].mi.mouseData = distance;
            sendinput = SendInput(1, InputArray, Marshal.SizeOf<INPUT>(InputArray[0]));
            InputArray[0].mi.dwFlags = 0;
        }
        public void scrollVerticle(float value)
        {
            InputArray[0].type = 0;
            InputArray[0].mi.dx = 0;
            InputArray[0].mi.dy = 0;
            InputArray[0].mi.dwFlags = 0x0800;
            InputArray[0].mi.time = 0;
            InputArray[0].mi.dwExtraInfo = 0;
            int a = (int)value; 
            InputArray[0].mi.mouseData = (uint)a;
            SendInput(1, InputArray, Marshal.SizeOf<INPUT>(InputArray[0]));

        }
        public void scrollHorizontal(float value)
        {
            InputArray[0].type = 0;
            InputArray[0].mi.dx = 0;
            InputArray[0].mi.dy = 0;
            InputArray[0].mi.dwFlags = 0x0800*2;
            InputArray[0].mi.time = 0;
            InputArray[0].mi.dwExtraInfo = 0;
            int a = (int)value;
            InputArray[0].mi.mouseData = (uint)a;
            SendInput(1, InputArray, Marshal.SizeOf<INPUT>(InputArray[0]));

        }
        public void MouseHorizontalWheel(uint distance)
        {
            InputArray[0].mi.dwFlags = 0x01000;
            InputArray[0].mi.mouseData = distance;
            sendinput = SendInput(1, InputArray, Marshal.SizeOf<INPUT>(InputArray[0]));
            InputArray[0].mi.dwFlags = 0;
        }
    }
}
