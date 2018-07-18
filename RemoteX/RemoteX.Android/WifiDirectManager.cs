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
[assembly: Xamarin.Forms.Dependency(typeof(RemoteX.Droid.WifiDirectManager))]
namespace RemoteX.Droid
{
    public partial class WifiDirectManager:IWifiDirectManager
    {
        private WifiP2pManager _DroidWifiP2pManager;
        private WifiP2pManager.Channel _Channel;

        private Receiver _StateChangedReceiver;
        private Receiver _PeersChangeFilter;
        private RequsetPeersListener _DiscoverPeersListener;
        

        public WifiDirectManager()
        {
            _StateChangedReceiver = new Receiver(this);
            _PeersChangeFilter = new Receiver(this);
            _DiscoverPeersListener = new RequsetPeersListener(this);
            _ConnectStateListener = new ConnectStateListener(this);

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
            
            IntentFilter stateChangedFilter = new IntentFilter(WifiP2pManager.WifiP2pStateChangedAction);
            IntentFilter thisDeviceChangedFilter = new IntentFilter(WifiP2pManager.WifiP2pThisDeviceChangedAction);

            Application.Context.RegisterReceiver(_StateChangedReceiver, stateChangedFilter);
            
        }

        public event WifiDirectPeersSearchHandler OnPeersFound;

        public void CreateClientConnection(IWifiDirectDevice targetDevice)
        {
            WifiP2pConfig config = new WifiP2pConfig();
            config.DeviceAddress = (targetDevice as WifiDirectDevice).DroidDevice.DeviceAddress;
        }

        public void SearchForPeers()
        {
            IntentFilter peersChangedFilter = new IntentFilter(WifiP2pManager.WifiP2pPeersChangedAction);
            Application.Context.RegisterReceiver(_PeersChangeFilter, peersChangedFilter);
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
                if(action == WifiP2pManager.WifiP2pStateChangedAction)
                {
                    Toast.MakeText(Application.Context, "FUCK", ToastLength.Short).Show();
                }
                else if(action == WifiP2pManager.WifiP2pPeersChangedAction)
                {
                    Toast.MakeText(Application.Context, "Found Over", ToastLength.Short).Show();
                    //mManager.requestPeers(mChannel, myPeerListListener);
                    _WifiDirectManager._DroidWifiP2pManager.RequestPeers(_WifiDirectManager._Channel, _WifiDirectManager._DiscoverPeersListener);
                    Application.Context.UnregisterReceiver(_WifiDirectManager._PeersChangeFilter);
                }
            }
        }

        private class RequsetPeersListener : Java.Lang.Object, WifiP2pManager.IActionListener, WifiP2pManager.IPeerListListener
        {
            WifiDirectManager _WifiDirectManager;
            public RequsetPeersListener(WifiDirectManager wifiDirectManager)
            {
                this._WifiDirectManager = wifiDirectManager;
            }

            public void OnFailure([GeneratedEnum] WifiP2pFailureReason reason)
            {
                throw new NotImplementedException();
            }

            public void OnPeersAvailable(WifiP2pDeviceList peers)
            {
                var droidDeviceList = peers.DeviceList;
                List<WifiDirectDevice> devices = new List<WifiDirectDevice>();
                foreach(WifiP2pDevice device in droidDeviceList)
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

            public string Address
            {
                get
                {
                    return DroidDevice.DeviceAddress;
                }
            }

            public string Name
            {
                get
                {
                    return DroidDevice.DeviceName;
                }
            }

            public WifiDirectDevice(WifiP2pDevice droidDevice)
            {
                DroidDevice = droidDevice;
                
            }

            public override string ToString()
            {
                return "Name:" + Name + ", " + "Address:" + Address;
            }


        }




    }
}