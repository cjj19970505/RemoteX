using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net.Wifi.P2p;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RemoteX.WifiDirect;
using RemoteX.Core;
using static Android.Net.Wifi.P2p.WifiP2pManager;
using Android.Net;

[assembly: Xamarin.Forms.Dependency(typeof(RemoteX.Droid.WifiDirectManager))]
namespace RemoteX.Droid
{
    public partial class WifiDirectManager : IWifiDirectManager
    {
        private WifiP2pManager _DroidWifiP2pManager;
        private WifiP2pManager.Channel _Channel;

        private Receiver _WifiP2pActonListener;
        private WifiP2pActionListener _DiscoverPeersListener;

        private WifiClientDirectConnection _ClientConnection;

        private WifiP2pInfo _LatestWifiP2pInfo;

        public WifiDirectManager()
        {
            _WifiP2pActonListener = new Receiver(this);
            _DiscoverPeersListener = new WifiP2pActionListener(this);

            _DroidWifiP2pManager = (WifiP2pManager)Application.Context.GetSystemService(Context.WifiP2pService);
            _Channel = _DroidWifiP2pManager.Initialize(Application.Context, Application.Context.MainLooper, null);
            /*
            IntentFilter startFilter = new IntentFilter(BluetoothAdapter.ActionDiscoveryStarted);
            IntentFilter foundFilter = new IntentFilter(BluetoothDevice.ActionFound);
            IntentFilter finshFilter = new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished);

            Application.Context.RegisterReceiver(_DiscoveryStartedReceiver, startFilter);
            Application.Context.RegisterReceiver(_DevicesFoundReceiver, foundFilter);
            Application.Context.RegisterReceiver(_DiscoveryFinishedReceiver, finshFilter);
            */

            IntentFilter connectionChangedFilter = new IntentFilter(WifiP2pManager.WifiP2pConnectionChangedAction);
            IntentFilter peersChangedFilter = new IntentFilter(WifiP2pManager.WifiP2pPeersChangedAction);
            IntentFilter stateChangedFilter = new IntentFilter(WifiP2pManager.WifiP2pStateChangedAction);
            IntentFilter thisDeviceChangedFilter = new IntentFilter(WifiP2pManager.WifiP2pThisDeviceChangedAction);

            Application.Context.RegisterReceiver(_WifiP2pActonListener, stateChangedFilter);
            Application.Context.RegisterReceiver(_WifiP2pActonListener, connectionChangedFilter);
            Application.Context.RegisterReceiver(_WifiP2pActonListener, peersChangedFilter);
            //Application.Context.RegisterReceiver()

        }

        public event WifiDirectPeersSearchHandler OnPeersFound;

        public IClientConnection CreateClientConnection(IWifiDirectDevice targetDevice)
        {
            WifiClientDirectConnection connection = new WifiClientDirectConnection(this, targetDevice);
            return connection;
        }

        public IWifiDirectDevice GetDevice(string address)
        {
            return new WifiDirectDevice(address);
        }

        public void SearchForPeers()
        {
            
            _DroidWifiP2pManager.DiscoverPeers(_Channel, _DiscoverPeersListener);
        }

        private class Receiver : BroadcastReceiver
        {
            private WifiDirectManager _WifiDirectManager;
            public Receiver(WifiDirectManager wifiDirectManager)
            {
                _WifiDirectManager = wifiDirectManager;
            }


            public override void OnReceive(Context context, Intent intent)
            {
                string action = intent.Action;
                if (action == WifiP2pManager.WifiP2pStateChangedAction)
                {
                    Toast.MakeText(Application.Context, "FUCK", ToastLength.Short).Show();
                }
                else if (action == WifiP2pManager.WifiP2pPeersChangedAction)
                {
                    Toast.MakeText(Application.Context, "Found Over", ToastLength.Short).Show();
                    //mManager.requestPeers(mChannel, myPeerListListener);
                    _WifiDirectManager._DroidWifiP2pManager.RequestPeers(_WifiDirectManager._Channel, _WifiDirectManager._DiscoverPeersListener);
                    Application.Context.UnregisterReceiver(_WifiDirectManager._WifiP2pActonListener);
                }
                else if(action == WifiP2pManager.WifiP2pConnectionChangedAction)
                {
                    NetworkInfo networkInfo = intent.GetParcelableExtra(WifiP2pManager.ExtraNetworkInfo) as NetworkInfo;
                    if (networkInfo.IsConnected)
                    {
                        WifiP2pActionListener connectionInfoListener = new WifiP2pActionListener(_WifiDirectManager);
                        _WifiDirectManager._DroidWifiP2pManager.RequestConnectionInfo(_WifiDirectManager._Channel, connectionInfoListener);
                        
                    }
                }
            }
        }

        private class WifiP2pActionListener : Java.Lang.Object, WifiP2pManager.IActionListener, WifiP2pManager.IPeerListListener, WifiP2pManager.IConnectionInfoListener
        {
            WifiDirectManager _WifiDirectManager;
            public WifiP2pActionListener(WifiDirectManager wifiDirectManager)
            {
                this._WifiDirectManager = wifiDirectManager;
            }

            public void OnConnectionInfoAvailable(WifiP2pInfo info)
            {
                _WifiDirectManager._LatestWifiP2pInfo = info;
                System.Diagnostics.Debug.WriteLine("SDFSDFHERE"+(info==null));
            }

            public void OnFailure([GeneratedEnum] WifiP2pFailureReason reason)
            {
                throw new NotImplementedException();
            }

            public void OnPeersAvailable(WifiP2pDeviceList peers)
            {
                var droidDeviceList = peers.DeviceList;
                List<WifiDirectDevice> devices = new List<WifiDirectDevice>();
                foreach (WifiP2pDevice device in droidDeviceList)
                {
                    WifiDirectDevice wifiDirectDevice = new WifiDirectDevice(device);
                    devices.Add(wifiDirectDevice);
                }
                if (devices.Count > 0)
                {
                    _WifiDirectManager.OnPeersFound?.Invoke(_WifiDirectManager, devices.ToArray());
                }

            }

            public void OnSuccess()
            {
                Toast.MakeText(Application.Context, "Found Someting", ToastLength.Short).Show();
            }
        }



        private class WifiDirectDevice : IWifiDirectDevice
        {
            public WifiP2pDevice DroidDevice { get; set; }
            public string _Address;
            public string Address
            {
                get
                {
                    if(DroidDevice == null)
                    {
                        return _Address;
                    } 
                    return DroidDevice.DeviceAddress;
                }
            }

            public string Name
            {
                get
                {
                    if(DroidDevice == null)
                    {
                        return "";
                    }
                    return DroidDevice.DeviceName;
                }
            }

            public WifiDirectDevice(WifiP2pDevice droidDevice)
            {
                DroidDevice = droidDevice;

            }

            public WifiDirectDevice(string address)
            {
                _Address = address;
            }

            public override string ToString()
            {
                return "Name:" + Name + ", " + "Address:" + Address;
            }


        }

    }
}