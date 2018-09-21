using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.Bluetooth;

namespace RemoteX
{
    public interface IManagerManager
    {
        IBluetoothManager BluetoothManager { get; }
    }
}
