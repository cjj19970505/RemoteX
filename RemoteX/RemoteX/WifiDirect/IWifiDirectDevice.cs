using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.WifiDirect
{
    public interface IWifiDirectDevice
    {
        string Address { get; }
        string Name { get; }
    }
}
