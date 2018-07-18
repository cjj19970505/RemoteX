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
using RemoteX;
using System.Threading.Tasks;
using RemoteX.Core;

namespace RemoteX.Droid
{
    public partial class WifiDirectManager
    {
        class WifiClientDirectConnection:IClientConnection
        {
            private ConnectStateListener _ConnectStateListener;
            private WifiDirectManager _WifiDirectManager;
            public IWifiDirectDevice WifiDirectDevice { get; private set; }

            public ConnectionType connectionType
            {
                get
                {
                    return ConnectionType.TCP;
                }
            }

            public ConnectionEstablishState ConnectionEstablishState { get; private set; }

            public event MessageHandler onReceiveMessage;
            public event ConnectionHandler OnConnectionEstalblishResult;

            

            public WifiClientDirectConnection(WifiDirectManager wifiDirectManager, IWifiDirectDevice wifiDirectDevice)
            {
                _WifiDirectManager = wifiDirectManager;
                this.WifiDirectDevice = wifiDirectDevice;
                _ConnectStateListener = new ConnectStateListener(this);
            }

            public void AbortConnecting()
            {
                throw new NotImplementedException();
            }
            private bool _ConnectingSucceeded = false;
            public async Task<ConnectionEstablishState> ConnectAsync()
            {
                WifiP2pConfig config = new WifiP2pConfig();
                config.DeviceAddress = (WifiDirectDevice as WifiDirectDevice).DroidDevice.DeviceAddress;
                WifiP2pManager droidWifiP2pManager = _WifiDirectManager._DroidWifiP2pManager;
                droidWifiP2pManager.Connect(_WifiDirectManager._Channel, config, _ConnectStateListener);
                while(!_ConnectingSucceeded)
                {
                    
                }
                if (_ConnectingSucceeded)
                {
                    ConnectionEstablishState = ConnectionEstablishState.Succeeded;
                }
                OnConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState);
                
                return ConnectionEstablishState;
                
            }

            

            public Task SendAsync(byte[] message)
            {
                throw new NotImplementedException();
            }

            private class ConnectStateListener : Java.Lang.Object, WifiP2pManager.IActionListener
            {
                private WifiClientDirectConnection _Connection;
                public ConnectStateListener(WifiClientDirectConnection connection)
                {
                    _Connection = connection;
                }
                public void OnFailure([GeneratedEnum] WifiP2pFailureReason reason)
                {
                    Toast.MakeText(Application.Context, "Connect Success", ToastLength.Short).Show();
                }

                public void OnSuccess()
                {
                    _Connection._ConnectingSucceeded = true;
                    Toast.MakeText(Application.Context, "Connect Failed", ToastLength.Short).Show();
                }
            }
        }
    }
    
}