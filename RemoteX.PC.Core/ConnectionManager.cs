using RemoteX.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.PC.Core
{
    public class ConnectionManager : IConnectionManager
    {
        private static ConnectionManager _Instance;
        public static ConnectionManager Instance
        {
            get
            {
                if(_Instance == null)
                {
                    _Instance = new ConnectionManager();
                }
                return _Instance;
            }
        }


        private IConnection _ControllerConnection;
        private event ConnectionHandler _OnControllerConnectionEstablishResult;
        public event ConnectionHandler onControllerConnectionEstalblishResult
        {
            add
            {
                _OnControllerConnectionEstablishResult += value;
                if (_ControllerConnection != null)
                {
                    if(_ControllerConnection is IServerConnection)
                    {
                        (_ControllerConnection as IServerConnection).OnConnectionEstalblishResult += value;
                    }
                    else if(_ControllerConnection is IClientConnection)
                    {
                        (_ControllerConnection as IClientConnection).OnConnectionEstalblishResult += value;
                    }
                }

            }
            remove
            {
                _OnControllerConnectionEstablishResult -= value;
                if (_ControllerConnection != null)
                {
                    if (_ControllerConnection is IServerConnection)
                    {
                        (_ControllerConnection as IServerConnection).OnConnectionEstalblishResult -= value;
                    }
                    else if (_ControllerConnection is IClientConnection)
                    {
                        (_ControllerConnection as IClientConnection).OnConnectionEstalblishResult -= value;
                    }
                }

            }
        }

        private event MessageHandler _OnControllerConnectionReceiveMessage;
        public event MessageHandler onControllerConnectionReceiveMessage
        {
            add
            {
                
                _OnControllerConnectionReceiveMessage += value;
                if(_ControllerConnection != null)
                {
                    _ControllerConnection.onReceiveMessage += value;
                }
                
            }
            remove
            {
                _OnControllerConnectionReceiveMessage -= value;
                if (_ControllerConnection != null)
                {
                    _ControllerConnection.onReceiveMessage -= value;
                }
            }
        }
        public ConnectionManager()
        {
            _ControllerConnection = null;
        }
        public IConnection ControllerConnection
        {
            get
            {
                if (_ControllerConnection != null && (_ControllerConnection.ConnectionEstablishState == ConnectionEstablishState.Abort || _ControllerConnection.ConnectionEstablishState == ConnectionEstablishState.Disconnect))
                {
                    ControllerConnection = null;
                }
                return _ControllerConnection;
            }
            set
            {
                if (_ControllerConnection != null)
                {
                    if (_ControllerConnection is IServerConnection)
                    {
                        (_ControllerConnection as IServerConnection).OnConnectionEstalblishResult -= _OnControllerConnectionEstablishResult;
                        (_ControllerConnection as IServerConnection).onReceiveMessage -= _OnControllerConnectionReceiveMessage;
                    }
                    else if (_ControllerConnection is IClientConnection)
                    {
                        (_ControllerConnection as IClientConnection).OnConnectionEstalblishResult -= _OnControllerConnectionEstablishResult;
                        (_ControllerConnection as IClientConnection).onReceiveMessage -= _OnControllerConnectionReceiveMessage;
                    }
                }
                _ControllerConnection = value;
                if (_ControllerConnection != null)
                {
                    if (_ControllerConnection is IServerConnection)
                    {
                        (_ControllerConnection as IServerConnection).OnConnectionEstalblishResult += _OnControllerConnectionEstablishResult;
                        (_ControllerConnection as IServerConnection).onReceiveMessage += _OnControllerConnectionReceiveMessage;
                    }
                    else if (_ControllerConnection is IClientConnection)
                    {
                        (_ControllerConnection as IClientConnection).OnConnectionEstalblishResult += _OnControllerConnectionEstablishResult;
                        (_ControllerConnection as IClientConnection).onReceiveMessage += _OnControllerConnectionReceiveMessage;
                    }
                }
            }
        }
    }
}
