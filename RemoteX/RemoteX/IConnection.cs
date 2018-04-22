using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX
{
    public enum ConnectionType { Bluetooth, UDP, TCP};
    public enum ConnectionEstablishState { Succeed, failed}
    public delegate void MessageHandler(IConnection connection ,byte[] message);
    public delegate void ConnectionHandler(IConnection connection, ConnectionEstablishState connectionEstablishState);
    /// <summary>
    /// 这个是对所有类型连接的抽象
    /// 可以包括本地连接，TCP连接，蓝牙连接
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// 
        /// </summary>
        ConnectionType connectionType { get; }
        event MessageHandler onReceiveMessage;
        event ConnectionHandler onConnectionEstalblishResult;
        Task sendAsync(byte[] message);

        /// <summary>
        /// 建立连接（务必实现异步）
        /// </summary>
        Task<ConnectionEstablishState> ConnectAsync();
    }
}
