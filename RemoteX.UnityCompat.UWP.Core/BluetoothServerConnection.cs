using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RemoteX.Core;

namespace RemoteX.UWP.Core
{
    partial class BluetoothManager
    {
        private const uint SERVICE_VERSION_ATTRIBUTE_ID = 0x0300;
        private const byte SERVICE_VERSION_ATTRIBUTE_TYPE = 0x0A;
        private const uint SERVICE_VERSION = 200;

        class BluetoothServerConnection : BluetoothConnection, IServerConnection
        {
            public Guid Uuid { get; private set; }

            public string ConnectCode
            {
                get
                {
                    return "Get ConnectCode In Editor";
                }
            }

            public event ConnectionHandler OnConnectionEstalblishResult;


            public BluetoothServerConnection(BluetoothManager bluetoothManager, Guid uuid) : base()
            {
                this.Uuid = uuid;

            }

            public void StartServer()
            {
            }
        }
    }

}
