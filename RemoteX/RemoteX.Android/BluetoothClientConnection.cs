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

namespace RemoteX.Droid
{
    partial class BluetoothManager
    {
        class BluetoothClientConnection : RemoteX.Bluetooth.IBluetoothConnection
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
                this.ConnectionEstablishState = ConnectionEstablishState.NoEstablishment;
            }
            public ConnectionType connectionType
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
                    return ConnectionEstablishState.failed;
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
                catch (Java.IO.IOException e)
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
                catch (Exception connectionException)
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
                try
                {
                    _InputStream = _BluetoothSocket.InputStream;
                    _OutputStream = _BluetoothSocket.OutputStream;
                }
                catch (Java.IO.IOException streamException)
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
                _LastSendDateTime = DateTime.Now;
                return ConnectionEstablishState.Succeed;
            }

            /// <summary>
            /// 将要发送的数据加上一些头部控制信息
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            private byte[] _PackMessage(byte[] message, int controlCode = 0)
            {
                TimeSpan timeSpan;
                if (controlCode == 0)
                {
                    DateTime currDateTime = DateTime.Now;
                    timeSpan = currDateTime - _LastSendDateTime;
                    _LastSendDateTime = currDateTime;
                }
                else
                {
                    timeSpan = new TimeSpan(0, 0, 0, 0, -controlCode);
                }
                
                byte[] dataLengthBytes = BitConverter.GetBytes(message.Length);
                byte[] timeMsg = BitConverter.GetBytes(timeSpan.TotalMilliseconds);
                byte[] packedMsg = new byte[message.Length + timeMsg.Length + dataLengthBytes.Length];
                dataLengthBytes.CopyTo(packedMsg, 0);
                timeMsg.CopyTo(packedMsg, dataLengthBytes.Length);
                message.CopyTo(packedMsg, dataLengthBytes.Length + timeMsg.Length);
                return packedMsg;
            }

            public async Task SendAsync(byte[] message)
            {
                byte[] packedMsg = _PackMessage(message);
                await _OutputStream.WriteAsync(packedMsg, 0, packedMsg.Length);
            }

            private async Task SendAsync(int controlCode)
            {
                try
                {
                    byte[] packedMsg = _PackMessage(new byte[] { 0 }, 4);
                    await _OutputStream.WriteAsync(packedMsg, 0, packedMsg.Length);
                }
                catch(Exception e)
                {
                    if(e.Message == "Broken pipe")
                    {
                        _ReleaseAllConnectionResource();
                        //this.ConnectionEstablishState = ConnectionEstablishState.Disconnect;
                        //onConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState.Disconnect);

                        this.ConnectionEstablishState = ConnectionEstablishState.Connecting;
                        ConnectAsync();
                        onConnectionEstalblishResult?.Invoke(this, ConnectionEstablishState.Connecting);
                    }
                }
            }
            
            public async Task<ConnectionEstablishState> ConnectAsync()
            {
                this.ConnectionEstablishState = ConnectionEstablishState.Connecting;
                _BluetoothManager._BluetoothConnections.Add(this);
                onConnectionEstalblishResult?.Invoke(this, this.ConnectionEstablishState);
                ConnectionEstablishState state;
                do
                {
                    state = await establishConnectionAsync();
                }
                while (state == ConnectionEstablishState.failed && !_AbortConnecting);
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
                if(this.ConnectionEstablishState == ConnectionEstablishState.Succeed)
                {
                    Device.StartTimer(new TimeSpan(0, 0, 0, 0, 500), sendControlCodeTimerFunc);
                }
                this.onConnectionEstalblishResult?.Invoke(this, this.ConnectionEstablishState);
                return state;
            }

            private bool sendControlCodeTimerFunc()
            {
                Task.Run(async () => { await SendAsync(4); });
                return true;
            }
            
            public void AbortConnecting()
            {
                _AbortConnecting = true;
            }

            private void _ReleaseAllConnectionResource()
            {
                
                if(_InputStream != null)
                {
                    _InputStream.Close();
                }
                if(_OutputStream != null)
                {
                    _OutputStream.Close();
                }
                if(_BluetoothSocket != null)
                {
                    _BluetoothSocket.Close();
                }
                ConnectionEstablishState = ConnectionEstablishState.Abort;
            }
        }
    } 
    
}