using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.Core;
using Windows.Devices.Bluetooth;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RemoteX.UWP.Core
{
    public partial class BluetoothManager
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

        private List<BluetoothConnection> _ConnectedConnections;
        private BluetoothAdapter _BluetoothAdapter;

        private BluetoothManager()
        {
            _ConnectedConnections = new List<BluetoothConnection>();
            Task<BluetoothAdapter> t = BluetoothAdapter.GetDefaultAsync().AsTask();
            while (!t.IsCompleted)
            {
                System.Diagnostics.Debug.WriteLine("NOT COMPLETED");
            }
            BluetoothAdapter bluetoothAdapter = t.Result;
            if (bluetoothAdapter != null)
            {
                Debug.WriteLine("MAC::" + bluetoothAdapter.BluetoothAddress);
                _BluetoothAdapter = bluetoothAdapter;
            }
            else
            {
                Debug.WriteLine("WTF");
            }
        }

        public IServerConnection CreateRfcommServerConnection(Guid guid)
        {
            BluetoothServerConnection bluetoothServerConnection = new BluetoothServerConnection(this, guid);
            return bluetoothServerConnection;
        }
    }
}
