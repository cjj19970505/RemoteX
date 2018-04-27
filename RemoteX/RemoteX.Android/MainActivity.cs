using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Bluetooth;

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

            base.OnCreate(bundle);
            
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
            
        }

        /// <summary>
        /// 这玩意确保这货只要一被用到就初始化
        /// </summary>
        private InputManager InputManager
        {
            get
            {
                if(_InputManager == null)
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
    }
}

