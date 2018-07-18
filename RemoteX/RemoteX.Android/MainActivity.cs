using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Bluetooth;
using ZXing.Mobile;

namespace RemoteX.Droid
{
    [Activity(Label = "RemoteX", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        /// <summary>
        /// 哎，没办法，获取Touch要从Activity拿
        /// </summary>
        private InputManager _InputManager;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            RequestWindowFeature(WindowFeatures.NoTitle);//设置无标题
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);//设置全屏
            RequestedOrientation = ScreenOrientation.Landscape;

            base.OnCreate(bundle);

            //ZXing
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            MobileBarcodeScanner.Initialize(Application);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());


        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            //base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <summary>
        /// 这玩意确保这货只要一被用到就初始化
        /// </summary>
        private InputManager InputManager
        {
            get
            {
                if (_InputManager == null)
                {
                    _InputManager = Xamarin.Forms.DependencyService.Get<RemoteX.Input.IInputManager>() as InputManager;
                }
                return _InputManager;
            }
        }

        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            InputManager.OnTouch(ev);
            return base.DispatchTouchEvent(ev);
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                //return false;
            }
            return base.OnKeyDown(keyCode, e);
        }

        public async void ScanQRCode()
        {
            // Initialize the scanner first so it can track the current context
            MobileBarcodeScanner.Initialize(Application);

            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            var result = await scanner.Scan();

            if (result != null)
                System.Diagnostics.Debug.WriteLine("Scanned Barcode: " + result.Text);
        }
    }
}

