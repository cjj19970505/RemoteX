using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using RemoteX.Core;
using System.Threading.Tasks;

namespace RemoteX.PC.Core
{
    partial class BluetoothManager
    {
        private const uint SERVICE_VERSION_ATTRIBUTE_ID = 0x0300;
        private const byte SERVICE_VERSION_ATTRIBUTE_TYPE = 0x0A;
        private const uint SERVICE_VERSION = 200;

        class BluetoothServerConnection:BluetoothConnection, IServerConnection
        {
            public Guid Uuid { get; private set; }

            public string ConnectCode
            {
                get
                {
                    return RemoteX.Data.Connection.EncodeBluetoothConnection(BluetoothManager._BluetoothAdapter.BluetoothAddress, Uuid);
                }
            }

            private RfcommServiceProvider _Provider;

            public event ConnectionHandler OnConnectionEstalblishResult;

            private Task _ReceiveTask;

            public BluetoothServerConnection(BluetoothManager bluetoothManager ,Guid uuid):base(bluetoothManager)
            {
                this.Uuid = uuid;
                
            }

            public async void StartAdvertisingAsync()
            {
                ConnectionEstablishState = ConnectionEstablishState.Connecting;
                OnConnectionEstalblishResult(this, ConnectionEstablishState);
                BluetoothManager._ConnectedConnections.Add(this);
                RfcommServiceId myId = RfcommServiceId.FromUuid(Uuid);
                _Provider = await RfcommServiceProvider.CreateAsync(myId);
                StreamSocketListener listener = new StreamSocketListener();
                listener.ConnectionReceived += _OnConnectionReceived;
                await listener.BindServiceNameAsync(_Provider.ServiceId.AsString(), SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                InitializeServiceSdpAttributes(_Provider);
                _Provider.StartAdvertising(listener, true);
            }

            private void InitializeServiceSdpAttributes(RfcommServiceProvider provider)
            {
                DataWriter writer = new DataWriter();
                // First write the attribute type
                writer.WriteByte(SERVICE_VERSION_ATTRIBUTE_TYPE);
                // Then write the data
                writer.WriteUInt32(SERVICE_VERSION);
                IBuffer data = writer.DetachBuffer();
                provider.SdpRawAttributes.Add(SERVICE_VERSION_ATTRIBUTE_ID, data);

            }

            private void _OnConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
            {
                _Provider.StopAdvertising();
                Socket = args.Socket;
                System.Diagnostics.Debug.WriteLine("OnConnect");
                if (Socket != null)
                {
                    SendDataWriter = new DataWriter(Socket.OutputStream);
                    System.Diagnostics.Debug.WriteLine("SUCCEES on Thread: " + Thread.CurrentThread.ManagedThreadId);
                    ConnectionEstablishState = ConnectionEstablishState.Succeed;
                    OnConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState);
                    
                }
                else
                {
                    ConnectionEstablishState = ConnectionEstablishState.failed;
                    OnConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState);
                    System.Diagnostics.Debug.WriteLine("FUCK NO");
                }
                _ReceiveTask = ReceiveAsync();
                //_HandlePackMessageBuffer();
                //receive();
            }

            public void StartServer()
            {
                StartAdvertisingAsync();
            }
        }
    }
    
}
