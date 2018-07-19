using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.Core;
namespace RemoteX.WifiDirect
{
    public delegate void WifiDirectPeersSearchHandler(IWifiDirectManager wifiDirectManager, IWifiDirectDevice[] devices);
    public interface IWifiDirectManager
    {
        event WifiDirectPeersSearchHandler OnPeersFound;
        void SearchForPeers();
        IClientConnection CreateClientConnection(IWifiDirectDevice targetDevice);
        IWifiDirectDevice GetDevice(string address);
    }
}
