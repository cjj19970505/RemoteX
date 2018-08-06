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

            public ConnectionType ConnectionType
            {
                get
                {
                    return ConnectionType.TCP;
                }
            }

            public ConnectionEstablishState ConnectionEstablishState { get; private set; }

            public event MessageHandler OnReceiveMessage;
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
            private bool _ReconnectMark = false;
            public async Task<ConnectionEstablishState> ConnectAsync()
            {
                await Task.Run(() =>
                {
                    WifiP2pConfig config = new WifiP2pConfig();
                    config.DeviceAddress = (WifiDirectDevice as WifiDirectDevice).Address;
                    WifiP2pManager droidWifiP2pManager = _WifiDirectManager._DroidWifiP2pManager;
                    droidWifiP2pManager.Connect(_WifiDirectManager._Channel, config, _ConnectStateListener);
                    while (!_ConnectingSucceeded)
                    {
                        if(_ReconnectMark)
                        {
                            _ReconnectMark = false;
                            droidWifiP2pManager.Connect(_WifiDirectManager._Channel, config, _ConnectStateListener);
                        }
                    }
                    WifiP2pActionListener connectionInfoListener = new WifiP2pActionListener(_WifiDirectManager);
                    _WifiDirectManager._DroidWifiP2pManager.RequestConnectionInfo(_WifiDirectManager._Channel, connectionInfoListener);
                    while (_WifiDirectManager._LatestWifiP2pInfo == null)
                    {
                        System.Diagnostics.Debug.WriteLine("HERE2");
                    }
                    WifiP2pInfo preInfo = null;
                    while (_WifiDirectManager._LatestWifiP2pInfo.GroupOwnerAddress==null)
                    {
                        if(preInfo != _WifiDirectManager._LatestWifiP2pInfo)
                        {
                            preInfo = _WifiDirectManager._LatestWifiP2pInfo;
                            _WifiDirectManager._DroidWifiP2pManager.RequestConnectionInfo(_WifiDirectManager._Channel, connectionInfoListener);
                            System.Diagnostics.Debug.WriteLine("CHANGE");
                        }
                        System.Diagnostics.Debug.WriteLine("HERE3");
                        
                    }
                    if (_ConnectingSucceeded)
                    {
                        ConnectionEstablishState = ConnectionEstablishState.Succeeded;
                    }
                    OnConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState);
                });
                try
                {
                    System.Diagnostics.Debug.WriteLine("Connect Succeed " + _WifiDirectManager._LatestWifiP2pInfo.GroupOwnerAddress.HostAddress);
                }
                catch(Exception)
                {
                    var fuck = _WifiDirectManager._LatestWifiP2pInfo;

                }
                

                return ConnectionEstablishState;
            }

            public Task SendAsync(byte[] message)
            {
                throw new NotImplementedException();
            }

            public void Cancel()
            {
                throw new NotImplementedException();
            }

            public void Abort()
            {
                throw new NotImplementedException();
            }

            public void Connect()
            {
                throw new NotImplementedException();
            }

            public void Send(byte[] message)
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
                    System.Diagnostics.Debug.WriteLine("Connect Failed " + reason);
                    _Connection._ReconnectMark = true;
                }

                public void OnSuccess()
                {
                    _Connection._ConnectingSucceeded = true;
                    _Connection._ReconnectMark = false;
                }
            }

            
        }
    }
    
}