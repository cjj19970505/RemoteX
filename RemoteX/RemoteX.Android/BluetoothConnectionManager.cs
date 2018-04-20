using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

[assembly: Xamarin.Forms.Dependency(typeof(RemoteX.Droid.BluetoothConnectionManager))]
namespace RemoteX.Droid
{
    /// <summary>
    /// 
    /// </summary>
    class BluetoothConnectionManager : RemoteX.Bluetooth.IBluetoothManager
    {
        BluetoothAdapter _BluetoothAdapter;
        bool _IsDiscoverying;
        Receiver _DiscoveryStartedReceiver;
        Receiver _DevicesFoundReceiver;
        Receiver _DiscoveryFinishedReceiver;

        public BluetoothConnectionManager()
        {
            _IsDiscoverying = false;
            _BluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            _DiscoveryStartedReceiver = new Receiver(this);
            _DevicesFoundReceiver = new Receiver(this);
            _DiscoveryFinishedReceiver = new Receiver(this);
        }
        public bool IsDiscoverying
        {
            get
            {
                return _IsDiscoverying;
            }
            private set
            {
                this._IsDiscoverying = value;
            }
        }
        public bool Supported
        {
            get
            {
                if (_BluetoothAdapter != null)
                {
                    return true;
                }
                return false;
            }
        }

        public event RemoteX.Bluetooth.BluetoothScanResultHandler onDevicesFound;
        public event RemoteX.Bluetooth.BluetoothStartEndScanHandler onDiscoveryFinished;
        public event RemoteX.Bluetooth.BluetoothStartEndScanHandler onDiscoveryStarted;

        public void searchForBlutoothDevices()
        {
            IntentFilter startFilter = new IntentFilter(BluetoothAdapter.ActionDiscoveryStarted);
            IntentFilter foundFilter = new IntentFilter(BluetoothDevice.ActionFound);
            IntentFilter finshFilter = new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished);

            Application.Context.RegisterReceiver(_DiscoveryStartedReceiver, startFilter);
            Application.Context.RegisterReceiver(_DevicesFoundReceiver, foundFilter);
            Application.Context.RegisterReceiver(_DiscoveryFinishedReceiver, finshFilter);

            _BluetoothAdapter.StartDiscovery();
        }

        public IConnection createRfcommClientConnection(RemoteX.Bluetooth.IBluetoothDevice device, Guid guid)
        {

            return null;
        }

        private class Receiver : BroadcastReceiver
        {
            BluetoothConnectionManager bluetoothConnectionManager;
            public Receiver(BluetoothConnectionManager bluetoothConnectionManager)
            {
                this.bluetoothConnectionManager = bluetoothConnectionManager;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                string action = intent.Action;
                if (BluetoothAdapter.ActionDiscoveryStarted == action)
                {
                    this.bluetoothConnectionManager.IsDiscoverying = true;
                    bluetoothConnectionManager.onDiscoveryStarted?.Invoke(bluetoothConnectionManager);
                    Application.Context.UnregisterReceiver(this);
                }
                if (BluetoothDevice.ActionFound == action)
                {
                    BluetoothDevice device = intent.GetParcelableExtra(BluetoothDevice.ExtraDevice) as BluetoothDevice;
                    /*
                    RemoteX.Bluetooth.BluetoothDevice bluetoothDevice = new RemoteX.Bluetooth.BluetoothDevice(bluetoothConnectionManager)
                    {
                        Name = device.Name,
                        MacAddress = device.Address,
                        AndroidInstance = device
                    };
                    */
                    BluetoothDeviceWrapper deviceWrapper = new BluetoothDeviceWrapper(device);

                    bluetoothConnectionManager.onDevicesFound?.Invoke(bluetoothConnectionManager, new RemoteX.Bluetooth.IBluetoothDevice[] { deviceWrapper });
                }
                if (BluetoothAdapter.ActionDiscoveryFinished == action)
                {
                    bluetoothConnectionManager.IsDiscoverying = false;
                    bluetoothConnectionManager.onDiscoveryFinished?.Invoke(bluetoothConnectionManager);
                    Application.Context.UnregisterReceiver(this);
                    Application.Context.UnregisterReceiver(bluetoothConnectionManager._DevicesFoundReceiver);
                }

            }
        }
    }
}