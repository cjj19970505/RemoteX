using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Util;
using RemoteX;

namespace RemoteX.Droid
{
    class BluetoothConnection : IConnection
    {
        private static string TAG = "BluetoothConnection";
        private BluetoothDevice _Device;
        private UUID _SdpUuid;
        private BluetoothAdapter _BluetoothAdapter;

        protected Stream _InputStream;
        protected Stream _OutputStream;
        protected BluetoothSocket _BluetoothSocket;

        public event MessageHandler onReceiveMessage;
        public event ConnectionHandler onConnectionEstalblishResult;

        public BluetoothConnection(BluetoothDevice device, UUID guid)
        {
            this._Device = device;
            this._SdpUuid = guid;
        }

        public ConnectionType connectionType
        {
            get
            {
                return ConnectionType.Bluetooth;
            }
        }

        public async void establishConnectionAsync()
        {
            if (_Device == null)
            {
                Log.Error(TAG, "You haven't set the target device");
                onConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState.failed);
                return;
            }
            BluetoothSocket tmp = null;
            if(_BluetoothAdapter == null)
            {
                _BluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            }
            try
            {
                tmp = _Device.CreateInsecureRfcommSocketToServiceRecord(_SdpUuid);
            }
            catch(Java.IO.IOException e)
            {
                Log.Error(TAG, e.Message);
                onConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState.failed);
                return;
            }
            _BluetoothSocket = tmp;
            _BluetoothAdapter.CancelDiscovery();
            try
            {
                await _BluetoothSocket.ConnectAsync();
            }
            catch(Exception connectionException)
            {
                Log.Error(TAG, connectionException.Message);
                try
                {
                    _BluetoothSocket.Close();
                }
                catch(Java.IO.IOException socketCloseException)
                {
                    Log.Error(TAG, socketCloseException.Message);
                    onConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState.failed);
                }
            }
            try
            {
                _InputStream = _BluetoothSocket.InputStream;
                _OutputStream = _BluetoothSocket.OutputStream;
            }
            catch(Java.IO.IOException streamException)
            {
                Log.Error(TAG, streamException.Message);
                onConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState.failed);
            }
            onConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState.Succeed);
        }
        
        public async void sendAsync(byte[] message)
        {
            
        }

        /// <summary>
        /// 会阻塞
        /// </summary>
        private void send(byte[] message)
        {

        }

    }
}