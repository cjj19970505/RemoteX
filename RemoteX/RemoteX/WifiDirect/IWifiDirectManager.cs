using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.WifiDirect
{
    public delegate void WifiDirectPeersSearchHandler(IWifiDirectManager wifiDirectManager, IWifiDirectDevice[] devices);
    public interface IWifiDirectManager
    {
        event WifiDirectPeersSearchHandler OnPeersFound;
        void SearchForPeers();
        void CreateClientConnection(IWifiDirectDevice targetDevice);
    }
}
