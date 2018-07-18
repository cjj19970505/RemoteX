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

namespace RemoteX.Droid
{
    public partial class WifiDirectManager
    {
        class WifiClientDirectConnection:IConnection
        {
            private ConnectStateListener _ConnectStateListener;

            public ConnectionType connectionType
            {
                get
                {
                    return ConnectionType.TCP;
                }
            }

            public ConnectionEstablishState ConnectionEstablishState => throw new NotImplementedException();

            public event MessageHandler onReceiveMessage;
            public event ConnectionHandler onConnectionEstalblishResult;

            public void AbortConnecting()
            {
                throw new NotImplementedException();
            }

            public Task<ConnectionEstablishState> ConnectAsync()
            {
                throw new NotImplementedException();
            }

            public Task SendAsync(byte[] message)
            {
                throw new NotImplementedException();
            }

            private class ConnectStateListener : Java.Lang.Object, WifiP2pManager.IActionListener
            {
                private WifiDirectManager _WifiDirectManager;
                public ConnectStateListener(WifiDirectManager wifiDirectManager)
                {
                    _WifiDirectManager = wifiDirectManager;
                }
                public void OnFailure([GeneratedEnum] WifiP2pFailureReason reason)
                {
                    Toast.MakeText(Application.Context, "Connect Success", ToastLength.Short).Show();
                }

                public void OnSuccess()
                {
                    Toast.MakeText(Application.Context, "Connect Failed", ToastLength.Short).Show();
                }
            }
        }
    }
    
}