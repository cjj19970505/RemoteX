using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RemoteX;
using RemoteX.Core;

[assembly: Xamarin.Forms.Dependency(typeof(RemoteX.Droid.ConnectionManager))]
namespace RemoteX.Droid
{
    /// <summary>
    /// 对RemoteX.IConnectionManager的具体实现
    /// </summary>
    class ConnectionManager : IConnectionManager
    {
        private IConnection _ControllerConnection;
        public event ConnectionHandler _OnControllerConnectionEstablishResult;
        public event ConnectionHandler onControllerConnectionEstalblishResult
        {
            add
            {
                _OnControllerConnectionEstablishResult += value;
                if (_ControllerConnection != null)
                {
                    _ControllerConnection.OnConnectionEstalblishResult += value;
                }

            }
            remove
            {
                _OnControllerConnectionEstablishResult -= value;
                if (_ControllerConnection != null)
                {
                    _ControllerConnection.OnConnectionEstalblishResult -= value;
                }

            }
        }

        public event MessageHandler _OnControllerConnectionReceiveMessage;
        public event MessageHandler onControllerConnectionReceiveMessage
        {
            add
            {
                _OnControllerConnectionReceiveMessage += value;
                _ControllerConnection.onReceiveMessage += value;
            }
            remove
            {
                _OnControllerConnectionReceiveMessage -= value;
                _ControllerConnection.onReceiveMessage -= value;
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
                if(_ControllerConnection!=null && (_ControllerConnection.ConnectionEstablishState == ConnectionEstablishState.Abort || _ControllerConnection.ConnectionEstablishState == ConnectionEstablishState.Disconnected))
                {
                    ControllerConnection = null;
                }
                return _ControllerConnection;
            }
            set
            {
                if (_ControllerConnection != null)
                {
                    _ControllerConnection.OnConnectionEstalblishResult -= _OnControllerConnectionEstablishResult;
                    _ControllerConnection.onReceiveMessage -= _OnControllerConnectionReceiveMessage;

                }
                _ControllerConnection = value;
                if (_ControllerConnection != null)
                {
                    _ControllerConnection.OnConnectionEstalblishResult += _OnControllerConnectionEstablishResult;
                    _ControllerConnection.onReceiveMessage += _OnControllerConnectionReceiveMessage;
                }
            }
        }
    }
}