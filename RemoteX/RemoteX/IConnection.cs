using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX
{
    public enum ConnectionType { Bluetooth, UDP, TCP};
    public enum ConnectionEstablishState { NoEstablishment ,Succeed, failed, Connecting, Abort}
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
        ConnectionEstablishState ConnectionEstablishState { get; }
        event MessageHandler onReceiveMessage;
        event ConnectionHandler onConnectionEstalblishResult;
        Task SendAsync(byte[] message);

        /// <summary>
        /// 若迟迟没有返回连接成功的结果，可以调用这个中止连接
        /// </summary>
        void AbortConnecting();

        /// <summary>
        /// 中止通信，删除连接
        /// </summary>
        void Abort();

        /// <summary>
        /// 建立连接（务必实现异步）
        /// 仅在连接成功时返回，否则一直阻塞
        /// </summary>
        Task<ConnectionEstablishState> ConnectAsync();
    }
}
