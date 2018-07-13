using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.Core;
namespace RemoteX.PC.Core
{
    public partial class BluetoothManager
    {
        private static BluetoothManager _BluetoothManager;
        private BluetoothManager()
        {

        }

        public static BluetoothManager Instance
        {
            get
            {
                if(_BluetoothManager == null)
                {
                    _BluetoothManager = new BluetoothManager();
                }
                return _BluetoothManager;
            }
        }

        public IServerConnection CreateRfcommServerConnection(Guid guid)
        {
            BluetoothServerConnection bluetoothServerConnection = new BluetoothServerConnection(guid);
            return bluetoothServerConnection;
        }
    }
}
