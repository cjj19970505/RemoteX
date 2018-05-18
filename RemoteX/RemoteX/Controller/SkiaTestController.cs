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

            AbsoluteLayout.SetLayoutFlags(canvasView, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(canvasView, new Rectangle(0.5, 0.5, 0.1, 0.1));

            AbsoluteLayout.SetLayoutFlags(stackLayout, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(stackLayout, new Rectangle(0, 0, 1, 1));

            absoluteLayout.Children.Add(stackLayout);
            absoluteLayout.Children.Add(canvasView);
            absoluteLayout.Children.Add(snackBar);

            System.Diagnostics.Debug.WriteLine("RECT::" + absoluteLayout.Bounds);
            Button animBtn = new Button();
            animBtn.Clicked += onAnimBtnClicked;
            Content = new StackLayout()
            {
                Children = {
                    new Frame
                    {
                        Content = new Entry(),
                        HasShadow = true
                    },
                    absoluteLayout,
                    animBtn

                }
            };
            absoluteLayout.RelRotateTo(180);

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
        private void paintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(new SKColor(255, 0, 0, 128));
        }
    }
}