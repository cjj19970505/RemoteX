using RemoteX.Bluetooth.LE.Gatt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

namespace RemoteX.Bluetooth.Win10.LE.Gatt
{
    public partial class GattClient:IGattClient
    {
        public IBluetoothDevice BluetoothDevice { get; }
        public BluetoothLEDevice UwpLEDevice { get; }
        public event EventHandler<IGattClientService[]> OnServicesDiscovered;

        private List<GattClientService> _ClientServices;

        internal GattClient(IBluetoothDevice bluetoothDevice, BluetoothLEDevice uwpLeDevice)
        {
            BluetoothDevice = bluetoothDevice;
            UwpLEDevice = uwpLeDevice;
            _ClientServices = new List<GattClientService>();

        }

        public async Task<IGattClientService[]> DiscoverAllPrimaryServiceAsync()
        {
            var result = await UwpLEDevice.GetGattServicesAsync();
            List<IGattClientService> discoveredServicesList = new List<IGattClientService>();
            foreach(var uwpService in result.Services)
            {
                discoveredServicesList.Add(GattClientService.FromUwpGattService(this, uwpService));
            }

            return discoveredServicesList.ToArray();
        }



        
    }

    
}
