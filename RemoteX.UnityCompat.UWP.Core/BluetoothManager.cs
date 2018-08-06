using RemoteX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteX.UWP.Core
{
    partial class BluetoothManager
    {
        private static BluetoothManager _BluetoothManager;
        public static BluetoothManager Instance
        {
            get
            {
                if (_BluetoothManager == null)
                {
                    _BluetoothManager = new BluetoothManager();
                }
                return _BluetoothManager;
            }
        }

        public IServerConnection CreateRfcommServerConnection(Guid guid)
        {
            BluetoothServerConnection bluetoothServerConnection = new BluetoothServerConnection(this, guid);
            return bluetoothServerConnection;
        }
    }
}
