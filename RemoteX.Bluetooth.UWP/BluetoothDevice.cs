using RemoteX.Bluetooth.LE.Gatt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using RemoteX.Bluetooth.LE;
using RemoteX.Bluetooth.Win10.LE.Gatt;

namespace RemoteX.Bluetooth.Win10
{
    public partial class BluetoothManager:IBluetoothManager
    {
        
        public class BluetoothDevice : IBluetoothDevice
        {
            public BluetoothManager BluetoothManager { get; }
            public BluetoothLEAdvertisementReceivedEventArgs LatestUwpBluetoothLEAdvertisementReceivedEventArgs { get; }

            public string Name { get; }

            public ulong Address { get; }

            public async Task<IGattClient> ConnectToGattServerAsync()
            {
                BluetoothLEDevice uwpLeDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(Address);
                if(uwpLeDevice == null)
                {
                    return null;
                }
                GattClient gattClient = new GattClient(this, uwpLeDevice);
                return gattClient;
            }

            private BluetoothDevice(BluetoothManager bluetoothManager,BluetoothLEAdvertisementReceivedEventArgs uwpAdvertisementReceivedEventArgs)
            {
                BluetoothManager = bluetoothManager;
                LatestUwpBluetoothLEAdvertisementReceivedEventArgs = uwpAdvertisementReceivedEventArgs;
                Name = uwpAdvertisementReceivedEventArgs.Advertisement.LocalName;
                Address = uwpAdvertisementReceivedEventArgs.BluetoothAddress;
            }

            public static BluetoothDevice GetBluetoothDeviceFromUwpLEAdvertisementReceivedEventArgs(BluetoothManager bluetoothManager, BluetoothLEAdvertisementReceivedEventArgs uwpLEAdvertisementReceivedEventArgs)
            {
                var exsitDevice = bluetoothManager._KnownBluetoothDevice.GetFromAddress(uwpLEAdvertisementReceivedEventArgs.BluetoothAddress);
                if (exsitDevice == null)
                {
                    exsitDevice = new BluetoothDevice(bluetoothManager, uwpLEAdvertisementReceivedEventArgs);
                    bluetoothManager._KnownBluetoothDevice.Add(exsitDevice);
                }
                return exsitDevice;
            }


            //============Rfcomm Zone===============================
            public Guid[] LastestFetchedUuids => throw new NotImplementedException();

            public bool IsFetchingUuids => throw new NotImplementedException();

            public event BluetoothDeviceGetUuidsHanlder OnUuidsFetched;

            public void FetchUuidsWithSdp()
            {
                throw new NotImplementedException();
            }

            public void stopFetchingUuidsWithSdp()
            {
                throw new NotImplementedException();
            }
            //========================================================

        }
    }
    
}
