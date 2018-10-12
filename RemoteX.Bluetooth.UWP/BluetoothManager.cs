using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteX.Bluetooth.LE.Gatt;
using RemoteX.Bluetooth.Win10.LE.Gatt;
using RemoteX.Core;

namespace RemoteX.Bluetooth.Win10
{
    public partial class BluetoothManager : IBluetoothManager
    {
        private static BluetoothManager _Instance;
        public static BluetoothManager Instance
        {
            get
            {
                if(_Instance == null)
                {
                    _Instance = new BluetoothManager();
                }
                return _Instance;
            }
        }

        public bool Supported => throw new NotImplementedException();

        public IBluetoothDevice[] PairedDevices => throw new NotImplementedException();

        public IGattServer GattSever => throw new NotImplementedException();

        private List<BluetoothDevice> _KnownBluetoothDevice;
        public event BluetoothScanResultHandler OnDevicesFound;
        public event BluetoothStartEndScanHandler OnDiscoveryFinished;
        public event BluetoothStartEndScanHandler OnDiscoveryStarted;

        private List<IGattClient> _ConnectedGattClient;

        public GattScanner GattScanner { get; }

        private BluetoothManager()
        {
            _KnownBluetoothDevice = new List<BluetoothDevice>();
            _ConnectedGattClient = new List<IGattClient>();
            GattScanner = new GattScanner(this);
        }

        

        public IClientConnection CreateRfcommClientConnection(IBluetoothDevice device, Guid guid)
        {
            throw new NotImplementedException();
        }



        public IBluetoothDevice GetBluetoothDevice(ulong macAddress)
        {
            throw new NotImplementedException();
        }

        public IGattCharacteristicBuilder NewGattCharacteristicBuilder()
        {
            throw new NotImplementedException();
        }

        public IGattDescriptorBuilder NewGattDescriptorBuilder()
        {
            throw new NotImplementedException();
        }

        public IGattServiceBuilder NewGattServiceBuilder()
        {
            throw new NotImplementedException();
        }

        public void SearchForBlutoothDevices()
        {
            throw new NotImplementedException();
        }
    }
}
