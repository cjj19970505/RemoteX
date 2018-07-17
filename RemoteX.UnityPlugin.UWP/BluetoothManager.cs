using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.Core;
using System.Diagnostics;
using System.Threading.Tasks;

#if NETFX_CORE
using Windows.Devices.Bluetooth;
#endif

namespace RemoteX.UnityPlugin.UWP
{
    public partial class BluetoothManager
    {
#if NETFX_CORE
        private static BluetoothManager _BluetoothManager;
#endif
        public static BluetoothManager Instance
        {
            get
            {
#if NETFX_CORE
                if(_BluetoothManager == null)
                {
                    _BluetoothManager = new BluetoothManager();
                }
                return _BluetoothManager;
#else
                return null;
#endif
            }
        }
#if NETFX_CORE
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
#endif

        public IServerConnection CreateRfcommServerConnection(Guid guid)
        {
#if NETFX_CORE
            BluetoothServerConnection bluetoothServerConnection = new BluetoothServerConnection(this, guid);
            return bluetoothServerConnection;
#else
            return null;
#endif
        }
    }
}
