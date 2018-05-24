using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace SkiaBehaviour
{
    /// <summary>
    /// 整个渲染流程的坐标转化
    /// Object   --> Canvas
    /// 物体坐标系-->画布坐标系
    /// 可能以后会添加一个摄像机坐标
    /// 整个库皆在提供一个与开发Skia环境完全无关的底层控件库
    /// 尽量模仿Unity
    /// 这个接口就是运行环境（UWP Xamarin WPF等）和画布之间的接口，需要调用者定义下面这些内容 
    /// </summary>
    public interface ICanvasInfoProvider
    {
        SKCanvas Canvas { get; }
        
    }
}
