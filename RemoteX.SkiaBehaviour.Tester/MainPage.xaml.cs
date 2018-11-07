using SkiaSharp;
using SkiaSharp.Views.UWP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using RemoteX.UWP.Input;
using RemoteX.SkiaComponent;
using SkiaBehaviour;
using System.Threading;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace RemoteX.SkiaBehaviour.Tester
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //SKXamlCanvas CanvasView;
        CanvasInfoProvider _CanvasInfoProvider;
        bool _FirstFrame = true;
        protected SkiaBehaviourEngine SkiaBehaviourEngine { get; private set; }
        ControlPanel _ControlPanel;
        InputManager InputManager;
        public MainPage()
        {
            this.InitializeComponent();
            
            //CanvasView = new SKXamlCanvas();
            InputManager = new InputManager(TouchRect);
            _CanvasInfoProvider = new CanvasInfoProvider();
            SkiaBehaviourEngine = new SkiaBehaviourEngine(_CanvasInfoProvider);
            _ControlPanel = new KeyboardControllerPanel(SkiaBehaviourEngine);
            SkiaInputManager skiaInputManager = SkiaBehaviourEngine.Instantiate<SkiaInputManager>();
            skiaInputManager.InputManager = InputManager;
            TouchRect.SizeChanged += CanvasView_SizeChanged;
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 16);
            dispatcherTimer.Start();
            System.Diagnostics.Debug.WriteLine("init:"+Thread.CurrentThread.ManagedThreadId);
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            //CanvasView.PaintSurface += CanvasView_PaintSurface;
            CanvasView.Invalidate();
            //CanvasView.PaintSurface += CanvasView_PaintSurface;
            //System.Diagnostics.Debug.WriteLine("TICK");
        }

        private void CanvasView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //CanvasView.Invalidate();
            
        }

        private void CanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            InputManager.EpxToPxCoefficient = (float)(CanvasView.CanvasSize.Height / CanvasView.RenderSize.Height);
            var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Center,
                TextSize = 24
            };
            e.Surface.Canvas.DrawColor(new SKColor(255, 0, 0));
            e.Surface.Canvas.DrawText("SkiaSharp", new SKPoint(500, 500), paint);
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            _CanvasInfoProvider.Canvas = canvas;
            canvas.Clear(SKColors.DarkBlue);
            if (_FirstFrame)
            {
                _ControlPanel.Start();
                _FirstFrame = false;
            }
            SkiaBehaviourEngine.Update();
            _ControlPanel.Update();

            SKMatrix m1 = new SKMatrix();
            m1.Values = new float[] { 1, 1, 0, 0, 1, 0, 0, 0, 1 };
            SKMatrix m2 = new SKMatrix
            {
                Values = new float[] { 1, 0, 1, 0, 1, 0, 0, 0, 1 }
            };
            SKMatrix m3 = new SKMatrix();
            SKMatrix.Concat(ref m3, m1, m2);
            string s = "";
            for(int i = 0; i < m3.Values.Length; i++)
            {
                s += m3.Values[i];
                s += ", ";
            }
            System.Diagnostics.Debug.WriteLine(s);
            

        }
    }
}
