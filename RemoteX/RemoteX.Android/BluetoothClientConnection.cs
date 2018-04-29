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
        public ConnectionEstablishState ConnectionEstablishState { get; private set; }

        public BluetoothClientConnection(BluetoothDevice device, UUID guid)
        {
            this._Device = device;
            this._SdpUuid = guid;
            this.ConnectionEstablishState = ConnectionEstablishState.NoEstablishment;
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
                return ConnectionEstablishState.failed;
            }
            _BluetoothSocket = tmp;
            _BluetoothAdapter.CancelDiscovery();
            try
            {
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
                    return ConnectionEstablishState.failed;
                }
                return ConnectionEstablishState.failed;
            }
            return ConnectionEstablishState.Succeed;
        }
        
        public async Task sendAsync(byte[] message)
        {
            //byte[] dataLengthBytes = BitConverter.GetBytes(message.Length);
            //await _OutputStream.WriteAsync(dataLengthBytes, 0, dataLengthBytes.Length);
            await _OutputStream.WriteAsync(message, 0, message.Length);
        }
        public async Task<ConnectionEstablishState> ConnectAsync()
        {
            this.ConnectionEstablishState = ConnectionEstablishState.Connecting;
            ConnectionEstablishState state;
            do
            {
                state = await establishConnectionAsync();
                System.Diagnostics.Debug.WriteLine("CONTINUEING BITCHES");
            }
            while (state == ConnectionEstablishState.failed && !_AbortConnecting);
            if(_AbortConnecting)
            {
                this.ConnectionEstablishState = ConnectionEstablishState.Abort;
                _AbortConnecting = false;
            }
            else
            {
                this.ConnectionEstablishState = state;
            }
            this.onConnectionEstalblishResult?.Invoke(this, this.ConnectionEstablishState);
            return state;
        }
        private bool _AbortConnecting = false;
        public void AbortConnecting()
        {
            _AbortConnecting = true;
        }

    }
}