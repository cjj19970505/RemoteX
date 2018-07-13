using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.Core;
namespace RemoteX.PC.Core
{
    public partial class BluetoothManager
    {
        private static BluetoothManager _BluetoothManager;
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

        private List<BluetoothConnection> _ConnectedConnections;

        private BluetoothManager()
        {
            _ConnectedConnections = new List<BluetoothConnection>();
        }

        public IServerConnection CreateRfcommServerConnection(Guid guid)
        {
            BluetoothServerConnection bluetoothServerConnection = new BluetoothServerConnection(this, guid);
            return bluetoothServerConnection;
        }
    }
}
