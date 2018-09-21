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
using Java.Util;
using RemoteX.Bluetooth;
using RemoteX.Bluetooth.LE.Gatt;
using RemoteX.Core;
using RemoteX.Droid.Bluetooth.LE.Gatt;

//[assembly: Xamarin.Forms.Dependency(typeof(RemoteX.Droid.BluetoothManager))]
namespace RemoteX.Droid
{
    /// <summary>
    /// 对IBluetoothManager的Android端的实现
    /// 使用使和Android.Bluetooth.BluetoothManager区分开来，两个不一样
    /// 
    /// </summary>
    public partial class BluetoothManager : RemoteX.Bluetooth.IBluetoothManager
    {
        public BluetoothAdapter BluetoothAdapter { get; private set;}
        bool _IsDiscoverying;
        Receiver _DiscoveryStartedReceiver;
        Receiver _DevicesFoundReceiver;
        Receiver _DiscoveryFinishedReceiver;
        

        /// <summary>
        /// 只有正在 建立连接 已经建立完连接 才有资格加入这里面
        /// 就是说只有需要管理的资源才加进来啦
        /// </summary>
        private List<BluetoothClientConnection> _BluetoothConnections;

        public Android.Bluetooth.BluetoothManager DroidBluetoothManager { get; private set; }

        public BluetoothManager()
        {
            _IsDiscoverying = false;
            BluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            _DiscoveryStartedReceiver = new Receiver(this);
            _DevicesFoundReceiver = new Receiver(this);
            _DiscoveryFinishedReceiver = new Receiver(this);
            _BluetoothConnections = new List<BluetoothClientConnection>();
            DroidBluetoothManager = Application.Context.GetSystemService(Context.BluetoothService) as Android.Bluetooth.BluetoothManager;
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
                if (BluetoothAdapter != null)
                {
                    return true;
                }
                return false;
            }
        }
        public IConnection DefaultConnection
        {
            get
            {
                if(_BluetoothConnections!=null && _BluetoothConnections.Count>0)
                {
                    return _BluetoothConnections[0];
                }
                return null;
            }
        }

        public event RemoteX.Bluetooth.BluetoothScanResultHandler OnDevicesFound;
        public event RemoteX.Bluetooth.BluetoothStartEndScanHandler OnDiscoveryFinished;
        public event RemoteX.Bluetooth.BluetoothStartEndScanHandler OnDiscoveryStarted;

        public void SearchForBlutoothDevices()
        {
            IntentFilter startFilter = new IntentFilter(BluetoothAdapter.ActionDiscoveryStarted);
            IntentFilter foundFilter = new IntentFilter(BluetoothDevice.ActionFound);
            IntentFilter finshFilter = new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished);

            Application.Context.RegisterReceiver(_DiscoveryStartedReceiver, startFilter);
            Application.Context.RegisterReceiver(_DevicesFoundReceiver, foundFilter);
            Application.Context.RegisterReceiver(_DiscoveryFinishedReceiver, finshFilter);

            BluetoothAdapter.StartDiscovery();
        }

        public IClientConnection CreateRfcommClientConnection(RemoteX.Bluetooth.IBluetoothDevice deviceWrapper, Guid guid)
        {
            UUID uuid = UUID.FromString(guid.ToString());
            if(!(deviceWrapper is BluetoothDeviceWrapper))
            {
                return null;
            }
            BluetoothDevice device = (deviceWrapper as BluetoothDeviceWrapper).BluetoothDevice;
            BluetoothClientConnection clientConnection = new BluetoothClientConnection(this, device, uuid);
            _BluetoothConnections.Add(clientConnection);
            return clientConnection;
        }

        public IBluetoothDevice GetBluetoothDevice(ulong macAddress)
        {
            byte[] addressBytesULong = BitConverter.GetBytes(macAddress);
            byte[] addressBytes = new byte[6];
            for(int i =0;i<addressBytes.Length;i++)
            {
                addressBytes[i] = addressBytesULong[5-i];
            }
            BluetoothDevice device = BluetoothAdapter.GetRemoteDevice(addressBytes);
            return new BluetoothDeviceWrapper(device);
        }

        public RemoteX.Bluetooth.IBluetoothDevice[] PairedDevices
        {
            get
            {
                ICollection<BluetoothDevice> pairedDevices = BluetoothAdapter.BondedDevices;
                List<RemoteX.Bluetooth.IBluetoothDevice> devices = new List<RemoteX.Bluetooth.IBluetoothDevice>();
                foreach(var droidDevice in pairedDevices)
                {
                    BluetoothDeviceWrapper bluetoothDeviceWrapper = new BluetoothDeviceWrapper(droidDevice);
                    devices.Add(bluetoothDeviceWrapper);
                }
                return devices.ToArray();
            }
            
            
        }


        private GattServer _GattServer;
        public IGattServer GattSever
        {
            get
            {
                if(_GattServer == null)
                {
                    _GattServer = new GattServer(this);
                }
                return _GattServer;
            }
        }

        private class Receiver : BroadcastReceiver
        {
            BluetoothManager _BluetoothManager;
            public Receiver(BluetoothManager bluetoothManager)
            {
                this._BluetoothManager = bluetoothManager;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                string action = intent.Action;
                if (BluetoothAdapter.ActionDiscoveryStarted == action)
                {
                    this._BluetoothManager.IsDiscoverying = true;
                    _BluetoothManager.OnDiscoveryStarted?.Invoke(_BluetoothManager);
                    Application.Context.UnregisterReceiver(this);
                }
                if (BluetoothDevice.ActionFound == action)
                {
                    BluetoothDevice device = intent.GetParcelableExtra(BluetoothDevice.ExtraDevice) as BluetoothDevice;
                    BluetoothDeviceWrapper deviceWrapper = new BluetoothDeviceWrapper(device);

                    _BluetoothManager.OnDevicesFound?.Invoke(_BluetoothManager, new RemoteX.Bluetooth.IBluetoothDevice[] { deviceWrapper });
                }
                if (BluetoothAdapter.ActionDiscoveryFinished == action)
                {
                    _BluetoothManager.IsDiscoverying = false;
                    _BluetoothManager.OnDiscoveryFinished?.Invoke(_BluetoothManager);
                    Application.Context.UnregisterReceiver(this);
                    Application.Context.UnregisterReceiver(_BluetoothManager._DevicesFoundReceiver);
                }

            }
        }
    }
}