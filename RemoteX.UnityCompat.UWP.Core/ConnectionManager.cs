using RemoteX.Core;
using RemoteX.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.UWP.Core
{
    public class ConnectionManager : IConnectionManager
    {
        private static ConnectionManager _Instance;
        public static ConnectionManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ConnectionManager();
                }
                return _Instance;
            }
        }

        public IConnection ControllerConnection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event ConnectionHandler onControllerConnectionEstalblishResult;
        public event MessageHandler onControllerConnectionReceiveMessage;

        public IClientConnection CreateClientConnection(IConnectionInfo connectionInfo)
        {
            throw new NotImplementedException();
        }
    }
}
