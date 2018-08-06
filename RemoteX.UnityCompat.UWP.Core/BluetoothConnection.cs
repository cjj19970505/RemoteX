using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.Core;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteX.UWP.Core
{
    public partial class BluetoothManager
    {
        class BluetoothConnection : IConnection
        {
            public ConnectionEstablishState ConnectionEstablishState => throw new NotImplementedException();

            public ConnectionType ConnectionType
            {
                get
                {
                    return ConnectionType.Bluetooth;
                }
            }

            public event ConnectionHandler OnConnectionEstalblishResult;
            public event MessageHandler OnReceiveMessage;

            public void Abort()
            {
                throw new NotImplementedException();
            }

            public void Cancel()
            {
                throw new NotImplementedException();
            }

            public void Send(byte[] message)
            {
                throw new NotImplementedException();
            }

            public Task SendAsync(byte[] message)
            {
                throw new NotImplementedException();
            }
        }
    }
}
