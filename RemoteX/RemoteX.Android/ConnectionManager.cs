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

[assembly: Xamarin.Forms.Dependency(typeof(RemoteX.Droid.ConnectionManager))]
namespace RemoteX.Droid
{
    /// <summary>
    /// 对RemoteX.IConnectionManager的具体实现
    /// </summary>
    class ConnectionManager:IConnectionManager
    {
        private IConnection _ControllerConnection;
        public event ConnectionHandler onControllerConnectionEstalblishResult;
        public event MessageHandler onControllerConnectionReceiveMessage;
        public ConnectionManager()
        {
            _ControllerConnection = null;
        }
        public IConnection ControllerConnection
        {
            get
            {
                return _ControllerConnection;
            }
            set
            {
                if(_ControllerConnection != null)
                {
                    _ControllerConnection.onConnectionEstalblishResult -= onControllerConnectionEstalblishResult;
                    _ControllerConnection.onReceiveMessage -= onControllerConnectionReceiveMessage;
                }
                _ControllerConnection = value;
                if(_ControllerConnection != null)
                {
                    _ControllerConnection.onConnectionEstalblishResult += onControllerConnectionEstalblishResult;
                    _ControllerConnection.onReceiveMessage += onControllerConnectionReceiveMessage;
                }
            }
        }
    }
}