using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.SkiaComponent
{
    public interface ISkiaInputComponent
    {
        /// <summary>
        /// 将整个输入面板的高度划分等级 比如手柄上一般摇杆比按钮要高 再操控摇杆时一般不会去碰到按钮
        /// </summary>
        int InputHeightLevel { get; }
        IArea FirstTouchArea { get; }
    }
}
