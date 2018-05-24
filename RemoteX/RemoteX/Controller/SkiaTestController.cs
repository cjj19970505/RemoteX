using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using RemoteX.UI;

namespace RemoteX.Controller
{
    public class SkiaTestController : ContentPage
    {
        AbsoluteLayout absoluteLayout;
        SnackBar snackBar;
        public SkiaTestController()
        {
            SKCanvasView canvasView = new SKCanvasView();
            canvasView.PaintSurface += paintSurface;
            canvasView.Margin = 0;

            StackLayout stackLayout = new StackLayout()
            {
                BackgroundColor = Color.Green,
                Padding = 10,
                Children =
                    {
                        new Image(){
                            Source =  ImageSource.FromResource("RemoteX.RedWarning.png"),
                            //Scale = 1
                        },
                        new Frame
                        {
                            Content = new Label { Text = "Welcome to Xamarin.Forms!" },
                            HasShadow = true,
                            Padding = new Thickness(5, 5),
                            Margin = 1,
                            //outline

                        },
                        new Frame
                        {
                            Content = new Label { Text = "Welcome to Xamarin.Forms!" },
                            HasShadow = true,
                            Padding = new Thickness(5, 5),
                            Margin = 1,
                            //outline

                        }
                    }
            };
            absoluteLayout = new AbsoluteLayout();
            snackBar = new SnackBar();
            Frame frame = new Frame
            {
                Padding = 5,
                HasShadow = true,
                Content = absoluteLayout,
                OutlineColor = Color.Accent
            };

            AbsoluteLayout.SetLayoutFlags(canvasView, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(canvasView, new Rectangle(0.5, 0.5, 200, 200));

            AbsoluteLayout.SetLayoutFlags(stackLayout, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(stackLayout, new Rectangle(0, 0, 1, 1));

            absoluteLayout.Children.Add(stackLayout);
            absoluteLayout.Children.Add(canvasView);
            absoluteLayout.Children.Add(snackBar);

            System.Diagnostics.Debug.WriteLine("RECT::" + absoluteLayout.Bounds);
            Button animBtn = new Button();
            animBtn.Clicked += onAnimBtnClicked;
            Content = absoluteLayout;
            //absoluteLayout.RelRotateTo(180);
            canvasView.InvalidateSurface();

        }
        private async void onAnimBtnClicked(object sender, EventArgs e)
        {
            //await absoluteLayout.RelRotateTo(180);
            switch(snackBar.BarType)
            {
                case BarType.Error:
                    snackBar.BarType = BarType.Succeed;
                    break;
                case BarType.Succeed:
                    snackBar.BarType = BarType.Warning;
                    break;
                case BarType.Warning:
                    snackBar.BarType = BarType.Error;
                    break;
            }
            //snackBar.BarType = BarType.Error;
        }
        SKPaint paint = new SKPaint()
        {
            Color = SKColors.Black

        };
        SKPaint pain2t = new SKPaint()
        {
            Color = SKColors.White

        };
        private void paintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(new SKColor(255, 0, 0, 128));
            canvas.DrawCircle(new SKPoint(550, 550), 100, paint);
            canvas.DrawCircle(new SKPoint(552, 552), 100, pain2t);
            System.Diagnostics.Debug.WriteLine("\nDEVICE  Widht: " + canvas.DeviceClipBounds.Width + ", " + "Height: " + canvas.DeviceClipBounds.Height + ", Top:" + canvas.DeviceClipBounds.Top + ", Left:" + canvas.DeviceClipBounds.Left + ", Right:" + canvas.DeviceClipBounds.Right + ", Bottom:" + canvas.DeviceClipBounds.Bottom + ", Location" + canvas.DeviceClipBounds.Location);
            System.Diagnostics.Debug.WriteLine("\nLOCAL  Widht: " + canvas.LocalClipBounds.Width + ", " + "Height: " + canvas.LocalClipBounds.Height + ", Top:" + canvas.LocalClipBounds.Top + ", Left:" + canvas.LocalClipBounds.Left + ", Right:" + canvas.LocalClipBounds.Right + ", Bottom:" + canvas.LocalClipBounds.Bottom + ", Location" + canvas.LocalClipBounds.Location);
        }
    }
}