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
using Xamarin.Forms;
using RemoteX.Core;

namespace RemoteX.Droid
{
    partial class BluetoothManager
    {
        class BluetoothClientConnection : RemoteX.Bluetooth.IBluetoothClientConnection
        {
            private BluetoothDevice _Device;
            private UUID _SdpUuid;
            private BluetoothAdapter _BluetoothAdapter;

            protected Stream _InputStream;
            protected Stream _OutputStream;
            protected BluetoothSocket _BluetoothSocket;

            public event MessageHandler OnReceiveMessage;
            public event ConnectionHandler OnConnectionEstalblishResult;
            public ConnectionEstablishState ConnectionEstablishState { get; private set; }

            private bool _AbortConnecting = false;

            /// <summary>
            /// 最后一次发送的时间
            /// 用来衡量发送的两个消息的时间差距，主要要在接收端做延迟防抖动处理
            /// </summary>
            private DateTime _LastSendDateTime;

            private BluetoothManager _BluetoothManager;
            public BluetoothClientConnection(BluetoothManager bluetoothManager, BluetoothDevice device, UUID guid)
            {
                this._BluetoothManager = bluetoothManager;
                this._Device = device;
                this._SdpUuid = guid;
                this.ConnectionEstablishState = ConnectionEstablishState.Created;
            }
            public ConnectionType ConnectionType
            {
                get
                {
                    return ConnectionType.Bluetooth;
                }
            }
            private async Task<ConnectionEstablishState> establishConnectionAsync()
            {
                if (_Device == null)
                {
                    return ConnectionEstablishState.Failed;
                }
                BluetoothSocket tmp = null;
                if (_BluetoothAdapter == null)
                {
                    _BluetoothAdapter = BluetoothAdapter.DefaultAdapter;
                }
                try
                {
                    tmp = _Device.CreateInsecureRfcommSocketToServiceRecord(_SdpUuid);
                }
                catch (Java.IO.IOException)
                {
                    return ConnectionEstablishState.Failed;
                }
                _BluetoothSocket = tmp;
                _BluetoothAdapter.CancelDiscovery();
                try
                {
                    await _BluetoothSocket.ConnectAsync();
                    System.Diagnostics.Debug.WriteLine("BLUETOOTH::SUCCESSFUL ");
                }
                catch (Exception)
                {
                    try
                    {
                        _BluetoothSocket.Close();
                    }
                    catch (Java.IO.IOException)
                    {
                        return ConnectionEstablishState.Failed;
                    }
                    return ConnectionEstablishState.Failed;
                }
                try
                {
                    _InputStream = _BluetoothSocket.InputStream;
                    _OutputStream = _BluetoothSocket.OutputStream;
                }
                catch (Java.IO.IOException)
                {
                    try
                    {
                        _BluetoothSocket.Close();
                    }
                    catch (Java.IO.IOException)
                    {
                        return ConnectionEstablishState.Failed;
                    }
                    return ConnectionEstablishState.Failed;
                }
                _LastSendDateTime = DateTime.Now;
                return ConnectionEstablishState.Succeeded;
            }

            /// <summary>
            /// 将要发送的数据加上一些头部控制信息
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            private byte[] _PackMessage(int controlCode, byte[] message)
            {
                byte[] controlCodeBytes = BitConverter.GetBytes(controlCode);
                byte[] dataLengthBytes = BitConverter.GetBytes(message.Length);
                byte[] packedMsg = new byte[controlCodeBytes.Length + dataLengthBytes.Length + message.Length];
                controlCodeBytes.CopyTo(packedMsg, 0);
                dataLengthBytes.CopyTo(packedMsg, controlCodeBytes.Length);
                message.CopyTo(packedMsg, controlCodeBytes.Length + dataLengthBytes.Length);
                return packedMsg;
            }

            public async Task SendAsync(byte[] message)
            {
                await SendAsync(2, message);
            }

            private async Task SendAsync(int controlCode, byte[] message)
            {
                byte[] packedMsg = _PackMessage(controlCode, message);
                await _OutputStream.WriteAsync(packedMsg, 0, packedMsg.Length);
            }

            public async Task<ConnectionEstablishState> ConnectAsync()
            {
                this.ConnectionEstablishState = ConnectionEstablishState.Connecting;
                _BluetoothManager._BluetoothConnections.Add(this);
                OnConnectionEstalblishResult?.Invoke(this, this.ConnectionEstablishState);
                ConnectionEstablishState state;
                do
                {
                    state = await establishConnectionAsync();
                }
                while (state == ConnectionEstablishState.Failed && !_AbortConnecting);
                if (_AbortConnecting)
                {
                    _BluetoothManager._BluetoothConnections.Remove(this);
                    this.ConnectionEstablishState = ConnectionEstablishState.Abort;
                    _AbortConnecting = false;
                }
                else
                {
                    this.ConnectionEstablishState = state;
                }
                if (this.ConnectionEstablishState == ConnectionEstablishState.Succeeded)
                {
                    Device.StartTimer(new TimeSpan(0, 0, 0, 0, 500), sendControlCodeTimerFunc);
                }
                this.OnConnectionEstalblishResult?.Invoke(this, this.ConnectionEstablishState);
                return state;
            }

            private bool sendControlCodeTimerFunc()
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await SendAsync(1, new byte[] { 0 });
                    }
                    catch (Exception e)
                    {
                        if (e.Message == "Broken pipe")
                        {
                            _ReleaseAllConnectionResource();
                            this.ConnectionEstablishState = ConnectionEstablishState.Connecting;
                            System.Diagnostics.Debug.WriteLine("BROKEN PIPE FUCK YOU ");
                            Task connectTask = ConnectAsync();
                            OnConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState.Connecting);
                        }
                    }
                });

                return true;
            }

            public void AbortConnecting()
            {
                _AbortConnecting = true;
            }

            private void _ReleaseAllConnectionResource()
            {

                if (_InputStream != null)
                {
                    _InputStream.Close();
                }
                if (_OutputStream != null)
                {
                    _OutputStream.Close();
                }
                if (_BluetoothSocket != null)
                {
                    _BluetoothSocket.Close();
                }
                ConnectionEstablishState = ConnectionEstablishState.Abort;
            }

            public void Cancel()
            {
                throw new NotImplementedException();
            }

            public void Abort()
            {
                throw new NotImplementedException();
            }

            public void Connect()
            {
                Task connectTask = ConnectAsync();
            }

            public void Send(byte[] message)
            {
                Task sendTask = SendAsync(message);
            }
        }
    }

}