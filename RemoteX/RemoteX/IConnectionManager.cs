using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX
{
    /// <summary>
    /// 可能现在用不到。。。。
    /// 可能采用子系统的方式
    /// 启动。。。终止。。。
    /// 实现目的：
    /// 获取当前连接
    /// 获取保存的历史连接
    /// 这个Manager的作用主要在，让Controller的程序员（们）可以仅仅操心对特定类型的Connection进行事件监听，而不用比如创建一个新的连接以后又得删除以往的连接的监听建立新的事件监听等之类的
    /// 就是让Controller的程序员可以无视连接的细节，例如哪个连接创建了哪个连接挂掉了什么的
    /// </summary>
    public interface IConnectionManager
    {
        /// <summary>
        /// 控制器使用的连接
        /// </summary>
        IConnection ControllerConnection { get; set; }
        event ConnectionHandler onControllerConnectionEstalblishResult;
        event MessageHandler onControllerConnectionReceiveMessage;
    }
}
