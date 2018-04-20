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
    class BluetoothClientConnection : IConnection
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

        public BluetoothClientConnection(BluetoothDevice device, UUID guid)
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

        public async Task<ConnectionEstablishState> establishConnectionAsync()
        {
            if (_Device == null)
            {
                onConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState.failed);
                return ConnectionEstablishState.failed;
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
                onConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState.failed);
                return ConnectionEstablishState.failed;
            }
            _BluetoothSocket = tmp;
            _BluetoothAdapter.CancelDiscovery();
            try
            {
                System.Diagnostics.Debug.WriteLine("BLUETOOTH::CONNECTING TO " + _Device.Name);
                await _BluetoothSocket.ConnectAsync();
                System.Diagnostics.Debug.WriteLine("BLUETOOTH::SUCCESSFUL ");
            }
            catch(Exception connectionException)
            {
                try
                {
                    _BluetoothSocket.Close();
                }
                catch(Java.IO.IOException socketCloseException)
                {
                    onConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState.failed);
                    return ConnectionEstablishState.failed;
                }
                return ConnectionEstablishState.failed;
            }
            try
            {
                _InputStream = _BluetoothSocket.InputStream;
                _OutputStream = _BluetoothSocket.OutputStream;
            }
            catch(Java.IO.IOException streamException)
            {
                try
                {
                    _BluetoothSocket.Close();
                }
                catch (Java.IO.IOException socketCloseException)
                {
                    onConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState.failed);
                    return ConnectionEstablishState.failed;
                }
                onConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState.failed);
                return ConnectionEstablishState.failed;
            }
            onConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState.Succeed);
            return ConnectionEstablishState.Succeed;
        }
        
        public async void sendAsync(byte[] message)
        {
            
        }
        public async Task<ConnectionEstablishState> ConnectAsync()
        {
            return await establishConnectionAsync();
        }

    }
}