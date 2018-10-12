using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteX.Bluetooth.Win10;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;
using System.Diagnostics;

namespace RemoteX.Bluetooth.Win10.LE.Gatt
{
    public class GattScanner
    {
        public event EventHandler<BluetoothManager.BluetoothDevice> OnReceived;
        public event EventHandler OnStoped;
        public BluetoothManager BluetoothManager { get; set; }
        private BluetoothLEAdvertisementWatcher _LEAdvertisementWatcher;
        internal GattScanner(BluetoothManager bluetoothManager)
        {
            BluetoothManager = bluetoothManager;
        }

        public void StartScan()
        {
            _LEAdvertisementWatcher = new BluetoothLEAdvertisementWatcher();
            _LEAdvertisementWatcher.Received += BluetoothLEWatcher_Received;
            _LEAdvertisementWatcher.Stopped += BluetoothLEWatcher_Stoped;
            _LEAdvertisementWatcher.ScanningMode = BluetoothLEScanningMode.Active;
            _LEAdvertisementWatcher.Start();
            
        }

        public void StopScan()
        {
            _LEAdvertisementWatcher.Stop();
        }

        private void BluetoothLEWatcher_Stoped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            OnStoped?.Invoke(this, null);
        }

        private void BluetoothLEWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            BluetoothManager.BluetoothDevice bluetoothDevice = BluetoothManager.BluetoothDevice.GetBluetoothDeviceFromUwpLEAdvertisementReceivedEventArgs(BluetoothManager, args);
            OnReceived?.Invoke(this, bluetoothDevice);
        }
    }
}
    
