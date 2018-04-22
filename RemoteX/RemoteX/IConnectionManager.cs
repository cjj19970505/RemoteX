using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX
{
    /// <summary>
    /// 可能现在用不到。。。。
    /// 可能采用子系统的方式
    /// 启动。。。终止。。。
    /// </summary>
    public interface IConnectionManager
    {
        IConnection DefaultConnection { get; }
    }
}
