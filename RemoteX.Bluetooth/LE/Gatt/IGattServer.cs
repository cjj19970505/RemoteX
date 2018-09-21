using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Bluetooth.LE.Gatt
{
    public interface IGattServer
    {
        ulong Address { get; }
        void AddService(IGattService service);
        void StartAdvertising();
        void NotifyTest();
        bool IsSupported { get; }
    }
}
