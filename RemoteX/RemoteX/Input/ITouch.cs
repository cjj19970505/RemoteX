using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.Mathf;

namespace RemoteX.Input
{

    /// <summary>
    /// 在不同的地方拿到的同一个Touch要确保实例相同
    /// 就是说在代码A处和代码B处拿到一个手指的TouchA和TouchB，这两个Touch必须满足TouchA==TouchB
    /// </summary>
    public interface ITouch
    {
        List<Vector2> HistoryPosition { get; }
        Vector2 Position { get; }
    }
}
