using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace Bluetooth_Mouse_Controller_Receiver
{
    class PressKeyboard
    {
        [DllImport("User32.dll")]
        public extern static uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        INPUT[] InputArray = new INPUT[1];
        uint sendInput;

        public PressKeyboard()
        {
            InputArray[0].type = 1;
        }
        
        /// <summary>
        /// 按下并立即释放指定按键
        /// </summary>
        /// <param name="虚拟按键码"></param>
        public void PressKey(ushort key)
        {
            InputArray[0].ki.wVk = key;
            sendInput = SendInput(1, InputArray, Marshal.SizeOf<INPUT>(InputArray[0]));
            InputArray[0].ki.dwFlags = 0x0002;
            sendInput = SendInput(1, InputArray, Marshal.SizeOf<INPUT>(InputArray[0]));
        }

        /// <summary>
        /// 长按指定按键
        /// </summary>
        /// <param name="虚拟按键码"></param>
        public void HoldKey(ushort key)
        {
            InputArray[0].ki.wVk = key;
            InputArray[0].ki.dwFlags = 0x0000;
            sendInput = SendInput(1, InputArray, Marshal.SizeOf<INPUT>(InputArray[0]));
        }


        /// <summary>
        /// 释放指定按键
        /// </summary>
        /// <param name="虚拟按键码"></param>
        public void ReleaseKey(ushort key)
        {
            InputArray[0].ki.wVk = key;
            InputArray[0].ki.dwFlags = 0x0002;
            sendInput = SendInput(1, InputArray, Marshal.SizeOf<INPUT>(InputArray[0]));
        }
    }
}
